using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAll
{
    public class GetAllHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllHandler _sut;
        private readonly EmptyRequest _validRequest;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _categories;

        public GetAllHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new EmptyRequest();

            _categories = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Category 2" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };

            // Happy path setup
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());


            _mocker.GetMock<IGetAllManager>()
                .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categories);

            _sut = _mocker.CreateInstance<GetAllHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoriesResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryListResponse>(result);
            Assert.Equal(_categories.Count, result.Categories.Count);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IGetAllManager>()
                .Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetAllManager>()
                .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

