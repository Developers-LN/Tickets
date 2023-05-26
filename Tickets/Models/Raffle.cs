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
    
    public partial class Raffle
    {
        public Raffle()
        {
            this.IdentifyBaches = new HashSet<IdentifyBach>();
            this.Invoices = new HashSet<Invoice>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
            this.NoteCredits = new HashSet<NoteCredit>();
            this.OverduelBills = new HashSet<OverduelBill>();
            this.OverduelBills1 = new HashSet<OverduelBill>();
            this.ProductionCosts = new HashSet<ProductionCost>();
            this.RRentabilidadSorteos = new HashSet<RRentabilidadSorteo>();
            this.ReturnedOpens = new HashSet<ReturnedOpen>();
            this.TicketAllocations = new HashSet<TicketAllocation>();
            this.TicketRePrints = new HashSet<TicketRePrint>();
            this.TicketReturns = new HashSet<TicketReturn>();
            this.RaffleAwards = new HashSet<RaffleAward>();
            this.ElectronicTicketSales = new HashSet<ElectronicTicketSale>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProspectId { get; set; }
        public int Commodity { get; set; }
        public int Statu { get; set; }
        public string Note { get; set; }
        public System.DateTime DateSolteo { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime StartReturnDate { get; set; }
        public System.DateTime EndReturnDate { get; set; }
        public Nullable<System.DateTime> EndAllocationDate { get; set; }
        public string Symbol { get; set; }
        public string Separator { get; set; }
        public Nullable<bool> Consignacion { get; set; }
        public Nullable<System.DateTime> EndElectronicSales { get; set; }
        public Nullable<System.DateTime> DueRaffleDate { get; set; }
        public Nullable<System.DateTime> StartElectronicSales { get; set; }
    
        public virtual ICollection<IdentifyBach> IdentifyBaches { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<NoteCredit> NoteCredits { get; set; }
        public virtual ICollection<OverduelBill> OverduelBills { get; set; }
        public virtual ICollection<OverduelBill> OverduelBills1 { get; set; }
        public virtual ICollection<ProductionCost> ProductionCosts { get; set; }
        public virtual Prospect Prospect { get; set; }
        public virtual ICollection<RRentabilidadSorteo> RRentabilidadSorteos { get; set; }
        public virtual ICollection<ReturnedOpen> ReturnedOpens { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<TicketAllocation> TicketAllocations { get; set; }
        public virtual ICollection<TicketRePrint> TicketRePrints { get; set; }
        public virtual ICollection<TicketReturn> TicketReturns { get; set; }
        public virtual ICollection<RaffleAward> RaffleAwards { get; set; }
        public virtual ICollection<ElectronicTicketSale> ElectronicTicketSales { get; set; }
    }
}
