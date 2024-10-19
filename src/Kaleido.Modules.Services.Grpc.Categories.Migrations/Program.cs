using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
});

builder.ConfigureServices((hostContext, services) =>
{
    services.AddDbContext<CategoryDbContext>(options =>
        options.UseNpgsql(hostContext.Configuration.GetConnectionString("Categories"),
            b => b.MigrationsAssembly("Kaleido.Modules.Services.Grpc.Categories.Migrations")));
});

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CategoryDbContext>();

    await context.Database.MigrateAsync();

    Console.WriteLine("Migration completed successfully.");
}
