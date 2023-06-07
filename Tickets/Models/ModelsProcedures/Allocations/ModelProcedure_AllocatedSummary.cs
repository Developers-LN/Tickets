namespace Tickets.Models.ModelsProcedures.Allocations
{
    public class ModelProcedure_AllocatedSummary
    {
        public bool Data { get; set; }
        public int RaffleId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientIdName { get; set; }
        public decimal TotalAllocated { get; set; }
        public decimal TotaConsignated { get; set; }
        public decimal TotalSold { get; set; }
        public decimal TotalReturned { get; set; }
        public decimal TotalCanceled { get; set; }
    }
}