using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Models.Ticket
{
    public class AwardNumberModel
    {
        public int Number { get; set; }
        public int Fraction { get; set; }
        public int RaffleId { get; set; }
    }
}