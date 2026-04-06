using Azure.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Experimental.ProjectCache;

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
            async (
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

                var author = await authorRepository.GetAuthorByName(username);
                if (author == null)
                {
                    return Results.NotFound("User not found (no response body)"); //returns status code 404
                }

                var follows = await followRepository.GetFollowed(username);
                var res = new GetFollowsResponse(
                    Follows: follows.Select(follow => follow.Followed).ToList()
                );
                return Results.Ok(res); // returns status code 200
            }
        );

        /* User follows/unfollows author*/
        app.MapPost(
            "/fllws/{username}",
            async (
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
                    await followRepository.AddFollowing(username, request.Follow);
                    return Results.NoContent(); //returns status code 204
                }

                if (!string.IsNullOrEmpty(request.Unfollow))
                {
                    await followRepository.RemoveFollowing(username, request.Unfollow);
                    return Results.NoContent(); //returns status code 204
                }

                return Results.NotFound("Must specify either 'Follow' or 'Unfollow'"); //returns status code 404
            }
        );

        app.MapGet("/latest", () => new { Latest });

        app.MapGet(
            "/msgs",
            async (
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

                var cheeps = await cheepRepository.GetCheepsLimited(no ?? 32);
                var res = cheeps.Select(cheep => new GetMessagesRequest(
                    cheep.Text,
                    cheep.Timestamp.ToString(),
                    cheep.Author.Name
                ));
                return Results.Ok(res); //returns status code 200 and res
            }
        );

        app.MapGet(
            "/msgs/{username}",
            async (
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

                var author = await authorRepository.GetAuthorByName(username);

                if (author is null)
                {
                    return Results.NotFound("User not found (no response body)");
                }

                var cheeps = await cheepRepository.GetCheepsFromAuthorLimited(no ?? 32, username);
                var res = cheeps.Select(cheep => new GetMessagesRequest(
                    cheep.Text,
                    cheep.Timestamp.ToString(),
                    cheep.Author.Name
                ));
                return Results.Ok(res); //returns status code 200 and res
            }
        );

        app.MapPost(
            "/msgs/{username}",
            async (
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

                var author = await authorRepository.GetAuthorByName(username);

                if (author is null)
                {
                    return Results.NotFound();
                }

                await cheepRepository.AddCheep(msgRequest.Content, author);

                return Results.NoContent(); //returns status code 204
            }
        );

        app.MapPost(
            "/register",
            async (
                [FromQuery(Name = "latest")] int? latests,
                [FromBody] SignUpRequest request,
                IAuthorRepository authorRepository
            ) =>
            {
                if (latests.HasValue)
                    Latest = latests.Value;

                try
                {
                    await authorRepository.CreateAuthor(request.Username, request.Email);
                }
                catch (Exception)
                { /*returns 400 HTTP code if createAuthor Fails*/
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
