using Chirp.Infrastructure.Chirp.Services;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class TopCheeps : TimelineModel
{
    public TopCheeps(IChirpService service)
        : base(service)
    {
        IsTopList = true;
    }

    public async Task<ActionResult> OnGet([FromQuery] int page)
    {
        var authorname = User.Identity?.Name;
        PageNumber = page;
        await HandlePageNumber();
        if (authorname != null)
        {
            Cheeps = await Service.GetTopLikedCheeps(authorname, PageNumber);
        }
        else
        {
            Cheeps = await Service.GetTopLikedCheeps("", PageNumber); //Can we find a better way to do this ?
        }

        return Page();
    }
}
