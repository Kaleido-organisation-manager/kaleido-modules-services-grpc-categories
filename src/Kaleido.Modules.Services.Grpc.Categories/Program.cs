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
// builder.Services.AddLifeCycleHandler(categoriesConnectionString, EntityTypeBuilders.CategoryEntityTypeBuilders, EntityTypeBuilders.BaseRevisionEntityTypeBuilders);

builder.Services.AddKaleidoEntityDbContext<CategoryEntity, CategoryEntityDbContext>(categoriesConnectionString);
builder.Services.AddKaleidoRevisionDbContext<BaseRevisionEntity, CategoryEntityRevisionDbContext>(categoriesConnectionString);
builder.Services.AddEntityRepository<CategoryEntity, CategoryEntityDbContext>();
builder.Services.AddRevisionRepository<CategoryEntityRevisionDbContext>();
builder.Services.AddLifeCycleHandler<CategoryEntity>();


// Create
// builder.Services.AddScoped<IBaseHandler<CreateCategoryRequest, CreateCategoryResponse>, CreateHandler>();
builder.Services.AddScoped<ICreateHandler, CreateHandler>();
builder.Services.AddScoped<ICreateManager, CreateManager>();
// builder.Services.AddScoped<IRequestValidator<CreateCategoryRequest>, CreateRequestValidator>();

// Delete
// builder.Services.AddScoped<IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse>, DeleteHandler>();
builder.Services.AddScoped<IDeleteHandler, DeleteHandler>();
builder.Services.AddScoped<IDeleteManager, DeleteManager>();
// builder.Services.AddScoped<IRequestValidator<DeleteCategoryRequest>, DeleteRequestValidator>();

// Exists
// builder.Services.AddScoped<IBaseHandler<CategoryExistsRequest, CategoryExistsResponse>, ExistsHandler>();
// builder.Services.AddScoped<IRequestValidator<CategoryExistsRequest>, ExistsRequestValidator>();

// Get
// builder.Services.AddScoped<IBaseHandler<GetCategoryRequest, GetCategoryResponse>, GetHandler>();
builder.Services.AddScoped<IGetHandler, GetHandler>();
builder.Services.AddScoped<IGetManager, GetManager>();
// builder.Services.AddScoped<IRequestValidator<GetCategoryRequest>, GetRequestValidator>();

// GetAll
// builder.Services.AddScoped<IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse>, GetAllHandler>();
builder.Services.AddScoped<IGetAllHandler, GetAllHandler>();
builder.Services.AddScoped<IGetAllManager, GetAllManager>();
// builder.Services.AddScoped<IRequestValidator<GetAllCategoriesRequest>, GetAllRequestValidator>();

// GetAllByName
// builder.Services.AddScoped<IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse>, GetAllByNameHandler>();
builder.Services.AddScoped<GetAllByNameRequestValidator>();
builder.Services.AddScoped<IGetAllByNameHandler, GetAllByNameHandler>();
builder.Services.AddScoped<IGetAllByNameManager, GetAllByNameManager>();
// builder.Services.AddScoped<IRequestValidator<GetAllCategoriesByNameRequest>, GetAllByNameRequestValidator>();

// GetAllRevisions
// builder.Services.AddScoped<IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse>, GetAllRevisionsHandler>();
builder.Services.AddScoped<IGetAllRevisionsHandler, GetAllRevisionsHandler>();
builder.Services.AddScoped<IGetAllRevisionsManager, GetAllRevisionsManager>();
// builder.Services.AddScoped<IRequestValidator<GetAllCategoryRevisionsRequest>, GetAllRevisionsRequestValidator>();

// GetRevision
// builder.Services.AddScoped<IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse>, GetRevisionHandler>();
builder.Services.AddScoped<GetCategoryRevisionRequestValidator>();
builder.Services.AddScoped<IGetRevisionHandler, GetRevisionHandler>();
builder.Services.AddScoped<IGetRevisionManager, GetRevisionManager>();
// builder.Services.AddScoped<IRequestValidator<GetCategoryRevisionRequest>, GetRevisionRequestValidator>();

// Update
// builder.Services.AddScoped<IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse>, UpdateHandler>();
builder.Services.AddScoped<CategoryActionValidator>();
builder.Services.AddScoped<IUpdateHandler, UpdateHandler>();
builder.Services.AddScoped<IUpdateManager, UpdateManager>();
// builder.Services.AddScoped<IRequestValidator<UpdateCategoryRequest>, UpdateRequestValidator>();

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
