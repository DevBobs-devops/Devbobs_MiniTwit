using Chirp.Core;
using Chirp.Infrastructure.Metrics;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

/// <summary>
/// Used to handle data logic for cheeps.
/// Includes methods for accessing and handling cheep data.
/// </summary>
public class CheepRepository : ICheepRepository
{
    private readonly CheepDbContext _context;

    public CheepRepository(CheepDbContext context)
    {
        this._context = context;
    }

    public async Task<List<Cheep>> GetCheeps(int page)
    {
        var query = (from cheep in _context.Cheeps orderby cheep.Timestamp descending select cheep)
            .Include(c => c.Author)
            .Skip((page - 1) * 32)
            .Take(32);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Cheep>> GetCheepsLimited(int amount)
    {
        var query = (from cheep in _context.Cheeps orderby cheep.Timestamp descending select cheep)
            .Include(c => c.Author)
            .Take(amount);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(int page, string authorName)
    {
        var query = (
            from cheep in _context.Cheeps
            where cheep.Author.Name == authorName
            orderby cheep.Timestamp descending
            select cheep
        )
            .Include(c => c.Author)
            .Skip((page - 1) * 32)
            .Take(32);
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> GetCheepsFromAuthorLimited(int amount, string authorName)
    {
        var query = (
            from cheep in _context.Cheeps
            where cheep.Author.Name == authorName
            orderby cheep.Timestamp descending
            select cheep
        )
            .Include(c => c.Author)
            .Take(amount);
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> GetAllCheepsFromAuthor(string authorName)
    {
        var query = (
            from cheep in _context.Cheeps
            where cheep.Author.Name == authorName
            orderby cheep.Timestamp descending
            select cheep
        ).Include(c => c.Author);
        var result = await query.ToListAsync();
        return result;
    }

    public async Task<List<Cheep>> GetAllCheepsFromFollowed(string authorName) //Made with the help of ChatGPT
    {
        var query = (
            from cheep in _context.Cheeps
            where
                (
                    from follow in _context.Follows
                    where follow.Follower == authorName
                    select follow.Followed
                ).Contains(cheep.Author.Name)
            select cheep
        ).Include(c => c.Author);

        var result = await query.ToListAsync();
        return result;
    }

    public async Task AddCheep(string text, Author author)
    {
        if (text.Length <= 0 || text.Length > 160)
        {
            throw new ArgumentException("Text must be between 0 and 160 characters");
        }

        Cheep cheep = new Cheep()
        {
            Author = author,
            AuthorId = author.Id,
            Text = text,
            Timestamp = DateTime.UtcNow
        };

        await _context.Cheeps.AddAsync(cheep);
        await _context.SaveChangesAsync();
        CheepMetrics.RecordCheep(author.Name);
    }

    public async Task AddLike(string authorName, int cheepId)
    {
        var currentLikes = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == cheepId);
        if (!currentLikes.Likes.Contains(authorName))
        {
            currentLikes.Likes.Add(authorName);
            currentLikes.NrLikes +=1;
            _context.SaveChanges();
        }
    }

    public async Task RemoveLike(string authorName, int cheepId)
    {
        var currentLikes = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == cheepId);
        if (currentLikes.Likes.Contains(authorName))
        {
            currentLikes.Likes.Remove(authorName);
            currentLikes.NrLikes -=1;
            _context.SaveChanges();
        }
    }

    public async Task<int> CountLikes(int cheepId)
    {
        var cheep = await _context.Cheeps.FirstAsync(cheep =>cheep.CheepId == cheepId);
        return cheep.NrLikes;
    }

    public async Task<List<Cheep>> GetAllLiked(string authorName)
    {
        //Fetch in memory, might be bad
        var Cheeps = await _context.Cheeps.Include(c => c.Author).ToListAsync();
        
        //var likedCheeps = await _context.Cheeps.Where(cheep => cheep.Likes.Contains(authorName)).Include(c => c.Author).ToListAsync();

        var likedCheeps = Cheeps.Where(c => c.Likes.Contains(authorName)).ToList();
        
        return likedCheeps;
    }

    public async Task DeleteAllLikes(string authorName)
    {
        //https://stackoverflow.com/questions/1586013/how-to-do-select-all-in-linq-to-sql
        var likedCheeps = await _context
            .Cheeps.Where(cheep => cheep.Likes.Contains(authorName))
            .ToListAsync();

        foreach (var likes in likedCheeps)
        {
            likes.Likes.Remove(authorName);
            likes.NrLikes -= 1;
        }

        _context.SaveChanges();
    }

    public async Task<List<Cheep>> GetTopLikedCheeps(int page) //This is not a great way to do it. But Keep It Simple Stupid
    {
        //https://stackoverflow.com/questions/5344805/linq-orderby-descending-query
        var query = (from cheep in _context.Cheeps
                .OrderByDescending(c => c.NrLikes)
            select cheep).Skip((page -1) * 32).Take(32).Include(c => c.Author);

        var cheeps = await query.ToListAsync();

        return cheeps;
    }


    public async Task DeleteCheep(long cheepId)
    {
        var cheep = await _context.Cheeps.FindAsync(cheepId);
        if (cheep != null)
        {
            _context.Cheeps.Remove(cheep);
            _context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Cheep not found");
            throw new ArgumentException("Cheep not found");
        }
    }
}
