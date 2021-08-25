using Cashless.Registration.Domain.Entities;
using Cashless.Registration.Domain.Interfaces;
using Cashless.Registration.Domain.Models;
using Cashless.Registration.Service.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cashless.Registration.Service.Test.Services
{
    public class CardServiceTest
    {
        private readonly IService _cardService;
        private readonly IMemoryCache _memoryCache;

        private Card _card;

        public CardServiceTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            _memoryCache = serviceProvider.GetService<IMemoryCache>();

            _cardService = new CardService(_memoryCache);

            Setup();
        }

        [Fact]
        public async Task ValidateCardRequest_GivenAValidRequest_ShouldReturnAValidResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 11111234,
                CustomerId = 1,
                CVV = 1111
            };

            var (isValid, validations) = await _cardService.ValidateCardRequest(cardRequest);

            Assert.True(isValid);
            Assert.Empty(validations);
        }

        [Fact]
        public async Task ValidateCardRequest_GivenAnInvalidRequest_ShouldReturnAnInvalidResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 1111123411111111111,
                CustomerId = 1,
                CVV = 111111
            };

            var (isValid, validations) = await _cardService.ValidateCardRequest(cardRequest);

            Assert.False(isValid);
            Assert.NotEmpty(validations);
            Assert.Equal(2, validations.Count());
        }

        [Fact]
        public void GenerateToken_GivenACardNumberAndTwoRotations_ShouldReturnATokenWithTwoRotations()
        {
            var token = _cardService.GenerateToken(11111234, 2);

            Assert.Equal(3412, token);
        }

        [Fact]
        public void SaveCard_GivenAValidRequest_ShouldSaveTheCardObject()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 11111234,
                CustomerId = 1,
                CVV = 1111
            };

            var card = _cardService.SaveCard(cardRequest, 3412);

            Assert.NotNull(card);
            Assert.True(card.CardId > 0);
        }

        [Fact]
        public void RetrieveCard_GivenAnInvalidCardId_ShouldNotReturnACard()
        {
            var card = _cardService.RetrieveCard(11);

            Assert.Null(card);
        }

        [Fact]
        public void RetrieveCard_GivenAValidCardId_ShouldReturnACard()
        {
            var card = _cardService.RetrieveCard(1);

            Assert.NotNull(card);
        }

        [Fact]
        public async Task ValidateTokenRequest_GivenAValidRequest_ShouldReturnAValidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 1,
                Token = 1234,
                CVV = 1111
            };

            var (isValid, validations) = await _cardService.ValidateTokenRequest(tokenRequest);

            Assert.True(isValid);
            Assert.Empty(validations);
        }

        [Fact]
        public async Task ValidateTokenRequest_GivenAnInvalidRequest_ShouldReturnAnInvalidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 1,
                Token = 1234,
                CVV = 1111111
            };

            var (isValid, validations) = await _cardService.ValidateTokenRequest(tokenRequest);

            Assert.False(isValid);
            Assert.NotEmpty(validations);
            Assert.Single(validations);
        }

        [Fact]
        public void ValidateToken_GivenAValidTokenRequest_ShouldReturnAValidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 1,
                Token = 2341,
                CVV = 1111
            };

            var isValid = _cardService.ValidateToken(tokenRequest);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidateToken_GivenATokenRequestPastTheTokenValidMinutes_ShouldReturnAnInvalidResult()
        {
            _card.CreatedOn = DateTime.UtcNow.AddMinutes(-31);

            var context = _memoryCache.Get<List<Card>>("CardContext");
            
            context.Clear();
            context.Add(_card);

            _memoryCache.Set("CardContext", context);

            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 1,
                Token = 2341,
                CVV = 1111
            };

            var isValid = _cardService.ValidateToken(tokenRequest);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_GivenATokenRequestWithInvalidCardId_ShouldRaiseAnException()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 11,
                Token = 2341,
                CVV = 1111
            };

            Assert.Throws<Exception>(() => _cardService.ValidateToken(tokenRequest));
        }

        [Fact]
        public void ValidateToken_GivenATokenRequestWithInvalidCustomerId_ShouldReturnAnInvalidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 2,
                CardId = 1,
                Token = 2341,
                CVV = 1111
            };

            var isValid = _cardService.ValidateToken(tokenRequest);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_GivenATokenRequestWithAWrongToken_ShouldReturnAnInvalidResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 2,
                CardId = 1,
                Token = 2314,
                CVV = 1111
            };

            var isValid = _cardService.ValidateToken(tokenRequest);

            Assert.False(isValid);
        }

        private void Setup()
        {
            _card = new Card
            {
                CardId = 1,
                CardNumber = 11111234,
                CreatedOn = DateTime.UtcNow,
                CustomerId = 1,
                CVV = 1111
            };

            var context = new List<Card>
            {
                _card
            };

            _memoryCache.Set("CardContext", context);
        }
    }
}