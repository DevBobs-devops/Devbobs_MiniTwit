using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Core;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Used to connect to the database.
/// Inherents from IdentityDbContext where IdentityRole has been overriden to make the primary key an int.
/// </summary>
public class CheepDbContext : IdentityDbContext<Author, IdentityRole<int>, int> //Overriden method to make primary key int
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Follow> Follows { get; set; }

    //Constructor
    public CheepDbContext(DbContextOptions<CheepDbContext> options)
        : base(options) { }
}
