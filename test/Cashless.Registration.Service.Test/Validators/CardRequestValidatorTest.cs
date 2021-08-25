using Cashless.Registration.Domain.Models;
using Cashless.Registration.Service.Validators;
using Xunit;

namespace Cashless.Registration.Service.Test.Validators
{
    public class CardRequestValidatorTest
    {
        private readonly CardRequestValidator _cardRequestValidator;

        public CardRequestValidatorTest()
        {
            _cardRequestValidator = new CardRequestValidator();
        }

        [Fact]
        public void Validate_GivenAValidInput_ShouldReturnAValidResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 11111234,
                CustomerId = 1,
                CVV = 1111
            };

            var result = _cardRequestValidator.Validate(cardRequest);

            Assert.NotNull(result);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenAnInvalidInput_ShouldReturnAnInvalidResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 1111123411111111111,
                CustomerId = -1,
                CVV = 111111
            };

            var result = _cardRequestValidator.Validate(cardRequest);

            Assert.NotNull(result);
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
        }
    }
}