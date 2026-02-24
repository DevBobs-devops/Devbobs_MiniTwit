using Chirp.Core;

namespace Chirp.Infrastructure.data;

public static class DbInitializer
{
    public static void SeedDatabase(CheepDbContext chirpContext)
    {
        
        chirpContext.SaveChanges();
        
    }
}