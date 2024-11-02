using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Delete;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Delete
{
    public class DeleteHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly DeleteHandler _sut;
        private readonly CategoryRequest _validRequest;

        public DeleteHandlerTests()
        {
            _mocker = new AutoMocker();

            _mocker.Use(new CategoryRequestValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            var categoryKey = Guid.NewGuid();
            _validRequest = new CategoryRequest
            {
                Key = categoryKey.ToString()
            };

            _sut = _mocker.CreateInstance<DeleteHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsDeleteCategoryResponse()
        {
            // Arrange
            // var categoryEntity = new CategoryEntity { Key = _validRequest.Key };
            var categoryEntity = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "valid-name" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid(), Key = Guid.Parse(_validRequest.Key) }
            };
            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(_validRequest.Key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(categoryEntity);

            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryResponse>(result);
            Assert.Equal(_validRequest.Key, result.Key);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IDeleteManager>()
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_InvalidKeyFormat_ThrowsValidationException()
        {
            // Arrange
            var invalidRequest = new CategoryRequest { Key = "invalid-key-format" }; // Simulate an invalid key format

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(invalidRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_EmptyKey_ThrowsValidationException()
        {
            // Arrange
            var emptyKeyRequest = new CategoryRequest { Key = string.Empty }; // Simulate an empty key

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(emptyKeyRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }
    }
}

