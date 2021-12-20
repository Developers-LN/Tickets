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
    
    public partial class Client
    {
        public Client()
        {
            this.OverduelBills = new HashSet<OverduelBill>();
            this.OverduelBills1 = new HashSet<OverduelBill>();
            this.IdentifyBaches = new HashSet<IdentifyBach>();
            this.Invoices = new HashSet<Invoice>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
            this.NoteCredits = new HashSet<NoteCredit>();
            this.PreviousDebtPayments = new HashSet<PreviousDebtPayment>();
            this.IdentifyBachPayments = new HashSet<IdentifyBachPayment>();
            this.ReceiptPayments = new HashSet<ReceiptPayment>();
            this.TicketAllocations = new HashSet<TicketAllocation>();
            this.TicketReturns = new HashSet<TicketReturn>();
            this.TicketSuscribers = new HashSet<TicketSuscriber>();
            this.ElectronicTicketSales = new HashSet<ElectronicTicketSale>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tradename { get; set; }
        public string RNC { get; set; }
        public string DocumentNumber { get; set; }
        public int MaritalStatus { get; set; }
        public int Gender { get; set; }
        public System.DateTime Birthday { get; set; }
        public int Province { get; set; }
        public int Town { get; set; }
        public int Section { get; set; }
        public string Addres { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public int Statu { get; set; }
        public decimal CreditLimit { get; set; }
        public string Comment { get; set; }
        public int ClientType { get; set; }
        public int GroupId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public int PriceId { get; set; }
        public Nullable<decimal> AmountDeposit { get; set; }
        public string DepositDocument { get; set; }
        public string AdminDocument { get; set; }
        public decimal Discount { get; set; }
        public decimal PreviousDebt { get; set; }
        public string ControlNumber { get; set; }
    
        public virtual Catalog Catalog { get; set; }
        public virtual ICollection<OverduelBill> OverduelBills { get; set; }
        public virtual ICollection<OverduelBill> OverduelBills1 { get; set; }
        public virtual DistTown DistTown { get; set; }
        public virtual Province Province1 { get; set; }
        public virtual Town Town1 { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<IdentifyBach> IdentifyBaches { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<NoteCredit> NoteCredits { get; set; }
        public virtual ICollection<PreviousDebtPayment> PreviousDebtPayments { get; set; }
        public virtual ICollection<IdentifyBachPayment> IdentifyBachPayments { get; set; }
        public virtual ICollection<ReceiptPayment> ReceiptPayments { get; set; }
        public virtual ICollection<TicketAllocation> TicketAllocations { get; set; }
        public virtual ICollection<TicketReturn> TicketReturns { get; set; }
        public virtual ICollection<TicketSuscriber> TicketSuscribers { get; set; }
        public virtual ICollection<ElectronicTicketSale> ElectronicTicketSales { get; set; }
    }
}
