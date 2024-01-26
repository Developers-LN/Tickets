namespace Tickets.Models.ModelsProcedures.Raffle
{
    public class ModelProcedure_RaffleSales
    {
        public int RaffleId { get; set; }

        public string RaffleName { get; set; }

        public string RaffleDate { get; set; }

        public decimal GrossSales { get; set; }

        public decimal NetSales { get; set; }

        public int Order {  get; set; }

        public string Award { get; set; }
    }
}
