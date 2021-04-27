using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tickets.Models.JSON
{

    public class TicketNumber
    {
        [JsonProperty("tiketNumber")]
        public string TiketNumber { get; set; }

        [JsonProperty("fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty("fractionTo")]
        public int FractionTo { get; set; }
    }

    public class TicketAllocationJSON
    {
        [JsonProperty("raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty("raffleDate")]
        public string RaffleDate { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ticketNumbers")]
        public List<TicketNumber> TicketNumbers { get; set; }
    }

    public class TicketNumberAward
    {
        [JsonProperty("tiketNumber")]
        public string TiketNumber { get; set; }

        [JsonProperty("fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty("fractionTo")]
        public int FractionTo { get; set; }

        [JsonProperty("awards")]
        public List<Award> Awards { get; set; }
    }

    public class Award
    {
        [JsonProperty("awardId")]
        public int AwardId { get; set; }

        [JsonProperty("awardName")]
        public string AwardName { get; set; }

        [JsonProperty("awardFractionPrice")]
        public decimal AwardFractionPrice { get; set; }

        [JsonProperty("awardPrice")]
        public decimal AwardPrice { get; set; }
    }

    public class AwardNumbesJSON
    {
        [JsonProperty("raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty("raffleDate")]
        public string RaffleDate { get; set; }

        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("ticketNumbers")]
        public List<TicketNumberAward> TicketNumbers { get; set; }
    }

    public class InvoiceJSON
    {
        [JsonProperty("raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [JsonProperty("raffleDate")]
        public string RaffleDate { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("ticketNumbers")]
        public List<TicketNumber> TicketNumbers { get; set; }
    }

    public class RequestResult
    {
        [JsonProperty("result")]
        public bool Result { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }

    public class IdentifyTicketJSON
    {
        [JsonProperty("raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty("clientId")]
        public int ClientId { get; set; }

        [JsonProperty("raffleDate")]
        public string RaffleDate { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("ticketNumbers")]
        public List<TicketNumber> TicketNumbers { get; set; }
    }
}