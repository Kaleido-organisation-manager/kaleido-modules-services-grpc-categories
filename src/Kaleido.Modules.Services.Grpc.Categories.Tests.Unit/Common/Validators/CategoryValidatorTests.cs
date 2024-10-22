using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using Kaleido.Grpc.Categories;
using Moq;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Common.Validators;

public class CategoryValidatorTests
{
    private readonly AutoMocker _mocker;
    private readonly CategoryValidator _sut;

    public CategoryValidatorTests()
    {
        _mocker = new AutoMocker();
        _sut = _mocker.CreateInstance<CategoryValidator>();
    }

    [Fact]
    public async Task ValidateCreateAsync_ValidCategory_ReturnsValidResult()
    {
        // Arrange
        var createCategory = new CreateCategory { Name = "Valid Category" };

        // Act
        var result = await _sut.ValidateCreateAsync(createCategory);

        // Assert
        Assert.Empty(result.Errors);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateUpdateAsync_ValidCategory_ReturnsValidResult()
    {
        // Arrange
        var category = new Category { Key = Guid.NewGuid().ToString(), Name = "Valid Category" };

        // Act
        var result = await _sut.ValidateUpdateAsync(category);

        // Assert
        Assert.Empty(result.Errors);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateKeyFormatAsync_ValidKey_ReturnsValidResult()
    {
        // Arrange
        var key = Guid.NewGuid().ToString();

        // Act
        var result = await _sut.ValidateKeyFormatAsync(key);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateCategoryNameAsync_ValidName_ReturnsValidResult()
    {
        // Arrange
        var name = "Valid Category Name";

        // Act
        var result = await _sut.ValidateCategoryNameAsync(name);

        // Assert
        Assert.Empty(result.Errors);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateCreateAsync_EmptyName_ReturnsInvalidResult()
    {
        // Arrange
        var createCategory = new CreateCategory { Name = "" };

        // Act
        var result = await _sut.ValidateCreateAsync(createCategory);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateUpdateAsync_InvalidKey_ReturnsInvalidResult()
    {
        // Arrange
        var category = new Category { Key = "invalid-key", Name = "Valid Category" };

        // Act
        var result = await _sut.ValidateUpdateAsync(category);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateKeyFormatAsync_EmptyKey_ReturnsInvalidResult()
    {
        // Arrange
        var key = "";

        // Act
        var result = await _sut.ValidateKeyFormatAsync(key);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateCategoryNameAsync_LongName_ReturnsInvalidResult()
    {
        // Arrange
        var name = new string('A', 101); // 101 characters

        // Act
        var result = await _sut.ValidateCategoryNameAsync(name);

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.False(result.IsValid);
    }
}

