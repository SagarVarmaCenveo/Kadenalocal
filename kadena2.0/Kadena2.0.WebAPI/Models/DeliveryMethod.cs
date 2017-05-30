﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kadena.WebAPI.Models
{
    public class DeliveryMethod
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool Opened { get; set; }
        public bool Disabled { get; set; }
        public string PricePrefix { get; set; }        
        public string Price { get; set; }
        public string DatePrefix { get; set; }
        public string Date { get; set; }
        public List<DeliveryService> items { get; set; }

        public void SetShippingOptions(IEnumerable<DeliveryService> services)
        {
            items = services.Where(s => s.CarrierId == this.Id).ToList();
        }

        public void CheckMethod(int id)
        {
            items.ForEach(i => i.Checked = false);
            var checkedItem = items.Where(i => i.Id == id).FirstOrDefault();

            if (checkedItem != null)
            {
                this.Opened = true;
                checkedItem.Checked = true;
            }
            else
            {
                this.Opened = false;
            }
        }
    }
}