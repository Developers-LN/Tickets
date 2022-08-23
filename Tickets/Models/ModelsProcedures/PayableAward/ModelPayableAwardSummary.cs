namespace Tickets.Models.ModelsProcedures.PayableAward
{
    public class ModelPayableAwardSummary
    {
        public bool Data { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public int CountAward { get; set; }
        public decimal TotalPayable { get; set; }
        public int CountPayed { get; set; }
        public decimal TotalPayed { get; set; }
        public int CountPending { get; set; }
        public decimal TotalPending { get; set; }
    }
}