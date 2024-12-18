using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Entities
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }

        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}
