using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Orders
{
    public class OrderVm
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public DateTime OrderDate { set; get; }
        public String UserName { set; get; }
        public string? Email { set; get; }
        public string? Address { set; get; }
        public string? Phone { set; get; }
        public string? PaymentMethod { set; get; }
        public int Status { set; get; }

        public List<OrderDetailVm> OrderDetails { get; set; } = new List<OrderDetailVm>();
    }

    public class OrderDetailVm
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal shippingCost { set; get; }
    }
}