using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketReprintNumberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "number")]
        public long Number { get; set; }

        [JsonProperty(PropertyName = "ticketAllocationNumberId")]
        public int TicketAllocationNumberId { get; set; }

        [JsonProperty(PropertyName = "ticketReprintId")]
        public int TicketReprintId { get; set; }

        internal TicketReprintNumberModel ToObject(TicketRePrintNumber model)
        {
            var context = new TicketsEntities();
            var number = new TicketReprintNumberModel()
            {
                Id = model.Id,
                TicketAllocationNumberId = model.TicketAllocationNumberId,
                Number = context.TicketAllocationNumbers.FirstOrDefault( n=> n.Id == model.TicketAllocationNumberId).Number,
                TicketReprintId = model.TicketRePrintId
            };

            return number;
        }

    }
}
