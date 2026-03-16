
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
        // TODO: Handle likes aswell somehow :/

        //adding convertes for likes and booleans
        //Based on the codumentation for ValueConverter found here here: https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations
        var bigintToBoolConverter = new ValueConverter<bool, long>(//First = .Net datatype Second = db provider type.
            v => boolToInt(v),
            v => intToBool(v)
        );


        var likesConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
        );

        var likesComparer = new ValueComparer<List<string>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList() // deep copy
        );

        //Specifying the table and column names, to match up with the ones in postgres
        //https://learn.microsoft.com/en-us/ef/core/modeling/ and help from chatgpt with structure (the  'cheep => {}' part and how to handle the efcore stuff)
        modelBuilder.Entity<Cheep>(cheep =>
        {
            cheep.ToTable("cheeps");
            cheep.Property(c => c.CheepId).HasColumnName("cheepid");
            cheep.Property(c => c.AuthorId).HasColumnName("authorid");
            cheep.Property(c => c.Text).HasColumnName("text");
            cheep.Property(c => c.Timestamp).HasColumnName("timestamp");//.HasConversion(timeToDateTimeConverter); //convert to DateTime
            cheep.Property(c=> c.Likes).HasColumnName("likes").HasConversion(likesConverter).Metadata.SetValueComparer(likesComparer);
        });

        modelBuilder.Entity<Follow>(follow =>
        {
            follow.ToTable("follows");
            follow.Property(f => f.Followed).HasColumnName("followed");
            follow.Property(f => f.Follower).HasColumnName("follower");
        });
        //modelBuilder.Entity<Author>().ToTable("authors");

        modelBuilder.Entity<Author>(author => //Represents authors
        {
            author.ToTable("aspnetusers");
            author.Property(a => a.AccessFailedCount).HasColumnName("accessfailedcount");
            author.Property(a => a.ConcurrencyStamp).HasColumnName("concurrencystamp");
            author.Property(a => a.Email).HasColumnName("email");
            author.Property(a => a.EmailConfirmed).HasColumnName("emailconfirmed").HasConversion(bigintToBoolConverter);
            author.Property(a => a.Id).HasColumnName("id");
            author.Property(a => a.LockoutEnabled).HasColumnName("lockoutenabled").HasConversion(bigintToBoolConverter);
            author.Property(a => a.LockoutEnd).HasColumnName("lockoutend");
            author.Property(a => a.Name).HasColumnName("name");
            author.Property(a => a.NormalizedEmail).HasColumnName("normalizedemail");
            author.Property(a => a.PasswordHash).HasColumnName("passwordhash");
            author.Property(a => a.PhoneNumber).HasColumnName("phonenumber");
            author.Property(a => a.PhoneNumberConfirmed).HasColumnName("phonenumberconfirmed").HasConversion(bigintToBoolConverter);
            author.Property(a => a.SecurityStamp).HasColumnName("securitystamp");
            author.Property(a => a.TwoFactorEnabled).HasColumnName("twofactorenabled").HasConversion(bigintToBoolConverter);
            author.Property(a => a.UserName).HasColumnName("username");
            author.Property(a => a.NormalizedUserName).HasColumnName("normalizedusername");
        });

        modelBuilder.Entity<IdentityRole<int>>(EFC =>
        {
            EFC.ToTable("aspnetroles");
            EFC.Property(e=>e.Id).HasColumnName("id");
            EFC.Property(e=>e.Name).HasColumnName("name");
            EFC.Property(e=>e.NormalizedName).HasColumnName("normalizedname");
            EFC.Property(e=>e.ConcurrencyStamp).HasColumnName("concurrencystamp");
        });

        modelBuilder.Entity<IdentityUserRole<int>>(EFC =>
        {
            EFC.ToTable("aspnetuserroles");
            EFC.Property(e=>e.RoleId).HasColumnName("roleid");
            EFC.Property(e=>e.UserId).HasColumnName("userid");
        });


        modelBuilder.Entity<IdentityUserClaim<int>>(EFC=>
        {
            EFC.ToTable("aspnetuserclaims");
            EFC.Property(e=>e.ClaimType).HasColumnName("claimtype");
            EFC.Property(e=>e.ClaimValue).HasColumnName("claimvalue");
            EFC.Property(e=>e.Id).HasColumnName("id");
            EFC.Property(e=>e.UserId).HasColumnName("userid");
            
        });

        modelBuilder.Entity<IdentityUserLogin<int>>(EFC =>
        {
            EFC.ToTable("aspnetuserlogins");
            EFC.Property(e=>e.LoginProvider).HasColumnName("loginprovider");
            EFC.Property(e=>e.ProviderKey).HasColumnName("providerkey");
            EFC.Property(e=>e.ProviderDisplayName).HasColumnName("providerdisplayname");
            EFC.Property(e=>e.UserId).HasColumnName("userid");
        });

        modelBuilder.Entity<IdentityRoleClaim<int>>(EFC =>
        {
            EFC.ToTable("aspnetroleclaims");

            EFC.Property(e=> e.Id).HasColumnName("id");
            EFC.Property(e=> e.ClaimType).HasColumnName("claimtype");
            EFC.Property(e=> e.ClaimValue).HasColumnName("claimvalue");
            EFC.Property(e=> e.RoleId).HasColumnName("roleid");
        });

        modelBuilder.Entity<IdentityUserToken<int>>(EFC =>
        {
            EFC.ToTable("aspnetusertokens");
            EFC.Property(e=> e.LoginProvider).HasColumnName("loginprovider");
            EFC.Property(e=> e.Name).HasColumnName("name");
            EFC.Property(e=> e.Value).HasColumnName("value");
            EFC.Property(e=> e.UserId).HasColumnName("userid");
        });
    }


    //Helper methods for convertion
       private long boolToInt(bool b)
    {
        if (b){
            return 1;
        }
        return 0;
    }

    private bool intToBool(long i)
    {
        if (i == 1)
        {
            return true;
        }
        return false;
    }
}