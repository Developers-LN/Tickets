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
    
    public partial class ProductionCost
    {
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public string Detalle { get; set; }
        public int Cantidad { get; set; }
        public decimal Monto { get; set; }
        public System.DateTime Created { get; set; }
        public bool Status { get; set; }
    
        public virtual Raffle Raffle { get; set; }
    }
}
