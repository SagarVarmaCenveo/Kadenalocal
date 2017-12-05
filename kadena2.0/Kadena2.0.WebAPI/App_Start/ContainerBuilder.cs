﻿using AutoMapper;
using DryIoc;
using Kadena.BusinessLogic.Contracts;
using Kadena.WebAPI.Factories;
using Kadena.BusinessLogic.Factories.Checkout;
using Kadena.Helpers;
using Kadena.WebAPI.Infrastructure;
using Kadena.WebAPI.KenticoProviders;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena.BusinessLogic.Services;
using Kadena2.MicroserviceClients.Clients;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.MicroserviceClients.Contracts.Base;
using Kadena.BusinessLogic.Infrastructure;

namespace Kadena.WebAPI
{
    public static class ContainerBuilder
    {
        public static Container RegisterBLL(this Container container)
        {
            container.Register<IShoppingCartService, ShoppingCartService>();
            container.Register<ISearchService, SearchService>();
            container.Register<ICustomerDataService, CustomerDataService>();
            container.Register<ISettingsService, SettingsService>();
            container.Register<ISiteDataService, SiteDataService>();
            container.Register<ITaxEstimationService, TaxEstimationService>();
            container.Register<IOrderService, OrderService>();
            container.Register<IKListService, KListService>();
            container.Register<ITemplateService, TemplateService>();
            container.Register<IMailTemplateService, MailTemplateService>();			
            container.Register<IFavoritesService, FavoritesService>();
            container.Register<IProductsService, ProductsService>();
            container.Register<ICreditCardService, CreditCardService>();
            container.Register<IPdfService, PdfService>();
            return container;
        }

        public static Container RegisterKentico(this Container container)
        {
            container.Register<IKenticoProviderService, KenticoProviderService>();
            container.Register<IKenticoUserProvider, KenticoUserProvider>();
            container.Register<IKenticoResourceService, KenticoResourceService>();
            container.Register<IKenticoSearchService, KenticoSearchService>();
            container.Register<IKenticoLogger, KenticoLogger>();
            container.Register<IKenticoMailProvider, KenticoMailProvider>();			
            container.Register<IKenticoFavoritesProvider, KenticoFavoritesProvider>();
            container.Register<IKenticoProductsProvider, KenticoProductsProvider>();
            container.Register<ISubmissionIdProvider, SubmissionIdProvider>();
            container.Register<IKenticoDocumentProvider, KenticoDocumentProvider>();
            return container;
        }

        public static Container RegisterMicroservices(this Container container)
        {
            container.Register<IMailingListClient, MailingListClient>();
            container.Register<IOrderSubmitClient, OrderSubmitClient>();
            container.Register<IOrderViewClient, OrderViewClient>();
            container.Register<ITaxEstimationServiceClient, TaxEstimationServiceClient>();
            container.Register<ITemplatedClient, TemplatedClient>();
            container.Register<IAddressValidationClient, AddressValidationClient>();
            container.Register<ISuppliantDomainClient, SuppliantDomain>();
            container.Register<IFileClient, FileClient>();
            container.Register<IMicroProperties, MicroProperties>();
            return container;
        }

        public static Container RegisterFactories(this Container container)
        {
            container.Register<IOrderListServiceFactory, OrderListServiceFactory>();
            container.Register<ICheckoutPageFactory, CheckoutPageFactory>();
            return container;
        }

        public static Container RegisterInfrastructure(this Container container)
        {
            container.RegisterInstance(typeof(IMapper), Mapper.Instance);
            container.Register<IBackgroundTaskScheduler, BackgroundTaskScheduler>();
            container.Register<ICache>(Reuse.Singleton, Made.Of(() => new InMemoryCache()));            
            return container;
        }
    }
}