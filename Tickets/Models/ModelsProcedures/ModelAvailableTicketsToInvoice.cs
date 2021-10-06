namespace Tickets.Models.ModelsProcedures
{
    public class ModelAvailableTicketsToInvoice
    {
        public bool Data { get; set; }
        public int AllocationNumberId { get; set; }
        public int RaffleId { get; set; }
        public int AvailableFractions { get; set; }
        public int Number { get; set; }
    }
}