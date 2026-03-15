using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Xunit;
using Assert = Xunit.Assert;

namespace Chirp.Web.Tests;


public class IntegrationTests : IAsyncLifetime
{

    private IAuthorRepository? _authorrepo;
    private ICheepRepository? _cheeprepo;
    private IFollowRepository? _followrepo;

    private TestUtilities? _utils;
    private CheepDbContext? _context;
    
    private IChirpService? _service;
    
    public async Task InitializeAsync()
    {
        _utils = new TestUtilities();
        _context = await _utils.CreateInMemoryDb();
        _authorrepo = new AuthorRepository(_context); 
        _cheeprepo = new CheepRepository(_context);
        _followrepo = new FollowRepository(_context);

        _service = new ChirpService(_cheeprepo, _authorrepo, _followrepo);
    }


    public Task DisposeAsync()
    {
        if (_utils == null)
        {
            return Task.CompletedTask;
        }
        _utils.CloseConnection();
        return Task.CompletedTask;
    }
    
    
    [Fact]
    public async Task TestAddCheep()
    {
        if (_utils == null || _cheeprepo == null || _authorrepo == null)
        {
            return;
        }
        var cheepsBefore = await _cheeprepo.GetCheepsFromAuthor(0, "Mellie Yost");
        
        var author = await _authorrepo.GetAuthorByName("Mellie Yost");

        if (author != null)
        {
            await _cheeprepo.AddCheep("hejj", author);
        
            var cheepsAfter = await _cheeprepo.GetCheepsFromAuthor(0, "Mellie Yost");
        
            Assert.True(cheepsBefore.Count != cheepsAfter.Count);
        
            await _utils.CloseConnection();
        }
    }
    


    
    [Fact]
    public async Task TestAddCheepchirpServiceNonExistingAuthor()
    {
        if (_service == null || _utils == null || _authorrepo == null)
        {
            return;
        }
        await _service.AddCheep("testest", "NewAuthor", "@newauthor.com");
    
        var author = await _authorrepo.GetAuthorByName("NewAuthor");
        
        Assert.NotNull(author);
        
        await _utils.CloseConnection();
        
    }
    
    [Fact]
    public async Task TestGetAllCheepsFromTimelineAndFollow()
    {
        //We first make a user follow another user. Show that the cheeps should now be the sum of their cheeps in their private timeline
        //Octavio Wagganer(15) follow Mellie Yost(7)
        if (_service == null)
        {
            return;
        }
        
        string authorname1 = "Octavio Wagganer";
        string authorname2 = "Mellie Yost";

        await _service.AddFollowing(authorname1, authorname2);
        var cheeps = await _service.GetCheepsForTimeline(authorname1, 1);
        
        Assert.Equal(22, cheeps.Count);

    }


    [Fact]
    public async Task TestCheepHasFollowIfAuthorFollows()
    {
        if (_service == null)
        {
            return;
        }
        
        string authorname1 = "Octavio Wagganer";
        string authorname2 = "Mellie Yost";

        await _service.AddFollowing(authorname1, authorname2);

        var cheeps = await _service.GetCheepsForTimeline(authorname1, 1);
        var cheep = cheeps.First();
        
        Assert.True(cheep.Follows);
    }


    [Fact]
    public async Task TestDeleteFollows()
    {
        if (_service == null || _authorrepo == null || _followrepo == null)
        {
            return;
        }
        
        await _authorrepo.CreateAuthor("erik", "hahaemail@gmail.com");
        await _authorrepo.CreateAuthor("lars", "bahaemail@gmail.com");
        await _followrepo.AddFollowing("erik", "lars");
        var oldfollows = await _service.GetFollowedDtos("erik");
        Assert.True(oldfollows.Count == 1);
        await _service.DeleteFromFollows("lars");
        
        var newfollows = await _service.GetFollowedDtos("erik");
        
        Assert.True(newfollows.Count == 0);


    }

    [Fact]
    public async Task TestCountingOfLikesOnCheep()
    {
        if (_service == null)
        {
            return;
        }
        
        await _service.AddLike("Octavio Wagganer", 1);
        await _service.AddLike("Mellie Yost", 1);

        var likes = await _service.CountLikes(1);
        
        Assert.Equal(2, likes);
    }


    [Fact]
    public async Task TestCanRemoveLike()
    {
        if (_service == null)
        {
            return;
        }
        
        await _service.AddLike("Octavio Wagganer", 1);
        var likes1Amount = await _service.CountLikes(1);
        await _service.RemoveLike("Octavio Wagganer", 1);
        var likes2Amount = await _service.CountLikes(1);
        
        Assert.True(likes1Amount != likes2Amount);
    }


    [Fact]
    public async Task TestCanRemoveLikeData()
    {
        if (_service == null)
        {
            return;
        }
        
        string author = "Octavio Wagganer";
        await _service.AddLike(author, 1);
        var likes1Amount = await _service.CountLikes(1);
        await _service.DeleteAllLikes(author);
        var likes2Amount = await _service.CountLikes(1);
        
        Assert.True(likes1Amount != likes2Amount);
        Assert.True(likes2Amount == 0);
    }
    
    [Fact]
    public async Task TestCanPostAndDelete()
    {
        if (_service == null || _authorrepo == null)
        {
            return;
        }
        
        await _authorrepo.CreateAuthor("erik", "hahaemail@gmail.com");
        var author = await _authorrepo.GetAuthorByName("erik");

        if (author == null)
        {
            return;
        }
        
        await _service.AddCheep("test", author.Name, author.Email);

        var authorCheepsBefore = await _service.GetAllCheepsFromAuthor(author.Name);
        var cheep = authorCheepsBefore.First();
        
        Assert.True(1 == authorCheepsBefore.Count);

        

        await _service.DeleteCheep(cheep.Id);
        
        
        var authorCheepsAfter = await _service.GetAllCheepsFromAuthor(author.Name);
        
        Assert.Empty(authorCheepsAfter);
    }
    
    [Fact]
    public async Task TestOtherUsersCantSeeDeletedCheeps()
    {
        if (_service == null || _authorrepo == null)
        {
            return;
        }
        
        await _authorrepo.CreateAuthor("erik", "hahaemail@gmail.com");
        var author = await _authorrepo.GetAuthorByName("erik");
        
        await _authorrepo.CreateAuthor("Lars", "larslarsen@gmail.com");
        var author2 = await _authorrepo.GetAuthorByName("Lars");

        if (author == null || author2 == null)
        {
            return;
        }

        await _service.AddCheep("tester", author.Name, author.Email);
        
        var author1CheepsBefore = await _service.GetCheepsFromAuthor(0, author.Name, author2.Name);
        var cheepBefore = author1CheepsBefore.First();
        
        Assert.True(1 == author1CheepsBefore.Count);
        Assert.Equal("tester", cheepBefore.Message);
        
        await _service.DeleteCheep(cheepBefore.Id);
        
        var author1CheepsAfter = await _service.GetCheepsFromAuthor(0, author.Name, author2.Name);
        
        Assert.Empty(author1CheepsAfter);
    }


    [Fact]
    public async Task TestUserCanAffectTopCheep()
    {
        if (_service == null)
        {
            return;
        }
        
        string author = "Octavio Wagganer";
        var topCheeps = await _service.GetTopLikedCheeps(author, 0);
        var topCheepBefore = topCheeps.First();
        
        await _service.AddLike(author, 5);
        
        
        var topCheeps2 = await _service.GetTopLikedCheeps(author, 0);
        
        var topCheepAfter = topCheeps2.First();
        
        Assert.NotEqual(topCheepBefore, topCheepAfter);
        Assert.Equal(5, topCheepAfter.Id);
        
    }


    [Fact]
    public async Task TestTopCheepGetsDeleted()
    {
        if (_service == null)
        {
            return;
        }
        
        string author = "Octavio Wagganer";
        await _service.AddLike(author, 5);
        var topCheeps = await _service.GetTopLikedCheeps(author, 0);
        var topCheepBefore = topCheeps.First();
        
        Assert.Equal(5, topCheepBefore.Id);

        await _service.DeleteCheep(5);
        var topCheepsAfter = await _service.GetTopLikedCheeps(author, 0);
        var topCheepAfter = topCheepsAfter.First();
        
        Assert.NotEqual(topCheepBefore, topCheepAfter);
    }
    
}
