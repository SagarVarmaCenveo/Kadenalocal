﻿using CMS.DataEngine;
using CMS.DocumentEngine.Types.KDA;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.Scheduler;
using CMS.SiteProvider;
using Kadena.Dto.EstimateDeliveryPrice.MicroserviceRequests;
using Kadena.Dto.General;
using Kadena.Dto.SubmitOrder.MicroserviceRequests;
using Kadena.Old_App_Code.Kadena.Constants;
using Kadena.Old_App_Code.Kadena.EmailNotifications;
using Kadena.Old_App_Code.Kadena.Enums;
using Kadena.Old_App_Code.Kadena.PDFHelpers;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena.Container.Default;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.WebAPI.KenticoProviders.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Kadena.Models.SiteSettings;
using Kadena2.BusinessLogic.Contracts.Orders;
using Kadena.BusinessLogic.Contracts.Orders;
using Kadena.Models.CampaignData;
using Kadena.Models.ShoppingCarts;
using Kadena.BusinessLogic.Contracts;
using AutoMapper;
using Kadena.Models;
using Kadena.Models.Checkout;

namespace Kadena.Old_App_Code.Kadena.Shoppingcart
{
    public class ShoppingCartHelper
    {
        private static ShoppingCartInfo Cart { get; set; }
        private static readonly IMapper mapper = DIContainer.Resolve<IMapper>();
        private static readonly IGetOrderDataService getOrderData = DIContainer.Resolve<IGetOrderDataService>();
        private static readonly IDeliveryEstimationDataService estimationData = DIContainer.Resolve<IDeliveryEstimationDataService>();
        private static readonly ITaxEstimationService taxEstimationService = DIContainer.Resolve<ITaxEstimationService>();
        private static readonly IShoppingCartProvider shoppingCartProvider = DIContainer.Resolve<IShoppingCartProvider>();
        /// <summary>
        /// Returns order dto
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static OrderDTO CreateOrdersDTO(ShoppingCartInfo cart, string orderType, decimal shippingCost)
        {
            try
            {
                var shoppingCart = shoppingCartProvider.GetShoppingCart(cart.ShoppingCartID);
                Cart = cart;
                var billingAddress = GetBillingAddress();
                var taxAddress = mapper.Map<DeliveryAddress>(billingAddress);
                shoppingCart.TotalTax = taxEstimationService.EstimateTax(taxAddress, shoppingCart.TotalPrice, shippingCost).Result;
                return new OrderDTO
                {
                    Type = orderType,
                    Campaign = GetCampaign(),
                    BillingAddress = billingAddress,
                    ShippingAddressSource = DIContainer.Resolve<IGetOrderDataService>().GetSourceAddressForDeliveryEstimation(),
                    ShippingAddressDestination = billingAddress,
                    ShippingOption = ShippingOption(),
                    Customer = GetCustomer(),
                    Site = GetSite(),
                    OrderStatus = new OrderStatusDTO()
                    {
                        KenticoOrderStatusID = DIContainer.Resolve<IKenticoOrderProvider>().GetOrderStatusId("Pending"),
                        OrderStatusName = "PENDING"
                    },
                    PaymentOption = new PaymentOptionDTO()
                    {
                        PaymentOptionName = "NoPaymentRequired",
                        PaymentGatewayCustomerCode = string.Empty,
                        PONumber = string.Empty,
                        TokenId = string.Empty,
                        TransactionKey = string.Empty
                    },
                    NotificationsData = GetNotification(),
                    Items = GetCartItems(cart.CartItems, shoppingCart.Items),
                    OrderDate = DateTime.Now,
                    Totals = new TotalsDto
                    {
                        Price = shoppingCart.TotalPrice,
                        Shipping = shippingCost,
                        Tax = shoppingCart.TotalTax,
                        PricedItemsTax = shoppingCart.PricedItemsTax
                    },
                    OrderCurrency = GetCurrencyDTO(Cart.Currency),
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "CreateOrdersDTO", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns user Cart IDs based on product type
        /// </summary>
        /// <returns></returns>
        public static List<int> GetCartsByUserID(int userID, ProductType type, int? campaignID)
        {
            try
            {
                return CartPDFHelper.GetLoggedInUserCartData(Convert.ToInt32(type), userID, campaignID).AsEnumerable().Select(x => x.Field<int>("ShoppingCartID")).Distinct().ToList();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "CreateOrdersDTO", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Calling shipping order submission service
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public static BaseResponseDto<string> CallOrderService(OrderDTO requestBody)
        {
            try
            {
                var microserviceClient = DIContainer.Resolve<IOrderSubmitClient>();
                var response = microserviceClient.SubmitOrder(requestBody).Result;

                if (!response.Success || response.Payload == null)
                {
                    EventLogProvider.LogInformation("DeliveryPriceEstimationClient", "ERROR", $"Call from to service resulted with error {response.Error?.Message ?? string.Empty}");
                }
                return response;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "CallOrderService", ex.Message);
                return null;
            }
        }
        /// <summary>
        ///Processes order and returns response
        /// </summary>
        /// <returns></returns>
        public static BaseResponseDto<string> ProcessOrder(ShoppingCartInfo cart, int userID, string orderType, OrderDTO ordersDTO, decimal shippingCost = default(decimal))
        {
            try
            {
                if (ordersDTO != null && ordersDTO.Campaign != null)
                {
                    UpdateDistributorsBusinessUnit(ordersDTO.Campaign.DistributorID);
                }
                var response = CallOrderService(ordersDTO);
                return response;
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("Kadena_CMSWebParts_Kadena_Cart_CartCheckout", "ProcessOrder", ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Updates business unit for distributor
        /// </summary>
        /// <param name="distributorID">Distributor ID</param>
        public static void UpdateDistributorsBusinessUnit(int distributorID)
        {
            AddressInfo distributor = AddressInfoProvider.GetAddressInfo(distributorID);
            long businessUnitNumber = ValidationHelper.GetLong(Cart.GetValue("BusinessUnitIDForDistributor"), default(long));
            if (distributor != null && businessUnitNumber != default(long))
            {
                distributor.SetValue("BusinessUnit", businessUnitNumber);
                AddressInfoProvider.SetAddressInfo(distributor);
            }
        }

        /// <summary>
        /// Gets target shipping address
        /// </summary>
        /// <returns></returns>
        private static AddressDto GetTargetAddress()
        {
            try
            {
                var distributorID = Cart.GetIntegerValue("ShoppingCartDistributorID", default(int));
                var distributorAddress = AddressInfoProvider.GetAddresses().WhereEquals("AddressID", distributorID).FirstOrDefault();
                var addressLines = new[]{
                                            distributorAddress.GetStringValue("AddressLine1",string.Empty),
                                            distributorAddress.GetStringValue("AddressLine2",string.Empty)
                                        }.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                var country = CountryInfoProvider.GetCountries().WhereEquals("CountryID", distributorAddress.GetStringValue("AddressCountryID", string.Empty))
                                    .Column("CountryTwoLetterCode").FirstOrDefault();
                var state = StateInfoProvider.GetStates().WhereEquals("StateID", distributorAddress.GetStringValue("AddressStateID", string.Empty)).Column("StateCode").FirstOrDefault();
                return new AddressDto()
                {
                    City = distributorAddress.GetStringValue("AddressCity", string.Empty),
                    Country = country?.CountryTwoLetterCode,
                    Postal = distributorAddress.GetStringValue("AddressZip", string.Empty),
                    State = state?.StateCode,
                    StreetLines = addressLines
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetTargetAddress", ex.Message);
                return null;
            }
        }



        /// <summary>
        /// Returns campaign details
        /// </summary>
        /// <returns></returns>
        private static CampaignDTO GetCampaign()
        {
            try
            {
                return new CampaignDTO
                {
                    ID = Cart.GetValue("ShoppingCartCampaignID", default(int)),
                    ProgramID = Cart.GetValue("ShoppingCartProgramID", default(int)),
                    DistributorID = Cart.GetIntegerValue("ShoppingCartDistributorID", 0),
                    BusinessUnitNumber = Cart.GetStringValue("BusinessUnitIDForDistributor", string.Empty)
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetCampaign", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets target shipping address
        /// </summary>
        /// <returns></returns>
        private static AddressDTO GetBillingAddress()
        {
            try
            {
                var distributorID = Cart.GetIntegerValue("ShoppingCartDistributorID", default(int));
                var distributorAddress = AddressInfoProvider.GetAddresses().WhereEquals("AddressID", distributorID).FirstOrDefault();
                var country = CountryInfoProvider.GetCountries().WhereEquals("CountryID", distributorAddress.GetStringValue("AddressCountryID", string.Empty)).FirstOrDefault();
                var state = StateInfoProvider.GetStates().WhereEquals("StateID", distributorAddress.GetStringValue("AddressStateID", string.Empty)).FirstOrDefault();
                return new AddressDTO()
                {
                    KenticoAddressID = distributorAddress.AddressID,
                    AddressLine1 = distributorAddress.AddressLine1,
                    AddressLine2 = distributorAddress.AddressLine2,
                    City = distributorAddress.AddressCity,
                    State = state.StateCode,
                    StateDisplayName = state.StateDisplayName,
                    Zip = distributorAddress.GetStringValue("AddressZip", string.Empty),
                    KenticoCountryID = distributorAddress.AddressCountryID,
                    Country = country.CountryName,
                    isoCountryCode = country.CountryTwoLetterCode,
                    KenticoStateID = distributorAddress.AddressStateID,
                    AddressPersonalName = distributorAddress.AddressPersonalName,
                    AddressCompanyName = distributorAddress.GetStringValue("CompanyName", string.Empty),
                    Phone = distributorAddress.AddressPhone,
                    Email = distributorAddress.GetStringValue("Email", string.Empty)
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetBillingAddress", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns shipping details
        /// </summary>
        /// <returns></returns>
        private static ShippingOptionDTO ShippingOption()
        {
            try
            {
                var carrier = CarrierInfoProvider.GetCarrierInfo(Cart.ShippingOption.ShippingOptionCarrierID);
                return new ShippingOptionDTO
                {
                    KenticoShippingOptionID = Cart.ShoppingCartShippingOptionID,
                    ShippingService = Cart.ShippingOption.ShippingOptionName.ToLower().Equals(Models.Shipping.ShippingOption.Ground.ToLower())
                        ? Models.Shipping.ShippingOption.Ground
                        : Cart.ShippingOption.ShippingOptionCarrierServiceName,
                    ShippingCompany = carrier != null ? carrier.CarrierName : Cart.ShippingOption.ShippingOptionName,
                    CarrierCode = Cart.ShippingOption.GetStringValue("ShippingOptionSAPName", string.Empty)
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "ShippingOption", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns customer details
        /// </summary>
        /// <returns></returns>
        private static CustomerDTO GetCustomer()
        {
            try
            {
                var settingKeyValue = DIContainer.Resolve<IKenticoResourceService>().GetSiteSettingsKey(Settings.KDA_SoldToGeneralInventory);
                var distributorID = Cart.GetIntegerValue("ShoppingCartDistributorID", default(int));
                var distributorAddress = AddressInfoProvider.GetAddresses().WhereEquals("AddressID", distributorID).FirstOrDefault();
                var customer = CustomerInfoProvider.GetCustomerInfo(distributorAddress.AddressCustomerID);
                return new CustomerDTO
                {
                    FirstName = customer.CustomerFirstName,
                    LastName = customer.CustomerLastName,
                    KenticoCustomerID = customer.CustomerID,
                    Email = customer.CustomerEmail,
                    CustomerNumber = settingKeyValue,
                    KenticoUserID = customer.CustomerUserID,
                    Phone = customer.CustomerPhone
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetCustomer", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns site details
        /// </summary>
        /// <returns></returns>
        private static SiteDTO GetSite()
        {
            var settingKeyValue = DIContainer.Resolve<IKenticoResourceService>().GetCustomerErpId();
            return new SiteDTO
            {
                KenticoSiteID = SiteContext.CurrentSiteID,
                KenticoSiteName = SiteContext.CurrentSiteName,
                ErpCustomerId = settingKeyValue
            };
        }

        /// <summary>
        /// Returns notification detaills
        /// </summary>
        /// <returns></returns>
        private static List<NotificationInfoDto> GetNotification()
        {
            try
            {
                return new List<NotificationInfoDto> {
                                        new NotificationInfoDto {
                                                Email=Cart.Customer.CustomerEmail,
                                                Language=LocalizationContext.CurrentCulture.CultureCode
                                        }
                           };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetNotification", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns Shopping cart Items
        /// </summary>
        /// <returns></returns>
        private static List<OrderItemDTO> GetCartItems(IEnumerable<ShoppingCartItemInfo> items, IEnumerable<CartItemEntity> cartItems)
        {
            var uomProvider = DIContainer.Resolve<IKenticoUnitOfMeasureProvider>();

            try
            {
                return items.GroupJoin(cartItems, i => i.SKUID, ci => ci.SKUID, (item, ci) =>
                    {
                        var cartItem = ci.DefaultIfEmpty().First();
                        var uom = item.SKU.GetStringValue("SKUUnitOfMeasure", string.Empty);
                        if (string.IsNullOrEmpty(uom))
                        {
                            uom = SKUUnitOfMeasure.Default;
                        }

                        return new OrderItemDTO
                        {
                            SKU = new SKUDTO
                            {
                                KenticoSKUID = cartItem?.SKUID ?? item.SKUID,
                                Name = item.SKU.SKUName,
                                SKUNumber = item.SKU.SKUNumber
                            },
                            UnitCount = cartItem?.Quantity ?? item.CartItemUnits,
                            UnitOfMeasure = uomProvider.GetUnitOfMeasure(uom).ErpCode,
                            RequiresApproval = (item.VariantParent ?? item.SKU).GetBooleanValue("SKUApprovalRequired", false),
                            UnitPrice = cartItem?.UnitPrice ?? ValidationHelper.GetDecimal(item.UnitPrice, default(decimal)),
                            TotalPrice = cartItem?.TotalPrice ?? ValidationHelper.GetDecimal(item.TotalPrice, default(decimal)),
                            DocumentId = item.GetIntegerValue("ProductPageID", 0)
                        };
                    })
                .ToList();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetCartItems", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns order total
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <returns></returns>
        private static CurrencyDTO GetCurrencyDTO(CurrencyInfo currency)
        {
            try
            {
                return new CurrencyDTO
                {
                    KenticoCurrencyID = currency.CurrencyID,
                    CurrencyCode = currency.CurrencyCode
                };
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetOrderTotal", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// returns Shipping total
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <returns></returns>
        public static decimal GetOrderShippingTotal(ShoppingCartInfo cart)
        {
            try
            {
                Cart = cart;
                var weight = shoppingCartProvider.GetCartWeight(cart.ShoppingCartID);
                return estimationData.GetShippingCost(CarrierInfoProvider.GetCarrierInfo(Cart.ShippingOption.ShippingOptionCarrierID).CarrierName,
                     Cart.ShippingOption.ShippingOptionName, weight, GetTargetAddress());
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetOrderTotal", ex.Message);
                return decimal.Zero;
            }
        }
        /// <summary>
        /// Closing the campaign
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ProcessOrders(int campaignID, int userID)
        {
            try
            {
                var campaign = CampaignProvider.GetCampaigns()
                    .WhereEquals("NodeSiteID", SiteContext.CurrentSiteID)
                    .WhereEquals("CampaignID", campaignID)
                    .FirstObject;
                if (campaign != null)
                {
                    var _failedOrders = DIContainer.Resolve<IFailedOrderStatusProvider>();
                    _failedOrders.UpdateCampaignOrderStatus(campaign.CampaignID);
                    TaskInfo runTask = TaskInfoProvider.GetTaskInfo(ScheduledTaskNames.PrebuyOrderCreation, SiteContext.CurrentSiteID);
                    if (runTask != null)
                    {
                        runTask.TaskRunInSeparateThread = true;
                        runTask.TaskEnabled = true;
                        runTask.TaskData = $"{campaign.CampaignID}|{userID}";
                        SchedulingExecutor.ExecuteTask(runTask);
                    }
                    var users = UserInfoProvider.GetUsers();
                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            ProductEmailNotifications.CampaignEmail(campaign.DocumentName, user.Email, "CampaignCloseEmail", campaignURL: campaign.AbsoluteURL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Kadena_CMSWebParts_Kadena_Cart_FailedOrdersCheckout", "ProcessOrders", ex, SiteContext.CurrentSiteID, ex.Message);
            }
        }
        /// <summary>
        /// update available sku quantity
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <returns></returns>
        public static void UpdateAvailableSKUQuantity(ShoppingCartInfo cart)
        {
            try
            {
                var product = DIContainer.Resolve<IKenticoSkuProvider>();
                cart.CartItems.ForEach(cartItem =>
                {
                    product.UpdateAvailableQuantity(cartItem.SKUID, -cartItem.CartItemUnits);
                });
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "UpdateAvailableSKUQuantity", ex.Message);
            }
        }
        /// <summary>
        /// update available sku quantity
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <returns></returns>
        public static void UpdateAllocatedProductQuantity(ShoppingCartInfo cart, int userID)
        {
            try
            {
                var productProvider = DIContainer.Resolve<IKenticoProductsProvider>();
                cart.CartItems.ForEach(cartItem =>
                {
                    productProvider.UpdateAllocatedProductQuantityForUser(cartItem.SKUID, userID, cartItem.CartItemUnits);
                });
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "UpdateAvailableSKUQuantity", ex.Message);
            }
        }

        /// <summary>
        /// returns open campaign
        /// </summary>
        /// <param name="inventoryType"></param>
        /// <returns></returns>
        public static CMS.DocumentEngine.Types.KDA.Campaign GetOpenCampaign()
        {
            try
            {
                return CampaignProvider.GetCampaigns().Columns("CampaignID,Name,StartDate,EndDate")
                                    .WhereEquals("OpenCampaign", true)
                                    .Where(new WhereCondition().WhereEquals("CloseCampaign", false).Or()
                                    .WhereEquals("CloseCampaign", null))
                                    .WhereEquals("NodeSiteID", SiteContext.CurrentSiteID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "GetOrderTotal", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Updates remaining budget for every order placed.
        /// </summary>
        /// <param name="orderDetails"></param>
        /// <returns></returns>
        public static void UpdateRemainingBudget(OrderDTO orderDetails, int userID)
        {
            try
            {
                var campaignFiscalYear = DIContainer.Resolve<IKenticoCampaignsProvider>().GetCampaignFiscalYear(orderDetails.Campaign.ID);
                var totalToBeDeducted = -(orderDetails.Totals.Price + orderDetails.Totals.Shipping ?? 0);
                var fiscalYear = orderDetails.Type == OrderType.generalInventory ?
                                 ValidationHelper.GetString(orderDetails.OrderDate.Year, string.Empty) :
                                 orderDetails.Type == OrderType.prebuy ? campaignFiscalYear : string.Empty;
                DIContainer.Resolve<IKenticoUserBudgetProvider>().AdjustUserRemainingBudget(fiscalYear, userID, totalToBeDeducted);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogInformation("ShoppingCartHelper", "UpdateRemainingBudget", ex.Message);
            }
        }
    }
}