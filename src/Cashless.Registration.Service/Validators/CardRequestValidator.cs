using Cashless.Registration.Domain.Models;
using FluentValidation;
using System;

namespace Cashless.Registration.Service.Validators
{
    public class CardRequestValidator : AbstractValidator<CardRequest>
    {
        private const int MaxCardNumberCharacters = 16;
        private const int MaxCVVCharacters = 5;

        public CardRequestValidator()
        {
            RuleFor(c => c.CustomerId)
                .GreaterThan(0);

            RuleFor(c => c.CardNumber)
                .Custom((cardNumber, context) => 
                {
                    if (Math.Floor(Math.Log10(cardNumber) + 1) > MaxCardNumberCharacters)
                        context.AddFailure($"Card number max allowed characters is {MaxCardNumberCharacters}");
                });

            RuleFor(c => c.CVV)
                .Custom((cvv, context) =>
                {
                    if (Math.Floor(Math.Log10(cvv) + 1) > MaxCVVCharacters)
                        context.AddFailure($"CVV max allowed characters is {MaxCVVCharacters}");
                });
        }
    }
}