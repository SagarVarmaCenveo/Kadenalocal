﻿using Kadena.Dto.General;
using Kadena.Dto.Payment.CreditCard.MicroserviceRequests;
using Kadena2.MicroserviceClients.Clients.Base;
using Kadena2.MicroserviceClients.Contracts;
using Kadena2.MicroserviceClients.Contracts.Base;
using System.Threading.Tasks;

namespace Kadena2.MicroserviceClients.Clients
{
    public class CreditCardManagerClient : ClientBase, ICreditCardManagerClient
    {
        private const string _serviceUrlSettingKey = "KDA_CreditCardManagerEndpoint";
        private readonly IMicroProperties _properties;

        public CreditCardManagerClient(IMicroProperties properties)
        {
            _properties = properties;
        }

        public async Task<BaseResponseDto<object>> CreateCustomerContainer(CreateCustomerContainerRequestDto request)
        {
            var url = _properties.GetServiceUrl(_serviceUrlSettingKey);
            url = $"{url.TrimEnd('/')}/api/Customer/";
            return await Post<object>(url, request).ConfigureAwait(false);
        }
    }
}
