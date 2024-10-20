using Moq;
using Moq.AutoMock;
using Xunit;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Services;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Services;

public class CategoryServiceTests
{
    private readonly AutoMocker _mocker;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _mocker = new AutoMocker();
        _sut = _mocker.CreateInstance<CategoryService>();
    }

    [Fact]
    public async Task CreateCategory_CallsHandleAsyncOnCreateHandler()
    {
        // Arrange
        var request = new CreateCategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.CreateCategory(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<CreateCategoryRequest, CreateCategoryResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_CallsHandleAsyncOnDeleteHandler()
    {
        // Arrange
        var request = new DeleteCategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.DeleteCategory(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<DeleteCategoryRequest, DeleteCategoryResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CategoryExists_CallsHandleAsyncOnExistsHandler()
    {
        // Arrange
        var request = new CategoryExistsRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.CategoryExists(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<CategoryExistsRequest, CategoryExistsResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCategory_CallsHandleAsyncOnGetHandler()
    {
        // Arrange
        var request = new GetCategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetCategory(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<GetCategoryRequest, GetCategoryResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategories_CallsHandleAsyncOnGetAllHandler()
    {
        // Arrange
        var request = new GetAllCategoriesRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategories(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<GetAllCategoriesRequest, GetAllCategoriesResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesByName_CallsHandleAsyncOnGetAllByNameHandler()
    {
        // Arrange
        var request = new GetAllCategoriesByNameRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategoriesByName(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<GetAllCategoriesByNameRequest, GetAllCategoriesByNameResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoryRevisions_CallsHandleAsyncOnGetAllRevisionsHandler()
    {
        // Arrange
        var request = new GetAllCategoryRevisionsRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategoryRevisions(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<GetAllCategoryRevisionsRequest, GetAllCategoryRevisionsResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCategoryRevision_CallsHandleAsyncOnGetRevisionHandler()
    {
        // Arrange
        var request = new GetCategoryRevisionRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetCategoryRevision(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<GetCategoryRevisionRequest, GetCategoryRevisionResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CallsHandleAsyncOnUpdateHandler()
    {
        // Arrange
        var request = new UpdateCategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.UpdateCategory(request, context);

        // Assert
        _mocker.GetMock<IBaseHandler<UpdateCategoryRequest, UpdateCategoryResponse>>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
