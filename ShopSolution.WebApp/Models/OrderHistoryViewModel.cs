using ShopSolution.Data.Entities;

namespace ShopSolution.WebApp.Models
{
    public class OrderHistoryViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
