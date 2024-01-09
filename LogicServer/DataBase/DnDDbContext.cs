using LogicServer.Models;
using Microsoft.EntityFrameworkCore;
using LogicServer.EntityPullStorage;

namespace LogicServer.DataBase
{
    public class DnDDbContext: DbContext
    {
        public DbSet<Entity> Entities { get; set; }

        public DnDDbContext(DbContextOptions options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>().HasData(EntityPull.Entities);
            base.OnModelCreating(modelBuilder);
        }
    }
}
