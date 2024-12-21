using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ApiIntegration
{
    public interface IShippingApiClient
    {
        Task<bool> CreateShip(ShippingRequest request);

        Task<List<ShippingVm>> GetAll();
        Task<int> Delete(int id);
    }
}
