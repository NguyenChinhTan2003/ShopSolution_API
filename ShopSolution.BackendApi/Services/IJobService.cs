using ShopSolution.ViewModels.Catalog.Products;

namespace ShopSolution.BackendApi.Services
{
    public interface IJobService
    {
        void SendEmail();

        Task UpdateIsFeatured();

    }
}
