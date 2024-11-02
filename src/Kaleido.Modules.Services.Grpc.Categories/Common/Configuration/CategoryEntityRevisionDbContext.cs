using Kaleido.Common.Services.Grpc.Configuration.Constants;
using Kaleido.Common.Services.Grpc.Configuration.Interfaces;
using Kaleido.Common.Services.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;

public class CategoryEntityRevisionDbContext : DbContext, IKaleidoDbContext<BaseRevisionEntity>
{
    public DbSet<BaseRevisionEntity> Items { get; set; }

    public CategoryEntityRevisionDbContext(DbContextOptions<CategoryEntityRevisionDbContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BaseRevisionEntity>(entity =>
        {
            entity.ToTable("CategoryRevisions");
            DefaultOnModelCreatingMethod.ForBaseEntity(entity);
            DefaultOnModelCreatingMethod.ForBaseRevisionEntity(entity);
        });
    }
}