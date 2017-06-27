﻿using Kadena.Dto.General;
using Kadena.Dto.TemplatedProduct.MicroserviceResponses;
using Kadena2.MicroserviceClients.Communication;
using System.Threading.Tasks;

namespace Kadena2.MicroserviceClients.Contracts
{
    public interface ITemplatedProductService
    {
        Task<BaseResponseDto<GeneratePdfTaskResponseDto>> RunGeneratePdfTask(string endpoint, string templateId, string settingsId);
        Task<BaseResponseDto<GeneratePdfTaskStatusResponseDto>> GetGeneratePdfTaskStatus(string endpoint, string templateId, string taskId);
    }
}
