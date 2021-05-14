using System;
using System.Collections.Generic;

namespace Tickets.Models.Ticket
{
    public class InvoiceModel
    {
        public int RaffleId { get; set; }
        public int ClientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int Condition { get; set; }
        public int PaymentType { get; set; }
        public List<int> AllocationIds { get; set; }
        public decimal Price { get; set; }
        public int InvoiceExpredDay { get; set; }
    }
}