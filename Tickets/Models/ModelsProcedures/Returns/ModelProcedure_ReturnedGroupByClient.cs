namespace Tickets.Models.ModelsProcedures.Returns
{
    public class ModelProcedure_ReturnedGroupByClient
    {
        public bool Data { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Client_Id_Name { get; set; }
        public int RaffleId { get; set; }
        public string RaffleName { get; set; }
        public string Raffle_Id_Name { get; set; }
        public string TicketNumber { get; set; }
        public string Grupo { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public int CreateUser { get; set; }
        public string UserName { get; set; }
        public string EmployerName { get; set; }
        public int Production { get; set; }
        public int TicketFraction { get; set; }
        public decimal Discount { get; set; }
    }
}
