using System;

namespace Tickets.Models.ModelsProcedures
{
    public class AllocationSummary
    {
        public bool Data { get; set; }
        public int AsignacionId { get; set; }
        public int ClientId { get; set; }
        public int RaffleId { get; set; }
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; }
        public int Fracciones { get; set; }
        public int Hojas { get; set; }
        public int Billetes { get; set; }
        public decimal Monto { get; set; }
        public decimal Descuento { get; set; }
        public decimal MontoAPagar { get; set; }
    }
}
