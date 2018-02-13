﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Kadena.BusinessLogic.Contracts;
using Kadena.Models.Checkout;
using Kadena.Models;
using Kadena.WebAPI.KenticoProviders.Contracts;
using Kadena.Models.Product;
using Kadena.BusinessLogic.Factories.Checkout;
using Kadena2.WebAPI.KenticoProviders.Contracts;
using System.Collections.Generic;
using CMS.DataEngine;
using System.Data;

namespace Kadena.BusinessLogic.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IKenticoSiteProvider kenticoSite;
        private readonly IKenticoLocalizationProvider localization;
        private readonly IKenticoPermissionsProvider permissions;
        private readonly IKenticoUserProvider kenticoUsers;
        private readonly IKenticoResourceService resources;
        private readonly ITaxEstimationService taxCalculator;
        private readonly IKListService mailingService;
        private readonly IShoppingCartProvider shoppingCart;
        private readonly ICheckoutPageFactory checkoutfactory;
        public readonly string loggedInUserCartData = "Ecommerce.Shoppingcart.LoggedInUserCartData";


        public ShoppingCartService(IKenticoSiteProvider kenticoSite,
                                   IKenticoLocalizationProvider localization,
                                   IKenticoPermissionsProvider permissions,
                                   IKenticoUserProvider kenticoUsers,
                                   IKenticoResourceService resources,
                                   ITaxEstimationService taxCalculator,
                                   IKListService mailingService,
                                   IShoppingCartProvider shoppingCart,
                                   ICheckoutPageFactory checkoutfactory)
        {
            if (kenticoSite == null)
            {
                throw new ArgumentNullException(nameof(kenticoSite));
            }
            if (localization == null)
            {
                throw new ArgumentNullException(nameof(localization));
            }
            if (permissions == null)
            {
                throw new ArgumentNullException(nameof(permissions));
            }
            if (kenticoUsers == null)
            {
                throw new ArgumentNullException(nameof(kenticoUsers));
            }
            if (resources == null)
            {
                throw new ArgumentNullException(nameof(resources));
            }
            if (taxCalculator == null)
            {
                throw new ArgumentNullException(nameof(taxCalculator));
            }
            if (mailingService == null)
            {
                throw new ArgumentNullException(nameof(mailingService));
            }
            if (shoppingCart == null)
            {
                throw new ArgumentNullException(nameof(shoppingCart));
            }
            if (checkoutfactory == null)
            {
                throw new ArgumentNullException(nameof(checkoutfactory));
            }

            this.kenticoSite = kenticoSite;
            this.localization = localization;
            this.permissions = permissions;
            this.kenticoUsers = kenticoUsers;
            this.resources = resources;
            this.taxCalculator = taxCalculator;
            this.mailingService = mailingService;
            this.shoppingCart = shoppingCart;
            this.checkoutfactory = checkoutfactory;
        }

        public CheckoutPage GetCheckoutPage()
        {
            var addresses = kenticoUsers.GetCustomerAddresses(AddressType.Shipping);
            var paymentMethods = shoppingCart.GetPaymentMethods();
            var cartItems = shoppingCart.GetShoppingCartItems();
            var cartItemsTotals = shoppingCart.GetShoppingCartTotals();
            var countOfItemsString = cartItems.Length == 1 ? resources.GetResourceString("Kadena.Checkout.ItemSingular") : resources.GetResourceString("Kadena.Checkout.ItemPlural");
            var userNotificationString = GetUserNotificationString();
            var otherAddressEnabled = GetOtherAddressSettingsValue();
            var emailConfirmationEnabled = resources.GetSettingsKey("KDA_UseNotificationEmailsOnCheckout") == bool.TrueString;

            var checkoutPage = new CheckoutPage()
            {
                EmptyCart = checkoutfactory.CreateCartEmptyInfo(cartItems),
                Products = checkoutfactory.CreateProducts(cartItems, cartItemsTotals, countOfItemsString),
                DeliveryAddresses = checkoutfactory.CreateDeliveryAddresses(addresses.ToList(), userNotificationString, otherAddressEnabled),
                PaymentMethods = checkoutfactory.CreatePaymentMethods(paymentMethods),
                Submit = checkoutfactory.CreateSubmitButton(),
                ValidationMessage = resources.GetResourceString("Kadena.Checkout.ValidationError"),
                EmailConfirmation = checkoutfactory.CreateNotificationEmail(emailConfirmationEnabled)
            };

            CheckCurrentOrDefaultAddress(checkoutPage);
            checkoutPage.PaymentMethods.CheckDefault();
            checkoutPage.PaymentMethods.CheckPayability();
            checkoutPage.SetDisplayType();
            SetPricesVisibility(checkoutPage);
            return checkoutPage;
        }
        
        public async Task<CheckoutPageDeliveryTotals> GetDeliveryAndTotals()
        {
            var deliveryAddress = shoppingCart.GetCurrentCartShippingAddress();

            var isShippingApplicable = shoppingCart.GetShoppingCartItems()
                .Any(item => !item.IsMailingList);
            if (!isShippingApplicable)
            {
                UnsetShipping();
            }

            var result = new CheckoutPageDeliveryTotals()
            {
                DeliveryMethods = GetDeliveryMethods(isShippingApplicable),
                Totals = new Totals()
                {
                    Title = string.Empty,
                    Description = null // resources.GetResourceString("Kadena.Checkout.Totals.Description"), if needed
                }
            };

            if (permissions.UserCanSeePrices())
            {
                await UpdateTotals(result, deliveryAddress);
            }

            SetPricesVisibility(result);
            return result;
        }

        public async Task<CheckoutPageDeliveryTotals> SetDeliveryAddress(DeliveryAddress deliveryAddress)
        {
            shoppingCart.SetShoppingCartAddress(deliveryAddress);
            return await GetDeliveryAndTotals();
        }

        private DeliveryCarriers GetDeliveryMethods(bool isShippingApplicable)
        {
            if (!isShippingApplicable)
            {
                var defaultDeliveryMethods = new DeliveryCarriers();
                return defaultDeliveryMethods;
            }

            var carriers = shoppingCart.GetShippingCarriers();
            var deliveryMethods = new DeliveryCarriers()
            {
                Title = resources.GetResourceString("Kadena.Checkout.Delivery.Title"),
                Description = resources.GetResourceString("Kadena.Checkout.DeliveryMethodDescription"),
                items = carriers.ToList()
            };

            deliveryMethods.RemoveCarriersWithoutOptions();

            CheckCurrentOrDefaultShipping(deliveryMethods);

            deliveryMethods.UpdateSummaryText(
                resources.GetResourceString("Kadena.Checkout.ShippingPriceFrom"),
                resources.GetResourceString("Kadena.Checkout.ShippingPrice"),
                resources.GetResourceString("Kadena.Checkout.CannotBeDelivered"),
                resources.GetResourceString("Kadena.Checkout.CustomerPrice")
            );

            return deliveryMethods;
        }

        private async Task UpdateTotals(CheckoutPageDeliveryTotals page, DeliveryAddress deliveryAddress)
        {
            var totals = page.Totals;
            totals.Title = resources.GetResourceString("Kadena.Checkout.Totals.Title");
            var shoppingCartTotals = shoppingCart.GetShoppingCartTotals();
            shoppingCartTotals.TotalTax = await taxCalculator.EstimateTotalTax(deliveryAddress);
            totals.Items = new Total[]
            {
                new Total()
                {
                    Title = resources.GetResourceString("Kadena.Checkout.Totals.Summary"),
                    Value = String.Format("$ {0:#,0.00}", shoppingCartTotals.TotalItemsPrice)
                },
                new Total()
                {
                    Title = resources.GetResourceString("Kadena.Checkout.Totals.Shipping"),
                    Value = String.Format("$ {0:#,0.00}", shoppingCartTotals.TotalShipping)
                },
                new Total()
                {
                    Title = resources.GetResourceString("Kadena.Checkout.Totals.Subtotal"),
                    Value = String.Format("$ {0:#,0.00}", shoppingCartTotals.Subtotal)
                },
                new Total()
                {
                    Title = resources.GetResourceString("Kadena.Checkout.Totals.Tax"),
                    Value = String.Format("$ {0:#,0.00}", shoppingCartTotals.TotalTax)
                },
                new Total()
                {
                    Title = resources.GetResourceString("Kadena.Checkout.Totals.Totals"),
                    Value = String.Format("$ {0:#,0.00}", shoppingCartTotals.TotalPrice)
                }
            }.ToList();
        }

        private void CheckCurrentOrDefaultAddress(CheckoutPage page)
        {
            if ((page?.DeliveryAddresses?.items?.Count ?? 0) == 0)
            {
                return;
            }

            var currentAddressId = shoppingCart.GetCurrentCartShippingAddress()?.Id ?? 0;
            if (currentAddressId != 0 && page.DeliveryAddresses.items.Any(a => a.Id == currentAddressId))
            {
                page.DeliveryAddresses.CheckAddress(currentAddressId);
            }
            else
            {
                var defaultAddressId = kenticoUsers.GetCurrentCustomer().DefaultShippingAddressId;
                if (defaultAddressId == 0)
                {
                    defaultAddressId = page.DeliveryAddresses.GetDefaultAddressId();
                }

                shoppingCart.SetShoppingCartAddress(defaultAddressId);
                page.DeliveryAddresses.CheckAddress(defaultAddressId);
            }
        }

        private string GetUserNotificationString()
        {
            var userNotification = string.Empty;
            var userNotificationLocalizationKey = kenticoSite.GetCurrentSiteCodeName() + ".Kadena.Settings.Address.NotificationMessage";
            if (!localization.IsCurrentCultureDefault())
            {
                userNotification = resources.GetResourceString(userNotificationLocalizationKey) == userNotificationLocalizationKey ? string.Empty : resources.GetResourceString(userNotificationLocalizationKey);
            }
            return userNotification;
        }

        private void CheckCurrentOrDefaultShipping(DeliveryCarriers deliveryMethods)
        {
            int currentShipping = shoppingCart.GetCurrentCartShippingOptionId();

            if (deliveryMethods.IsPresent(currentShipping) && !deliveryMethods.IsDisabled(currentShipping))
            {
                deliveryMethods.CheckMethod(currentShipping);
            }
            else
            {
                SetDefaultShipping(deliveryMethods);
            }
        }

        private void SetDefaultShipping(DeliveryCarriers deliveryMethods)
        {
            int defaultMethodId = deliveryMethods.GetDefaultMethodId();
            shoppingCart.SelectShipping(defaultMethodId);
            deliveryMethods.CheckMethod(defaultMethodId);
        }

        private void UnsetShipping()
        {
            shoppingCart.SelectShipping(0);
        }

        private void SetPricesVisibility(CheckoutPage page)
        {
            if (!permissions.UserCanSeePrices())
            {
                page.Products.HidePrices();
            }
        }

        private void SetPricesVisibility(CheckoutPageDeliveryTotals page)
        {
            if (!permissions.UserCanSeePrices())
            {
                page.DeliveryMethods.HidePrices();
            }
        }
       
        public CheckoutPage SelectShipipng(int id)
        {
            shoppingCart.SelectShipping(id);
            return GetCheckoutPage();
        }

        public CheckoutPage SelectAddress(int id)
        {
            shoppingCart.SetShoppingCartAddress(id);
            var checkoutPage = GetCheckoutPage();
            checkoutPage.DeliveryAddresses.CheckAddress(id);
            return checkoutPage;
        }

        public CheckoutPage ChangeItemQuantity(int id, int quantity)
        {
            shoppingCart.SetCartItemQuantity(id, quantity);
            return GetCheckoutPage();
        }

        public CheckoutPage RemoveItem(int id)
        {
            shoppingCart.RemoveCartItem(id);
            var itemsCount = shoppingCart.GetShoppingCartItemsCount();
            if (itemsCount == 0)
            {
                shoppingCart.ClearCart();
            }

            return GetCheckoutPage();
        }

        public CartItemsPreview ItemsPreview()
        {
            bool userCanSeePrices = permissions.UserCanSeePrices();
            var cartItems = shoppingCart.GetShoppingCartItems(userCanSeePrices);

            var preview = new CartItemsPreview
            {
                EmptyCartMessage = resources.GetResourceString("Kadena.Checkout.CartIsEmpty"),
                SummaryPrice = new CartPrice(),

                Items = cartItems.ToList()
            };

            if (userCanSeePrices)
            {
                var cartItemsTotals = shoppingCart.GetShoppingCartTotals();
                preview.SummaryPrice = new CartPrice()
                {
                    PricePrefix = resources.GetResourceString("Kadena.Checkout.ItemPricePrefix"),
                    Price = string.Format("{0:#,0.00}", cartItemsTotals.TotalItemsPrice)
                };
            }

            return preview;
        }

        public async Task<AddToCartResult> AddToCart(NewCartItem item)
        {
            var mailingList = await mailingService.GetMailingList(item.ContainerId);
            var addedItem = shoppingCart.AddCartItem(item, mailingList);
            var result = new AddToCartResult
            {
                CartPreview = ItemsPreview(),
                Confirmation = new RequestResult
                {
                    AlertMessage = resources.GetResourceString("Kadena.Product.ItemsAddedToCart")
                }
            };
            return result;
        }

        bool GetOtherAddressSettingsValue()
        {
            var settingsKey = resources.GetSettingsKey("KDA_AllowCustomShippingAddress");
            bool otherAddressAvailable = false;
            bool.TryParse(settingsKey, out otherAddressAvailable);
            return otherAddressAvailable;
        }
        public List<int> GetLoggedInUserCartData(int inventoryType, int userID, int? campaignID)
        {
            var query = new DataQuery(loggedInUserCartData);
            QueryDataParameters queryParams = new QueryDataParameters();
            queryParams.Add("@ShoppingCartUserID", userID);
            queryParams.Add("@ShoppingCartInventoryType", inventoryType);
            queryParams.Add("@ShoppingCartCampaignID", campaignID);
            var cartDataSet = ConnectionHelper.ExecuteQuery(query.QueryText, queryParams, QueryTypeEnum.SQLQuery, true);
            return cartDataSet.Tables[0].AsEnumerable().Select(x => x.Field<int>("ShoppingCartID")).Distinct().ToList(); ;
        }
    }
}
