using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Configuration;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers;
using Kaleido.Modules.Services.Grpc.Categories.Common.Mappers.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Common.Services;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators.Interfaces;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Exists;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Common
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryValidator, CategoryValidator>();

var Configuration = builder.Configuration;
builder.Services.AddDbContext<CategoryDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("Categories")));

// Create
builder.Services.AddScoped<IBaseHandler<CreateCategoryRequest, CreateCategoryResponse>, CreateHandler>();
builder.Services.AddScoped<ICreateManager, CreateManager>();
builder.Services.AddScoped<IRequestValidator<CreateCategoryRequest>, CreateRequestValidator>();

// Delete
builder.Services.AddScoped<IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse>, DeleteHandler>();
builder.Services.AddScoped<IDeleteManager, DeleteManager>();
builder.Services.AddScoped<IRequestValidator<DeleteCategoryRequest>, DeleteRequestValidator>();

// Exists
builder.Services.AddScoped<IBaseHandler<CategoryExistsRequest, CategoryExistsResponse>, ExistsHandler>();
builder.Services.AddScoped<IExistsManager, ExistsManager>();
builder.Services.AddScoped<IRequestValidator<CategoryExistsRequest>, ExistsRequestValidator>();

// Get
builder.Services.AddScoped<IBaseHandler<GetCategoryRequest, GetCategoryResponse>, GetHandler>();
builder.Services.AddScoped<IGetManager, GetManager>();
builder.Services.AddScoped<IRequestValidator<GetCategoryRequest>, GetRequestValidator>();

// GetAll
builder.Services.AddScoped<IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse>, GetAllHandler>();
builder.Services.AddScoped<IGetAllManager, GetAllManager>();
builder.Services.AddScoped<IRequestValidator<GetAllCategoriesRequest>, GetAllRequestValidator>();

// GetAllByName
builder.Services.AddScoped<IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse>, GetAllByNameHandler>();
builder.Services.AddScoped<IGetAllByNameManager, GetAllByNameManager>();
builder.Services.AddScoped<IRequestValidator<GetAllCategoriesByNameRequest>, GetAllByNameRequestValidator>();

// GetAllRevisions
builder.Services.AddScoped<IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse>, GetAllRevisionsHandler>();
builder.Services.AddScoped<IGetAllRevisionsManager, GetAllRevisionsManager>();
builder.Services.AddScoped<IRequestValidator<GetAllCategoryRevisionsRequest>, GetAllRevisionsRequestValidator>();

// GetRevision
builder.Services.AddScoped<IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse>, GetRevisionHandler>();
builder.Services.AddScoped<IGetRevisionManager, GetRevisionManager>();
builder.Services.AddScoped<IRequestValidator<GetCategoryRevisionRequest>, GetRevisionRequestValidator>();

// Update
builder.Services.AddScoped<IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse>, UpdateHandler>();
builder.Services.AddScoped<IUpdateManager, UpdateManager>();
builder.Services.AddScoped<IRequestValidator<UpdateCategoryRequest>, UpdateRequestValidator>();

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
