﻿using AutoMapper;
using CMS.Ecommerce;
using Kadena.Models;
using Kadena.WebAPI.KenticoProviders.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kadena.WebAPI.KenticoProviders
{
    public class KenticoAddressBookProvider : IKenticoAddressBookProvider
    {
        public static string CustomerDefaultShippingAddresIDFieldName => "CustomerDefaultShippingAddresID";

        private readonly IMapper mapper;
        public KenticoAddressBookProvider(IMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            this.mapper = mapper;
        }
        public void DeleteAddress(int addressID)
        {
            var address = AddressInfoProvider.GetAddressInfo(addressID);
            if (address != null)
            {
                AddressInfoProvider.DeleteAddressInfo(addressID);
            }
        }

        public Dictionary<int, string> GetAddressNames()
        {
            return AddressInfoProvider.GetAddresses().ToDictionary(x => x.AddressID, x => x.AddressName);
        }
        public List<DeliveryAddress> GetAddressesByAddressIds(List<int> addressIds)
        {
            var addresses = AddressInfoProvider.GetAddresses().WhereIn("AddressID", addressIds).ToList();
            return mapper.Map<List<DeliveryAddress>>(addresses);
        }

        public List<AddressData> GetAddressesList(int customerID)
        {
            var addressesList =  AddressInfoProvider.GetAddresses(customerID).Columns("AddressID", "AddressPersonalName").WhereEquals("Status", true).ToList();
            return mapper.Map<List<AddressData>>(addressesList);
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
            return mapper.Map<DeliveryAddress[]>(query.ToArray());
        }

        public DeliveryAddress[] GetCustomerShippingAddresses(int customerId)
        {
            var addresses = AddressInfoProvider.GetAddresses(customerId)
                .Where(a => a.GetStringValue("AddressType", string.Empty) == AddressType.Shipping)
                .ToArray();

            return mapper.Map<DeliveryAddress[]>(addresses.ToArray());
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

        public void SaveShippingAddress(DeliveryAddress address, int customerId = 0)
        {
            CustomerInfo customer = customerId > 0
                ? CustomerInfoProvider.GetCustomerInfo(customerId)
                : ECommerceContext.CurrentCustomer;

            var info = new AddressInfo
            {
                AddressID = address.Id,
                AddressLine1 = address.Address1,
                AddressLine2 = address.Address2,
                AddressCity = address.City,
                AddressStateID = address.State.Id,
                AddressCountryID = address.Country.Id,
                AddressZip = address.Zip,
                AddressCustomerID = customer.CustomerID,
                AddressPersonalName = $"{customer.CustomerFirstName} {customer.CustomerLastName}",
                AddressPhone = address.Phone
            };
            info.AddressName = $"{info.AddressPersonalName}, {info.AddressLine1}, {info.AddressCity}";
            info.SetValue("AddressType", AddressType.Shipping.Code);
            info.SetValue("CompanyName", address.CustomerName);
            info.SetValue("Email", address.Email);

            AddressInfoProvider.SetAddressInfo(info);
            address.Id = info.AddressID;
        }
    }
}
