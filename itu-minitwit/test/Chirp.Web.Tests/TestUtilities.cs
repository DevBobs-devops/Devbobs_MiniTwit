using Chirp.Core;
using Chirp.Infrastructure.data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Web.Tests;

public class TestUtilities
{
    public SqliteConnection? Connection { get; set; }

    public async Task<CheepDbContext> CreateInMemoryDb()
    {
        Connection = new SqliteConnection("Filename=:memory:");
        await Connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<CheepDbContext>().UseSqlite(Connection);

        var context = new CheepDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        DbInitializer.SeedDatabase(context);

        return context;
    }

    public Task CloseConnection()
    {
        if (Connection != null)
            Connection.Close();
        return Task.CompletedTask;
    }
}
