using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

/// <summary>
/// Used to handle data logic for author.
/// Includes methods for accessing and handling author data.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    
    private readonly CheepDbContext _context;


    public AuthorRepository(CheepDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<Author?> GetAuthorByName(string authorName)
    {
        var query = (from author in _context.Authors
            where author.Name == authorName
            select author);
        var result = await query.ToListAsync();
        
        if ( result.Count == 0 )
        {
            return null;
        }
        return result[0];
    }


    public async Task<Author?> GetAuthorByEmail(string email)
    {
        var query = (from author in _context.Authors
                where author.Email == email
                select author);
        var result = await query.ToListAsync();
        
        if ( result.Count == 0 )
        {
            return null;
        }
        return result[0];
    }

    
    public async Task CreateAuthor(string authorName, string email)
    {

        //Extra check for input validation
        if ( authorName.Contains('/') || authorName.Contains('\\') )
        {
            throw new ArgumentException();
        }
       
        //Check if authorName already exists
        if (await GetAuthorByName(authorName) != null)
        {
            return;
        }
        
        int maxId; 
        
        if(_context.Cheeps.Count() == 0)
        {
            maxId = -1; //-1 so it gets assigned to 0
        }
        else
        {
            maxId = _context.Cheeps.Max(cheep => cheep.CheepId);     
        }
        
        //Create new author
        var newAuthor = new Author()
        {
            Id = maxId + 1,
            Name = authorName,
            Email = email
        };
        
        await _context.Authors.AddAsync(newAuthor);
        await _context.SaveChangesAsync();
    }
    
}