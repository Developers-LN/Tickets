//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tickets.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TicketRePrint
    {
        public TicketRePrint()
        {
            this.TicketRePrintNumbers = new HashSet<TicketRePrintNumber>();
        }
    
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public string Note { get; set; }
        public int Statu { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public Nullable<int> TicketRePrintSequence { get; set; }
    
        public virtual Raffle Raffle { get; set; }
        public virtual ICollection<TicketRePrintNumber> TicketRePrintNumbers { get; set; }
    }
}
