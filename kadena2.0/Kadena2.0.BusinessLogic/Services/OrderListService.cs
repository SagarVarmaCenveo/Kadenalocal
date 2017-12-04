﻿using Kadena.BusinessLogic.Contracts;
using Kadena.Models.RecentOrders;
using System.Collections.Generic;
using AutoMapper;
using Kadena2.MicroserviceClients.Contracts;
using System.Threading.Tasks;
using Kadena.Models;
using System.Linq;
using Kadena.Dto.Order;
using Kadena.Dto.General;
using Kadena.WebAPI.KenticoProviders.Contracts;

namespace Kadena.BusinessLogic.Services
{
    public class OrderListService : IOrderListService
    {
        private readonly IMapper _mapper;
        private readonly IOrderViewClient _orderClient;
        private readonly IKenticoUserProvider _kenticoUsers;
        private readonly IKenticoResourceService _kenticoResources;
        private readonly IKenticoProviderService _kentico;
        private readonly IKenticoLogger _logger;

        private readonly string _orderDetailUrl;

        private int _pageCapacity;
        private string _pageCapacityKey;

        public string PageCapacityKey
        {
            get
            {
                return _pageCapacityKey;
            }

            set
            {
                _pageCapacityKey = value;
                string settingValue = _kenticoResources?.GetSettingsKey(_pageCapacityKey);
                _pageCapacity = int.Parse(string.IsNullOrWhiteSpace(settingValue) ? "5" : settingValue);
            }
        }

        public bool EnablePaging { get; set; }

        public OrderListService(IMapper mapper, IOrderViewClient orderClient, IKenticoUserProvider kenticoUsers,
            IKenticoResourceService kenticoResources, IKenticoProviderService kentico,
            IKenticoLogger logger)
        {
            _mapper = mapper;
            _orderClient = orderClient;
            _kenticoUsers = kenticoUsers;
            _kenticoResources = kenticoResources;
            _kentico = kentico;
            _logger = logger;

            _orderDetailUrl = kentico.GetDocumentUrl(kenticoResources.GetSettingsKey("KDA_OrderDetailUrl"));
        }

        public async Task<OrderHead> GetHeaders()
        {
            var orderList = _mapper.Map<OrderList>(await GetOrders(1));
            MapOrdersStatusToGeneric(orderList?.Orders);
            int pages = 0;
            if (EnablePaging && _pageCapacity > 0)
            {
                pages = orderList.TotalCount / _pageCapacity + (orderList.TotalCount % _pageCapacity > 0 ? 1 : 0);
            }
            return new OrderHead
            {
                Headings = new List<string> {
                    _kenticoResources.GetResourceString("Kadena.OrdersList.OrderNumber"),
                    _kenticoResources.GetResourceString("Kadena.OrdersList.OrderDate"),
                    _kenticoResources.GetResourceString("Kadena.OrdersList.OrderedItems"),
                    _kenticoResources.GetResourceString("Kadena.OrdersList.OrderStatus"),
                    //_kenticoResources.GetResourceString("Kadena.OrdersList.DeliveryDate"),
                    _kenticoResources.GetResourceString("Kadena.OrdersList.ShippingDate"),
                    string.Empty
                },
                PageInfo = new Pagination
                {
                    RowsCount = orderList.TotalCount,
                    RowsOnPage = _pageCapacity,
                    PagesCount = pages
                },
                NoOrdersMessage = _kenticoResources.GetResourceString("Kadena.OrdersList.NoOrderItems"),
                Rows = orderList.Orders.Select(o =>
                {
                    o.ViewBtn = new Button { Text = _kenticoResources.GetResourceString("Kadena.OrdersList.View"), Url = $"{_orderDetailUrl}?orderID={o.Id}" };
                    return o;
                })
            };
        }

        public async Task<OrderBody> GetBody(int pageNumber)
        {
            var orderList = _mapper.Map<OrderList>(await GetOrders(pageNumber));
            MapOrdersStatusToGeneric(orderList?.Orders);
            return new OrderBody
            {
                Rows = orderList.Orders.Select(o =>
                {
                    o.ViewBtn = new Button { Text = _kenticoResources.GetResourceString("Kadena.OrdersList.View"), Url = $"{_orderDetailUrl}?orderID={o.Id}" };
                    return o;
                })
            };
        }

        private void MapOrdersStatusToGeneric(IEnumerable<Order> orders)
        {
            if (orders == null)
                return;

            foreach (var o in orders)
            {
                o.Status = _kentico.MapOrderStatus(o.Status);
            }
        }


        private async Task<OrderListDto> GetOrders(int pageNumber)
        {
            var siteName = _kenticoResources.GetKenticoSite().Name;
            BaseResponseDto<OrderListDto> response = null;
            if (_kentico.IsAuthorizedPerResource("Kadena_Orders", "KDA_SeeAllOrders", siteName))
            {
                response = await _orderClient.GetOrders(siteName, pageNumber, _pageCapacity);
            }
            else
            {
                var customer = _kenticoUsers.GetCurrentCustomer();
                response = await _orderClient.GetOrders(customer?.Id ?? 0, pageNumber, _pageCapacity);
            }

            if (response?.Success ?? false)
            {
                return response.Payload;
            }
            else
            {
                _logger.LogError("OrderListService - GetOrders", response?.Error?.Message);
                return new OrderListDto();
            }
        }
    }
}