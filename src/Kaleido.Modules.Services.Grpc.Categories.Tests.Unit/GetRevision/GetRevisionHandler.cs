using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetRevision;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetRevision
{
    public class GetRevisionHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetRevisionHandler _sut;
        private readonly GetCategoryRevisionRequest _validRequest;
        private readonly CategoryRevision _validRevision;

        public GetRevisionHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetRevisionHandler>();

            _validRequest = new GetCategoryRevisionRequest { Key = "valid-key", Revision = 1 };
            _validRevision = new CategoryRevision { Key = "valid-key", Name = "Test Category", Revision = 1 };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<GetCategoryRevisionRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetCategoryRevisionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IGetRevisionManager>()
                .Setup(m => m.GetRevisionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validRevision);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetCategoryRevisionResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetCategoryRevisionResponse>(result);
            Assert.Equal(_validRevision, result.Revision);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<GetCategoryRevisionRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IGetRevisionManager>()
                .Verify(m => m.GetRevisionAsync(_validRequest.Key, _validRequest.Revision, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");
            _mocker.GetMock<IRequestValidator<GetCategoryRevisionRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetCategoryRevisionRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
        }

        [Fact]
        public async Task HandleAsync_ManagerReturnsNull_ThrowsRpcException()
        {
            // Arrange
            _mocker.GetMock<IGetRevisionManager>()
                .Setup(m => m.GetRevisionAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryRevision?)null);

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

