﻿using System.Collections.Generic;
using System.Linq;

namespace Kadena.Models
{
    public class TieredPricingRange
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public bool Validate(List<string> errors)
        {
            bool isValid = true;
            if (Quantity <= 0)
            {
                isValid = false;
                errors.Add("Quantity must be > 0");
            }
            if (Price < 0)
            {
                isValid = false;
                errors.Add("Price must be > 0");
            }
            return isValid;
        }   

        public static bool ValidateRanges(List<TieredPricingRange> ranges, List<string> errors, bool allowEmptyRanges = false)
        {
            if ((ranges == null || ranges.Count == 0) && !allowEmptyRanges)
            {
                errors.Add("No Dynamic pricing ranges specified");
                return false;
            }

            if (ranges.Select(r => r.Quantity).Distinct().Count() != ranges.Count)
            {
                errors.Add("Range with the same quantity is duplicated");
                return false;
            }

            for (int i = 0; i < ranges.Count; i++)
            {
                if (!ranges[i].Validate(errors))
                {
                    return false;
                }
            }

            return true;
        }
    }    
}