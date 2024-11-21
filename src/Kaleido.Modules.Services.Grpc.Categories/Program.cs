using Kaleido.Common.Services.Grpc.Configuration.Extensions;
using Kaleido.Common.Services.Grpc.Repositories.Extensions;
using Kaleido.Common.Services.Grpc.Handlers.Extensions;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Services;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Common
builder.Services.AddAutoMapper(typeof(CategoryMappingProfile));
builder.Services.AddScoped<CategoryRequestValidator>();
builder.Services.AddScoped<CategoryValidator>();
builder.Services.AddScoped<KeyValidator>();
builder.Services.AddScoped<NameValidator>();

var Configuration = builder.Configuration;
var categoriesConnectionString = Configuration.GetConnectionString("categories");
if (string.IsNullOrEmpty(categoriesConnectionString))
{
    throw new ArgumentNullException(nameof(categoriesConnectionString), "No connection string found to connect to the categories database");
}

builder.Services.AddKaleidoEntityDbContext<CategoryEntity, CategoryEntityDbContext>(categoriesConnectionString);
builder.Services.AddKaleidoRevisionDbContext<BaseRevisionEntity, CategoryEntityRevisionDbContext>(categoriesConnectionString);
builder.Services.AddEntityRepository<CategoryEntity, CategoryEntityDbContext>();
builder.Services.AddRevisionRepository<CategoryEntityRevisionDbContext>();
builder.Services.AddLifeCycleHandler<CategoryEntity>();


// Create
builder.Services.AddScoped<ICreateHandler, CreateHandler>();
builder.Services.AddScoped<ICreateManager, CreateManager>();

// Delete
builder.Services.AddScoped<IDeleteHandler, DeleteHandler>();
builder.Services.AddScoped<IDeleteManager, DeleteManager>();

// Get
builder.Services.AddScoped<IGetHandler, GetHandler>();
builder.Services.AddScoped<IGetManager, GetManager>();

// GetAll
builder.Services.AddScoped<IGetAllHandler, GetAllHandler>();
builder.Services.AddScoped<IGetAllManager, GetAllManager>();

// GetAllByName
builder.Services.AddScoped<IGetAllFilteredHandler, GetAllFilteredHandler>();
builder.Services.AddScoped<IGetAllFilteredManager, GetAllFilteredManager>();

// GetAllRevisions
builder.Services.AddScoped<IGetAllRevisionsHandler, GetAllRevisionsHandler>();
builder.Services.AddScoped<IGetAllRevisionsManager, GetAllRevisionsManager>();

// GetRevision
builder.Services.AddScoped<IGetRevisionHandler, GetRevisionHandler>();
builder.Services.AddScoped<IGetRevisionManager, GetRevisionManager>();

// Update
builder.Services.AddScoped<IUpdateHandler, UpdateHandler>();
builder.Services.AddScoped<IUpdateManager, UpdateManager>();

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddHealthChecks();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.MapGrpcService<CategoryService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
