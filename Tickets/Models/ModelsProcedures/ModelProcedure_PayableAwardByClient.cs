namespace Tickets.Models.ModelsProcedures
{
    public class ModelProcedure_PayableAwardByClient
    {
        public bool Data { get; set; }
        public int TanId { get; set; }
        public int TaId { get; set; }
        public string Id_Name { get; set; }
        public string ControlNumber { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public int Number { get; set; }
        public string NameAward { get; set; }
        public decimal Value { get; set; }
        public int Fracciones { get; set; }
        public int RaffleAwardId { get; set; }
        public decimal ValorPagar { get; set; }
    }
}
