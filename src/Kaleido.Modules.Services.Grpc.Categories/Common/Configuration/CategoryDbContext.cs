using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;

public class CategoryDbContext : DbContext
{
    public DbSet<CategoryEntity> Categories { get; set; }

    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryEntity>().ToTable("Categories");

        modelBuilder.Entity<CategoryEntity>().HasKey(c => c.Id);
        modelBuilder.Entity<CategoryEntity>().Property(c => c.Id).ValueGeneratedOnAdd().HasColumnType("uuid");
        modelBuilder.Entity<CategoryEntity>().Property(c => c.Key).IsRequired().HasColumnType("uuid");
        modelBuilder.Entity<CategoryEntity>().Property(c => c.Name).IsRequired().HasColumnType("varchar(100)");
        modelBuilder.Entity<CategoryEntity>().Property(c => c.CreatedAt).IsRequired().HasColumnType("timestamp with time zone");
        modelBuilder.Entity<CategoryEntity>().Property(c => c.Revision).IsRequired().HasColumnType("int");
        modelBuilder.Entity<CategoryEntity>().Property(c => c.Status).IsRequired().HasColumnType("int");
    }
}