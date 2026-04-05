using Chirp.Core;

namespace Chirp.Infrastructure.Chirp.Repositories;

/// <summary>
/// Defines method signatures for handling cheeps
/// </summary>
public interface ICheepRepository
{
    /// <summary>
    /// Gets cheeps for a given public page
    /// </summary>
    /// <param name="page">The page number</param>
    /// <returns>List of cheeps</returns>
    public Task<List<Cheep>> GetCheeps(int page);

    /// <summary>
    /// Gets a given number of cheeps
    /// </summary>
    /// <param name="amount">The number of cheeps</param>
    /// <returns>List of cheeps</returns>
    public Task<List<Cheep>> GetCheepsLimited(int amount);

    /// <summary>
    /// Gets cheeps made by a specific author and page
    /// </summary>
    /// <param name="page">The given page</param>
    /// <param name="authorName">The given Author </param>
    /// <returns>List of cheeps made the author for the specific page</returns>
    public Task<List<Cheep>> GetCheepsFromAuthor(int page, string authorName);

    /// <summary>
    /// Gets cheeps made by a specific author
    /// </summary>
    /// <param name="amount">The number of cheeps</param>
    /// <param name="authorName">The given Author </param>
    /// <returns>List of cheeps made the author for the specific page</returns>
    public Task<List<Cheep>> GetCheepsFromAuthorLimited(int amount, string authorName);

    /// <summary>
    /// Gets all cheeps from an author
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns>List of all cheeps made by the author</returns>
    public Task<List<Cheep>> GetAllCheepsFromAuthor(string authorName);

    /// <summary>
    /// Gets all the cheeps from all the different authors that an author follows
    /// </summary>
    /// <param name="authorName">Name of author who follows</param>
    /// <returns>List of cheeps</returns>
    public Task<List<Cheep>> GetAllCheepsFromFollowed(string authorName);

    /// <summary>
    /// Used to create a new cheep
    /// </summary>
    /// <param name="text">The cheep message</param>
    /// <param name="author">Author of cheep</param>
    public Task AddCheep(string text, Author author);

    /// <summary>
    /// Allows an author be added to the list of authors that have liked a cheep
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <param name="cheepId">The ID of the cheep</param>
    /// <returns>Task</returns>
    public Task AddLike(string authorName, long cheepId);

    /// <summary>
    /// Removes an author from the list of authors that have liked a cheep
    /// </summary>
    /// <param name="authorName"> The name of the author</param>
    /// <param name="cheepId">The ID of the cheep</param>
    /// <returns></returns>
    public Task RemoveLike(string authorName, long cheepId);

    /// <summary>
    /// Counts the amount of likes a cheep has
    /// </summary>
    /// <param name="cheepId">Id of cheep</param>
    /// <returns>Amount of likes</returns>
    public Task<int> CountLikes(long cheepId);

    /// <summary>
    /// Gets a list of all liked cheeps for a given author
    /// </summary>
    /// <param name="authorName">The name of the author in question</param>
    /// <returns>A list of cheeps</returns>
    public Task<List<Cheep>> GetAllLiked(string authorName);

    /// <summary>
    /// Finds and removes all instances of the authors name from the liked list of cheeps
    /// </summary>
    /// <param name="authorName">The name of the author</param>
    /// <returns>Task</returns>
    public Task DeleteAllLikes(string authorName);

    /// <summary>
    /// Gets a list of the 32 most liked cheeps
    /// </summary>
    /// <param name="page">The page number</param>
    /// <returns>A list of 32 cheeps</returns>
    public Task<List<Cheep>> GetTopLikedCheeps(int page);

    /// <summary>
    /// Deletes a cheep from the database
    /// </summary>
    /// <param name="cheepId">The id of the cheep</param>
    public Task DeleteCheep(long cheepId);

}
