using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.DataTransferObjects;

namespace Chirp.Infrastructure.Chirp.Services;

public interface IChirpService
{
    /// <summary>
    /// This method is for use on the public timeline.
    /// It returns a list of 0-32 cheeps, ordered from newest to oldest, by any author.
    /// The first 32*limit cheeps are skipped, to allow pagination
    /// </summary>
    /// <param name="limit"> The page number </param>
    /// <returns> A list of CheepDto objects </returns>
    public Task<List<CheepDto>> GetCheeps(int limit);

    /// <summary>
    /// This method is for use on the public timeline.
    /// It returns a list of 0-32 cheeps, ordered from newest to oldest, by any author.
    /// The first 32*limit cheeps are skipped, to allow pagination
    /// This method sets the "following" attribute on the returned cheepdtos correctly, in order to show the correct follow/ unfollow button
    /// </summary>
    /// <param name="limit"> The page number </param>
    /// <param name="followerName"> The user who is viewing the timeline </param>
    /// <returns> A list of CheepDto objects </returns>
    public Task<List<CheepDto>> GetCheeps(int limit, string followerName);

    /// <summary>
    /// This method is for use on a private timeline.
    /// It returns a list of 0-32 cheeps, ordered from newest to oldest, by the specified author.
    /// The first 32*limit cheeps are skipped, to allow pagination
    /// </summary>
    /// <param name="page"> The page number </param>
    /// <param name="authorName"> The name of the author whose cheeps you want </param>
    /// <param name="spectatingAuthorName"> The name of the author viewing the cheeps </param>
    /// <returns> A list of CheepDto objects </returns>
    public Task<List<CheepDto>> GetCheepsFromAuthor(
        int page,
        string authorName,
        string spectatingAuthorName
    );

    /// <summary>
    /// This method allows adding new cheeps to the database
    /// </summary>
    /// <param name="text"> The contents of the cheep </param>
    /// <param name="authorName"> The username of the author </param>
    /// <param name="email"> The email of the author </param>
    /// <returns> Task </returns>
    public Task AddCheep(string text, string authorName, string email);

    /// <summary>
    /// This method allows for getting an Author object from their username
    /// </summary>
    /// <param name="authorName"> The username of the author </param>
    /// <returns> The Author object matching the username </returns>
    public Task<AuthorDto?> GetAuthorDtoByName(string authorName);

    /// <summary>
    /// This method allows adding new tuples to the Follow relation
    /// </summary>
    /// <param name="followerName"> The username of the author that should follow another </param>
    /// <param name="followedName"> The username of the author that should be followed </param>
    /// <returns> Task </returns>
    public Task AddFollowing(string followerName, string followedName);

    /// <summary>
    /// This method allows removing tuples from the Follow relation
    /// </summary>
    /// <param name="followerName"> The username of the author that should follow another </param>
    /// <param name="followedName"> The username of the author that should be followed </param>
    /// <returns> Task </returns>
    public Task RemoveFollowing(string followerName, string followedName);

    /// <summary>
    /// This method allows getting all the names of users followed by a specified author
    /// </summary>
    /// <param name="followerName"> The username of the author </param>
    /// <returns> A list of FollowDTOs</returns>
    public Task<List<FollowDto>> GetFollowedDtos(string followerName);

    /// <summary>
    /// This method allows getting all cheeps by a specified author
    /// </summary>
    /// <param name="authorName"> The username of the author</param>
    /// <returns> A list of all cheeps by the author </returns>
    public Task<List<CheepDto>> GetAllCheepsFromAuthor(string authorName);

    /// <summary>
    /// This method allows getting cheeps for an authors timeline
    /// </summary>
    /// <param name="authorName"> The username of the author </param>
    /// <param name="page"> The page number </param>
    /// <returns></returns>
    public Task<List<CheepDto>> GetCheepsForTimeline(string authorName, int page);

    /// <summary>
    /// Used to delete all instances where user is followed by others or follows others
    /// </summary>
    /// <param name="authorName">Name of the author you want to remove from the follow table</param>
    /// <returns></returns>
    public Task DeleteFromFollows(string authorName);

    /// <summary>
    /// Used to make an author like a cheep
    /// </summary>
    /// <param name="authorName">Name of author who likes</param>
    /// <param name="cheepId">The id of cheep the author likes</param>
    /// <returns>Task</returns>
    public Task AddLike(string authorName, int cheepId);

    /// <summary>
    /// Used to remove a like on a cheep by an author
    /// </summary>
    /// <param name="authorName">Name of author</param>
    /// <param name="cheepId">ID of the cheep</param>
    /// <returns>Task</returns>
    public Task RemoveLike(string authorName, int cheepId);

    /// <summary>
    /// Gets the number of likes for a given cheep
    /// </summary>
    /// <param name="cheepId">The ID of the cheep in question</param>
    /// <returns>The amount as an integer</returns>
    public Task<int> CountLikes(int cheepId);

    /// <summary>
    /// Gets a list of all cheeps liked by a given author, as CheepDTOs
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>A list of CheepDTOs</returns>
    public Task<List<CheepDto>> GetAllLiked(string authorName);

    /// <summary>
    /// Deletes alle likes by a specific author from the database, to be used when deleting an account
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>Task</returns>
    public Task DeleteAllLikes(string authorName);

    /// <summary>
    /// Gets a list of the 32 most liked cheeps
    /// </summary>
    /// /// <param name="authorName">The name of the author</param>
    /// /// <param name="page">The page number</param>
    /// <returns>A list of 32 CheepDTOs</returns>
    public Task<List<CheepDto>> GetTopLikedCheeps(string authorName, int page);

    /// <summary>
    /// Deletes a cheep from the database
    /// </summary>
    /// <param name="cheepId">The id of the cheep</param>
    public Task DeleteCheep(long cheepId);
}

public class ChirpService : IChirpService
{
    private ICheepRepository _cheepRepository;
    private IAuthorRepository _authorRepository;
    private IFollowRepository _followRepository;

    public ChirpService(
        ICheepRepository cheepRepository,
        IAuthorRepository authorRepository,
        IFollowRepository followRepository
    )
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
    }

    public async Task<List<CheepDto>> GetCheeps(int page)
    {
        if (page == 0)
        {
            page = 1;
        }
        var queryresult = await _cheepRepository.GetCheeps(page);

        var follows = await _followRepository.GetFollowed("");

        var result = await ConvertCheepsToCheepDtos(queryresult, "", follows); // follower is empty since it doesnt matter for users that arent logged in
        return result;
    }

    public async Task<List<CheepDto>> GetCheeps(int page, string followerName) //for use when logged in, allows us to display the correct button, either follow or unfollow
    {
        if (page == 0)
        {
            page = 1;
        }
        var queryresult = await _cheepRepository.GetCheeps(page);
        var follows = await _followRepository.GetFollowed(followerName);
        var result = await ConvertCheepsToCheepDtos(queryresult, followerName, follows);
        return result;
    }

    public async Task<List<CheepDto>> GetCheepsFromAuthor(
        int page,
        string authorName,
        string spectatingAuthorName
    )
    {
        if (page == 0)
        {
            page = 1;
        }
        var queryresult = await _cheepRepository.GetCheepsFromAuthor(page, authorName);
        var follows = await _followRepository.GetFollowed(spectatingAuthorName);
        var result = await ConvertCheepsToCheepDtos(queryresult, spectatingAuthorName, follows);
        return result;
    }

    public async Task<AuthorDto?> GetAuthorDtoByName(string authorName)
    {
        var author = await _authorRepository.GetAuthorByName(authorName);

        if (author != null)
        {
            var dto = new AuthorDto { Username = author.Name, Email = author.Email };

            return dto;
        }
        return null;
    }

    public async Task AddCheep(string text, string authorName, string email)
    {
        if (await _authorRepository.GetAuthorByName(authorName) == null) //if statement will never be true, but it was a requirement to handle this in this way
        {
            await _authorRepository.CreateAuthor(authorName, email);
        }
        var author = await _authorRepository.GetAuthorByEmail(email);
        if (author == null)
        {
            return;
        }
        await _cheepRepository.AddCheep(text, author);
    }

    public async Task AddFollowing(string followerName, string followedName)
    {
        await _followRepository.AddFollowing(followerName, followedName);
    }

    public async Task RemoveFollowing(string followerName, string followedName)
    {
        await _followRepository.RemoveFollowing(followerName, followedName);
    }

    public async Task<List<FollowDto>> GetFollowedDtos(string followerName)
    {
        var followedDtos = new List<FollowDto>();
        var followed = await _followRepository.GetFollowed(followerName);
        foreach (var followee in followed)
        {
            var dto = new FollowDto { Followed = followee.Followed };
            followedDtos.Add(dto);
        }
        return followedDtos;
    }

    public async Task DeleteFromFollows(string authorName)
    {
        //Delete all relations where user is followed by others
        var follows = await _followRepository.GetFollowers(authorName);
        foreach (var follow in follows)
        {
            await _followRepository.RemoveFollowing(follow.Follower, follow.Followed);
        }
        //Delete all relations where others follow the user
        follows = await _followRepository.GetFollowed(authorName);
        foreach (var follow in follows)
        {
            await _followRepository.RemoveFollowing(follow.Follower, follow.Followed);
        }
    }

    public async Task<List<CheepDto>> GetAllCheepsFromAuthor(string authorName)
    {
        var cheeps = await _cheepRepository.GetAllCheepsFromAuthor(authorName);
        var follows = await _followRepository.GetFollowed(authorName);
        var dtos = await ConvertCheepsToCheepDtos(cheeps, authorName, follows);
        return dtos;
    }

    //Gets a complete, sorted list of all cheeps that could go on a timeline
    private async Task<List<CheepDto>> GetAllCheepsForTimeline(string authorName)
    {
        var follows = await _followRepository.GetFollowed(authorName);

        var cheepsByAuthor = await _cheepRepository.GetAllCheepsFromAuthor(authorName);
        var cheepsByFollowed = await _cheepRepository.GetAllCheepsFromFollowed(authorName);

        var cheepsByAuthorDtos = await ConvertCheepsToCheepDtos(
            cheepsByAuthor,
            authorName,
            follows
        );
        var cheepsByFollowedDtos = await ConvertCheepsToCheepDtos(
            cheepsByFollowed,
            authorName,
            follows
        );

        // Combine lists
        cheepsByAuthorDtos.AddRange(cheepsByFollowedDtos);

        var result = cheepsByAuthorDtos.OrderByDescending(c => c.Timestamp).ToList();
        return result;
    }

    public async Task<List<CheepDto>> GetCheepsForTimeline(string authorName, int page) //ensures only 32 cheeps are returned
    {
        var allDtos = await GetAllCheepsForTimeline(authorName);
        var result = new List<CheepDto>();
        var start = (page - 1) * 32;
        if (start < 0)
            start = 0;
        if (start < allDtos.Count)
        {
            for (int i = start; i < allDtos.Count && i - start < 32; i++)
            {
                result.Add(allDtos[i]);
            }
        }
        return result;
    }

    /// <summary>
    /// Converts a lists of cheeps to cheepDTOs, automatically setting all fields correctly
    /// </summary>
    /// <param name="cheeps"> The list of cheeps to be converted </param>
    /// <param name="authorName"> The name of the author who will view the cheeps</param>
    /// <returns> A list of cheepDTOs</returns>
    private async Task<List<CheepDto>> ConvertCheepsToCheepDtos(
        List<Cheep> cheeps,
        string authorName,
        List<Follow> follows
    )
    {
        var result = new List<CheepDto>();
        foreach (var cheep in cheeps)
        {
            bool isFollowing = false;
            bool isLiking = false;
            //int likesamount = cheep.Likes.Count;
            foreach (var follow in follows) // this could be more efficient
            {
                if (follow.Followed == cheep.Author.Name)
                {
                    isFollowing = true;
                }
            }

            if (cheep.Likes.Contains(authorName))
            {
                isLiking = true;
            }

            var dto = new CheepDto
            {
                Author = cheep.Author.Name,
                Message = cheep.Text,
                Timestamp = cheep.Timestamp,
                Follows = isFollowing,
                Liked = isLiking,
                Likes = cheep.NrLikes,
                Id = cheep.CheepId,
            };
            result.Add(dto);
        }
        return result;
    }

    public async Task AddLike(string authorName, int cheepId)
    {
        await _cheepRepository.AddLike(authorName, cheepId);
    }

    public async Task RemoveLike(string authorName, int cheepId)
    {
        await _cheepRepository.RemoveLike(authorName, cheepId);
    }

    public async Task<int> CountLikes(int cheepId)
    {
        return await _cheepRepository.CountLikes(cheepId);
    }

    public async Task<List<CheepDto>> GetAllLiked(string authorName)
    {
        var cheeps = await _cheepRepository.GetAllLiked(authorName);
        var follows = await _followRepository.GetFollowed(authorName);
        var dtos = await ConvertCheepsToCheepDtos(cheeps, authorName, follows);
        return dtos;
    }

    public async Task DeleteAllLikes(string authorName)
    {
        await _cheepRepository.DeleteAllLikes(authorName);
    }

    public async Task<List<CheepDto>> GetTopLikedCheeps(string authorName, int page)
    {
        var cheeps = await _cheepRepository.GetTopLikedCheeps(page);
        var follows = await _followRepository.GetFollowed(authorName);
        var cheepDtos = await ConvertCheepsToCheepDtos(cheeps, authorName, follows);
        return cheepDtos;
    }

    public async Task DeleteCheep(long cheepId)
    {
        await _cheepRepository.DeleteCheep(cheepId);
    }
}
