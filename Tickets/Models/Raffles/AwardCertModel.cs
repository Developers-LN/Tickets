using Newtonsoft.Json;
using Tickets.Models.Prospects;

namespace Tickets.Models.Raffles
{
    public class AwardCertModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "awardId")]
        public int AwardId { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleNomenclature")]
        public string RaffleNomenclature { get; set; }

        [JsonProperty(PropertyName = "controlNumber")]
        public long ControlNumber { get; set; }

        [JsonProperty(PropertyName = "fraction")]
        public int Fraction { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public System.DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "raffleAwardType")]
        public int RaffleAwardType { get; set; }

        [JsonProperty(PropertyName = "award")]
        public AwardModel Award { get; set; }

        [JsonProperty(PropertyName = "sequenceNumberRaffle")]
        public int? SequenceNumberRaffle { get; set; }
    }
}