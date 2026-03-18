using Chirp.Core;

namespace Chirp.Infrastructure.Chirp.Repositories;

/// <summary>
/// Defines method signatures for handling follows
/// </summary>
public interface IFollowRepository
{
    /// <summary>
    /// Used to add a new Follow - A Author follows another Author
    /// </summary>
    /// <param name="followerName">The name of the author who is following</param>
    /// <param name="followedName">The name of the author who is getting followed</param>
    public Task AddFollowing(string followerName, string followedName);

    /// <summary>
    /// Used to remove a Follow - An author no longer follows another author
    /// </summary>
    /// <param name="followerName">The name of the author who is following</param>
    /// <param name="followedName">The name of the author who is getting followed</param>
    public Task RemoveFollowing(string followerName, string followedName);

    /// <summary>
    /// Gets a list of follows where an author follows others.
    /// </summary>
    /// <param name="followerName">The name of the author who is following</param>
    /// <returns>List of follows</returns>
    public Task<List<Follow>> GetFollowed(string followerName);

    /// <summary>
    /// Gets a list of follows where others follows an author.
    /// </summary>
    /// <param name="followedName">The name of the author who is followed</param>
    /// <returns></returns>
    public Task<List<Follow>> GetFollowers(string followedName);
}
