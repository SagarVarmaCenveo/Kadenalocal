﻿using AutoMapper;
using CMS.Ecommerce;
using CMS.Membership;
using CMS.SiteProvider;
using Kadena.Models;
using Kadena.WebAPI.KenticoProviders.Contracts;
using System;
using System.Linq;

namespace Kadena.WebAPI.KenticoProviders
{
    public class KenticoUserProvider : IKenticoUserProvider
    {
        public static string CustomerDefaultShippingAddresIDFieldName => "CustomerDefaultShippingAddresID";

        private readonly IKenticoLogger _logger;
        private readonly IMapper _mapper;

        public KenticoUserProvider(IKenticoLogger logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public DeliveryAddress[] GetCustomerAddresses(AddressType addressType)
        {
            var customer = ECommerceContext.CurrentCustomer;
            return GetCustomerAddresses(customer.CustomerID, addressType);
        }

        public DeliveryAddress[] GetCustomerAddresses(int customerId, AddressType addressType)
        {
            var query = AddressInfoProvider.GetAddresses(customerId);
            if (addressType != null)
            {
                query = query.Where($"AddressType ='{addressType}'");
            }
            return _mapper.Map<DeliveryAddress[]>(query.ToArray());
        }

        public DeliveryAddress[] GetCustomerShippingAddresses(int customerId)
        {
            var addresses = AddressInfoProvider.GetAddresses(customerId)
                .Where(a => a.GetStringValue("AddressType", string.Empty) == AddressType.Shipping)
                .ToArray();

            return _mapper.Map<DeliveryAddress[]>(addresses.ToArray());
        }

        public Customer GetCurrentCustomer()
        {
            return _mapper.Map<Customer>(ECommerceContext.CurrentCustomer);
        }

        public Customer GetCustomer(int customerId)
        {
            return _mapper.Map<Customer>(CustomerInfoProvider.GetCustomerInfo(customerId));
        }

        public User GetCurrentUser()
        {
            return _mapper.Map<User>(MembershipContext.AuthenticatedUser);
        }

        public User GetUser(string mail)
        {
            return _mapper.Map<User>(UserInfoProvider.GetUserInfo(mail));
        }

        public bool SaveLocalization(string code)
        {
            try
            {
                var user = MembershipContext.AuthenticatedUser;
                if (user != null)
                {
                    user.PreferredCultureCode = code;
                    UserInfoProvider.SetUserInfo(user);
                }
            }
            catch (Exception exc)
            {
                _logger.LogException("UserProvider - Saving Localization", exc);
                throw;
            }
            return true;
        }

        public void SetDefaultShippingAddress(int addressId)
        {
            var customer = ECommerceContext.CurrentCustomer;

            if (customer != null)
            {
                customer.SetValue(CustomerDefaultShippingAddresIDFieldName, addressId);
                CustomerInfoProvider.SetCustomerInfo(customer);
            }
        }

        public void UnsetDefaultShippingAddress()
        {
            SetDefaultShippingAddress(0);
        }

        public bool UserIsInCurrentSite(int userId)
        {
            var user = UserInfoProvider.GetUserInfo(userId);
            return user?.IsInSite(SiteContext.CurrentSiteName) ?? false;
        }

        public User GetUserByUserId(int userId)
        {
            return _mapper.Map<User>(UserInfoProvider.GetUserInfo(userId));
        }

        /// <summary>
        /// Creates and saves new Customer
        /// </summary>
        /// <returns>ID of new Customer</returns>
        public int CreateCustomer(Customer customer)
        {
            var customerInfo = _mapper.Map<CustomerInfo>(customer);
            customerInfo.CustomerID = 0;
            customerInfo.Insert();
            return customerInfo.CustomerID;
        }

        public void UpdateCustomer(Customer customer)
        {
            var customerInfo = CustomerInfoProvider.GetCustomerInfo(customer.Id);

            if (customerInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(customer.Id), "Existing Customer with given Id not found");
            }

            customerInfo.CustomerFirstName = customer.FirstName;
            customerInfo.CustomerLastName = customer.LastName;
            customerInfo.CustomerEmail = customer.Email;
            customerInfo.CustomerPhone = customer.Phone;
            customerInfo.CustomerCompany = customer.Company;
            customerInfo.Update();
        }

        /// <summary>
        /// Creates and saves new User
        /// </summary>
        /// <returns>ID of new User</returns>
        public int CreateUser(User user)
        {
            var newUser = new UserInfo()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}"
            };

            newUser.Insert();
            return newUser.UserID;
        }

        public void UpdateUser(User user)
        {
            var userInfo = UserInfoProvider.GetUserInfo(user.UserId);

            if (userInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(user.UserId), "Existing User with given Id not found");
            }

            userInfo.UserName = user.UserName;
            userInfo.FirstName = user.FirstName;
            userInfo.LastName = user.LastName;
            userInfo.Email = user.Email;
            
            userInfo.Update();
        }

        public void LinkCustomerToUser(int customerId, int userId)
        {
            var customer = CustomerInfoProvider.GetCustomerInfo(customerId);
            customer.CustomerUserID = userId;
            customer.Update();
        }
    }
}