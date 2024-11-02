using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.GetAllByName
{
    public class GetAllByNameRequestValidatorTests
    {
        private readonly GetAllByNameRequestValidator _sut;

        public GetAllByNameRequestValidatorTests()
        {
            _sut = new GetAllByNameRequestValidator();
        }

        [Fact]
        public void Validate_ValidRequest_ReturnsSuccess()
        {
            var request = new GetAllCategoriesByNameRequest { Name = "Test" };
            var result = _sut.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_InvalidRequest_ReturnsFailure()
        {
            var request = new GetAllCategoriesByNameRequest { Name = "" };
            var result = _sut.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Validate_WhitespaceStringRequest_ReturnsFailure()
        {
            var result = _sut.Validate(new GetAllCategoriesByNameRequest { Name = "      " });
            Assert.False(result.IsValid);
        }
    }
}

