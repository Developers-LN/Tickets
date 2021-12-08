namespace Tickets.Models.ModelsProcedures
{
    public class ModelReturnedNumbersByClient
    {
        public bool datos { get; set; }
        public int RaffleId { get; set; }
        public int Billetes { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int FraccionesRestantes { get; set; }
        public int Fracciones { get; set; }
        public decimal PrecioFraccion { get; set; }
        public decimal PorcientoDescuento { get; set; }
        public decimal Factura { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }
}
