using Cashless.Registration.Api.Controllers;
using Cashless.Registration.Domain.Entities;
using Cashless.Registration.Domain.Interfaces;
using Cashless.Registration.Domain.Models;
using Cashless.Registration.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Cashless.Registration.Api.Test.Controllers
{
    public class ControllerTest
    {
        private readonly ApiController _controller;
        private readonly IService _cardService;
        private readonly IMemoryCache _memoryCache;

        public ControllerTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();

            var serviceProvider = services.BuildServiceProvider();

            _memoryCache = serviceProvider.GetService<IMemoryCache>();

            _cardService = new CardService(_memoryCache);
            _controller = new ApiController(_cardService);

            Setup();
        }

        [Fact]
        public async Task ReceiveCardInformation_GivenAValidRequest_ShoudlReturnAnOkObjectResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 11111234,
                CustomerId = 1,
                CVV = 1111
            };

            var result = await _controller.ReceiveCardInformation(cardRequest);

            Assert.True(result is OkObjectResult);
        }

        [Fact]
        public async Task ReceiveCardInformation_GivenAnInvalidRequest_ShoudlReturnABadRequestObjectResult()
        {
            var cardRequest = new CardRequest
            {
                CardNumber = 11111234,
                CustomerId = 0,
                CVV = 1111
            };

            var result = await _controller.ReceiveCardInformation(cardRequest);

            Assert.True(result is BadRequestObjectResult);
        }

        [Fact]
        public async Task ValidateToken_GivenAValidRequest_ShoudlReturnAnOkObjectResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 1,
                Token = 1111,
                CVV = 1111
            };

            var result = await _controller.ValidateToken(tokenRequest);

            Assert.True(result is OkObjectResult);
        }

        [Fact]
        public async Task ValidateToken_GivenAnInvalidRequest_ShoudlReturnABadRequestObjectResult()
        {
            var tokenRequest = new TokenRequest
            {
                CustomerId = 1,
                CardId = 2,
                Token = 1111,
                CVV = 1111
            };

            var result = await _controller.ValidateToken(tokenRequest);

            Assert.True(result is BadRequestObjectResult);
        }

        private void Setup()
        {
            var context = new List<Card>
            {
                new Card
                {
                    CardId = 1,
                    CardNumber = 11111234,
                    CreatedOn = new DateTime(2021, 08, 23, 13, 00, 00),
                    CustomerId = 1,
                    CVV = 1111
                }
            };

            _memoryCache.Set("CardContext", context);
        }
    }
}