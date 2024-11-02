using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllByNameHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllByNameHandler _sut;
        private readonly GetAllCategoriesByNameRequest _validRequest;
        private readonly List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>> _validCategories;

        public GetAllByNameHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new GetAllCategoriesByNameRequest { Name = "Test" };
            _validCategories = new List<EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>>
            {
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Test Category 1" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                },
                new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity>
                {
                    Entity = new CategoryEntity { Name = "Test Category 2" },
                    Revision = new BaseRevisionEntity { Id = Guid.NewGuid() }
                }
            };

            // Happy path setup
            _mocker.Use(new GetAllByNameRequestValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });

            _mocker.Use(mapper.CreateMapper());

            _mocker.GetMock<IGetAllByNameManager>()
                .Setup(m => m.GetAllByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validCategories);

            _sut = _mocker.CreateInstance<GetAllByNameHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoriesByNameResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryListResponse>(result);
            Assert.Equal(_validCategories.Count, result.Categories.Count);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IGetAllByNameManager>()
                .Verify(m => m.GetAllByNameAsync(_validRequest.Name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            _mocker.Use(new GetAllByNameRequestValidator());
            var invalidRequest = new GetAllCategoriesByNameRequest { Name = "" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(invalidRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetAllByNameManager>()
                .Setup(m => m.GetAllByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

