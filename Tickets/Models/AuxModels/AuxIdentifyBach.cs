using System;
using System.Collections.Generic;

namespace Tickets.Models.AuxModels
{
    public class AuxIdentifyBach
    {
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public int ClientId { get; set; }
        public int DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string WinnerName { get; set; }
        public string WinnerPhone { get; set; }
        public string Notes { get; set; }
        public int Type { get; set; }
        public Nullable<int> WinnerId { get; set; }

        public virtual ICollection<IdentifyNumber> IdentifyNumbers { get; set; }
    }
}