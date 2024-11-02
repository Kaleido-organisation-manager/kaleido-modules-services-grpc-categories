using Kaleido.Grpc.Categories;
using Kaleido.Modules.Services.Grpc.Categories.Common.Validators;

namespace Kaleido.Modules.Services.Grpc.Categories.Tests.Unit.Update
{
    public class UpdateRequestValidatorTests
    {
        private readonly CategoryActionValidator _sut;

        public UpdateRequestValidatorTests()
        {
            _sut = new CategoryActionValidator();
        }

        [Fact]
        public async Task ValidateAsync_ValidRequest_ReturnsSuccess()
        {
            var request = new CategoryActionRequest { Key = Guid.NewGuid().ToString(), Category = new Category { Name = "Test" } };
            var result = await _sut.ValidateAsync(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_InvalidKey_ReturnsError()
        {
            var request = new CategoryActionRequest { Key = "invalid-key", Category = new Category { Name = "Test" } };
            var result = await _sut.ValidateAsync(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_InvalidCategory_ReturnsError()
        {
            var request = new CategoryActionRequest { Key = Guid.NewGuid().ToString(), Category = new Category { Name = "" } };
            var result = await _sut.ValidateAsync(request);
            Assert.False(result.IsValid);
        }
    }
}

