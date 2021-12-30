namespace Tickets.Models.ModelsProcedures
{
    public class ModelCuadreSorteoResumido
    {
        public bool Data { get; set; }
        public int RaffleId { get; set; }
        public string Premio { get; set; }
        public int ProspectoFracciones { get; set; }
        public decimal ProspectoPremio { get; set; }
        public int CasaFracciones { get; set; }
        public decimal CasaMonto { get; set; }
        public int CalleFracciones { get; set; }
        public decimal CalleMonto { get; set; }
        public int NoImpresoFracciones { get; set; }
        public decimal NoImpresoMonto { get; set; }
    }
}