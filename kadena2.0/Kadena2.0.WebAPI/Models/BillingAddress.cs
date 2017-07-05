﻿using System.Collections.Generic;

namespace Kadena.WebAPI.Models
{
    public class BillingAddress
    {
        public List<string> Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Zip { get; set; }
    }
}