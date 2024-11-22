using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Update;
using Kaleido.Common.Services.Grpc.Exceptions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly UpdateHandler _sut;
        private readonly CategoryActionRequest _validRequest;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _updatedCategory;

        public UpdateHandlerTests()
        {
            _mocker = new AutoMocker();

            var categoryKey = Guid.NewGuid().ToString();
            _validRequest = new CategoryActionRequest
            {
                Key = categoryKey,
                Category = new Category { Name = "Updated Category" }
            };

            _updatedCategory = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "Updated Category" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
            };

            // Happy path setup
            _mocker.Use(new KeyValidator());
            _mocker.Use(new CategoryValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_updatedCategory);

            _sut = _mocker.CreateInstance<UpdateHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsUpdateCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryResponse>(result);
            Assert.Equal(_updatedCategory.Key.ToString(), result.Key);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IUpdateManager>()
                .Verify(m => m.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var invalidRequest = new CategoryActionRequest
            {
                Key = Guid.NewGuid().ToString(),
                Category = new Category { Name = "" }
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(invalidRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IUpdateManager>()
                .Setup(m => m.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

