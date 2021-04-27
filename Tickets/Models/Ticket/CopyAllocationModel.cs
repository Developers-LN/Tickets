using Newtonsoft.Json;

namespace Tickets.Models.Ticket
{
    public class CopyAllocationModel
    {
        [JsonProperty(PropertyName = "sourceTicketRaffleId")]
        public int SourceTicketRaffleId { get; set; }

        [JsonProperty(PropertyName = "sourcePoolRaffleId")]
        public int SourcePoolRaffleId { get; set; }

        [JsonProperty(PropertyName = "targetRaffleId")]
        public int TargetRaffleId { get; set; }

    }
}