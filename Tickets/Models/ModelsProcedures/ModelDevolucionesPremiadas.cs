namespace Tickets.Models.ModelsProcedures
{
    public class ModelDevolucionesPremiadas
    {
        public bool Data { get; set; }
        public int RaffleId { get; set; }
        public string ReturnedGroup { get; set; }
        public int ClientId { get; set; }
        public string Cliente { get; set; }
        public int Numero { get; set; }
        public string Premio { get; set; }
        public int CantidadFracciones { get; set; }
        public decimal ValorFraccion { get; set; }
        public decimal Monto { get; set; }
    }
}
