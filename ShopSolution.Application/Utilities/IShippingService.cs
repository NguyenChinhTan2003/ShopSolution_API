using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Utilities
{
    public interface IShippingService
    {
        Task<int> Create(ShippingRequest request);
        Task<ShippingVm> GetById(int id);
        Task<List<ShippingVm>> GetAll();
        Task<int> Delete(int id);

    }
}
