﻿using AutoMapper;
using Kadena.BusinessLogic.Contracts.Orders;
using Kadena.Dto.OrderManualUpdate.Requests;
using Kadena.Models.Orders;
using Kadena.WebAPI.Infrastructure;
using Kadena.WebAPI.Infrastructure.Filters;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kadena.WebAPI.Controllers
{
    [CustomerAuthorizationFilter]
    public class OrderUpdateController : ApiControllerBase
    {
        private readonly IOrderManualUpdateService updateService;
        private readonly IMapper mapper;

        public OrderUpdateController(IOrderManualUpdateService updateService, IMapper mapper)
        {
            this.updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        [Route("api/orderupdate")]
        public async Task<IHttpActionResult> UpdateOrder(OrderUpdateDto request)
        {
            var requestModel = mapper.Map<OrderUpdate>(request);
            await updateService.UpdateOrder(requestModel);
            return ResponseJson((string)null);
        }
    }
}