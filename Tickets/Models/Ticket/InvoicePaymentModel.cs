namespace Tickets.Models.Ticket
{
    public class InvoicePaymentModel
    {
        public decimal totalInvoice { get; set; }
        public decimal totalPayment { get; set; }
        public decimal totalReturned { get; set; }
        public decimal totalRestant { get; set; }
        public decimal totalCreditNote { get; set; }
        public decimal discount { get; set; }
    }
}