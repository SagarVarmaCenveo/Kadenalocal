﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kadena.Models;

namespace Kadena.WebAPI.Contracts
{
    public interface IKListService
    {
        Task<bool> UseOnlyCorrectAddresses(Guid containerId);

        Task<bool> UpdateAddresses(Guid containerId, IEnumerable<MailingAddress> addresses);

        Task<MailingList> GetMailingList(Guid containerId);
    }
}
