﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    //using System.Data.Objects;
    using System.Data.Entity.Core.Objects;
    //using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class TicketsEntities : DbContext
    {
        public TicketsEntities()
            : base("name=TicketsEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<Cash> Cashes { get; set; }
        public DbSet<CashClose> CashCloses { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<CertificationNumber> CertificationNumbers { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<DistTown> DistTowns { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<IdentifyBach> IdentifyBaches { get; set; }
        public DbSet<IdentifyBachPayment> IdentifyBachPayments { get; set; }
        public DbSet<IdentifyNumber> IdentifyNumbers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<InvoiceTicket> InvoiceTickets { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<NoteCredit> NoteCredits { get; set; }
        public DbSet<NoteCreditReceiptPayment> NoteCreditReceiptPayments { get; set; }
        public DbSet<OverduelBill> OverduelBills { get; set; }
        public DbSet<PreviousDebtPayment> PreviousDebtPayments { get; set; }
        public DbSet<ProductionCost> ProductionCosts { get; set; }
        public DbSet<Prospect> Prospects { get; set; }
        public DbSet<Prospect_Price> Prospect_Price { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Raffle> Raffles { get; set; }
        public DbSet<ReceiptPayment> ReceiptPayments { get; set; }
        public DbSet<ReceivablePayment> ReceivablePayments { get; set; }
        public DbSet<ReportByEmail> ReportByEmails { get; set; }
        public DbSet<ReturnedOpen> ReturnedOpens { get; set; }
        public DbSet<Rol_Module> Rol_Module { get; set; }
        public DbSet<RRentabilidadSorteo> RRentabilidadSorteos { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<TicketAllocation> TicketAllocations { get; set; }
        public DbSet<TicketAllocationNumber> TicketAllocationNumbers { get; set; }
        public DbSet<TicketRePrint> TicketRePrints { get; set; }
        public DbSet<TicketRePrintNumber> TicketRePrintNumbers { get; set; }
        public DbSet<TicketReturn> TicketReturns { get; set; }
        public DbSet<TicketSuscriber> TicketSuscribers { get; set; }
        public DbSet<TicketSuscriberNumber> TicketSuscriberNumbers { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<TypesAward> TypesAwards { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowProccess> WorkflowProccesses { get; set; }
        public DbSet<WorkflowType> WorkflowTypes { get; set; }
        public DbSet<WorkflowType_User> WorkflowType_User { get; set; }
        public DbSet<AwardCertification> AwardCertification { get; set; }
        public DbSet<RaffleAward> RaffleAwards { get; set; }
        public DbSet<ElectronicTicketSale> ElectronicTicketSales { get; set; }
        public DbSet<TaxReceipt> TaxReceipts { get; set; }
        public DbSet<TaxReceiptNumber> TaxReceiptNumbers { get; set; }
    
        public virtual int procDelOverduelBill()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("procDelOverduelBill");
        }
    
        public virtual int procPopulateOveduelBill()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("procPopulateOveduelBill");
        }
    
        public virtual int sp_ReporteRentabilidadProspecto(Nullable<int> raffleId)
        {
            var raffleIdParameter = raffleId.HasValue ?
                new ObjectParameter("raffleId", raffleId) :
                new ObjectParameter("raffleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_ReporteRentabilidadProspecto", raffleIdParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> sp_GetRentabilidadProspecto(Nullable<int> raffleId)
        {
            var raffleIdParameter = raffleId.HasValue ?
                new ObjectParameter("raffleId", raffleId) :
                new ObjectParameter("raffleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("sp_GetRentabilidadProspecto", raffleIdParameter);
        }
    
        public virtual ObjectResult<Nullable<decimal>> sp_GETCostos(Nullable<int> raffleId)
        {
            var raffleIdParameter = raffleId.HasValue ?
                new ObjectParameter("raffleId", raffleId) :
                new ObjectParameter("raffleId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<decimal>>("sp_GETCostos", raffleIdParameter);
        }
    }
}
