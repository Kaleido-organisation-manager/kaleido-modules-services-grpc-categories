using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Client.Models;
using static Kaleido.Grpc.Categories.GrpcCategories;

namespace Kaleido.Modules.Services.Grpc.Categories.Client.Client;

public class CategoryClient : ICategoryClient
{
    private readonly GrpcCategoriesClient _client;
    private readonly IMapper _mapper;

    public CategoryClient(
        GrpcCategoriesClient client,
        IMapper mapper)
    {
        _client = client;
        _mapper = mapper;
    }

    public async Task<CategoryDto> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var request = new Category { Name = name };
        var response = await _client.CreateCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<CategoryDto> CreateAsync(CategoryEntityDto category, CancellationToken cancellationToken = default)
    {
        var request = _mapper.Map<Category>(category);
        var response = await _client.CreateCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<CategoryDto> DeleteAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var request = new CategoryRequest { Key = key.ToString() };
        var response = await _client.DeleteCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<CategoryDto> GetAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var request = new CategoryRequest { Key = key.ToString() };
        var response = await _client.GetCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAllCategoriesAsync(new EmptyRequest(), cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<CategoryDto>>(response);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllFilteredAsync(string name, CancellationToken cancellationToken = default)
    {
        var request = new GetAllCategoriesFilteredRequest { Name = name };
        var response = await _client.GetAllCategoriesFilteredAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<CategoryDto>>(response);
    }

    public async Task<IEnumerable<CategoryDto>> GetAllRevisionsAsync(Guid key, CancellationToken cancellationToken = default)
    {
        var request = new CategoryRequest { Key = key.ToString() };
        var response = await _client.GetAllCategoryRevisionsAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<CategoryDto>>(response);
    }

    public async Task<CategoryDto> GetRevisionAsync(Guid key, DateTime createdAt, CancellationToken cancellationToken = default)
    {
        var request = new GetCategoryRevisionRequest
        {
            Key = key.ToString(),
            CreatedAt = Timestamp.FromDateTime(createdAt.ToUniversalTime())
        };
        var response = await _client.GetCategoryRevisionAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<CategoryDto> UpdateAsync(Guid key, string name, CancellationToken cancellationToken = default)
    {
        var request = new CategoryActionRequest
        {
            Key = key.ToString(),
            Category = new Category { Name = name }
        };
        var response = await _client.UpdateCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }

    public async Task<CategoryDto> UpdateAsync(Guid key, CategoryEntityDto category, CancellationToken cancellationToken = default)
    {
        var request = _mapper.Map<CategoryActionRequest>(category);
        request.Key = key.ToString();
        var response = await _client.UpdateCategoryAsync(request, cancellationToken: cancellationToken);
        return _mapper.Map<CategoryDto>(response);
    }
}