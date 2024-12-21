using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Orders;
using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Catalog.Order
{
    public interface IOrderService
    {
        Task<List<OrderVm>> GetAll();

        Task<DashboardViewModel> GetDashboardData();
    }
}