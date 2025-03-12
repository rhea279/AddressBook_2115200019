using ModelLayer.Model;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AddressBookEntry> AddressBookEntries { get; set; }
        
    }
}
