namespace Tickets.Models.Ticket
{
    public class TicketModel
    {
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public int ClientId { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public string Number { get; set; }
    }
}