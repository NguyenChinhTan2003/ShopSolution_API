using Microsoft.EntityFrameworkCore;
using ShopSolution.Data.EF;
using ShopSolution.ViewModels.Utilities.Slides;

namespace ShopSolution.Application.Utilities.Slide
{
    public class SlideService : ISlideService
    {
        private readonly ShopDBContext _context;

        public SlideService(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<List<SlideVm>> GetAll()
        {
            var slides = await _context.Slides.OrderBy(x => x.SortOrder)
                .Select(x => new SlideVm()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Url = x.Url,
                    Image = x.Image
                }).ToListAsync();
            return slides;
        }
    }
}