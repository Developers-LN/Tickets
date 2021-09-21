namespace Tickets.Models.ModelsProcedures
{
    public class ModelPayableAwards
    {
        public bool premios { get; set; }
        public int ClientId { get; set; }
        public int tanId { get; set; }
        public int number { get; set; }
        public int raffle { get; set; }
        public int quantity { get; set; }
        public string nameaward { get; set; }
        public int terminal { get; set; }
        public decimal value { get; set; }
        public int fracciones { get; set; }
        public decimal valorpagar { get; set; }
    }
}