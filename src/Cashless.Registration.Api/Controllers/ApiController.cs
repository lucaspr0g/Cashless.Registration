 using Cashless.Registration.Domain.Interfaces;
using Cashless.Registration.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cashless.Registration.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IService _cardService;

        public ApiController(IService cardInformationService)
        {
            _cardService = cardInformationService;
        }

        [HttpPost("receive-card-information")]
        public async Task<IActionResult> ReceiveCardInformation([FromBody] CardRequest cardRequest)
        {
            try
            {
                var (isValid, validations) = await _cardService.ValidateCardRequest(cardRequest);

                if (!isValid)
                    return new BadRequestObjectResult(validations);

                var token = _cardService.GenerateToken(cardRequest.CardNumber, cardRequest.CVV);

                var cardInformation = _cardService.SaveCard(cardRequest, token);

                var response = new CardInformationResponse
                {
                    CardId = cardInformation.CardId,
                    RegistrationDate = cardInformation.CreatedOn,
                    Token = token
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                var (isValid, validations) = await _cardService.ValidateTokenRequest(tokenRequest);

                if (!isValid)
                    return new BadRequestObjectResult(validations);

                isValid = _cardService.ValidateToken(tokenRequest);

                return new OkObjectResult(new TokenResponse { Validated = isValid });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}