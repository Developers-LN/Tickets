using System;

namespace Tickets.Models.Ticket
{
    public class OtherIncomesModel
    {
        public int Id { get; set; }

        public int OtherIncomesGroupId { get; set; }

        public int OtherIncomeId { get; set; }

        public string OtherIncomeDetailDescription { get; set; }

        public int BankAccountCatalogId { get; set; }

        public decimal TotalPayment { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}