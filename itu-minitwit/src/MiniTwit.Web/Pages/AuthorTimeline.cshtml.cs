using Chirp.Infrastructure.Chirp.Services;
using Chirp.Web.Pages.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class UserTimelineModel : TimelineModel
{
    public UserTimelineModel(IChirpService service)
        : base(service) { }

    public async Task<ActionResult> OnGet([FromQuery] int page, string authorName)
    {
        PageNumber = page;
        await HandlePageNumber();
        //Add so get cheeps from author also gets the cheeps that the author is following
        if (User.Identity != null && User.Identity.Name == authorName)
        {
            Cheeps = await Service.GetCheepsForTimeline(authorName, PageNumber);
        }
        else
        {
            var spectatingAuthorName = User.Identity?.Name;
            if (spectatingAuthorName != null)
            {
                Cheeps = await Service.GetCheepsFromAuthor(
                    PageNumber,
                    authorName,
                    spectatingAuthorName
                );
            }
            else
            {
                Cheeps = await Service.GetCheepsFromAuthor(PageNumber, authorName, "");
            }
        }

        return Page();
    }
}
