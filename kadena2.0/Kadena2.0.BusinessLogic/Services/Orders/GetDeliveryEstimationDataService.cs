﻿using Kadena.BusinessLogic.Contracts.Orders;
using Kadena.Dto.EstimateDeliveryPrice.MicroserviceRequests;
using Kadena.WebAPI.KenticoProviders.Contracts;
using System;
using System.Linq;

namespace Kadena.BusinessLogic.Services.Orders
{
    public class GetDeliveryEstimationDataService : IGetDeliveryEstimationDataService
    {
        private readonly IKenticoResourceService resources;
        public GetDeliveryEstimationDataService(IKenticoResourceService resources)
        {
            this.resources = resources ?? throw new ArgumentNullException(nameof(resources));
        }

        public AddressDto GetSourceAddressForDeliveryEstimation()
        {
            var addressLines = new[]
            {
                resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderAddressLine1"),
                resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderAddressLine2")
            }.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

            return new AddressDto()
            {
                City = resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderCity"),
                Country = resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderCountry"),
                Postal = resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderPostal"),
                State = resources.GetSiteSettingsKey("KDA_EstimateDeliveryPrice_SenderState"),
                StreetLines = addressLines
            };

        }

        public WeightDto GetWeight()
        {
            throw new NotImplementedException();
        }
    }
}
