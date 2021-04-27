using System;

namespace Tickets.Models
{
    public class PaymentReceivableReportModel
    {
        public int Id { get; set; }

        public string ClientName { get; set; }

        public string PaymentType { get; set; }

        public decimal Value { get; set; }
        public int UserId { get; set; }
        public int RaffleId { get; set; }
        public DateTime CreateDate { get; set; }
        public IdentifyBach IdentifyBach { get; set; }
        public int ReceivablePercent { get; set; }
        public int CreditNoteId { get; set; }
        public string Concept { get; set; }
    }
}