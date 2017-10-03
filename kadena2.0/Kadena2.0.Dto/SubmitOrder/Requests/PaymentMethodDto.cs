﻿using System.ComponentModel.DataAnnotations;

namespace Kadena.Dto.SubmitOrder.Requests
{
    public class PaymentMethodDto
    {
        public int Id { get; set; }
        [MaxLength(25)]
        public string Invoice { get; set; }
    }
}