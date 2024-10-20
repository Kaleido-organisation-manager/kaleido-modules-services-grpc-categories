using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllByNameHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllByNameHandler _sut;
        private readonly GetAllCategoriesByNameRequest _validRequest;
        private readonly List<Category> _validCategories;

        public GetAllByNameHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllByNameHandler>();

            _validRequest = new GetAllCategoriesByNameRequest { Name = "Test Category" };
            _validCategories = new List<Category>
            {
                new Category { Key = "key1", Name = "Test Category 1" },
                new Category { Key = "key2", Name = "Test Category 2" }
            };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<GetAllCategoriesByNameRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoriesByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IGetAllByNameManager>()
                .Setup(m => m.GetAllByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validCategories);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoriesByNameResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetAllCategoriesByNameResponse>(result);
            Assert.Equal(_validCategories.Count, result.Categories.Count);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<GetAllCategoriesByNameRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IGetAllByNameManager>()
                .Verify(m => m.GetAllByNameAsync(_validRequest.Name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Name"], "Invalid name format");
            _mocker.GetMock<IRequestValidator<GetAllCategoriesByNameRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoriesByNameRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
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

