using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Kaleido.Common.Services.Grpc.Configuration.Extensions;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
});

builder.ConfigureServices((hostContext, services) =>
{
    var connectionString = hostContext.Configuration.GetConnectionString("Categories");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentNullException(nameof(connectionString), "Expected a value for the categories db connection string");
    }
    var assemblyName = "Kaleido.Modules.Services.Grpc.Categories.Migrations";
    services.AddKaleidoMigrationEntityDbContext<CategoryEntity, CategoryEntityDbContext>(connectionString, assemblyName);
    services.AddKaleidoMigrationRevisionDbContext<BaseRevisionEntity, CategoryEntityRevisionDbContext>(connectionString, assemblyName);

});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var entityContext = services.GetRequiredService<CategoryEntityDbContext>();
    var revisionContext = services.GetRequiredService<CategoryEntityRevisionDbContext>();

    await entityContext.Database.MigrateAsync();
    await revisionContext.Database.MigrateAsync();

    Console.WriteLine("Migration completed successfully.");
}
