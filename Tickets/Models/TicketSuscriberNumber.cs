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
    
    public partial class TicketSuscriberNumber
    {
        public int Id { get; set; }
        public int TicketSuscriberId { get; set; }
        public int Number { get; set; }
    
        public virtual TicketSuscriber TicketSuscriber { get; set; }
    }
}
