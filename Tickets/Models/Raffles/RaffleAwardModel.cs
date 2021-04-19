using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Models.Raffles
{
    public class RaffleAwardModel
    {
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public int AwardId { get; set; }
        public int ControlNumber { get; set; }
        public int Fraction { get; set; }
        public int RaffleAwardType { get; set; }
    }
}