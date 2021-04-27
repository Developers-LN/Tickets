using System;

namespace Tickets.Models.Ticket
{
    public class OpenReturnedModel
    {
        public int RaffleId { get; set; }
        public string Note { get; set; }
        public DateTime EndReturnedDate { get; set; }
    }

}