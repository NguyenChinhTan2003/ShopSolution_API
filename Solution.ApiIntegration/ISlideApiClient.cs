using ShopSolution.ViewModels.Utilities.Slides;

namespace ShopSolution.ApiIntegration
{
    public interface ISlideApiClient
    {
        Task<List<SlideVm>> GetAll();
    }
}