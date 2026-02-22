using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web;

//API, made after the specification in the Stub API.
public static class Api
{
    private static int Latest;
    public record FollowRequest(string? Follow, string? Unfollow);
    public record MessageRequest(string Content);
    public record SignUpRequest(string Username, string Email, string Pwd);

    public record GetMessagesRequest(string Content, string User);

    public record GetFollowsRequest(List<string> follows);
    
    public static void MapProductEndpoints(this WebApplication app)
    {
        app.MapGet("/fllws/{username}",
            (string username,[FromQuery (Name = "latest")] int? latests,[FromQuery (Name = "no")] int? no, IFollowRepository followRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                var res = followRepository.GetFollowed(username).Result.Select(follow => follow.Followed).ToList();
                return new GetFollowsRequest(res);
            });
            
        app.MapPost("/fllws/{username}",
            (string username, [FromQuery (Name = "latest")] int? latests ,[FromBody] FollowRequest request, IFollowRepository followRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;
                
                if (!string.IsNullOrEmpty(request.Follow))
                {
                    return followRepository.AddFollowing(username,request.Follow);
                }

                if (!string.IsNullOrEmpty(request.Unfollow))
                {
                    return followRepository.RemoveFollowing(username,request.Unfollow);
                }
                
                return Task.FromResult(Results.BadRequest("Must specify either 'Follow' or 'Unfollow'"));
            });
        
        app.MapGet("/latest",() => new {Latest});
        
        app.MapGet("/msgs",
            ([FromQuery (Name = "latest")] int? latests,[FromQuery (Name = "no")] int? no, ICheepRepository  cheepRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;
                return cheepRepository.GetCheeps(0).Result.Select(cheep => new GetMessagesRequest(cheep.Text, cheep.Author.Name));
            });
        
        app.MapGet("/msgs/{username}",
            (string username,[FromQuery (Name = "latest")] int? latests,[FromQuery (Name = "no")] int? no, ICheepRepository cheepRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;
                
                return cheepRepository.GetAllCheepsFromAuthor(username).Result.Select(cheep => new GetMessagesRequest(cheep.Text, cheep.Author.Name));
            });
        
        app.MapPost("/msgs/{username}",
            (string username,[FromQuery (Name = "latest")] int? latests,[FromBody] MessageRequest msgRequest, ICheepRepository cheepRepository, IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;
                var author = authorRepository.GetAuthorByName(username).Result;
                if (author != null)
                    return cheepRepository.AddCheep(msgRequest.Content, author);
                else
                {
                    return Task.FromResult(Results.BadRequest("Author does not exist"));
                }
            });
        
  
        app.MapPost("/register",
            ([FromQuery (Name = "latest")] int? latests,[FromBody] SignUpRequest request, IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;
                return authorRepository.CreateAuthor(request.Username, request.Email);
            });
    }
}