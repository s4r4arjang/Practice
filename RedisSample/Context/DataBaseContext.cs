using Microsoft.EntityFrameworkCore;
using System;

namespace RedisSample.Context
{
    public class DataBaseContext : DbContext    
    {
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Family).HasMaxLength(100).IsRequired();
            });
        }
    }
}
