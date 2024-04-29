using Videos.Models;
using Microsoft.EntityFrameworkCore;

namespace Animes.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Anime> Animes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("DataSource=animes.sqlite;Cache=Shared");
    }
}