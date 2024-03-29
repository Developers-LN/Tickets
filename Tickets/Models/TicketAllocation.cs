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
    
    public partial class TicketAllocation
    {
        public TicketAllocation()
        {
            this.TicketAllocationNumbers = new HashSet<TicketAllocationNumber>();
            this.TicketAllocationNumber_Delete = new HashSet<TicketAllocationNumber_Delete>();
        }
    
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public int Type { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public int Statu { get; set; }
        public string Agente { get; set; }
        public Nullable<int> SequenceNumber { get; set; }
        public Nullable<int> AllocationSequence { get; set; }
        public Nullable<int> ImpresionType { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Raffle Raffle { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<TicketAllocationNumber> TicketAllocationNumbers { get; set; }
        public virtual ICollection<TicketAllocationNumber_Delete> TicketAllocationNumber_Delete { get; set; }
    }
}
