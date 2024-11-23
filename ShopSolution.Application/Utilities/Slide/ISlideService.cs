using ShopSolution.ViewModels.Utilities.Slides;

namespace ShopSolution.Application.Utilities.Slide
{
    public interface ISlideService
    {
        Task<List<SlideVm>> GetAll();
    }
}