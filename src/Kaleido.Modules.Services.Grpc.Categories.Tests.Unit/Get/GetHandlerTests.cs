using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Get;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Get
{
    public class GetHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetHandler _sut;
        private readonly CategoryRequest _validRequest;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _validCategory;

        public GetHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new CategoryRequest { Key = Guid.NewGuid().ToString() };
            _validCategory = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "Test Category" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
            };

            _mocker.Use(new CategoryRequestValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            // Happy path setup
            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validCategory);

            _sut = _mocker.CreateInstance<GetHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryResponse>(result);
            Assert.Equal(_validCategory.Entity.Name, result.Category.Name);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetManager>()
                .Setup(m => m.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

