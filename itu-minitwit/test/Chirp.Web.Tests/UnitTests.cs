using Chirp.Core;
using Chirp.Infrastructure.Chirp.Repositories;
using Chirp.Infrastructure.Chirp.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Web.Tests;

public class UnitTests : IAsyncLifetime
{
    private TestUtilities? _utils;
    private CheepDbContext? _context;
    private ICheepRepository? _cheepRepository;
    private IAuthorRepository? _authorRepository;
    private IFollowRepository? _followRepository;
    private IChirpService? _chirpService;

    public async Task InitializeAsync()
    {
        //Arrange
        _utils = new TestUtilities();
        _context = await _utils.CreateInMemoryDb();
        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);
        _followRepository = new FollowRepository(_context);
        _chirpService = new ChirpService(_cheepRepository, _authorRepository, _followRepository);
    }

    public Task DisposeAsync()
    {
        _utils?.CloseConnection();
        return Task.CompletedTask;
    }

    // ------- CheepRepository --------
    [Fact]
    public async Task TestGetCheepsAmount()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(0);

        //Assert
        Assert.Equal(32, cheeps.Count);
    }

    [Fact]
    public async Task TestWhenGetCheepsFromNegativePage()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(-1);
        var cheep = cheeps[0];

        //Assert
        Assert.Equal("Starbuck now is what we hear the worst.", cheep.Text); //returns to page 1
    }

    [Fact]
    public async Task TestGetCheepsPage1()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(0);
        var cheep = cheeps[0];

        //Assert
        Assert.Equal("Starbuck now is what we hear the worst.", cheep.Text);
    }

    [Fact]
    public async Task TestGetCheepsPage2()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(2);
        var cheep = cheeps[0];

        //Assert
        Assert.Equal(
            "In the morning of the wind, some few splintered planks, of what present avail to him.",
            cheep.Text
        );
    }

    [Fact]
    public async Task TestGetCheepsLastPage()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(21);

        //Assert
        Assert.True(cheeps.Count == 17);
    }

    [Fact]
    public async Task TestGetCheepsBeyondLimit()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheeps(32);

        //Assert
        Assert.True(cheeps.Count == 0);
    }

    [Fact]
    public async Task TestGetCheepsFromExistingAuthor()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(0, "Jacqualine Gilcoine");
        var cheep = cheeps[0];

        //Assert
        Assert.True(cheep.Author.Name == "Jacqualine Gilcoine" && cheeps.Count == 32); //we know Jacqualine is the first author on the public timeline.
    }

    [Fact]
    public async Task TestGetCheepsFromNonExistingAuthor()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(0, "Eksisterer Ikke");

        //Assert
        Assert.True(cheeps.Count == 0);
    }

    [Fact]
    public async Task TestGetCheepsFromAuthorNegativePage()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(-1, "Jacqualine Gilcoine");
        var cheep = cheeps[0];

        //Assert
        Assert.Equal("Starbuck now is what we hear the worst.", cheep.Text);
    }

    [Fact]
    public async Task TestGetCheepsFromAuthorPage2()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(2, "Jacqualine Gilcoine");
        var cheep = cheeps[0];

        //Assert
        Assert.Equal("What a relief it was the place examined.", cheep.Text);
    }

    [Fact]
    public async Task TestGetCheepsFromAuthorLastPage()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(12, "Jacqualine Gilcoine");
        var cheep = cheeps[0];

        //Assert
        Assert.True(cheeps.Count == 7 && cheep.Author.Name == "Jacqualine Gilcoine");
    }

    [Fact]
    public async Task TestGetCheepsFromAuthorBeyondLimit()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetCheepsFromAuthor(20, "Jacqualine Gilcoine");

        //Assert
        Assert.True(cheeps.Count == 0);
    }

    [Fact]
    public async Task TestGetAllCheepsFromAuthor()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var result = (await _cheepRepository.GetAllCheepsFromAuthor("Adrian")).Count;

        //Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task TestGetAllCheepsFromNonexistingAuthor()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var result = (await _cheepRepository.GetAllCheepsFromAuthor("migg")).Count;

        //Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task TestAddCheepCorrectInput()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }
        //Arrange
        var cheepsBefore = _context.Cheeps.Count();
        Author author = new Author()
        {
            Id = _context.Authors.Count() + 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        //Act
        await _cheepRepository.AddCheep("Hejsa", author);
        var cheepsAfter = _context.Cheeps.Count();

        //Assert
        Assert.True(cheepsAfter == cheepsBefore + 1);
    }

    [Fact]
    public async Task TestAddCheepLengthConstraint()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }
        //Arrange
        Author author = new Author()
        {
            Id = 200,
            Name = "Test1",
            Email = "test1@mail.com",
        };
        var invalidCheep = new String('a', 161);

        //Assert and act
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _cheepRepository.AddCheep(invalidCheep, author)
        );
    }

    [Fact]
    public async Task TestAddLike()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        var likesBefore = _context.Cheeps.ToList()[1].Likes.Count;

        //Act
        await _cheepRepository.AddLike("Mellie Yost", 2);
        await _context.SaveChangesAsync();
        var likesAfter = _context.Cheeps.ToList()[1].Likes.Count;

        //Assert
        Assert.True(likesAfter == likesBefore + 1);
    }

    [Fact]
    public async Task TestAddLikeInvalidCheepId()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act and assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cheepRepository.AddLike("hej", 800000)
        );
    }

    [Fact]
    public async Task TestRemoveLike()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        _context.Cheeps.ToList()[1].Likes.Add("Mellie Yost");
        var likesBefore = _context.Cheeps.ToList()[1].Likes.Count;

        //Act
        await _cheepRepository.RemoveLike("Mellie Yost", 2);
        var likesAfter = _context.Cheeps.ToList()[1].Likes.Count;

        //Assert
        Assert.True(likesAfter == likesBefore - 1);
    }

    [Fact]
    public async Task TestRemoveLikeInvalidAuthor()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        var cheep = _context.Cheeps.ToList()[1];
        cheep.Likes.Add("Mellie Yost");
        var before = cheep.Likes.Count;
        //Act
        await _cheepRepository.RemoveLike("TestAuthor", cheep.CheepId);
        var after = cheep.Likes.Count;

        //Assert
        Assert.Equal(before, after);
    }

    [Fact]
    public async Task TestRemoveLikeInvalidCheepId()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Assert and act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cheepRepository.RemoveLike("Mellie Yost", 80000)
        );
    }

    [Fact]
    public async Task TestCountLikes()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        var before = await _cheepRepository.CountLikes(5);
        var currentLikes = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        currentLikes.Likes.Add("Mellie Yost");
        currentLikes.NrLikes = currentLikes.Likes.Count;

        await _context.SaveChangesAsync();

        //Act
        var after = await _cheepRepository.CountLikes(5);

        //Assert
        Assert.True(after == before + 1);
    }

    [Fact]
    public async Task TestCountLikesInvalidCheepId()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Assert and act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _cheepRepository.CountLikes(80000)
        );
    }

    [Fact]
    public async Task TestGetAllLikedNoLikes()
    {
        if (_cheepRepository == null || _utils == null)
        {
            return;
        }

        //Act
        var cheeps = await _cheepRepository.GetAllLiked("Mellie Yost");

        //Assert
        Assert.Empty(cheeps);
    }

    [Fact]
    public async Task TestGetAllLikedReturnsCorrectCheep()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        var currentLikes = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        currentLikes.Likes.Add("Mellie Yost");
        await _context.SaveChangesAsync();

        //Act
        var cheeps = await _cheepRepository.GetAllLiked("Mellie Yost");
        var cheep = cheeps[0].CheepId;

        //Assert
        Assert.Equal(5, cheep);
    }

    [Fact]
    public async Task TestDeleteAllLikes()
    {
        if (_cheepRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Arrange
        var currentLikes = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        currentLikes.Likes.Add("Mellie Yost");
        await _context.SaveChangesAsync();
        var cheep = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        var before = cheep.Likes.Count;

        //Act
        await _cheepRepository.DeleteAllLikes("Mellie Yost");
        var after = cheep.Likes.Count;

        //Assert
        Assert.NotEqual(before, after);
    }

    [Fact]
    public async Task TestGetTopLikedCheeps()
    {
        if (_context == null || _cheepRepository == null || _utils == null)
        {
            return;
        }

        //Arrange
        var cheep1 = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        cheep1.Likes.Add("Mellie Yost");
        cheep1.Likes.Add("Adrian");
        cheep1.NrLikes = cheep1.Likes.Count;
        var cheep2 = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 1);
        cheep2.Likes.Add("Mellie Yost");
        cheep2.NrLikes = cheep2.Likes.Count;
        await _context.SaveChangesAsync();

        //Act
        var result = await _cheepRepository.GetTopLikedCheeps(1);
        var topcheep = result[0];
        var secondtopcheep = result[1];

        //Assert
        Assert.Equal(5, topcheep.CheepId);
        Assert.Equal(1, secondtopcheep.CheepId);
    }

    [Fact]
    public async Task TestGetTopLikedCheepsNegativePage()
    {
        if (_context == null || _cheepRepository == null || _utils == null)
        {
            return;
        }

        //Arrange
        var cheep1 = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 5);
        cheep1.Likes.Add("Mellie Yost");
        cheep1.Likes.Add("Adrian");
        cheep1.NrLikes = cheep1.Likes.Count;

        var cheep2 = await _context.Cheeps.FirstAsync(cheep => cheep.CheepId == 1);
        cheep2.Likes.Add("Mellie Yost");
        cheep2.NrLikes = cheep2.Likes.Count;

        await _context.SaveChangesAsync();

        //Act
        var result = await _cheepRepository.GetTopLikedCheeps(-10);
        var topcheep = result[0];
        var secondtopcheep = result[1];

        //Assert
        Assert.Equal(5, topcheep.CheepId);
        Assert.Equal(1, secondtopcheep.CheepId);
    }

    [Fact]
    public async Task CanDeleteSingleCheep()
    {
        if (_cheepRepository == null || _context == null)
        {
            return;
        }

        //Arrange
        Cheep cheep1 = _context.Cheeps.FirstOrDefault(cheep => cheep.CheepId == 1)!;

        //Act
        await _cheepRepository.DeleteCheep(1);

        Cheep cheep2 = _context.Cheeps.FirstOrDefault(cheep => cheep.CheepId == 1)!;

        //Assert
        Assert.NotNull(cheep1);
        Assert.Null(cheep2);
    }

    [Fact]
    public async Task CanNotDeleteNonExistingCheep()
    {
        if (_cheepRepository == null)
        {
            return;
        }
        try
        {
            //Arrange
            int cheepId = 9999;

            //Act
            await _cheepRepository.DeleteCheep(cheepId);
            cheepId = 0;
            Assert.Equal(0, cheepId);
        }
        catch (ArgumentException e)
        {
            //Assert
            Assert.Equal("Cheep not found", e.Message);
        }
    }

    // ------- Author Repository --------

    [Fact]
    public async Task TestGetAuthorByNameExistingName()
    {
        if (_authorRepository == null)
        {
            return;
        }

        //Act
        var result = await _authorRepository.GetAuthorByName("Mellie Yost");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("Mellie Yost", result.Name);
    }

    [Fact]
    public async Task TestGetAuthorByNameNonExistingName()
    {
        if (_authorRepository == null)
        {
            return;
        }

        //Act
        var result = await _authorRepository.GetAuthorByName("TestName");

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TestGetAuthorByEmailExistingEmail()
    {
        if (_authorRepository == null)
        {
            return;
        }

        //Act
        var result = await _authorRepository.GetAuthorByEmail("Malcolm-Janski@gmail.com");

        //Assert
        Assert.NotNull(result);
        Assert.Equal("Malcolm Janski", result.Name);
    }

    [Fact]
    public async Task TestGetAuthorByNameNonExistingEmail()
    {
        if (_authorRepository == null)
        {
            return;
        }

        //Act
        var result = await _authorRepository.GetAuthorByEmail("TestEmail");

        //Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task TestCreateAuthor()
    {
        if (_authorRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Act
        await _authorRepository.CreateAuthor("Filifjonken", "fili@mail.com");
        var query = (
            from author in _context.Authors
            where author.Name == "Filifjonken"
            select author
        );
        var result = query.ToList();

        //Assert
        Assert.True(result[0].Name == "Filifjonken");
    }

    [Fact]
    public async Task TestCreateDuplicateAuthorName()
    {
        if (_authorRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Act
        await _authorRepository.CreateAuthor("Filifjonken", "test@mail.com");
        await _authorRepository.CreateAuthor("Filifjonken", "fili@mail.com");
        var query = (
            from author in _context.Authors
            where author.Name == "Filifjonken"
            select author
        );
        var result = query.ToList();

        //Assert
        Assert.True(result.Count == 1); // only 1 user with name filifjonken exists
    }

    [Fact]
    public async Task TestCreateDuplicateAuthorEmail()
    {
        if (_authorRepository == null || _utils == null || _context == null)
        {
            return;
        }

        //Act
        await _authorRepository.CreateAuthor("LilleMy", "fili@mail.com");
        await _authorRepository.CreateAuthor("Filifjonken", "fili@mail.com");

        var query = (
            from author in _context.Authors
            where author.Email == "fili@mail.com"
            select author
        );
        var result = await query.ToListAsync();

        //Assert
        Assert.True(result.Count == 2);
    }

    [Fact]
    public async Task TestUsernameCannotContainSlash()
    {
        if (_authorRepository == null)
        {
            return;
        }

        //Act and assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _authorRepository.CreateAuthor("/Haha", "hahaemail@gmail.com")
        );
    }

    // ----  FollowRepository ----

    [Fact]
    public async Task CanAddFollowerToDb()
    {
        if (_followRepository == null || _context == null)
        {
            return;
        }
        //Arrange
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        //Act
        await _followRepository.AddFollowing(author1.Name, author2.Name);
        var follow = await _context.Follows.FirstOrDefaultAsync();

        //Assert
        Assert.NotNull(follow);
        Assert.Equal(author1.Name, follow.Follower);
    }

    [Fact]
    public async Task CantFollowSameUserTwice()
    {
        if (_followRepository == null || _context == null)
        {
            return;
        }
        //Arrange
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        //Act
        await _followRepository.AddFollowing(author1.Name, author2.Name);
        await _followRepository.AddFollowing(author1.Name, author2.Name);

        var query = (
            from follow in _context.Follows
            where follow.Followed == author2.Name
            select follow
        );
        var result = await query.ToListAsync();

        //Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task CanRemoveFollowerFromDb()
    {
        if (_context == null || _followRepository == null)
        {
            return;
        }

        //Arrange
        var author1 = "hej";
        var author2 = "meddig";
        var follow = new Follow() { Follower = author1, Followed = author2 };
        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        var followBefore = _context.Follows.Count();
        //Check that the Follow has been added
        Assert.NotNull(follow);

        await _followRepository.RemoveFollowing(author1, author2);

        //Check that the follow has been removed
        var followAfter = _context.Follows.Count();

        //Assert
        Assert.True(followAfter == followBefore - 1);
    }

    [Fact]
    public async Task CanGetFollowFromDb()
    {
        if (_followRepository == null || _context == null)
        {
            return;
        }

        //Arrange
        var author1 = "Test1";
        var author2 = "Test2";

        var follow = new Follow() { Follower = author1, Followed = author2 };

        _context.Follows.Add(follow);
        await _context.SaveChangesAsync();

        //Act
        var follows = await _followRepository.GetFollowed(author1);

        //Assert
        Assert.NotNull(follows);
        Assert.Equal(author2, follows[0].Followed);
    }

    [Fact]
    public async Task CanGetFollowsFromDb()
    {
        if (_followRepository == null || _context == null)
        {
            return;
        }

        //Arrange
        var author1 = "Test1";
        var author2 = "Test2";
        var author3 = "Test3";

        var follow1 = new Follow() { Follower = author1, Followed = author2 };
        var follow2 = new Follow() { Follower = author1, Followed = author3 };

        _context.Follows.Add(follow1);
        _context.Follows.Add(follow2);
        await _context.SaveChangesAsync();

        var follows = await _followRepository.GetFollowed(author1);

        Assert.NotNull(follows);
        Assert.Equal(author2, follows[0].Followed);
        Assert.Equal(author3, follows[1].Followed);
    }

    // ------ Chirpservice -------
    [Fact]
    public async Task TestCanGetAllCheeps()
    {
        if (_chirpService == null)
        {
            return;
        }
        var cheeps = await _chirpService.GetAllCheepsFromAuthor("Octavio Wagganer");

        Assert.Equal(15, cheeps.Count);
    }

    [Fact]
    public async Task TestGetCheepsPageOne()
    {
        if (_chirpService == null)
        {
            return;
        }

        var cheeps = await _chirpService.GetCheeps(1);
        Assert.Equal(32, cheeps.Count);
    }

    [Fact]
    public async Task TestGetAuthorDtoByNameReturnsNullWhenNotFound()
    {
        if (_chirpService == null)
        {
            return;
        }

        var author = await _chirpService.GetAuthorDtoByName(null!);
        Assert.Null(author);
    }

    [Fact]
    public async Task TestGetAuthorDtoByNameReturnsAuthorDto()
    {
        if (_chirpService == null)
        {
            return;
        }

        var author = await _chirpService.GetAuthorDtoByName("Octavio Wagganer");
        // add assert
        if (author != null)
            Assert.Equal("Octavio Wagganer", author.Username);
    }

    [Fact]
    public async Task TestGetCheepsIsFollowingAuthor()
    {
        if (_chirpService == null || _followRepository == null || _context == null)
        {
            return;
        }

        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        await _followRepository.AddFollowing(author1.Name, "Jacqualine Gilcoine");

        var cheeps = await _chirpService.GetCheeps(1, author1.Name);

        Assert.True(cheeps[0].Follows);
        Assert.False(cheeps[3].Follows);
    }

    [Fact]
    public async Task TestPage0IsPage1()
    {
        if (_chirpService == null)
        {
            return;
        }

        var cheepsPage0 = await _chirpService.GetCheeps(0);
        var cheepsPage1 = await _chirpService.GetCheeps(1);

        Assert.Equivalent(cheepsPage0, cheepsPage1);
    }

    [Fact]
    public async Task TestPage1IsNotPage2()
    {
        if (_chirpService == null)
        {
            return;
        }

        var cheepsPage1 = await _chirpService.GetCheeps(1);
        var cheepsPage2 = await _chirpService.GetCheeps(2);

        Assert.False(cheepsPage1.Equals(cheepsPage2));
    }

    [Fact]
    public async Task TestCanAddFollowerToDbWithchirpService()
    {
        if (_chirpService == null || _context == null)
        {
            return;
        }
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        await _chirpService.AddFollowing(author1.Name, author2.Name);

        var follow = await _context.Follows.FirstOrDefaultAsync();

        Assert.NotNull(follow);
        Assert.Equal(author1.Name, follow.Follower);
    }

    [Fact]
    public async Task TestGetCheepsFromAuthorOnlyContainsCheepsFromFollowing()
    {
        if (_chirpService == null || _followRepository == null || _context == null)
        {
            return;
        }

        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        await _followRepository.AddFollowing(author1.Name, "Jacqualine Gilcoine");

        var cheeps = await _chirpService.GetCheepsFromAuthor(
            1,
            "Jacqualine Gilcoine",
            author1.Name
        );

        foreach (var cheep in cheeps)
        {
            Assert.True(cheep.Follows);
        }
    }

    [Fact]
    public async Task TestAddCheep()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }

        await _chirpService.AddCheep("Hello Chirp!", "Test1", "test1@mail.com");

        var cheeps = await _cheepRepository.GetCheepsFromAuthor(1, "Test1");

        Assert.Single(cheeps);
    }

    [Fact]
    public async Task TestCanRemoveFollower()
    {
        if (_chirpService == null || _context == null || _followRepository == null)
        {
            return;
        }
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        await _followRepository.AddFollowing(author1.Name, author2.Name);
        var follow1 = await _context.Follows.FirstOrDefaultAsync();

        if (follow1 != null)
            Assert.Equal(author1.Name, follow1.Follower);

        await _chirpService.RemoveFollowing(author1.Name, author2.Name);

        var follow2 = await _context.Follows.FirstOrDefaultAsync();

        Assert.Null(follow2);
        if (follow2 != null)
            Assert.NotEqual(author1.Name, follow2.Follower);
    }

    [Fact]
    public async Task TestCanGetFollowedDtos()
    {
        if (_chirpService == null || _followRepository == null)
        {
            return;
        }
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        await _followRepository.AddFollowing(author1.Name, author2.Name);

        var followers = await _chirpService.GetFollowedDtos("Test1");

        Assert.NotNull(followers);
        Assert.Single(followers);
    }

    [Fact]
    public async Task TestCanGetCheepsForTimeline()
    {
        if (_chirpService == null)
        {
            return;
        }

        var cheeps = await _chirpService.GetCheepsForTimeline("Octavio Wagganer", 1);

        Assert.Equal(15, cheeps.Count);
    }

    [Fact]
    public async Task TestCanDeleteAllFollows()
    {
        if (_chirpService == null || _followRepository == null)
        {
            return;
        }
        Author author1 = new Author()
        {
            Id = 1,
            Name = "Test1",
            Email = "test1@mail.com",
        };

        Author author2 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        Author author3 = new Author()
        {
            Id = 2,
            Name = "Test2",
            Email = "test2@mail.com",
        };

        await _followRepository.AddFollowing(author1.Name, author2.Name);
        await _followRepository.AddFollowing(author3.Name, author2.Name);
        await _followRepository.AddFollowing(author2.Name, author1.Name);

        await _chirpService.DeleteFromFollows(author2.Name);

        var followersTest2 = await _chirpService.GetFollowedDtos(author2.Name); // not sure if using this is legal
        var followersTest1 = await _chirpService.GetFollowedDtos(author1.Name); // not sure if using this is legal

        Assert.Empty(followersTest2);
        Assert.Empty(followersTest1);
    }

    [Fact]
    public async Task TestCanAddLike()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }
        var likes1 = await _cheepRepository.CountLikes(10);

        Assert.Equal(0, likes1);

        await _chirpService.AddLike("Jacquine Gilcoine", 10);

        var likes2 = await _cheepRepository.CountLikes(10);
        Assert.Equal(1, likes2);
    }

    [Fact]
    public async Task TestAuthorCanLikeCheepOnlyOnce()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }
        var likes1 = await _cheepRepository.CountLikes(10);

        Assert.Equal(0, likes1);

        await _chirpService.AddLike("Jacquine Gilcoine", 10);
        await _chirpService.AddLike("Jacquine Gilcoine", 10);

        var likes2 = await _cheepRepository.CountLikes(10);
        Assert.Equal(1, likes2);
    }

    [Fact]
    public async Task TestCanRemoveLike()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }
        var likes1 = await _cheepRepository.CountLikes(10);

        Assert.Equal(0, likes1);

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);

        var likes2 = await _cheepRepository.CountLikes(10);
        Assert.Equal(1, likes2);

        await _chirpService.RemoveLike("Jacquine Gilcoine", 10);

        Assert.Equal(0, likes1);
    }

    [Fact]
    public async Task TestAuthorCanOnlyRemoveOneLike()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }
        var likes1 = await _cheepRepository.CountLikes(10);

        Assert.Equal(0, likes1);

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);

        var likes2 = await _cheepRepository.CountLikes(10);
        Assert.Equal(1, likes2);

        await _chirpService.RemoveLike("Jacquine Gilcoine", 10);
        await _chirpService.RemoveLike("Jacquine Gilcoine", 10);
        await _chirpService.RemoveLike("Jacquine Gilcoine", 10);

        Assert.Equal(0, likes1);
    }

    [Fact]
    public async Task TestCanCountLikes()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }
        var likes1 = await _chirpService.CountLikes(10);

        Assert.Equal(0, likes1);

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);
        await _cheepRepository.AddLike("Quintin Sitts", 10);
        await _cheepRepository.AddLike("Luanna Muro", 10);

        var likes2 = await _chirpService.CountLikes(10);
        Assert.Equal(3, likes2);
    }

    [Fact]
    public async Task TestCanGetLikedCheepsChirpService()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);
        await _cheepRepository.AddLike("Jacquine Gilcoine", 11);
        await _cheepRepository.AddLike("Jacquine Gilcoine", 13);

        var likes = await _chirpService.GetAllLiked("Jacquine Gilcoine");

        Assert.Equal(3, likes.Count);
    }

    [Fact]
    public async Task TestCanDeleteAllLikes()
    {
        if (_chirpService == null || _cheepRepository == null)
        {
            return;
        }

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);
        await _cheepRepository.AddLike("Jacquine Gilcoine", 11);
        await _cheepRepository.AddLike("Jacquine Gilcoine", 13);

        var likes = await _cheepRepository.GetAllLiked("Jacquine Gilcoine");

        Assert.Equal(3, likes.Count);

        await _chirpService.DeleteAllLikes("Jacquine Gilcoine");

        var likesAfterDelete = await _cheepRepository.GetAllLiked("Jacquine Gilcoine");
        Assert.Empty(likesAfterDelete);
    }

    [Fact]
    public async Task TestCanGetTopLikedCheeps()
    {
        if (_chirpService == null || _cheepRepository == null || _context == null)
        {
            return;
        }

        await _cheepRepository.AddLike("Jacquine Gilcoine", 10);
        await _cheepRepository.AddLike("Quintin Sitts", 10);
        await _cheepRepository.AddLike("Luanna Muro", 10);

        await _cheepRepository.AddLike("Quintin Sitts", 9);
        await _cheepRepository.AddLike("Luanna Muro", 9);

        var topCheeps = await _chirpService.GetTopLikedCheeps("test", 1);

        Assert.Equal(10, topCheeps[0].Id);
        Assert.Equal(9, topCheeps[1].Id);
    }
}
