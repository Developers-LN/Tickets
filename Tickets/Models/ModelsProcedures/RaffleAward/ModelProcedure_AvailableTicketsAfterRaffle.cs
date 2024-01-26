namespace Tickets.Models.ModelsProcedures.RaffleAward
{
    public class ModelProcedure_AvailableTicketsAfterRaffle
    {
        public bool Data { get; set; }

        public int RaffleId { get; set; }

        public int TicketNumber { get; set; }

        public int AvailableFractions { get; set; }
    }
}
