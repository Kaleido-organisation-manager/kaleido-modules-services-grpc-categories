using Grpc.Core;
using Kaleido.Common.Services.Grpc.Constants;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Client.Models;
using Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Fixtures;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Integrations.Create;

[Collection(nameof(InfrastructureCollection))]
public class CreateIntegrationTests : IAsyncLifetime
{
    private readonly InfrastructureFixture _fixture;

    public CreateIntegrationTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _fixture.ClearDatabase();

    [Fact]
    public async Task CreateCategory_ValidName_CreatesNewCategory()
    {
        // Arrange
        var name = "Test Category";

        // Act
        var result = await _fixture.Client.CreateAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Key);
        Assert.Equal(name, result.Category.Name);
        Assert.NotNull(result.Revision);
        Assert.Equal(RevisionAction.Created, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateCategory_InvalidName_ThrowsException(string invalidName)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.CreateAsync(invalidName));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_NameTooLong_ThrowsException()
    {
        // Arrange
        var longName = new string('a', 101); // Over 100 characters

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.CreateAsync(longName));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_UsingEntityDto_CreatesNewCategory()
    {
        // Arrange
        var categoryDto = new CategoryEntityDto
        {
            Name = "Test Category"
        };

        // Act
        var result = await _fixture.Client.CreateAsync(categoryDto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Key);
        Assert.Equal(categoryDto.Name, result.Category.Name);
        Assert.NotNull(result.Revision);
        Assert.Equal(RevisionAction.Created, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Fact]
    public async Task CreateCategory_UsingEntityDtoWithInvalidName_ThrowsException()
    {
        // Arrange
        var categoryDto = new CategoryEntityDto
        {
            Name = string.Empty
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(
            () => _fixture.Client.CreateAsync(categoryDto));

        Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_DuplicateName_CreatesNewCategory()
    {
        // Arrange
        var name = "Test Category";

        // Act
        var result1 = await _fixture.Client.CreateAsync(name);
        var result2 = await _fixture.Client.CreateAsync(name);

        // Assert
        Assert.NotEqual(result1.Key, result2.Key);
        Assert.Equal(name, result1.Category.Name);
        Assert.Equal(name, result2.Category.Name);
    }

    [Fact]
    public async Task CreateCategory_DeletedCategory_CreatesNewCategory()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);
        await _fixture.Client.DeleteAsync(created.Key);

        // Act
        var result = await _fixture.Client.CreateAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(created.Key, result.Key); // Should create new category, not restore old one
        Assert.Equal(name, result.Category.Name);
        Assert.Equal(RevisionAction.Created, result.Revision.Action);
        Assert.Equal(RevisionStatus.Active, result.Revision.Status);
    }

    [Fact]
    public async Task CreateCategory_CreatedCategoryCanBeRetrieved()
    {
        // Arrange
        var name = "Test Category";
        var created = await _fixture.Client.CreateAsync(name);

        // Act
        var retrieved = await _fixture.Client.GetAsync(created.Key);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(created.Key, retrieved.Key);
        Assert.Equal(name, retrieved.Category.Name);
        Assert.Equal(created.Revision.Key, retrieved.Revision.Key);
    }
}
