using System;

namespace Cashless.Registration.Domain.Entities
{
    public class Card
    {
        public int CardId { get; set; }

        public int CustomerId { get; set; }

        public int CVV { get; set; }

        public long CardNumber { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Token { get; set; }
    }
}