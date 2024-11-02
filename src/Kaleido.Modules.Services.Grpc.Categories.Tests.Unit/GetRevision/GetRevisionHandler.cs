using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Common.Services.Grpc.Exceptions;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetRevision
{
    public class GetRevisionHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetRevisionHandler _sut;
        private readonly GetCategoryRevisionRequest _validRequest;
        private readonly EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> _validRevision;

        public GetRevisionHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new GetCategoryRevisionRequest { Key = Guid.NewGuid().ToString(), Revision = 1 };
            _validRevision = new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
            {
                Entity = new CategoryEntity { Name = "Test Category" },
                Revision = new BaseRevisionEntity { Id = Guid.NewGuid(), Revision = 1, Key = Guid.Parse(_validRequest.Key) }
            };

            // Happy path setup
            _mocker.Use(new GetCategoryRevisionRequestValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            _mocker.GetMock<IGetRevisionManager>()
                .Setup(m => m.GetRevisionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validRevision);

            _sut = _mocker.CreateInstance<GetRevisionHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetCategoryRevisionResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryResponse>(result);
            Assert.Equal(_validRevision.Key.ToString(), result.Key);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IGetRevisionManager>()
                .Verify(m => m.GetRevisionAsync(_validRequest.Key, _validRequest.Revision, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var invalidRequest = new GetCategoryRevisionRequest { Key = "invalid-key", Revision = 1 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(invalidRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetRevisionManager>()
                .Setup(m => m.GetRevisionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.NotFound, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetRevisionManager>()
                .Setup(m => m.GetRevisionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

