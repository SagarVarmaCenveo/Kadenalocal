﻿namespace Kadena.Dto.ViewOrder.MicroserviceResponses
{
    public class OrderItemDTO
    {
        public int SkuId { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public string TrackingId { get; set; }
        public string Name { get; set; }
        public string MailingList { get; set; }
        public string FileUrl { get; set; }
        public double TotalPrice { get; set; }
    }
}
