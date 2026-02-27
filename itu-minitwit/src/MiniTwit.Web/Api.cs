using Azure.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;
using SQLitePCL;


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
        /* Returns list of author followed by username*/
        app.MapGet("/fllws/{username}",
            (string username,[FromHeader (Name = "Authorization")] string authorization, [FromQuery (Name = "latest")] int? latests,[FromQuery (Name = "no")] int? no, IFollowRepository followRepository,  IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.StatusCode(403);
                } 

                var author = authorRepository.GetAuthorByName(username).Result;
                if (author == null)
                {
                    return Results.StatusCode(404);
                }

                var res = followRepository.GetFollowed(username).Result.Select(follow => follow.Followed).ToList();
                return Results.Ok(res);
            });

        /* User follows/unfollows author*/    
        app.MapPost("/fllws/{username}",
            (string username, [FromHeader (Name = "Authorization")] string authorization, [FromQuery (Name = "latest")] int? latests ,[FromBody] FollowRequest request, IFollowRepository followRepository, IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.StatusCode(403);
                } 

                if (!string.IsNullOrEmpty(request.Follow))
                {
                    var res = followRepository.AddFollowing(username,request.Follow);
                    return Results.NoContent();
                }

                if (!string.IsNullOrEmpty(request.Unfollow))
                {
                    var res = followRepository.RemoveFollowing(username,request.Unfollow);
                    return Results.NoContent();
                }
                
                return Results.BadRequest("Must specify either 'Follow' or 'Unfollow'");
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
            (string username,[FromHeader (Name = "Authorization")] string authorization, [FromQuery (Name = "latest")] int? latests,[FromQuery (Name = "no")] int? no, ICheepRepository cheepRepository) =>
            {
                if (latests.HasValue)
                {
                    Latest = latests.Value;
                }

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.StatusCode(403);
                }  
                
                var res = cheepRepository.GetAllCheepsFromAuthor(username).Result.Select(cheep => new GetMessagesRequest(cheep.Text, cheep.Author.Name));
                return Results.Ok(res);
            });
        
        
        app.MapPost("/msgs/{username}",
            (string username,[FromHeader (Name = "Authorization")] string authorization, [FromQuery (Name = "latest")] int? latests,[FromBody] MessageRequest msgRequest, ICheepRepository cheepRepository, IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                {
                    Latest = latests.Value;
                }

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.StatusCode(403);
                }    

                var author = authorRepository.GetAuthorByName(username).Result;
                cheepRepository.AddCheep(msgRequest.Content, author);

                return Results.NoContent();
                
            });
  
        app.MapPost("/register",
            ([FromQuery (Name = "latest")] int? latests,[FromBody] SignUpRequest request, IAuthorRepository authorRepository) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                var res = authorRepository.CreateAuthor(request.Username, request.Email);

                /*returns 400 HTTP code if createAuthor Fails*/
                if(res == null)
                {
                    return Results.BadRequest("Possible reasons:\n - missing username \n- invalid email \n- password missing \n- username already taken");
                }

                /*returns 204 HTTP code*/
                return  Results.NoContent();
            });
    }
}