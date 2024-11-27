using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Mvc;
using ShopSolution.ApiIntegration;
using ShopSolution.WebApp.Models;
using System.Globalization;

namespace ShopSolution.WebApp.Controllers.Components
{
    public class SideViewComponent : ViewComponent
    {
        private readonly ILogger<SideViewComponent> _logger;
        private readonly ISharedCultureLocalizer _loc;
        private readonly ISlideApiClient _slideApiClient;

        public SideViewComponent(ILogger<SideViewComponent> logger,
           ISlideApiClient slideApiClient,
           ISharedCultureLocalizer loc)
        {
            _logger = logger;
            _loc = loc;
            _slideApiClient = slideApiClient;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var culture = CultureInfo.CurrentCulture.Name;
            var viewModel = new HomeViewModel
            {
                Slides = await _slideApiClient.GetAll(),
            };
            return View(viewModel);
        }
    }
}
