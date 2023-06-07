namespace Tickets.Models.ModelsProcedures.IdentifiBachDetails
{
    public class ModelProcedure_IdentifyBachNumbers
    {
        public bool Data { get; set; }

        public int Id { get; set; }

        public string Numero { get; set; }

        public int FractionFrom { get; set; }

        public int FractionTo { get; set; }

        public string NombrePremio { get; set; }

        public int CantidadFraccionesPremiadas { get; set; }

        public decimal PremioPorFraccion { get; set; }

        public decimal TotalEnPremio { get; set; }

        public decimal Descuento { get; set; }

        public decimal MontoPagar { get; set; }
    }
}
