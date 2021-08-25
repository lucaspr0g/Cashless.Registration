using Cashless.Registration.Domain.Models;
using Cashless.Registration.Service.Validators;
using Xunit;

namespace Cashless.Registration.Service.Test.Validators
{
    public class TokenRequestValidatorTest
    {
        private readonly TokenRequestValidator _tokenRequestValidator;

        public TokenRequestValidatorTest()
        {
            _tokenRequestValidator = new TokenRequestValidator();
        }

        [Fact]
        public void Validate_GivenAValidInput_ShouldReturnAValidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CardId = 1,
                CustomerId = 1,
                CVV = 1111,
                Token = 1111
            };

            var result = _tokenRequestValidator.Validate(tokenRequest);

            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenAnInvalidInput_ShouldReturnAnInvalidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CardId = 1,
                CustomerId = -1,
                CVV = 111111,
                Token = 1111
            };

            var result = _tokenRequestValidator.Validate(tokenRequest);

            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count);
        }
    }
}