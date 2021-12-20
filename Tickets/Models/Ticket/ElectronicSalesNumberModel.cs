using Newtonsoft.Json;
using System;
using System.Linq;

namespace Tickets.Models.Ticket
{
    public class ElectronicSalesNumberModel
    {
        /*[JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "production")]
        public int Production { get; set; }

        [JsonProperty(PropertyName = "returnedGroup")]
        public string ReturnedGroup { get; set; }

        [JsonProperty(PropertyName = "returnedSubGroup")]
        public string ReturnedSubGroup { get; set; }

        [JsonProperty(PropertyName = "returnedDate")]
        public DateTime ReturnedDate { get; set; }*/

        /*[JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "userDesc")]
        public string UserDesc { get; set; }*/

        [JsonProperty(PropertyName = "fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty(PropertyName = "fractionTo")]
        public int FractionTo { get; set; }

        /*[JsonProperty(PropertyName = "statuId")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "statusDesc")]
        public string StatusDesc { get; set; }

        [JsonProperty(PropertyName = "numberId")]
        public int NumberId { get; set; }*/

        [JsonProperty(PropertyName = "numberDesc")]
        public long NumberDesc { get; set; }

        internal ElectronicSalesNumberModel ToObject(ElectronicTicketSale electronicTicket)
        {
            var context = new TicketsEntities();
            var model = new ElectronicSalesNumberModel()
            {
                //Id = electronicTicket.Id,
                //RaffleId = electronicTicket.RaffleId,
                //RaffleDesc = electronicTicket.Raffle.Name,
                //Production = electronicTicket.Raffle.Prospect.Production,
                //ClientId = electronicTicket.ClientId,
                //ClientDesc = electronicTicket.Client.Name,
                //CreateDate = electronicTicket.CreateDate,
                //CreateUser = electronicTicket.CreateUser,
                //UserDesc = electronicTicket.User.Name,
                FractionTo = electronicTicket.FractionTo,
                FractionFrom = electronicTicket.FractionFrom,
                //Status = electronicTicket.Statu,
                //StatusDesc = context.Catalogs.FirstOrDefault(c => c.Id == electronicTicket.Statu).NameDetail,
                //NumberId = electronicTicket.TicketAllocationNumber.Id,
                NumberDesc = electronicTicket.TicketAllocationNumber.Number
            };
            return model;
        }
    }
}
