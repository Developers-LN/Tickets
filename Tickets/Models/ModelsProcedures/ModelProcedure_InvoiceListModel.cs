using System;

namespace Tickets.Models.ModelsProcedures
{
    public class ModelProcedure_InvoiceListModel
    {
        public bool Data { get; set; }
        public int Id { get; set; }
        public int SequenceNumberInvoice { get; set; }
        public int ClientId { get; set; }
        public string ClientDesc { get; set; }
        public int RaffleId { get; set; }
        public decimal totalInvoice { get; set; }
        public decimal discount { get; set; }
        public decimal totalQuantity { get; set; }
        public decimal totalRestant { get; set; }
        public decimal totalPayer { get; set; }
        public int PaymentStatu { get; set; }
        public string InvoiceDate { get; set; }
        public string Nomenclatura { get; set; }
        public string xpiredDate { get; set; }
        public string PaymentStatuDesc { get; set; }
	}
}
