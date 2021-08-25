using System;

namespace Cashless.Registration.Domain.Models
{
    public class CardInformationResponse
    {
        public DateTime RegistrationDate { get; set; }

        public long Token { get; set; }

        public int CardId { get; set; }
    }
}