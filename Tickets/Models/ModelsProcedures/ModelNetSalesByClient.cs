namespace Tickets.Models.ModelsProcedures
{
    public class ModelNetSalesByClient
    {
        public bool Data { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ProspectId { get; set; }
        public int RaffleId { get; set; }
        public int TicketFractions { get; set; }
        public int TicketNumber { get; set; }
        public decimal FractionPrice { get; set; }
        public int IdAllocationNumber { get; set; }
        public int IdAllocation { get; set; }
        public int AvailableFractions { get; set; }
        public int ReturnFractions { get; set; }
        public decimal NetSale { get; set; }
    }
}
