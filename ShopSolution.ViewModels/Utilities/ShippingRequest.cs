using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Utilities
{
    public class ShippingRequest
    {
        public decimal? Price { get; set; } 
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}
