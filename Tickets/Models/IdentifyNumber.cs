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
    
    public partial class IdentifyNumber
    {
        public int Id { get; set; }
        public int NumberId { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public int IdentifyBachId { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> IdentifyBachNumberType { get; set; }
        public Nullable<int> FirstValid { get; set; }
        public Nullable<System.DateTime> ElectronicPayedDate { get; set; }
    
        public virtual IdentifyBach IdentifyBach { get; set; }
        public virtual TicketAllocationNumber TicketAllocationNumber { get; set; }
    }
}
