using Chirp.Core;
using Chirp.Infrastructure.Chirp.Services;
using Chirp.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace Chirp.Web.Pages.Shared;

public class TimelineModel : PageModel
{
    protected readonly IChirpService Service;
    public int PageNumber { get; set; }

    [BindProperty]
    [Required]
    public string? CheepMessage { get; set; }

    [BindProperty]
    public string? FollowsName { get; set; }

    [BindProperty]
    public int? LikedCheepId { get; set; }

    public List<CheepDto>? Cheeps { get; set; }

    public string? PageName { get; set; }

    public bool IsTopList { get; set; }

    public TimelineModel(IChirpService service)
    {
        Service = service;
    }

    public Task HandlePageNumber()
    {
        if (PageNumber <= 0)
        {
            PageNumber = 1;
        }

        return Task.CompletedTask;
    }

    public async Task<IActionResult> OnPost()
    {
        //We check if any validation rules has exceeded
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var authorName = User.Identity?.Name;
        if (authorName == null)
        {
            return Page();
        }
        var authorDto = await Service.GetAuthorDtoByName(authorName);
        if (authorDto == null)
        {
            return Page();
        }

        if (CheepMessage != null)
            await Service.AddCheep(CheepMessage, authorDto.Username, authorDto.Email);
        if (IsTopList)
        {
            return RedirectToPage("Public");
        }
        return RedirectToPage(PageName);
    }

    public async Task<IActionResult> OnPostFollow()
    {
        var authorName = User.Identity?.Name;

        if (authorName != null)
            if (FollowsName != null)
                await Service.AddFollowing(authorName, FollowsName);

        return RedirectToPage(PageName);
    }

    public async Task<IActionResult> OnPostUnfollow()
    {
        var authorName = User.Identity?.Name;

        if (authorName != null)
            if (FollowsName != null)
                await Service.RemoveFollowing(authorName, FollowsName);

        return RedirectToPage(PageName);
    }

    public async Task<IActionResult> OnPostLike()
    {
        var authorName = User.Identity?.Name;

        if (authorName != null && LikedCheepId != null)
        {
            await Service.AddLike(authorName, LikedCheepId.Value);
        }

        return RedirectToPage(PageName);
    }

    public async Task<IActionResult> OnPostUnlike()
    {
        var authorName = User.Identity?.Name;

        if (authorName != null && LikedCheepId != null)
        {
            await Service.RemoveLike(authorName, LikedCheepId.Value);
        }

        return RedirectToPage(PageName);
    }

    public async Task<IActionResult> OnPostDeleteCheep()
    {
        if (LikedCheepId != null)
        {
            try
            {
                await Service.DeleteCheep(LikedCheepId.Value);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
            }
        }

        return RedirectToPage(PageName);
    }
}
