using Cashless.Registration.Domain.Entities;
using Cashless.Registration.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cashless.Registration.Domain.Interfaces
{
    public interface IService
    {
        public Task<(bool isValid, IEnumerable<string> validations)> ValidateCardRequest(IRequest obj);
        public Task<(bool isValid, IEnumerable<string> validations)> ValidateTokenRequest(IRequest obj);

        public int GenerateToken(long cardNumber, int rotations);

        public Card SaveCard(CardRequest cardRequest, int token);

        public Card RetrieveCard(int cardId);

        public bool ValidateToken(TokenRequest tokenRequest);
    }
}