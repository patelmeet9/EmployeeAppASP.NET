using Lane4Task.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;


namespace Lane4Task.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }

        
    }
}
