using AutoMapper;
using Kadena.BusinessLogic.Contracts;
using Kadena.Dto.SubmitOrder.MicroserviceRequests;
using Kadena.Helpers;
using Kadena.Helpers.Routes;
using Kadena.Models;
using Kadena.Models.Checkout;
using Kadena.Models.Common;
using Kadena.Models.OrderDetail;
using Kadena.Models.Orders;
using Kadena.Models.Product;
using Kadena.Models.Shipping;
using Kadena.Models.SiteSettings.Permissions;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.WebAPI.KenticoProviders.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace Kadena.BusinessLogic.Services.Orders
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IMapper mapper;
        private readonly IOrderViewClient orderViewClient;
        private readonly IMailingListClient mailingClient;
        private readonly IKenticoOrderProvider kenticoOrder;
        private readonly IOrderManualUpdateClient orderHistoryClient;
        private readonly IShoppingCartProvider shoppingCart;
        private readonly IKenticoProductsProvider products;
        private readonly IKenticoCustomerProvider kenticoCustomers;
        private readonly IKenticoResourceService resources;
        private readonly IKenticoLogger kenticoLog;
        private readonly IKenticoLocalizationProvider localization;
        private readonly IKenticoPermissionsProvider permissions;
        private readonly IKenticoBusinessUnitsProvider businessUnits;
        private readonly IKenticoSiteProvider site;
        private readonly IImageService imageService;
        private readonly IPdfService pdfService;
        private readonly IKenticoUnitOfMeasureProvider units;

        public OrderDetailService(IMapper mapper,
            IOrderViewClient orderViewClient,
            IMailingListClient mailingClient,
            IKenticoOrderProvider kenticoOrder,
            IOrderManualUpdateClient orderHistoryClient,
            IShoppingCartProvider shoppingCart,
            IKenticoProductsProvider products,
            IKenticoCustomerProvider kenticoCustomers,
            IKenticoResourceService resources,
            IKenticoLogger kenticoLog,
            IKenticoLocalizationProvider localization,
            IKenticoPermissionsProvider permissions,
            IKenticoBusinessUnitsProvider businessUnits,
            IKenticoSiteProvider site,
            IImageService imageService,
            IPdfService pdfService,
            IKenticoUnitOfMeasureProvider units
            )
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.orderViewClient = orderViewClient ?? throw new ArgumentNullException(nameof(orderViewClient));
            this.kenticoOrder = kenticoOrder ?? throw new ArgumentNullException(nameof(kenticoOrder));
            this.orderHistoryClient = orderHistoryClient ?? throw new ArgumentNullException(nameof(orderHistoryClient));
            this.shoppingCart = shoppingCart ?? throw new ArgumentNullException(nameof(shoppingCart));
            this.products = products ?? throw new ArgumentNullException(nameof(products));
            this.kenticoCustomers = kenticoCustomers ?? throw new ArgumentNullException(nameof(kenticoCustomers));
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
            this.mailingClient = mailingClient ?? throw new ArgumentNullException(nameof(mailingClient));
            this.kenticoLog = kenticoLog ?? throw new ArgumentNullException(nameof(kenticoLog));
            this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
            this.permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            this.businessUnits = businessUnits ?? throw new ArgumentNullException(nameof(businessUnits));
            this.site = site ?? throw new ArgumentNullException(nameof(site));
            this.imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.units = units ?? throw new ArgumentNullException(nameof(units));
        }

        public async Task<OrderDetail> GetOrderDetail(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var orderNumber = OrderNumber.Parse(orderId);

            var microserviceResponse = await orderViewClient.GetOrderByOrderId(orderId);

            if (!microserviceResponse.Success || microserviceResponse.Payload == null)
            {
                kenticoLog.LogError("GetOrderDetail", microserviceResponse.ErrorMessages);
                return null;
            }

            var data = microserviceResponse.Payload;
            var genericStatus = kenticoOrder.MapOrderStatus(data.Status);
            
            var businessUnitName = "";
            if (long.TryParse(data.campaign?.BusinessUnitNumber, out var bun))
            {
                businessUnitName = businessUnits.GetBusinessUnitName(bun);
            }

            var customer = kenticoCustomers.GetCustomer(data.ClientId) ?? Customer.Unknown;
            var isWaitingForApproval = data.StatusId == (int)OrderStatus.WaitingForApproval;
            var canCurrentUserApproveOrder = isWaitingForApproval && IsCurrentUserApproverFor(customer);
            var canCurrentUserEditInApproval = permissions.CurrentUserHasPermission(ModulePermissions.KadenaOrdersModule, ModulePermissions.KadenaOrdersModule.EditOrdersInApproval);
            var showApprovalButtons = canCurrentUserApproveOrder;
            var showEditButton = canCurrentUserApproveOrder && canCurrentUserEditInApproval;
            var showOrderHistory = isWaitingForApproval;
            if (!showOrderHistory)
            {
                var history = await orderHistoryClient.Get(orderId);
                showOrderHistory = history.Payload
                    .Any(a => a.StatusId == (int)OrderStatus.WaitingForApproval);
            }

            CheckOrderDetailPermisson(orderNumber, kenticoCustomers.GetCurrentCustomer(), canCurrentUserApproveOrder);

            var orderDetail = new OrderDetail()
            {
                DateTimeNAString = resources.GetResourceString("Kadena.Order.ItemShippingDateNA"),

                General = new OrderInfo
                {
                    OrderId = orderId,
                    CustomerId = customer.Id,
                    CustomerName = customer.FullName
                },

                EditOrders = showEditButton
                    ? new DialogButton<EditOrderDialog>
                    {
                        Button = resources.GetResourceString("Kadena.Order.ButtonEdit"),
                        ProceedUrl = $"/{Routes.Order.OrderUpdate}",
                        Dialog = new EditOrderDialog
                        {
                            Title = resources.GetResourceString("Kadena.Order.EditTite"),
                            Description = resources.GetResourceString("Kadena.Order.EditDescription"),
                            SuccessMessage = resources.GetResourceString("Kadena.Order.EditSuccess"),
                            ValidationMessage = resources.GetResourceString("Kadena.Order.EditValidation"),
                            Buttons = new EditOrderDialogButtons
                            {
                                Cancel = resources.GetResourceString("Kadena.Order.EditCancel"),
                                Proceed = resources.GetResourceString("Kadena.Order.EditProceed"),
                                Remove = resources.GetResourceString("Kadena.Order.EditRemove")
                            }
                        }
                    }
                    : null,
                Actions = showApprovalButtons
                    ? new OrderActions
                    {
                        Accept = new DialogButton<Dialog>
                        {
                            Button = resources.GetResourceString("Kadena.Order.ButtonAccept"),
                            ProceedUrl = '/' + Routes.Order.Approve,
                        },
                        Reject = new DialogButton<Dialog>
                        {
                            Button = resources.GetResourceString("Kadena.Order.ButtonReject"),
                            ProceedUrl = '/' + Routes.Order.Reject,
                            Dialog = new Dialog
                            {
                                CancelButton = resources.GetResourceString("Kadena.Order.DialogReject.Cancel"),
                                ProceedButton = resources.GetResourceString("Kadena.Order.DialogReject.Proceed"),
                                Title = resources.GetResourceString("Kadena.Order.DialogReject.Title")
                            }
                        },
                        Comment = new TitleValuePair<string>
                        {
                            Title = resources.GetResourceString("Kadena.Order.Comment.Title")
                        }
                    }
                    : null,

                CommonInfo = new CommonInfo()
                {
                    OrderDate = new TitleValuePair<DateTime>
                    {
                        Title = resources.GetResourceString("Kadena.Order.OrderDateTitle"),
                        Value = data.OrderDate
                    },
                    ShippingDate = new TitleValuePair<DateTime?>
                    {
                        Title = resources.GetResourceString("Kadena.Order.ShippingDatePrefix"),
                        Value = data.ShippingInfo?.ShippingDate
                    },
                    Status = new OrderStatusInfo
                    {
                        Title = resources.GetResourceString("Kadena.Order.StatusPrefix"),
                        Value = genericStatus,
                        OrderHistory = showOrderHistory
                            ? new Link
                            {
                                Label = resources.GetResourceString("Kadena.Order.Status.OrderHistory"),
                                Url = UrlHelper.GetUrlForOrderHistory(orderId)
                            }
                            : null
                    },
                    TotalCost = new TitleValuePair<string>
                    {
                        Title = resources.GetResourceString("Kadena.Order.TotalCostPrefix"),
                        Value = String.Format("$ {0:#,0.00}", data.PaymentInfo.Summary + data.PaymentInfo.Shipping + data.PaymentInfo.Tax)
                    }
                },
                PaymentInfo = new PaymentInfo()
                {
                    Date = data.PaymentInfo.CapturedDate,
                    PaidBy = data.PaymentInfo.PaymentMethod,
                    PaymentDetail = string.Empty,
                    PaymentIcon = GetPaymentMethodIcon(data.PaymentInfo.PaymentMethod),
                    Title = resources.GetResourceString("Kadena.Order.PaymentSection"),
                    DatePrefix = resources.GetResourceString("Kadena.Order.PaymentDatePrefix"),
                    BUnitLabel = resources.GetResourceString("Kadena.Order.BusinessUnitLabel"),
                    BUnitName = businessUnitName
                },
                PricingInfo = new PricingInfo()
                {
                    Title = resources.GetResourceString("Kadena.Order.PricingSection"),
                    Items = new List<PricingInfoItem>()
                    {
                        new PricingInfoItem()
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingSummary"),
                            Value = String.Format("$ {0:#,0.00}", data.PaymentInfo.Summary)
                        },
                        new PricingInfoItem()
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingShipping"),
                            Value = String.Format("$ {0:#,0.00}", data.PaymentInfo.Shipping)
                        },
                        new PricingInfoItem()
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingSubtotal"),
                            Value = String.Format("$ {0:#,0.00}",data.PaymentInfo.Summary + data.PaymentInfo.Shipping)
                        },
                        new PricingInfoItem()
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingTax"),
                            Value = String.Format("$ {0:#,0.00}",data.PaymentInfo.Tax)
                        },
                        new PricingInfoItem()
                        {
                            Title = resources.GetResourceString("Kadena.Order.PricingTotals"),
                            Value = String.Format("$ {0:#,0.00}",data.PaymentInfo.Summary + data.PaymentInfo.Shipping + data.PaymentInfo.Tax)
                        }
                    }
                },
            };

            var mailingTypeCode = OrderItemTypeDTO.Mailing.ToString();
            var hasOnlyMailingListProducts = data.Items?.All(item => item.Type == mailingTypeCode) ?? false;
            if (hasOnlyMailingListProducts)
            {
                orderDetail.ShippingInfo = new ShippingInfo
                {
                    Title = resources.GetResourceString("Kadena.Order.ShippingSection"),
                    Message = resources.GetResourceString("Kadena.Checkout.UndeliverableText")
                };
            }
            else
            {
                orderDetail.ShippingInfo = new ShippingInfo
                {
                    Title = resources.GetResourceString("Kadena.Order.ShippingSection"),
                    DeliveryMethod = shoppingCart.GetShippingProviderIcon(data.ShippingInfo.Provider),
                    Address = mapper.Map<DeliveryAddress>(data.ShippingInfo.AddressTo),
                };
                orderDetail.ShippingInfo.Address.Country = localization
                    .GetCountries()
                    .FirstOrDefault(s => s.Code.Equals(data.ShippingInfo.AddressTo.isoCountryCode));
            }

            orderDetail.OrderedItems = await ProcessOrderedItems(data.Items, data.Id, mailingTypeCode);
            orderDetail.OrderedItems.OrderItemsByLineNumber();

            if (!permissions.UserCanSeePrices())
            {
                orderDetail.HidePrices();
            }

            return orderDetail;
        }

        private async Task<OrderedItems> ProcessOrderedItems(List<Dto.ViewOrder.MicroserviceResponses.OrderItemDTO> responseItems, string id, string mailingTypeCode)
        {
            // create base instance
            var orderedItems = CreateOrderedItems();

            // map items
            var items = await MapOrderedItems(responseItems, id);

            // filter out mailing items and process them
            items = items.Where(item => !ProcessMailingItem(item, orderedItems, mailingTypeCode)).ToList();

            // expand items to match each TrackingInfo object
            var expandedItems = ExpandItems(items);

            // sort expanded items to open || shipped sections and tracking groups
            expandedItems.GroupBy(x => x.Item2?.Id).ToList().ForEach(group =>
            {
                if(group.Key == null)
                    // open
                    ProcessOpenItems(orderedItems, group);
                else
                    // shipped
                    ProcessShippedItemsGroup(orderedItems, group);
            });

            return orderedItems;
        }

        private List<(OrderedItem, TrackingInfo)> ExpandItems(List<OrderedItem> items)
        {
            var expandedItems = new List<(OrderedItem, TrackingInfo)>();
            items.ForEach(item =>
            {
                if (item.Tracking == null || item.Tracking.Count() == 0)
                {
                    var itemClone = mapper.Map<OrderedItem>(item);

                    expandedItems.Add((itemClone, null));
                    return;
                }

                if (item.QuantityShipped < item.Quantity)
                {
                    var itemClone = mapper.Map<OrderedItem>(item);
                    itemClone.Quantity = itemClone.Quantity - itemClone.QuantityShipped;
                    itemClone.QuantityShipped = 0;
                    itemClone.Tracking = null;

                    expandedItems.Add((itemClone, null));
                }

                foreach (var trackingInfo in item.Tracking)
                {
                    var itemClone = mapper.Map<OrderedItem>(item);
                    itemClone.QuantityShipped = trackingInfo.QuantityShipped;
                    itemClone.Tracking = null;

                    expandedItems.Add((itemClone, trackingInfo));
                }
            });

            return expandedItems;
        }

        private OrderedItems CreateOrderedItems()
        {
            var orderedItems = new OrderedItems
            {
                ShippedItems = new OrderedItemsSection
                {
                    Title = resources.GetResourceString("Kadena.Order.OrderedItemsShippedSection"),
                    Items = new List<OrderedItemsGroup>()
                },
                OpenItems = new OrderedItemsSection()
                {
                    Title = resources.GetResourceString("Kadena.Order.OrderedItemsOpenSection"),
                    Items = new List<OrderedItemsGroup>()
                },
                MailingItems = new OrderedItemsSection()
                {
                    Title = resources.GetResourceString("Kadena.Order.OrderedItemsMailingSection"),
                    Items = new List<OrderedItemsGroup>()
                }
            };

            return orderedItems;
        }

        private void ProcessShippedItemsGroup(OrderedItems orderedItems, IGrouping<string, (OrderedItem, TrackingInfo)> group)
        {
            var first = group.FirstOrDefault();
            orderedItems.ShippedItems.Items.Add(new OrderedItemsGroup
            {
                Tracking = new OrderedItemsGroupTracking
                {
                    Prefix = resources.GetResourceString("Kadena.Order.OrderedItems.TrackingPrefix"),
                    Id = first.Item2?.Id,
                    Url = first.Item2?.Url
                },
                ShippingDate = new OrderedItemsGroupShippingDate
                {
                    Prefix = resources.GetResourceString("Kadena.Order.OrderedItems.ShippingDatePrefix"),
                    Date = first.Item2.ShippingDate
                },
                Orders = group.Select(x => x.Item1).ToList()
            });
        }

        private void ProcessOpenItems(OrderedItems orderedItems, IGrouping<string, (OrderedItem, TrackingInfo)> group)
        {
            if (orderedItems.OpenItems.Items.Count == 0)
            {
                orderedItems.OpenItems.Items.Add(new OrderedItemsGroup
                {
                    Tracking = new OrderedItemsGroupTracking
                    {
                        Prefix = resources.GetResourceString("Kadena.Order.OrderedItems.TrackingPrefix"),
                        Id = resources.GetResourceString("Kadena.Order.OrderedItems.TrackingNotFound"),
                        Url = null
                    },
                    ShippingDate = new OrderedItemsGroupShippingDate
                    {
                        Prefix = resources.GetResourceString("Kadena.Order.OrderedItems.ShippingDatePrefix"),
                        Date = resources.GetResourceString("Kadena.Order.OrderedItems.ShippingDateNotFound")
                    },
                    Orders = new List<OrderedItem>()
                });
            }

            group.Select(x => x.Item1).ToList().ForEach(item =>
            {
                orderedItems.OpenItems.Items[0].Orders.Add(item);
            });
        }

        private bool ProcessMailingItem(OrderedItem item, OrderedItems orderedItems, string mailingTypeCode)
        {
            if (item.Type == mailingTypeCode)
            {
                // mailling items have always only one group
                if (orderedItems.MailingItems.Items.Count == 0)
                {
                    orderedItems.MailingItems.Items.Add(new OrderedItemsGroup
                    {
                        Orders = new List<OrderedItem>()
                    });
                }

                orderedItems.MailingItems.Items[0].Orders.Add(item);

                return true;
            }

            return false;
        }

        private bool IsCurrentUserApproverFor(Customer customer)
        {
            var currentUserId = kenticoCustomers.GetCurrentCustomer().UserID;
            return currentUserId == customer.ApproverUserId;
        }

        private string GetPdfUrl(string orderId, Dto.ViewOrder.MicroserviceResponses.OrderItemDTO orderItem, Product orderedProduct)
        {
            if (orderItem.Type.Contains(OrderItemTypeDTO.TemplatedProduct.ToString()) ||
                orderItem.Type.Contains(OrderItemTypeDTO.Mailing.ToString()))
            {
                if (orderedProduct == null)
                {
                    kenticoLog.LogError("GetPdfUrl", $"Couldn't find product for item line {orderItem.LineNumber} from order {orderId}");
                    return string.Empty;
                }

                if (orderedProduct.HiResPdfDownloadEnabled)
                {
                    return pdfService.GetHiresPdfUrl(orderId, orderItem.LineNumber);
                }

                return pdfService.GetLowresPdfUrl(orderItem.TemplateId, orderedProduct.TemplateLowResSettingId);
            }

            return string.Empty;
        }

        private async Task<List<OrderedItem>> MapOrderedItems(List<Dto.ViewOrder.MicroserviceResponses.OrderItemDTO> items, string orderId)
        {
            if (items == null)
            {
                return new List<OrderedItem>();
            }

            var orderedItems = items
                .Where(i => i.Quantity > 0)
                .Select(i =>
                {
                    var oi = mapper.Map<OrderedItem>(i);

                    var templatedProduct = i.TemplateId != Guid.Empty ? products.GetProductBySkuId(i.SkuId) : null;
                    var previewUrl = UrlHelper.GetUrlForTemplatePreview(i.TemplateId, templatedProduct?.TemplateLowResSettingId ?? Guid.Empty);
                    var previewAbsoluteUrl = site.GetAbsoluteUrl(previewUrl);

                    oi.Image = imageService.GetThumbnailLink(products.GetSkuImageUrl(i.SkuId));
                    oi.DownloadPdfURL = GetPdfUrl(orderId, i, templatedProduct);
                    oi.UnitOfMeasure = units.GetDisplaynameByCode(oi.UnitOfMeasure);
                    oi.QuantityPrefix = (i.Type ?? string.Empty).Contains("Mailing") ?
                        resources.GetResourceString("Kadena.Order.QuantityPrefixAddresses")
                        : resources.GetResourceString("Kadena.Order.QuantityPrefix");
                    oi.QuantityShippedPrefix = resources.GetResourceString("Kadena.Order.QuantityShippedPrefix");
                    oi.MailingListPrefix = resources.GetResourceString("Kadena.Order.MailingListPrefix");
                    oi.ShippingDatePrefix = resources.GetResourceString("Kadena.Order.ItemShippingDatePrefix");
                    oi.TemplatePrefix = resources.GetResourceString("Kadena.Order.TemplatePrefix");
                    oi.TrackingPrefix = resources.GetResourceString("Kadena.Order.TrackingIdPrefix");
                    oi.ProductStatusPrefix = resources.GetResourceString("Kadena.Order.ProductStatusPrefix");
                    oi.ProductStatus = products.GetProductStatus(i.SkuId);
                    oi.Preview = new Button
                    {
                        Exists = templatedProduct != null,
                        Text = resources.GetResourceString("Kadena.Checkout.PreviewButton"),
                        Url = previewAbsoluteUrl
                    };
                    oi.EmailProof = new Button
                    {
                        Exists = templatedProduct != null,
                        Text = resources.GetResourceString("Kadena.EmailProof.ButtonLabel"),
                        Url = GetPdfUrl(orderId, i, templatedProduct)
                    };
                    if (i.Attributes != null)
                    {
                        oi.Options = i.Attributes.Select(a => new ItemOption { Name = products.GetOptionCategory(a.Key)?.DisplayName ?? a.Key, Value = a.Value });
                    }

                    return oi;
                }).ToList();

            await SetMailingListNames(orderedItems);

            return orderedItems;
        }

        private async Task SetMailingListNames(List<OrderedItem> orderedItems)
        {
            var mailingResponse = await mailingClient.GetMailingListsForCustomer();

            if (mailingResponse == null || mailingResponse.Success == false || mailingResponse.Payload == null)
            {
                kenticoLog.LogError("MailingList client", $"Call to microservice failed. {mailingResponse?.ErrorMessages}");
                return;
            }

            var mailingLists = mailingResponse.Payload;
            var itemsWithMailing = orderedItems.Where(i => !string.IsNullOrWhiteSpace(i.MailingList) && i.MailingList != Guid.Empty.ToString());

            foreach (var item in itemsWithMailing)
            {
                var matchingList = mailingLists.FirstOrDefault(m => m.Id == item.MailingList);

                if (matchingList != null)
                {
                    item.MailingList = matchingList.Name;
                }
            }
        }

        private void CheckOrderDetailPermisson(OrderNumber orderId, Customer customer, bool canCurrentUserApproveOrder)
        {
            if (orderId == null)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            // Allow admin who has set permission to see all orders in Kentico
            // or Allow orders belonging to currently logged Customer
            var isAdmin = permissions.UserCanSeeAllOrders();
            var isOrderOwner = (orderId.CustomerId == customer.Id);

            var canViewOrder = isAdmin || isOrderOwner || canCurrentUserApproveOrder;
            if (!canViewOrder)
            {
                throw new SecurityException("Permission denied");
            }
        }

        private string GetPaymentMethodIcon(string paymentMethod)
        {
            var methods = shoppingCart.GetPaymentMethods();
            var method = methods.FirstOrDefault(m => m.Title == paymentMethod);
            return method?.Icon ?? string.Empty;
        }
    }
}