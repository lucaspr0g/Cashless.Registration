using Cashless.Registration.Domain.Interfaces;

namespace Cashless.Registration.Domain.Models
{
    public class CardRequest : IRequest
    {
        public int CustomerId { get; set; }

        public long CardNumber { get; set; }

        public int CVV { get; set; }
    }
}