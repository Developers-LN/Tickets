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
    
    public partial class TicketRePrintNumber
    {
        public int Id { get; set; }
        public int TicketRePrintId { get; set; }
        public int TicketAllocationNumberId { get; set; }
        public string Serie { get; set; }
    
        public virtual TicketAllocationNumber TicketAllocationNumber { get; set; }
        public virtual TicketRePrint TicketRePrint { get; set; }
    }
}
