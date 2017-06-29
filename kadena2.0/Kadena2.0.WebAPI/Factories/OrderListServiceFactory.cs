﻿using System;
using Kadena.WebAPI.Contracts;
using AutoMapper;
using Kadena2.MicroserviceClients.Contracts;
using Kadena.WebAPI.Services;

namespace Kadena.WebAPI.Factories
{
    public class OrderListServiceFactory : IOrderListServiceFactory
    {
        private readonly IMapper _mapper;
        private readonly IOrderViewClient _orderClient;
        private readonly IKenticoResourceService _kenticoResources;
        private readonly IKenticoProviderService _kentico;
        private readonly IKenticoLogger _logger;

        public OrderListServiceFactory(IMapper mapper, IOrderViewClient orderClient,
            IKenticoResourceService kenticoResources, IKenticoProviderService kentico,
            IKenticoLogger logger)
        {
            _mapper = mapper;
            _orderClient = orderClient;
            _kenticoResources = kenticoResources;
            _kentico = kentico;
            _logger = logger;
        }

        public IOrderListService GetDashboard()
        {
            return new OrderListService(_mapper, _orderClient, _kenticoResources, _kentico, _logger)
            {
                PageCapacityKey = "KDA_DashboardOrdersPageCapacity"
            };
        }

        public IOrderListService GetRecentOrders()
        {
            return new OrderListService(_mapper, _orderClient, _kenticoResources, _kentico, _logger)
            {
                PageCapacityKey = "KDA_RecentOrdersPageCapacity",
                EnablePaging = true
            };
        }
    }
}