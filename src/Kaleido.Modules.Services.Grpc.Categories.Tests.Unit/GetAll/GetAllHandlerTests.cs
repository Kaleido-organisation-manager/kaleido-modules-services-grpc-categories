using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAll;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAll
{
    public class GetAllHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllHandler _sut;
        private readonly GetAllCategoriesRequest _validRequest;
        private readonly List<Category> _categories;

        public GetAllHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllHandler>();

            _validRequest = new GetAllCategoriesRequest();

            _categories = new List<Category>
            {
                new Category { Key = "key1", Name = "Category 1" },
                new Category { Key = "key2", Name = "Category 2" }
            };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<GetAllCategoriesRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoriesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IGetAllManager>()
                .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(_categories);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoriesResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetAllCategoriesResponse>(result);
            Assert.Equal(_categories.Count, result.Categories.Count);
            Assert.Equal(_categories[0].Key, result.Categories[0].Key);
            Assert.Equal(_categories[0].Name, result.Categories[0].Name);
            Assert.Equal(_categories[1].Key, result.Categories[1].Key);
            Assert.Equal(_categories[1].Name, result.Categories[1].Name);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<GetAllCategoriesRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IGetAllManager>()
                .Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["SomeField"], "Invalid format");
            _mocker.GetMock<IRequestValidator<GetAllCategoriesRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoriesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
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

