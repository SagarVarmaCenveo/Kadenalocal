﻿using AutoMapper;
using Kadena.Container.Default;
using Xunit;

namespace Kadena.Tests.Infrastructure
{
    public class AutoMapperTests
    {
        [Fact(DisplayName = "AutoMapper Configuration" )]
        public void ConfigurationValid()
        {
            var sut = DIContainer.Resolve<IMapper>();

            // Assert
            sut.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}