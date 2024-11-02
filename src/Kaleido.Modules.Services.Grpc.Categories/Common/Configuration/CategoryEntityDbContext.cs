
using Kaleido.Common.Services.Grpc.Configuration.Constants;
using Kaleido.Common.Services.Grpc.Configuration.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;

public class CategoryEntityDbContext : DbContext, IKaleidoDbContext<CategoryEntity>
{
    public DbSet<CategoryEntity> Items { get; set; }

    public CategoryEntityDbContext(DbContextOptions<CategoryEntityDbContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CategoryEntity>(entity =>
        {
            entity.Property(c => c.Name).IsRequired().HasColumnType("varchar(100)");
            entity.ToTable("Categories");
            DefaultOnModelCreatingMethod.ForBaseEntity(entity);
        });
    }
}