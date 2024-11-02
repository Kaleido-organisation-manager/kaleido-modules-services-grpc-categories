using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllRevisions
{
    public class GetAllRevisionsHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllRevisionsHandler _sut;
        private readonly CategoryRequest _validRequest;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _validRevisions;

        public GetAllRevisionsHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new CategoryRequest { Key = Guid.NewGuid().ToString() };
            _validRevisions = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Revision 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Revision 2" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };

            // Happy path setup
            _mocker.Use(new CategoryRequestValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            _mocker.GetMock<IGetAllRevisionsManager>()
                .Setup(m => m.HandleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validRevisions);

            _sut = _mocker.CreateInstance<GetAllRevisionsHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoryRevisionsResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryListResponse>(result);
            Assert.Equal(_validRevisions.Count(), result.Categories.Count());
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IGetAllRevisionsManager>()
                .Verify(m => m.HandleAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var invalidRequest = new CategoryRequest { Key = "" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(invalidRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetAllRevisionsManager>()
                .Setup(m => m.HandleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsEmptyList_ReturnsEmptyResponse()
        {
            // Arrange
            _mocker.GetMock<IGetAllRevisionsManager>()
                .Setup(m => m.HandleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>());

            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryListResponse>(result);
            Assert.Empty(result.Categories);
        }
    }
}

