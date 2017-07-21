﻿using Kadena.Models.RecentOrders;
using System.Threading.Tasks;

namespace Kadena.WebAPI.Contracts
{
    public interface IOrderListService
    {
        string PageCapacityKey { get; set; }

        bool EnablePaging { get; set; }

        Task<OrderHead> GetHeaders();

        Task<OrderBody> GetBody(int pageNumber);
    }
}
