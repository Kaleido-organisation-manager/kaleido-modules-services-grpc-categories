using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Create;
using Microsoft.Extensions.Logging;
using Kaleido.Common.Services.Grpc.Exceptions;
using Kaleido.Modules.Services.Grpc.Categories.Common.Models;
using Kaleido.Common.Services.Grpc.Models;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;
using AutoMapper;
using Kaleido.Modules.Services.Grpc.Categories.Mappers;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Create
{
    public class CreateHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly CreateHandler _sut;
        private Category _validRequest;

        public CreateHandlerTests()
        {
            _mocker = new AutoMocker();

            _validRequest = new Category { Name = "Test Category" };

            // Happy path setup
            _mocker.Use(new CategoryValidator());

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryMappingProfile>();
            });
            _mocker.Use(mapper.CreateMapper());

            _mocker.GetMock<ICreateManager>()
                .Setup(m => m.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityLifeCycleResult<CategoryEntity, BaseRevisionEntity> { Entity = new CategoryEntity { Name = "Test Category" }, Revision = new BaseRevisionEntity() });

            _sut = _mocker.CreateInstance<CreateHandler>();
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsCreateCategoryResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoryResponse>(result);
            Assert.Equal("Test Category", result.Category.Name);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert

            _mocker.GetMock<ICreateManager>()
                .Verify(m => m.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsRpcException()
        {
            // Arrange
            _validRequest = new Category { Name = "" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.InvalidArgument, exception.Status.StatusCode);
        }

        [Fact]
        public async Task HandleAsync_ManagerThrowsException_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<ICreateManager>()
                .Setup(m => m.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<RpcException>(() => _sut.HandleAsync(_validRequest));
            Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        }
    }
}

