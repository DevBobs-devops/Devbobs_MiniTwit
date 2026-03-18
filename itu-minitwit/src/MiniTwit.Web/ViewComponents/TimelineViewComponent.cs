using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.ViewComponents;

public class TimelineViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(TimelineModel model)
    {
        return Task.FromResult<IViewComponentResult>(View("Timeline"));
    }
}
