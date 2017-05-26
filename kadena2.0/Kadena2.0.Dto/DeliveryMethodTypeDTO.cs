﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kadena2._0.Dto
{
    public class DeliveryMethodTypeDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Checked { get; set; }
        public string PricePrefix { get; set; }
        public string Price { get; set; }
        public string DatePrefix { get; set; }
        public string Date { get; set; }
        public bool Disabled { get; set; }
    }
}
