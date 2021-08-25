using Cashless.Registration.Domain.Interfaces;

namespace Cashless.Registration.Domain.Models
{
    public class TokenRequest : IRequest
    {
        public int CustomerId { get; set; }

        public int CardId { get; set; }

        public long Token { get; set; }

        public int CVV { get; set; }
    }
}