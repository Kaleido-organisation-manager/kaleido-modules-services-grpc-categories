using Moq;
using Moq.AutoMock;
using Xunit;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Services;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Modules.Services.Grpc.Categories.Update;

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
        var request = new Category();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.CreateCategory(request, context);

        // Assert
        _mocker.GetMock<ICreateHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCategory_CallsHandleAsyncOnDeleteHandler()
    {
        // Arrange
        var request = new CategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.DeleteCategory(request, context);

        // Assert
        _mocker.GetMock<IDeleteHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCategory_CallsHandleAsyncOnGetHandler()
    {
        // Arrange
        var request = new CategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetCategory(request, context);

        // Assert
        _mocker.GetMock<IGetHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategories_CallsHandleAsyncOnGetAllHandler()
    {
        // Arrange
        var request = new EmptyRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategories(request, context);

        // Assert
        _mocker.GetMock<IGetAllHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoriesByName_CallsHandleAsyncOnGetAllByNameHandler()
    {
        // Arrange
        var request = new GetAllCategoriesFilteredRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategoriesFiltered(request, context);

        // Assert
        _mocker.GetMock<IGetAllFilteredHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllCategoryRevisions_CallsHandleAsyncOnGetAllRevisionsHandler()
    {
        // Arrange
        var request = new CategoryRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.GetAllCategoryRevisions(request, context);

        // Assert
        _mocker.GetMock<IGetAllRevisionsHandler>()
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
        _mocker.GetMock<IGetRevisionHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategory_CallsHandleAsyncOnUpdateHandler()
    {
        // Arrange
        var request = new CategoryActionRequest();
        var context = new Mock<ServerCallContext>().Object;

        // Act
        await _sut.UpdateCategory(request, context);

        // Assert
        _mocker.GetMock<IUpdateHandler>()
            .Verify(x => x.HandleAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
