using Chirp.Core;
using Chirp.Infrastructure.Metrics;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Chirp.Repositories;

/// <summary>
/// Used to handle data logic for Follows.
/// Includes methods for accessing and handling follow data.
/// </summary>
public class FollowRepository : IFollowRepository
{
    private readonly CheepDbContext _context;
    private readonly FollowMetrics _metrics;


    public FollowRepository(CheepDbContext context)
    {
        _context = context;
        _metrics = new FollowMetrics();
    }
    
    
    public async Task AddFollowing(string followerName, string followedName)
    {
        var follow = _context.Follows.FirstOrDefault(follow => follow.Follower == followerName && follow.Followed == followedName);
        if (follow != null) //we must ensure it is not already in the database
        {
            return;
        }
        var newFollow = new Follow()
        {
            Follower = followerName,
            Followed = followedName
        };
        
        await _context.Follows.AddAsync(newFollow);
        await _context.SaveChangesAsync();
        _metrics.IncrementFollower(followedName);
    }


   
    public async Task RemoveFollowing(string followerName, string followedName)
    {
        //lambda expression inspiration:https://stackoverflow.com/questions/30928566/how-to-delete-a-row-from-database-using-lambda-linq
        //We first find the follow that we want to remove
        var follow = _context.Follows.FirstOrDefault(follow => follow.Follower == followerName && follow.Followed == followedName);

        if ( follow != null )
        {
            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();
            _metrics.DecrementFollower(followedName);
        }
        
    }


    
    public async Task<List<Follow>> GetFollowed(string followerName)
    {
        var query = (from follow in _context.Follows
            where follow.Follower == followerName
            select follow);
        var result = await query.ToListAsync();
        return result;

    }
    
    public async Task<List<Follow>> GetFollowers(string followedName)
    {
        var query = (from follow in _context.Follows
            where follow.Followed == followedName
            select follow);
        var result = await query.ToListAsync();
        return result;

    }
}