namespace Tickets.Models.ModelsProcedures
{
    public class ModelProcedure_NetSalesByClient
    {
        public bool Data { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public int ProspectId { get; set; }
        public int ProspectFractions { get; set; }
        public string ClientName { get; set; }
        public string Id_Name { get; set; }
        public int TotalConsigned { get; set; }
        public int ConsignedTickets { get; set; }
        public int ConsignedFractions { get; set; }
        public int TotalReturned { get; set; }
        public int ReturnedTickets { get; set; }
        public int ReturnedFractions { get; set; }
        public int TotalAvailable { get; set; }
        public int AvailableTickets { get; set; }
        public int AvailableFractions { get; set; }
        public decimal Percentage { get; set; }
    }
}
