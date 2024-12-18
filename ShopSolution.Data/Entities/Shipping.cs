using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Entities
{
    public class Shipping
    {
        public int Id { get; set; }
        public decimal? Price { get; set; } = 0;
        public string Ward { get; set; }
        public string District { get; set; }
        public string City { get; set; }
    }
}
