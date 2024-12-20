using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Orders
{
    public class OrderEditVm
    {
        public Guid OrderId { get; set; }
        public int Status { get; set; }
    }
}