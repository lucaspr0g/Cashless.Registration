using Cashless.Registration.Domain.Entities;
using Cashless.Registration.Domain.Interfaces;
using Cashless.Registration.Domain.Models;
using Cashless.Registration.Service.Validators;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cashless.Registration.Service.Services
{
    public class CardService : IService
    {
        private const int MaxTokenValidMinutes = 30;
        private const string ContextName = "CardContext";
        private readonly IMemoryCache _memoryCache;

        private readonly CardRequestValidator _cardRequestValidator;
        private readonly TokenRequestValidator _tokenRequestValidator;

        public CardService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _cardRequestValidator = new CardRequestValidator();
            _tokenRequestValidator = new TokenRequestValidator();
        }

        public async Task<(bool isValid, IEnumerable<string> validations)> ValidateCardRequest(IRequest obj)
        {
            var result = await _cardRequestValidator.ValidateAsync((CardRequest)obj);

            return (result.IsValid, result.Errors?.Select(s => s.ErrorMessage));
        }

        public int GenerateToken(long cardNumber, int rotations)
        {
            var cardLastFourDigits = (cardNumber % 10000)
                .ToString()
                .Select(s => int.Parse(s.ToString()))
                .ToArray();

            var arrayLength = cardLastFourDigits.Length - 1;

            for (int i = 0; i < rotations; i++)
                cardLastFourDigits = cardLastFourDigits.Skip(arrayLength).Concat(cardLastFourDigits.Take(arrayLength)).ToArray();

            int.TryParse(string.Join(string.Empty, cardLastFourDigits), out int token);

            return token;
        }

        public Card SaveCard(CardRequest cardInformationRequest, int token)
        {
            var cardInformation = new Card
            {
                CardId = new Random().Next(1, 1000),
                CustomerId = cardInformationRequest.CustomerId,
                CVV = cardInformationRequest.CVV,
                CreatedOn = DateTime.UtcNow,
                CardNumber = cardInformationRequest.CardNumber
            };

            var context = _memoryCache.Get<List<Card>>(ContextName);

            if (context is null)
                context = new List<Card>();

            context.Add(cardInformation);

            _memoryCache.Set(ContextName, context);

            return cardInformation;
        }

        public Card RetrieveCard(int cardId)
        {
            var context = _memoryCache.Get<List<Card>>(ContextName);

            return context?.FirstOrDefault(s => s.CardId == cardId);
        }

        public async Task<(bool isValid, IEnumerable<string> validations)> ValidateTokenRequest(IRequest obj)
        {
            var result = await _tokenRequestValidator.ValidateAsync((TokenRequest)obj);

            return (result.IsValid, result.Errors?.Select(s => s.ErrorMessage));
        }

        public bool ValidateToken(TokenRequest tokenRequest)
        {
            var card = RetrieveCard(tokenRequest.CardId);

            if (card is null)
                throw new Exception("Invalid card");

            if ((DateTime.UtcNow - card.CreatedOn).TotalMinutes > MaxTokenValidMinutes)
                return false;

            if (tokenRequest.CustomerId != card.CustomerId)
                return false;

            Console.WriteLine($"Card number: {card.CardNumber}");

            var token = GenerateToken(card.CardNumber, card.CVV);

            return token == tokenRequest.Token;
        }
    }
}