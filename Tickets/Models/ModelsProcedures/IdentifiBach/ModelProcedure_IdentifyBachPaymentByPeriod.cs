namespace Tickets.Models.ModelsProcedures.IdentifiBach
{
    public class ModelProcedure_IdentifyBachPaymentByPeriod
    {
        public bool Data { get; set; }
        
        public string DocumentNumber { get; set; }

        public string DocumentType { get; set; }

        public string Winner { get; set; }
        
        public string Genre { get; set; }

        public int GenreId { get; set; }

        public int RaffleId { get; set; }
        
        public int BachId { get; set; }

        public string TicketNumber { get; set; }

        public string AwardName { get; set; }

        public decimal AwardByFraction { get; set; }

        public int FractionFrom { get; set; }

        public int FractionTo { get; set; }

        public string PaymentDate { get; set; }

        public string PaymentType { get; set; }

        public int Day { get; set; }
        
        public int Month { get; set; }
        
        public int Year { get; set; }
        
        public decimal TotalPayed { get; set; }
        
        public int PayedFractions { get; set; }
    }
}
