using Chirp.Infrastructure.Chirp.Services;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class PublicModel : TimelineModel
{
    public PublicModel(IChirpService service)
        : base(service) { }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        var authorName = User.Identity?.Name;
        PageNumber = page;
        await HandlePageNumber();
        if (authorName == null)
        {
            Cheeps = await Service.GetCheeps(PageNumber);
        }
        else
        {
            Cheeps = await Service.GetCheeps(PageNumber, authorName);
        }
        return Page();
    }
}
