using Microsoft.EntityFrameworkCore;

namespace HackerNewsFeed.Data
{
    public class FeedDbContext : DbContext
    {
        public FeedDbContext(DbContextOptions<FeedDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Item> Items { get; set; }
    }
}