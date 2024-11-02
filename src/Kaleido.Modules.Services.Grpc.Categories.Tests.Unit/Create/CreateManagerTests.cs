using Moq;
using Moq.AutoMock;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Common.Services.Grpc.Handlers.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Create;

public class CreateManagerTests
{
    private readonly AutoMocker _mocker;
    private readonly CreateManager _sut;
    private readonly CategoryEntity _categoryEntity;

    public CreateManagerTests()
    {
        _mocker = new AutoMocker();
        _sut = _mocker.CreateInstance<CreateManager>();

        _categoryEntity = new CategoryEntity
        {
            Id = Guid.NewGuid(),
            Name = "Test Category"
        };

        _mocker.Use(() =>
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            return mapper.CreateMapper();
        });

        _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
            .Setup(r => r.CreateAsync(_categoryEntity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> { Entity = _categoryEntity, Revision = new BaseRevisionEntity() });
    }

    [Fact]
    public async Task CreateAsync_ShouldCallRepositoryCreateAsync()
    {
        // Act
        await _sut.CreateAsync(_categoryEntity);

        // Assert
        _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
            .Verify(r => r.CreateAsync(_categoryEntity, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task CreateAsync_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var cancellationToken = new CancellationToken();

        // Act
        await _sut.CreateAsync(_categoryEntity, cancellationToken);

        // Assert
        _mocker.GetMock<IEntityLifecycleHandler<CategoryEntity, BaseRevisionEntity>>()
            .Verify(r => r.CreateAsync(_categoryEntity, cancellationToken), Times.Once);
    }
}
