using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Entities
{
    public class Vnpay
    {
        
        public int Id { get; set; }  
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentId { get; set; }
        public DateTime CreatedDate { get; set; }
      
        
    }
}
