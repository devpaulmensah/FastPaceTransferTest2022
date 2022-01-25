using FastPaceTransferTest2022.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FastPaceTransferTest2022.Api.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options
           
        ) : base(options)
        {
           
        }
    }
}