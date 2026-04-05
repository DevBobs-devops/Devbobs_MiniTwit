using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chirp.Core;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


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
    public CheepDbContext(DbContextOptions<CheepDbContext> options) : base(options)
    {   
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {   
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cheep>()
        .Property(c => c.Timestamp)
        .HasColumnType("timestamp with time zone")
        .ValueGeneratedOnAdd();


        var likesConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        var likesComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList() // deep copy
        );
        
        modelBuilder.Entity<Cheep>().Property(c=> c.Likes).HasColumnName("likes").HasConversion(likesConverter).Metadata.SetValueComparer(likesComparer);


    }

}
