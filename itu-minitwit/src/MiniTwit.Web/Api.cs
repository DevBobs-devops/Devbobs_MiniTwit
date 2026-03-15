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

    public record GetMessagesRequest(string Content, string Pub_Date, string User); //Message

    public record MessageRequest(string Content); //PostMessage

    public record SignUpRequest(string Username, string Email, string Pwd); //RegisterRequest

    public record FollowRequest(string? Follow, string? Unfollow); //FollowAction

    public record GetFollowsResponse(List<string> Follows); //FollowResponse

    public static void MapProductEndpoints(this WebApplication app)
    {
        /* Returns list of authors followed by username*/
        app.MapGet(
            "/fllws/{username}",
            (
                string username,
                [FromHeader(Name = "Authorization")] string authorization,
                [FromQuery(Name = "latest")] int? latests,
                [FromQuery(Name = "no")] int? no,
                IFollowRepository followRepository,
                IAuthorRepository authorRepository
            ) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.Forbid(); //returns status code 403
                }

                var author = authorRepository.GetAuthorByName(username).Result;
                if (author == null)
                {
                    return Results.NotFound("User not found (no response body)"); //returns status code 404
                }

                var res = new GetFollowsResponse(
                    Follows: followRepository
                        .GetFollowed(username)
                        .Result.Select(follow => follow.Followed)
                        .ToList()
                );
                return Results.Ok(res); // returns status code 200
            }
        );

        /* User follows/unfollows author*/
        app.MapPost(
            "/fllws/{username}",
            (
                string username,
                [FromHeader(Name = "Authorization")] string authorization,
                [FromQuery(Name = "latest")] int? latests,
                [FromBody] FollowRequest request,
                IFollowRepository followRepository
            ) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.Forbid(); //returns status code 403
                }

                if (!string.IsNullOrEmpty(request.Follow))
                {
                    var res = followRepository.AddFollowing(username, request.Follow);
                    return Results.NoContent(); //returns status code 204
                }

                if (!string.IsNullOrEmpty(request.Unfollow))
                {
                    var res = followRepository.RemoveFollowing(username, request.Unfollow);

                    return Results.NoContent(); //returns status code 204
                }

                return Results.NotFound("Must specify either 'Follow' or 'Unfollow'"); //returns status code 404
            }
        );

        app.MapGet("/latest", () => new { Latest });

        app.MapGet(
            "/msgs",
            (
                [FromHeader(Name = "Authorization")] string authorization,
                [FromQuery(Name = "latest")] int? latests,
                [FromQuery(Name = "no")] int? no,
                ICheepRepository cheepRepository
            ) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.Forbid(); //returns status code 403
                }

                var res = cheepRepository
                    .GetCheepsLimited(no ?? 32)
                    .Result.Select(cheep => new GetMessagesRequest(
                        cheep.Text,
                        cheep.Timestamp.ToString(),
                        cheep.Author.Name
                    ));
                return Results.Ok(res); //returns status code 200 and res
            }
        );

        app.MapGet(
            "/msgs/{username}",
            (
                string username,
                [FromHeader(Name = "Authorization")] string authorization,
                [FromQuery(Name = "latest")] int? latests,
                [FromQuery(Name = "no")] int? no,
                ICheepRepository cheepRepository,
                IAuthorRepository authorRepository
            ) =>
            {
                if (latests.HasValue)
                {
                    Latest = latests.Value;
                }

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.Forbid(); //returns status code 403
                }
                if (authorRepository.GetAuthorByName(username).Result is null)
                {
                    return Results.NotFound("User not found (no response body)");
                }

                var res = cheepRepository
                    .GetCheepsFromAuthorLimited(no ?? 32, username)
                    .Result.Select(cheep => new GetMessagesRequest(
                        cheep.Text,
                        cheep.Timestamp.ToString(),
                        cheep.Author.Name
                    ));
                return Results.Ok(res); //returns status code 200 and res
            }
        );

        app.MapPost(
            "/msgs/{username}",
            (
                string username,
                [FromHeader(Name = "Authorization")] string authorization,
                [FromQuery(Name = "latest")] int? latests,
                [FromBody] MessageRequest msgRequest,
                ICheepRepository cheepRepository,
                IAuthorRepository authorRepository
            ) =>
            {
                if (latests.HasValue)
                {
                    Latest = latests.Value;
                }

                if (authorization != "Basic c2ltdWxhdG9yOnN1cGVyX3NhZmUh")
                {
                    return Results.Forbid(); //returns status code 403
                }

                var author = authorRepository.GetAuthorByName(username).Result;

                if (author is null)
                {
                    return Results.NotFound();
                }

                cheepRepository.AddCheep(msgRequest.Content, author);

                return Results.NoContent(); //returns status code 204
            }
        );

        app.MapPost(
            "/register",
            (
                [FromQuery(Name = "latest")] int? latests,
                [FromBody] SignUpRequest request,
                IAuthorRepository authorRepository
            ) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                var res = authorRepository.CreateAuthor(request.Username, request.Email);

                /*returns 400 HTTP code if createAuthor Fails*/
                if (res == null)
                {
                    return Results.BadRequest(
                        "Possible reasons:\n - missing username \n- invalid email \n- password missing \n- username already taken"
                    );
                }

                /*returns 204 HTTP code*/
                return Results.NoContent();
            }
        );
    }
}
