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
    
    public partial class ReceiptPayment
    {
        public ReceiptPayment()
        {
            this.NoteCreditReceiptPayments = new HashSet<NoteCreditReceiptPayment>();
        }
    
        public int Id { get; set; }
        public int CashId { get; set; }
        public int ClientId { get; set; }
        public int InvoiceId { get; set; }
        public System.DateTime ReceiptDate { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalCheck { get; set; }
        public int ReceiptType { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string Recibo { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Observaciones { get; set; }
        public string Telefono { get; set; }
        public string Codigo { get; set; }
        public string Notas { get; set; }
        public Nullable<int> SequenceNumber { get; set; }
        public Nullable<int> SequenceType { get; set; }
        public string Nomenclature { get; set; }
        public Nullable<int> ReceiptSequence { get; set; }
        public Nullable<int> Digits { get; set; }
    
        public virtual Cash Cash { get; set; }
        public virtual Client Client { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual ICollection<NoteCreditReceiptPayment> NoteCreditReceiptPayments { get; set; }
    }
}
