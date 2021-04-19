using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tickets.Models.Enums;
using Tickets.Models;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketInvoiceNumberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "invoiceId")]
        public int InvoiceId { get; set; }

        [JsonProperty(PropertyName = "pricePerFraction")]
        public decimal PricePerFraction { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "ticketAllocationNumberId")]
        public int TicketAllocationNumberId { get; set; }

        [JsonProperty(PropertyName = "number")]
        public long Number { get; set; }

        internal TicketInvoiceNumberModel ToObject(InvoiceTicket model)
        {
            var number = new TicketInvoiceNumberModel()
            {
                Id = model.Id,
                InvoiceId = model.InvoiceId,
                Number = model.TicketAllocationNumber.Number,
                PricePerFraction = model.PricePerFraction,
                Quantity = model.Quantity,
                TicketAllocationNumberId = model.TicketNumberAllocationId
            };

            return number;
        }
    }
}
