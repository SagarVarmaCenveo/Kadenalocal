﻿using System;

namespace Kadena.Models.Checkout
{
    public class NewCartItem
    {
        public int DocumentId { get; set; }
        public string CustomProductName { get; set; }
        public int Quantity { get; set; }
        public Guid TemplateId { get; set; }
        public Guid ContainerId { get; set; }
    }
}
