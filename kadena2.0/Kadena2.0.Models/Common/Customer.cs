﻿namespace Kadena.Models
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
        public int ApproverUserId { get; set; }
        public string PreferredLanguage { get; set; }
        public int DefaultShippingAddressId { get; set; }

        public override string ToString() => $"{FirstName} {LastName}".Trim();

        public string FullName => ToString();

        public static Customer Unknown => new Customer
        {
            Id = 0,
            FirstName = "Unknown"
        };
    }
}