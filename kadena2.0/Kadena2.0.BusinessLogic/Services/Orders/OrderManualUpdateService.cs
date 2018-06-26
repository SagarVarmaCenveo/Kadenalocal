﻿using AutoMapper;
using Kadena.BusinessLogic.Contracts;
using Kadena.BusinessLogic.Contracts.Approval;
using Kadena.BusinessLogic.Contracts.Orders;
using Kadena.Dto.EstimateDeliveryPrice.MicroserviceRequests;
using Kadena.Dto.OrderManualUpdate.MicroserviceRequests;
using Kadena.Dto.ViewOrder.MicroserviceResponses;
using Kadena.Models.OrderDetail;
using Kadena.Models.Orders;
using Kadena.Models.Product;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.MicroserviceClients.MicroserviceRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kadena.BusinessLogic.Services.Orders
{
    public class OrderManualUpdateService : IOrderManualUpdateService
    {
        private class UpdatedItemCheckData
        {
            public Product Product { get; set; }
            public OrderItemUpdate UpdatedItem { get; set; }
            public Sku Sku { get; set; }
            public OrderItemDTO OriginalItem { get; set; }
            public ItemUpdateDto ManuallyUpdatedItem { get; set; }
        }

        private readonly IOrderManualUpdateClient updateService;
        private readonly IOrderViewClient orderService;
        private readonly IApproverService approvers;
        private readonly IKenticoProductsProvider productsProvider;
        private readonly IKenticoSkuProvider skuProvider;
        private readonly IOrderItemCheckerService orderChecker;
        private readonly IProductsService products;
        private readonly ITaxEstimationServiceClient taxes;
        private readonly IShippingCostServiceClient shippingCosts;
        private readonly IKenticoResourceService resources;
        private readonly IDeliveryEstimationDataService deliveryData;
        private readonly IMapper mapper;

        public OrderManualUpdateService(IOrderManualUpdateClient updateService,
                                        IOrderViewClient orderService,
                                        IApproverService approvers,
                                        IKenticoProductsProvider productsProvider,
                                        IKenticoSkuProvider skuProvider,
                                        IOrderItemCheckerService orderChecker,
                                        IProductsService products,
                                        ITaxEstimationServiceClient taxes,
                                        IShippingCostServiceClient shippingCosts,
                                        IKenticoResourceService resources,
                                        IDeliveryEstimationDataService deliveryData,
                                        IMapper mapper)
        {
            this.updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.approvers = approvers ?? throw new ArgumentNullException(nameof(approvers));
            this.productsProvider = productsProvider ?? throw new ArgumentNullException(nameof(productsProvider));
            this.skuProvider = skuProvider ?? throw new ArgumentNullException(nameof(skuProvider));
            this.orderChecker = orderChecker ?? throw new ArgumentNullException(nameof(orderChecker));
            this.products = products ?? throw new ArgumentNullException(nameof(products));
            this.taxes = taxes ?? throw new ArgumentNullException(nameof(taxes));
            this.shippingCosts = shippingCosts ?? throw new ArgumentNullException(nameof(shippingCosts));
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
            this.deliveryData = deliveryData ?? throw new ArgumentNullException(nameof(deliveryData));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrderUpdateResult> UpdateOrder(OrderUpdate request)
        {
            var orderDetailResult = await orderService.GetOrderByOrderId(request.OrderId);

            if (!orderDetailResult.Success || orderDetailResult.Payload == null)
            {
                throw new Exception($"Failed to retireve data for order {request.OrderId}");
            }

            var orderDetail = orderDetailResult.Payload;

            approvers.CheckIsCustomersEditor(orderDetail.ClientId);
            
            var documentIds = orderDetail.Items.Select(i => i.DocumentId).Distinct().ToArray();
            var skuIds = orderDetail.Items.Select(i => i.SkuId).Distinct().ToArray();
            var products = productsProvider.GetProductsByDocumentIds(documentIds);
            var skus = skuProvider.GetSKUsByIds(skuIds);

            var updatedItemsData = request.Items.Join(orderDetail.Items,
                                                       chi => chi.LineNumber,
                                                       oi => oi.LineNumber,
                                                       (chi, oi) => new UpdatedItemCheckData
                                                           {
                                                               OriginalItem = oi,
                                                               UpdatedItem = chi,
                                                               Sku = skus.First(s => s.SkuId == oi.SkuId),
                                                               Product = products.First(p => p.SkuId == oi.SkuId)
                                                           }
                                                       ).ToList();

            updatedItemsData.ForEach(d => d.ManuallyUpdatedItem = CreateChangedItem(d));

            var changedItems = updatedItemsData.Select(d => d.ManuallyUpdatedItem).ToList();

            var requestDto = new OrderManualUpdateRequestDto
            {
                OrderId = request.OrderId,
                Items = changedItems,
            };

            await DoEstimations(requestDto, updatedItemsData, orderDetail);
            var updateResult = await updateService.UpdateOrder(requestDto);
            if (!updateResult.Success)
            {
                throw new Exception("Failed to call order update microservice. " + updateResult.ErrorMessages);
            }

            UpdateAvailableItems(updatedItemsData);

            return GetUpdatesForFrontend(updatedItemsData, requestDto);
        }

        OrderUpdateResult GetUpdatesForFrontend(IEnumerable<UpdatedItemCheckData> updateData, OrderManualUpdateRequestDto requestDto)
        {            
            var result = new OrderUpdateResult
            {
                PricingInfo = new[]
                {
                        new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingSummary"),
                            Value = String.Format("$ {0:#,0.00}", requestDto.TotalPrice)
                        },
                        new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingShipping"),
                            Value = String.Format("$ {0:#,0.00}", requestDto.TotalShipping)
                        },
                        new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingSubtotal"),
                            Value = String.Format("$ {0:#,0.00}",requestDto.TotalPrice + requestDto.TotalShipping)
                        },
                        new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingTax"),
                            Value = String.Format("$ {0:#,0.00}",requestDto.TotalTax)
                        },
                        new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingTotals"),
                            Value = String.Format("$ {0:#,0.00}",requestDto.TotalPrice + requestDto.TotalShipping + requestDto.TotalTax)
                        }

                },

                OrdersPrice = updateData.Select(d => new ItemUpdateResult
                {
                    LineNumber = d.ManuallyUpdatedItem.LineNumber,
                    Price = String.Format("$ {0:#,0.00}", d.ManuallyUpdatedItem.TotalPrice)  
                }).ToArray()
            };

            return result;
        }

        void UpdateAvailableItems(IEnumerable<UpdatedItemCheckData> updateData)
        {
            var inventoryProductsData = updateData
                .Where(u => ProductTypes.IsOfType(u.Product.ProductType, ProductTypes.InventoryProduct))
                .ToList();

            inventoryProductsData.ForEach(data =>
            {
                var addedQuantity = data.UpdatedItem.Quantity - data.OriginalItem.Quantity;

                // Not using Set... because when waiting for result of OrderUpdate, quantity can change
                skuProvider.IncreaseSkuAvailableQty(data.Sku.SKUNumber, addedQuantity);
            });
        }

        async Task DoEstimations(OrderManualUpdateRequestDto request, IEnumerable<UpdatedItemCheckData> updateData, GetOrderByOrderIdResponseDTO orderDetail)
        {
            request.TotalPrice = 0.0m;

            orderDetail.Items.ForEach(i =>
            {
                var updatedItem = updateData.FirstOrDefault(d => d.ManuallyUpdatedItem.LineNumber == i.LineNumber);

                if (updatedItem != null)
                {
                    request.TotalPrice += updatedItem.ManuallyUpdatedItem.TotalPrice;
                }
                else
                {
                    request.TotalPrice += (decimal)i.TotalPrice;
                }
            }
            );

            var shippableWeight = updateData
                .Where(u => u.Sku.NeedsShipping)
                .Sum(u => u.Sku.Weight * u.ManuallyUpdatedItem.Quantity);

            request.TotalShipping = 0.0m;

            var sourceAddress = deliveryData.GetSourceAddress();
            var targetAddress = mapper.Map<AddressDto>(orderDetail.ShippingInfo.AddressTo);
            targetAddress.Country = orderDetail.ShippingInfo.AddressTo.isoCountryCode;

            if (updateData.Any(u => u.ManuallyUpdatedItem.Quantity > 0) && !orderDetail.ShippingInfo.Provider.EndsWith("Customer"))
            {
                var shippingCostRequest = deliveryData.GetDeliveryEstimationRequestData(orderDetail.ShippingInfo.Provider, 
                                                                           orderDetail.ShippingInfo.ShippingService, 
                                                                           (decimal)shippableWeight,
                                                                           targetAddress);
                
                var totalShippingResult = await shippingCosts.EstimateShippingCost(shippingCostRequest);

                if (totalShippingResult.Success == false || totalShippingResult.Payload.Length < 1 || !totalShippingResult.Payload[0].Success)
                {
                    throw new Exception($"Cannot be delivered by original provider and service. Request error: '{totalShippingResult.ErrorMessages}', Item error: '{totalShippingResult.Payload?[0]?.ErrorMessage}'");
                }

                request.TotalShipping = totalShippingResult.Payload[0].Cost;
            }

            request.TotalTax = await EstimateTax(request.TotalPrice, request.TotalShipping, sourceAddress, targetAddress);
        }
        
        async Task<decimal> EstimateTax(decimal totalBasePrice, decimal shipppingCosts, AddressDto sourceAddress, AddressDto targetAddress)
        {
            if (totalBasePrice == 0.0m)
            {
                return 0.0m;
            }

            var taxRequest = new TaxCalculatorRequestDto
            {
                ShipCost = (double)shipppingCosts,
                TotalBasePrice = (double)totalBasePrice,

                ShipFromCity = sourceAddress.City,
                ShipFromState = sourceAddress.State,
                ShipFromZip = sourceAddress.Postal,

                ShipToCity = targetAddress.City,
                ShipToState = targetAddress.State,
                ShipToZip = targetAddress.Postal
            };

            var taxResult = await taxes.CalculateTax(taxRequest);

            if (!taxResult.Success)
            {
                throw new Exception("Failed to estimate tax");
            }

            return taxResult.Payload;
        }

        ItemUpdateDto CreateChangedItem(UpdatedItemCheckData data)
        {
            orderChecker.CheckMinMaxQuantity(data.Sku, data.UpdatedItem.Quantity);

            var addedQuantity = data.UpdatedItem.Quantity - data.OriginalItem.Quantity;

            if (ProductTypes.IsOfType(data.Product.ProductType, ProductTypes.InventoryProduct))
            {
                orderChecker.EnsureInventoryAmount(data.Sku, addedQuantity, data.UpdatedItem.Quantity);
            }

            var unitPrice = products.GetPriceByCustomModel( data.OriginalItem.DocumentId, data.UpdatedItem.Quantity);
            if (unitPrice == decimal.MinusOne)
            {
                unitPrice = data.Sku.Price;
            }

            return new ItemUpdateDto
            {
                LineNumber = data.OriginalItem.LineNumber,
                Quantity = data.UpdatedItem.Quantity,
                TotalPrice = unitPrice * data.UpdatedItem.Quantity,
                UnitPrice = unitPrice
            };
        }
    }
}