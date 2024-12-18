using ShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Entities
{
    public class Order
    {
        public int Id { get; set; } 
        public Guid OrderId { get; set; }
        public DateTime OrderDate { set; get; }
        public String UserName { set; get; }
        public string? Email { set; get; }
        public string? Address {  set; get; }

        public string? Phone { set; get; }
        public string? PaymentMethod { set; get; }
        public List<OrderDetail>? OrderDetails { get; set; }
        public OrderStatus Status { set; get; }
       


    }
}
