﻿namespace Kadena.Models.Product
{
    public class Sku
    {
        public int SkuId { get; set; }
        public double Weight { get; set; }
        public bool NeedsShipping { get; set; }
    }
}