using Cashless.Registration.Domain.Models;
using FluentValidation;
using System;

namespace Cashless.Registration.Service.Validators
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        private const int MaxCVVCharacters = 5;

        public TokenRequestValidator()
        {
            RuleFor(c => c.CustomerId)
                .GreaterThan(0);

            RuleFor(c => c.CVV)
                .Custom((cvv, context) =>
                {
                    if (Math.Floor(Math.Log10(cvv) + 1) > MaxCVVCharacters)
                        context.AddFailure($"CVV max allowed characters is {MaxCVVCharacters}");
                });
        }
    }
}