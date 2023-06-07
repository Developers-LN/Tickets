using System;

namespace Tickets.Models.ModelsProcedures
{
    public class ModelProcedure_SalesAndPendingPayments
    {
        public bool Data { get; set; }
        public int ReportType { get; set; }
        public int IdClient { get; set; }
        public string NameClient { get; set; }
        public string TypeClient { get; set; }
        public int IdRaffle { get; set; }
        public string NameRaffle { get; set; }
        //public int TicketReturn { get; set; }
        //public int FractionReturn { get; set; }
        public int IdInvoice { get; set; }
        public DateTime DateInvoice { get; set; }
        public string StatusInvoice { get; set; }
        public int TotalTickets { get; set; }
        public int TotalFractions { get; set; }
        public decimal PriceTicket { get; set; }
        public decimal TotalInvoice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal CashPayment { get; set; }
        public decimal NoteCreditPayment { get; set; }
        public decimal TotalPayed { get; set; }
        public decimal TotalPending { get; set; }
    }
}
