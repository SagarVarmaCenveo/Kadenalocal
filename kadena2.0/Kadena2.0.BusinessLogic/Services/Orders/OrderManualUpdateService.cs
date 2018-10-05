using AutoMapper;
using Kadena.BusinessLogic.Contracts;
using Kadena.BusinessLogic.Contracts.Approval;
using Kadena.BusinessLogic.Contracts.Orders;
using Kadena.Dto.OrderManualUpdate.MicroserviceRequests;
using Kadena.Models.CampaignData;
using Kadena.Models.Checkout;
using Kadena.Models.OrderDetail;
using Kadena.Models.Orders;
using Kadena.Models.Product;
using Kadena.Models.ShoppingCarts;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.WebAPI.KenticoProviders.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using Kadena.Dto.OrderManualUpdate.MicroserviceRequests.UpdateShipping;
using Newtonsoft.Json;
using System.Collections.Generic;
using Kadena2.BusinessLogic.Contracts.OrderPayment;

namespace Kadena.BusinessLogic.Services.Orders
{
    public class OrderManualUpdateService : IOrderManualUpdateService
    {
        private readonly IOrderManualUpdateClient updateService;
        private readonly IOrderViewClient orderService;
        private readonly IApproverService approvers;
        private readonly IKenticoProductsProvider productsProvider;
        private readonly IKenticoSkuProvider skuProvider;
        private readonly IKenticoPermissionsProvider permissions;
        private readonly IOrderItemCheckerService orderChecker;
        private readonly IProductsService products;
        private readonly IKenticoResourceService resources;
        private readonly IMapper mapper;
        private readonly IKenticoUserBudgetProvider budgetProvider;
        private readonly IDistributorShoppingCartService distributorShoppingCartService;
        private readonly IShoppingCartProvider shoppingCartProvider;
        private readonly ITaxEstimationService taxEstimationService;
        private readonly IKenticoLocalizationProvider localization;
        private readonly ISubmissionService submissionService;
        private readonly ICreditCard3dsi creditCard3dsi;

        public OrderManualUpdateService(IOrderManualUpdateClient updateService,
                                        IOrderViewClient orderService,
                                        IApproverService approvers,
                                        IKenticoProductsProvider productsProvider,
                                        IKenticoSkuProvider skuProvider,
                                        IKenticoPermissionsProvider permissions,
                                        IOrderItemCheckerService orderChecker,
                                        IProductsService products,
                                        IKenticoResourceService resources,
                                        IMapper mapper,
                                        IDistributorShoppingCartService distributorShoppingCartService,
                                        IShoppingCartProvider shoppingCartProvider,
                                        IKenticoUserBudgetProvider budgetProvider,
                                        ITaxEstimationService taxEstimationService,
                                        IKenticoLocalizationProvider localization,
                                        ISubmissionService submissionService,
                                        ICreditCard3dsi creditCard3dsi)
        {
            this.updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.approvers = approvers ?? throw new ArgumentNullException(nameof(approvers));
            this.productsProvider = productsProvider ?? throw new ArgumentNullException(nameof(productsProvider));
            this.skuProvider = skuProvider ?? throw new ArgumentNullException(nameof(skuProvider));
            this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            this.orderChecker = orderChecker ?? throw new ArgumentNullException(nameof(orderChecker));
            this.products = products ?? throw new ArgumentNullException(nameof(products));
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.distributorShoppingCartService = distributorShoppingCartService ?? throw new ArgumentNullException(nameof(distributorShoppingCartService));
            this.shoppingCartProvider = shoppingCartProvider ?? throw new ArgumentNullException(nameof(shoppingCartProvider));
            this.budgetProvider = budgetProvider ?? throw new ArgumentNullException(nameof(budgetProvider));
            this.taxEstimationService = taxEstimationService ?? throw new ArgumentNullException(nameof(taxEstimationService));
            this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
            this.submissionService = submissionService ?? throw new ArgumentNullException(nameof(submissionService));
            this.creditCard3dsi = creditCard3dsi ?? throw new ArgumentNullException(nameof(creditCard3dsi));
        }

        public async Task<UpdateOrdersShippingsResult> UpdateOrdersShippings(UpdateShippingRow[] request)
        {
            var updateRequest = new UpdateShippingsRequestDto();
            var shippingUpdates = new List<UpdateShippingsOrderDto>();

            var shippingOptions = shoppingCartProvider.GetShippingOptions();

            foreach (var orderGroup in request.GroupBy(x => x.OrderNumber))
            {
                var updateShippingOrder = new UpdateShippingsOrderDto
                {
                    OrderId = orderGroup.Key,
                    Items = new List<UpdateShippingsOrderItemDto>()
                };

                foreach (var updateShippingRow in orderGroup)
                {
                    var shippingProvider = shippingOptions
                        .FirstOrDefault(x => x.Service == updateShippingRow.ShippingMethod)
                        ?.CarrierCode;

                    updateShippingOrder.Items.Add(new UpdateShippingsOrderItemDto
                    {
                        LineNumber = updateShippingRow.LineNumber,
                        Shippings = new[]
                                {
                                    new UpdateShippingsOrderItemShippingDto
                                    {
                                        ItemId = updateShippingRow.TrackingInfoId,
                                        QuantityShipped = updateShippingRow.ShippedQuantity,
                                        ShippingDate = updateShippingRow.ShippingDate,
                                        TrackingNumber = updateShippingRow.TrackingNumber,
                                        ShippingMethod = new UpdateShippingsOrderItemShippingMethod
                                        {
                                            Provider = shippingProvider,
                                            ShippingService = updateShippingRow.ShippingMethod.Replace("#", "")
                                        }
                                    }
                                }
                    });
                }

                shippingUpdates.Add(updateShippingOrder);
            }

            updateRequest.ShippingUpdates = shippingUpdates.ToArray();

            var response = await updateService.UpdateOrdersShippings(updateRequest);
            if (response.Success)
                return new UpdateOrdersShippingsResult
                {
                    Success = true,
                    Message = resources.GetResourceString("Kadena.OrderReport.Manage.SubmitSuccess")
                };

            return new UpdateOrdersShippingsResult
            {
                Success = false,
                Message = response.ErrorMessages
            };
        }

        public async Task<OrderUpdateResult> UpdateOrder(OrderUpdateItems request)
        {
            CheckRequestData(request);

            var orderDetailResult = await orderService.GetOrderByOrderId(request.OrderId);

            if (!orderDetailResult.Success || orderDetailResult.Payload == null)
            {
                throw new Exception($"Failed to retireve data for order {request.OrderId}");
            }

            var orderDetail = orderDetailResult.Payload;

            if (orderDetail.Type == OrderType.prebuy)
            {
                throw new InvalidOperationException("Editing of order isn't supported for Pre-buy orders.");
            }

            var itemsWithoutDocument = orderDetail.Items.Where(i => i.DocumentId == 0).Select(i => i.Name);

            if (itemsWithoutDocument.Any())
            {
                throw new Exception("Following items were ordered with empty documentId : " + string.Join(", ", itemsWithoutDocument));
            }

            approvers.CheckIsCustomersEditor(orderDetail.ClientId);

            if (request.Items.Any(i => orderDetail.Items.Select(oi => oi.LineNumber == i.LineNumber).Count() < 1))
            {
                throw new Exception("Couldn't match all given line numbers in original order");
            }

            var updateItems = orderDetail.Items
                .GroupJoin(request.Items,
                    oi => oi.LineNumber,
                    ri => ri.LineNumber,
                    (oi, ri) =>
                    {
                        var ui = ri.DefaultIfEmpty().First();
                        return new
                        {
                            oi.LineNumber,
                            oi.DocumentId,
                            AdjustedQuantity = (ui?.Quantity ?? oi.Quantity) - oi.Quantity,
                            NewQuantity = ui?.Quantity ?? oi.Quantity,
                            oi.TemplateId,
                            oi.SkuId
                        };
                    })
                .ToList();

            if (IsCreditCardPayment(orderDetail.PaymentInfo.PaymentMethod) && updateItems.Any(i => i.AdjustedQuantity > 0))
            {
                throw new Exception("Can't increase item quantity, if payment method is credit card.");
            }

            // validate modification
            if (orderDetail.Type == OrderType.generalInventory)
            {
                foreach (var i in updateItems)
                {
                    if (i.AdjustedQuantity > 0)
                    {
                        distributorShoppingCartService.ValidateItem(i.SkuId, i.AdjustedQuantity, orderDetail.Customer.KenticoUserID);
                    }
                }
            }
            else
            {
                updateItems
                    .ForEach(d =>
                    {
                        ValidateItem(d.DocumentId, d.NewQuantity, d.AdjustedQuantity);
                    });
            }

            // create fake cart with new data
            var cart = mapper.Map<ShoppingCart>(orderDetail);
            cart.Address.Country = localization.GetCountries().FirstOrDefault(c => c.Code.Equals(cart.Address.Country.Code));
            cart.Address.State = localization
                .GetStates()
                .FirstOrDefault(s => s.StateCode.Equals(cart.Address.State.StateCode) && s.CountryId == cart.Address.Country.Id);
            var destinationAddress = cart.Address;
            cart.Items = updateItems
                .Select(i => new CartItemEntity
                {
                    SKUID = i.SkuId,
                    Quantity = i.NewQuantity,
                    CartItemPrice = GetPrice(i.DocumentId, i.NewQuantity)
                })
                .ToList();
            cart = shoppingCartProvider.Evaluate(cart);

            // get updated data from cart
            var requestDto = mapper.Map<OrderManualUpdateItemsRequestDto>(cart, opt => updateItems
                .ForEach(i => opt.Items.Add(i.SkuId.ToString(), i.LineNumber)));
            requestDto.OrderId = request.OrderId;
            requestDto.Items
                .AddRange(updateItems
                    .Where(i => i.NewQuantity < 1)
                    .Select(i => new ItemUpdateDto
                    {
                        LineNumber = i.LineNumber
                    }));

            requestDto.TotalTax = await taxEstimationService.EstimateTax(destinationAddress, requestDto.TotalPrice, requestDto.TotalShipping);

            var updateResult = await updateService.UpdateOrder(requestDto);
            if (!updateResult.Success)
            {
                throw new Exception("Failed to call order update microservice. " + updateResult.ErrorMessages);
            }

            // adjust available quantity and budget
            if (orderDetail.Type == OrderType.generalInventory)
            {
                updateItems
                    .ForEach(i =>
                    {
                        skuProvider.UpdateAvailableQuantity(i.SkuId, -i.AdjustedQuantity);
                        productsProvider.UpdateAllocatedProductQuantityForUser(i.SkuId, orderDetail.Customer.KenticoUserID, i.AdjustedQuantity);
                    });

                budgetProvider.AdjustUserRemainingBudget(
                    orderDetail.OrderDate.Year.ToString(),
                    orderDetail.Customer.KenticoUserID,
                    requestDto.TotalShipping - Convert.ToDecimal(orderDetail.PaymentInfo.Shipping));
            }
            else
            {
                updateItems
                    .ForEach(i =>
                    {
                        skuProvider.UpdateAvailableQuantity(i.SkuId, -i.AdjustedQuantity);
                    });
            }

            return GetUpdatesForFrontend(requestDto);
        }

        public async Task<OrderUpdatePaymentResult> UpdateOrderPayment(OrderUpdatePayment request)
        {

            var orderDetailResult = await orderService.GetOrderByOrderId(request.OrderId);

            if (!orderDetailResult.Success || orderDetailResult.Payload == null)
            {
                throw new Exception($"Failed to retireve data for order {request.OrderId}");
            }

            var orderDetail = orderDetailResult.Payload;

            if (orderDetail.Type == OrderType.prebuy)
            {
                throw new InvalidOperationException("Editing of order isn't supported for Pre-buy orders.");
            }

            var paymentMethods = shoppingCartProvider.GetPaymentMethods();
            var selectedPayment = paymentMethods.FirstOrDefault(p => p.Id == (request.PaymentMethod?.Id ?? -1));

            if (IsCreditCardPayment(selectedPayment.ShortClassName))
            {
                // there should be a check here to see if this is the first request and no CC information has been collected yet
                if (string.IsNullOrEmpty(request.PaymentMethod.Card))
                {
                    // prep for redirect to cc page
                    var submission = submissionService.GenerateNewSubmission(orderDetail.SiteId, orderDetail.Customer.KenticoUserID, orderDetail.ClientId, string.Empty);
                    return new OrderUpdatePaymentResult()
                    {
                        RedirectURL = creditCard3dsi.GetInsertCardDetailsUrl(submission.SubmissionId.ToString()),
                        Success = true
                    };
                }
                else
                {
                    // submit approval information
                    // modify request to micros
                }


            }

            var requestDto = new OrderManualUpdatePaymentRequestDto()
            {
                OrderId = orderDetail.Id,
                PaymentOption = new PaymentOptionsDto()
                {
                    PaymentOptionName = selectedPayment.ShortClassName,
                    PONumber = request.PaymentMethod.Invoice,
                    PaymentGatewayCustomerCode = string.Empty, // TODO: Populate when appropriate
                    PaymentGatewayName = string.Empty, // TODO: Populate when appropriate
                    TokenId = string.Empty, // TODO: Populate when appropriate
                    TransactionKey = string.Empty // TODO: Populate when appropriate
                }              
            };

            // send updated order to the microservice
            var updateResult = await updateService.UpdateOrderPayment(requestDto);
            if (!updateResult.Success)
            {
                throw new Exception("Failed to call order update microservice. " + updateResult.ErrorMessages);
            }

            return null;
        }

        private decimal? GetPrice(int documentId, int quantity)
        {
            var unitPrice = products.GetPriceByCustomModel(documentId, quantity);
            if (unitPrice == decimal.MinusOne)
            {
                return null;
            }
            return unitPrice;
        }

        void CheckRequestData(OrderUpdateItems request)
        {
            OrderNumber.Parse(request.OrderId);

            if ((request.Items?.Count() ?? 0) == 0)
            {
                throw new Exception("No items were submitted to process");
            }

            if (request.Items.GroupBy(i => i.LineNumber).Count() != request.Items.Count())
            {
                throw new Exception("Line numbers of changed items are not unique");
            }

            if (request.Items.Any(i => i.Quantity < 0))
            {
                throw new Exception("New item quantity cannot be < 0");
            }
        }

        OrderUpdateResult GetUpdatesForFrontend(OrderManualUpdateItemsRequestDto requestDto)
        {
            if (!permissions.UserCanSeePrices())
            {
                return null;
            }

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

                OrdersPrice = requestDto.Items.Select(d => new ItemUpdateResult
                {
                    LineNumber = d.LineNumber,
                    Price = String.Format("$ {0:#,0.00}", d.TotalPrice)
                }).ToArray()
            };

            return result;
        }

        private bool IsCreditCardPayment(string paymentMethod)
        {
            return paymentMethod == "CreditCard" || paymentMethod == "CreditCardDemo";
        }

        void ValidateItem(int documentId, int newQuantity, int adjustedQuantity)
        {
            var product = productsProvider.GetProductByDocumentId(documentId)
                ?? throw new Exception($"Unable to find product with id '{documentId}'.");

            if (product.HasProductTypeFlag(ProductTypes.MailingProduct))
            {
                throw new Exception("Cannot change quantity of Mailing product item");
            }

            var sku = skuProvider.GetSKU(product.SkuId)
                ?? throw new Exception($"Unable to find SKU with id '{product.SkuId}'.");

            orderChecker.CheckMinMaxQuantity(sku, newQuantity);

            if (product.HasProductTypeFlag(ProductTypes.InventoryProduct))
            {
                orderChecker.EnsureInventoryAmount(sku, adjustedQuantity, newQuantity);
            }
        }
    }
}
