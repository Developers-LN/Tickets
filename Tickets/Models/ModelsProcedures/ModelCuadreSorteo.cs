namespace Tickets.Models.ModelsProcedures
{
    public class ModelCuadreSorteo
    {
        public bool Data { get; set; }
        public int RaffleId { get; set; }
        public string Premio { get; set; }
        public int CantidadPremios { get; set; }
        public int PremioOrden { get; set; }
        public int ProspectoFracciones { get; set; }
        public decimal MontoEnPremios { get; set; }
        public decimal MontoPremioProspecto { get; set; }
        public int CasaFracciones { get; set; }
        public decimal CasaMonto { get; set; }
        public int CalleFracciones { get; set; }
        public decimal CalleMonto { get; set; }
        public int NoImpresoFracciones { get; set; }
        public decimal NoImpresoMonto { get; set; }
        public int FraccionesPagadas { get; set; }
        public decimal MontoFraccionesPagadas { get; set; }
        public int FraccionesNoPagadas { get; set; }
        public decimal MontoFraccionesNoPagadas { get; set; }
    }
}