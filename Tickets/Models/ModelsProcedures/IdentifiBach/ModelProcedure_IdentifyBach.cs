using System.Collections.Generic;

namespace Tickets.Models.ModelsProcedures.IdentifiBach
{
    public class ModelProcedure_IdentifyBach
    {
        public bool Data { get; set; }
        public int IdLote { get; set; }
        public int CodigoEstadoLote { get; set; }
        public string Cedula { get; set; }
        public string PagadoA { get; set; }
        public string Telefono { get; set; }
        public decimal PorcentajeComision { get; set; }
        public decimal ComisionMayorista { get; set; }
        public string Nota { get; set; }
        public string NombreSorteo { get; set; }
        public string PropietarioLote { get; set; }
        public int ClientId { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoRestante { get; set; }
        public string EstadoLote { get; set; }
        public decimal MontoPagar { get; set; }
        public decimal MontoEnPremios { get; set; }
        public int PagoNotaCredito { get; set; }
        public int PagoEfectivo { get; set; }

        public virtual ICollection<ModelProcedure_IdentifyBachNumbers> IdentifyBachNumbers { get; set; }
    }
}
