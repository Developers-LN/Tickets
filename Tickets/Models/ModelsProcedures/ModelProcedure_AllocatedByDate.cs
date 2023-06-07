namespace Tickets.Models.ModelsProcedures
{
    public class ModelProcedure_AllocatedByDate
    {
        public bool Data { get; set; }
        public string AllocateDate { get; set; }
        public int TotalTickets { get; set; }
        public decimal TicketPrice { get; set; }
        public decimal MountSold { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}