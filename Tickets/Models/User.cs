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
    
    public partial class User
    {
        public User()
        {
            this.Agencies = new HashSet<Agency>();
            this.Awards = new HashSet<Award>();
            this.Cashes = new HashSet<Cash>();
            this.CertificationNumbers = new HashSet<CertificationNumber>();
            this.Clients = new HashSet<Client>();
            this.Employees = new HashSet<Employee>();
            this.IdentifyBaches = new HashSet<IdentifyBach>();
            this.Invoices = new HashSet<Invoice>();
            this.InvoiceDetails = new HashSet<InvoiceDetail>();
            this.PreviousDebtPayments = new HashSet<PreviousDebtPayment>();
            this.Raffles = new HashSet<Raffle>();
            this.ReceivablePayments = new HashSet<ReceivablePayment>();
            this.TicketAllocations = new HashSet<TicketAllocation>();
            this.TicketAllocationNumbers = new HashSet<TicketAllocationNumber>();
            this.TicketReturns = new HashSet<TicketReturn>();
            this.TicketSuscribers = new HashSet<TicketSuscriber>();
            this.TypesAwards = new HashSet<TypesAward>();
            this.WorkflowTypes = new HashSet<WorkflowType>();
            this.WorkflowType_User = new HashSet<WorkflowType_User>();
            this.Workflows = new HashSet<Workflow>();
            this.WorkflowProccesses = new HashSet<WorkflowProccess>();
            this.webpages_Roles = new HashSet<webpages_Roles>();
            this.AwardCertification = new HashSet<AwardCertification>();
            this.ElectronicTicketSales = new HashSet<ElectronicTicketSale>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Statu { get; set; }
        public Nullable<int> EmpleadoId { get; set; }
        public Nullable<int> Connected { get; set; }
    
        public virtual ICollection<Agency> Agencies { get; set; }
        public virtual ICollection<Award> Awards { get; set; }
        public virtual ICollection<Cash> Cashes { get; set; }
        public virtual Catalog Catalog { get; set; }
        public virtual ICollection<CertificationNumber> CertificationNumbers { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<IdentifyBach> IdentifyBaches { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual ICollection<PreviousDebtPayment> PreviousDebtPayments { get; set; }
        public virtual ICollection<Raffle> Raffles { get; set; }
        public virtual ICollection<ReceivablePayment> ReceivablePayments { get; set; }
        public virtual ICollection<TicketAllocation> TicketAllocations { get; set; }
        public virtual ICollection<TicketAllocationNumber> TicketAllocationNumbers { get; set; }
        public virtual ICollection<TicketReturn> TicketReturns { get; set; }
        public virtual ICollection<TicketSuscriber> TicketSuscribers { get; set; }
        public virtual ICollection<TypesAward> TypesAwards { get; set; }
        public virtual ICollection<WorkflowType> WorkflowTypes { get; set; }
        public virtual ICollection<WorkflowType_User> WorkflowType_User { get; set; }
        public virtual ICollection<Workflow> Workflows { get; set; }
        public virtual ICollection<WorkflowProccess> WorkflowProccesses { get; set; }
        public virtual ICollection<webpages_Roles> webpages_Roles { get; set; }
        public virtual ICollection<AwardCertification> AwardCertification { get; set; }
        public virtual ICollection<ElectronicTicketSale> ElectronicTicketSales { get; set; }
    }
}
