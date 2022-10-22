using Newtonsoft.Json;

namespace Tickets.Models.Ticket
{
    public class ElectronicSalesNumberModel
    {
        [JsonProperty(PropertyName = "dateSold")]
        public string DateSold { get; set; }

        [JsonProperty(PropertyName = "totalSold")]
        public int TotalSold { get; set; }

        /*internal ElectronicSalesNumberModel ToObject(ElectronicTicketSale electronicTicket)
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
                //FractionTo = electronicTicket.FractionTo,
                //FractionFrom = electronicTicket.FractionFrom,
                //Status = electronicTicket.Statu,
                //StatusDesc = context.Catalogs.FirstOrDefault(c => c.Id == electronicTicket.Statu).NameDetail,
                //NumberId = electronicTicket.TicketAllocationNumber.Id,
                //NumberDesc = electronicTicket.TicketAllocationNumber.Number
            };
            return model;
        }*/
    }
}
