namespace Tickets.Models.Ticket
{
    public class NumberTransactionModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public long Date { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public string ClientDesc { get; set; }
        public string UserDesc { get; set; }
        public string Group { get; set; }
    }
}