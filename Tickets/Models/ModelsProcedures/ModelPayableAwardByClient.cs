namespace Tickets.Models.ModelsProcedures
{
    public class ModelPayableAwardByClient
    {
        public bool Data { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public int number { get; set; }
        public string nameaward { get; set; }
        public decimal value { get; set; }
        public int fracciones { get; set; }
        public decimal valorpagar { get; set; }
    }
}