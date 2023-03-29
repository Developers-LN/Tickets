namespace Tickets.Models.ModelsProcedures.Returns
{
    public class ReturnsSummary
    {
        public bool Data { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int RaffleId { get; set; }
        public int Assigned { get; set; }
        public int Printed { get; set; }
        public int Consignate { get; set; }
        public int TotalFractionReturned { get; set; }
        public int TicketReturned { get; set; }
        public int FractionReturned { get; set; }
        public int TotalTicketSold { get; set; }
        public int TicketSold { get; set; }
        public int FractionSold { get; set; }
    }
}
