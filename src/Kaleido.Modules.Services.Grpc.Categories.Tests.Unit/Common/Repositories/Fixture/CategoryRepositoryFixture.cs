using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq.AutoMock;

namespace Kaleido.Common.Services.Grpc.Tests.Unit.Repositories.Fixture;

public class CategoryRepositoryFixture : IDisposable
{
    private ServiceProvider _provider { get; set; }

    public CategoryDbContext DbContext { get; private set; }
    public ICategoryRepository Repository { get; private set; }

    public CategoryRepositoryFixture()
    {
        var services = new ServiceCollection();
        services.AddDbContext<CategoryDbContext>(options =>
            options.UseInMemoryDatabase(databaseName: "TestEntities"));
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddLogging();

        _provider = services.BuildServiceProvider();

        DbContext = _provider.GetRequiredService<CategoryDbContext>();
        Repository = _provider.GetRequiredService<ICategoryRepository>();

        DbContext.Database.EnsureCreated();

    }

    public void Dispose()
    {
        _provider.Dispose();
        DbContext.Dispose();
    }

    public void ResetDatabase()
    {
        DbContext.Categories.RemoveRange(DbContext.Categories);
        DbContext.SaveChanges();
    }
}