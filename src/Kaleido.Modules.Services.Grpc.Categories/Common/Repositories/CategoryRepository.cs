using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;
using Kaleido.Common.Services.Grpc.Repositories;
using Microsoft.EntityFrameworkCore;
using Kaleido.Common.Services.Grpc.Constants;

namespace Kaleido.Modules.Services.Grpc.Categories.Common.Repositories;

public class CategoryRepository : BaseRepository<CategoryEntity, CategoryDbContext>, ICategoryRepository
{
    public CategoryRepository(
        ILogger<CategoryRepository> logger,
        CategoryDbContext context) : base(logger, context, context.Categories)
    {
    }

    public async Task<IEnumerable<CategoryEntity>> GetAllByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .Where(p => p.Name.ToLower().Contains(name.ToLower()))
            .Where(p => p.Status == EntityStatus.Active)
            .ToListAsync(cancellationToken);
    }
}