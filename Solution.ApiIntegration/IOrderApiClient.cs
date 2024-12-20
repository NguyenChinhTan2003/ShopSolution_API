using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration
{
    public interface IOrderApiClient
    {
        Task<List<OrderVm>> GetAll();
        Task<OrderVm> GetById(Guid orderId); // Thêm phương thức GetById

        Task<bool> UpdateStatus(Guid orderId, int status); // Thêm phương thức UpdateStatus
    }
}