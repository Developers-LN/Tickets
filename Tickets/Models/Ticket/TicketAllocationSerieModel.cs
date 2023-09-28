using Newtonsoft.Json;

namespace Tickets.Models.Ticket
{
    public class TicketAllocationSerieModel
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "serie")]
        public string Serie { get; set; }
    }
}
