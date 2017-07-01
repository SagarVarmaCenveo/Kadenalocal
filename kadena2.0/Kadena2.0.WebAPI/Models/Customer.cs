﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kadena.WebAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int UserID { get; set; }
        public string CustomerNumber { get; set; }
        public string Company { get; set; }
        public int SiteId { get; set; }
    }
}