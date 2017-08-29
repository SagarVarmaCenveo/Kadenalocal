﻿using Kadena.Dto.General;
using Kadena.Models.Site;
using Kadena.ScheduledTasks.DeleteExpiredMailingLists;
using Kadena.ScheduledTasks.Infrastructure;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena2.MicroserviceClients.Contracts;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Kadena.Tests.ScheduledTasks
{
    public class DeleteExpiredMailingListsServiceTests
    {
        private Task<BaseResponseDto<object>> GetSuccessFullMicroserviceResponse()
        {
            return Task.FromResult(new BaseResponseDto<object> { Success = true });
        }

        private Task<BaseResponseDto<object>> GetUnsuccessFullMicroserviceResponse()
        {
            return Task.FromResult(new BaseResponseDto<object> { Success = false });
        }

        [Fact]
        public async Task Service_ShouldNotRun_WhenNoIntervalIsSpecified()
        {
            // Arrange
            var site = "site1";
            var sites = new[] { new Site { Name = site } };
            var kenticoProvider = Mock.Of<IKenticoProviderService>(kp => kp.GetSites() == sites);
            var mailingService = new Mock<IMailingListClient>(MockBehavior.Strict);
            var configuration = new MailingListConfiguration { DeleteMailingListsPeriod = null };
            var configurationProvider = Mock.Of<IConfigurationProvider>(cp =>
                cp.Get<MailingListConfiguration>(site) == configuration);
            var sut = new DeleteExpiredMailingListsService(configurationProvider, kenticoProvider, mailingService.Object);

            // Act
            await sut.Delete();

            // Assert
            // mailing is strict, so it throws when any method is called
        }

        [Fact]
        public async Task Service_ShouldThrowException_WhenMicroserviceFails()
        {
            // Arrange
            var site = "site1";
            var sites = new[] { new Site { Name = site } };
            var kenticoProvider = Mock.Of<IKenticoProviderService>(kp => kp.GetSites() == sites);
            var mailingService = Mock.Of<IMailingListClient>(mlc =>
                mlc.RemoveMailingList(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()) == GetUnsuccessFullMicroserviceResponse());
            var configuration = new MailingListConfiguration { DeleteMailingListsPeriod = 10 };
            var configurationProvider = Mock.Of<IConfigurationProvider>(cp =>
                cp.Get<MailingListConfiguration>(site) == configuration);
            var sut = new DeleteExpiredMailingListsService(configurationProvider, kenticoProvider, mailingService);

            // Act
            var result = sut.Delete();

            // Assert
            await Assert.ThrowsAsync<Exception>(() => result);
        }

        [Fact]
        public async Task Service_ShouldCalculateCorrectMinimalValidToDate()
        {
            // Arrange
            var configuration = new MailingListConfiguration { DeleteMailingListsPeriod = 10 };
            var currentTime = new DateTime(2000, 1, 1);
            var minimalTime = currentTime.AddDays(-configuration.DeleteMailingListsPeriod.Value);
            var site = "site1";
            var sites = new[] { new Site { Name = site } };
            var kenticoProvider = Mock.Of<IKenticoProviderService>(kp => kp.GetSites() == sites);
            var mailingService = Mock.Of<IMailingListClient>(mlc => 
                mlc.RemoveMailingList(It.IsAny<string>(), site, minimalTime) == GetSuccessFullMicroserviceResponse());
            var configurationProvider = Mock.Of<IConfigurationProvider>(cp =>
                cp.Get<MailingListConfiguration>(site) == configuration);
            var sut = new DeleteExpiredMailingListsService(configurationProvider, kenticoProvider, mailingService);
            sut.GetCurrentTime = () => currentTime;

            // Act
            await sut.Delete();

            // Assert
            Mock.Get(mailingService)
                .Verify(mlc => mlc.RemoveMailingList(It.IsAny<string>(), site, minimalTime), Times.Once());
        }
    }
}