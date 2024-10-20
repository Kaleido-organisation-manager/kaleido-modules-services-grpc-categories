using Xunit;
using Moq;
using Moq.AutoMock;
using Grpc.Core;
using Kaleido.Common.Services.Grpc.Models.Validations;
using Kaleido.Common.Services.Grpc.Validators;
using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kaleido.Common.Services.Grpc.Exceptions;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllRevisions
{
    public class GetAllRevisionsHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly GetAllRevisionsHandler _sut;
        private readonly GetAllCategoryRevisionsRequest _validRequest;
        private readonly List<CategoryRevision> _validRevisions;

        public GetAllRevisionsHandlerTests()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<GetAllRevisionsHandler>();

            _validRequest = new GetAllCategoryRevisionsRequest { Key = "valid-key" };
            _validRevisions = new List<CategoryRevision>
            {
                new CategoryRevision { Key = "valid-key", Name = "Revision 1", Revision = 1 },
                new CategoryRevision { Key = "valid-key", Name = "Revision 2", Revision = 2 }
            };

            // Happy path setup
            _mocker.GetMock<IRequestValidator<GetAllCategoryRevisionsRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoryRevisionsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mocker.GetMock<IGetAllRevisionsManager>()
                .Setup(m => m.HandleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_validRevisions);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_ReturnsGetAllCategoryRevisionsResponse()
        {
            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetAllCategoryRevisionsResponse>(result);
            Assert.Equal(_validRevisions.Count, result.Revisions.Count);
        }

        [Fact]
        public async Task HandleAsync_ValidRequest_CallsValidatorAndManager()
        {
            // Act
            await _sut.HandleAsync(_validRequest);

            // Assert
            _mocker.GetMock<IRequestValidator<GetAllCategoryRevisionsRequest>>()
                .Verify(v => v.ValidateAsync(_validRequest, It.IsAny<CancellationToken>()), Times.Once);

            _mocker.GetMock<IGetAllRevisionsManager>()
                .Verify(m => m.HandleAsync(_validRequest.Key, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_ValidationFails_ThrowsValidationException()
        {
            // Arrange
            var validationResult = new ValidationResult();
            validationResult.AddInvalidFormatError(["Key"], "Invalid key format");
            _mocker.GetMock<IRequestValidator<GetAllCategoryRevisionsRequest>>()
                .Setup(v => v.ValidateAsync(It.IsAny<GetAllCategoryRevisionsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _sut.HandleAsync(_validRequest));
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
                .ReturnsAsync(new List<CategoryRevision>());

            // Act
            var result = await _sut.HandleAsync(_validRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<GetAllCategoryRevisionsResponse>(result);
            Assert.Empty(result.Revisions);
        }
    }
}

