using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Service.Products.DTO
{
    public class ProductCreateRequest
    {
        public decimal Price { set; get; }
        public string Name { set; get; }
    }
}
