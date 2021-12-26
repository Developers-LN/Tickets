namespace Tickets.Models.ModelsProcedures
{
    public class ReturnedByGroupModel
    {
        public bool Datos { get; set; }
        public int RaffleId { get; set; }
        public string Grupo { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int TotalRegistros { get; set; }
        public decimal Hojas { get; set; }
        public int Fracciones { get; set; }
        public decimal PrecioFraccion { get; set; }
        public decimal Total { get; set; }
    }
}