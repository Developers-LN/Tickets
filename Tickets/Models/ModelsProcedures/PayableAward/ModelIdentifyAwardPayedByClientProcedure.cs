namespace Tickets.Models.ModelsProcedures.PayableAward
{
    public class ModelIdentifyAwardPayedByClientProcedure
    {
        public bool Data { get; set; }
        public int RaffleId { get; set; }
        public string RaffleName { get; set; }
        public string Id_Name_Raffle { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Id_Name_Client { get; set; }
        public int Fracciones { get; set; }
        public decimal Monto { get; set; }
    }
}
