﻿namespace Kadena.Models.Product
{
    public class ProductLink
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsFavorite { get; set; }
    }
}