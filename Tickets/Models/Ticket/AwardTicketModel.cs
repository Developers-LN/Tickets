using System.Linq;
using Tickets.Models.Enums;

namespace Tickets.Models.Ticket
{
    public class AwardTicketModel
    {
        public int NumberId { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public int RaffleId { get; set; }
        public int Type { get; set; }
        public int ClientId { get; set; }

        internal object GetAwardsList(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(d => d.Id == raffleId);
            var raffleAwards = raffle.RaffleAwards.Where(w => w.Award.TypesAward.Id == (int)Tickets.Models.Enums.AwardTypeEnum.Mayors || w.Award.TypesAward.Id == (int)Tickets.Models.Enums.AwardTypeEnum.WinFraction);
            var identifyNumbers = context.IdentifyNumbers.Where(w => w.IdentifyBach.RaffleId == raffleId);

            var awards = (from r in identifyNumbers.AsEnumerable()
                          join a in raffleAwards.AsEnumerable() on r.TicketAllocationNumber.Number equals a.ControlNumber
                          select new
                          {
                              RaffleAwardId = a.Id,
                              certificationId = r.Status == (int)AwardCertificationStatuEnum.Certified ? context.AwardCertification.FirstOrDefault(w => w.IdentifyNumberId == r.Id).Id : 0,
                              INumberId = r.Id,
                              NumberId = r.TicketAllocationNumber.Id,
                              Number = r.TicketAllocationNumber.Number,
                              ClientDesc = r.IdentifyBach.Client.Name,
                              AwardName = a.Award.Name,
                              AwardId = a.AwardId,
                              Frac = a.Fraction,
                              Byfrac = a.Award.ByFraction,
                              ff = r.FractionFrom,
                              ft = r.FractionTo,
                              FractionFrom = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : r.FractionFrom,
                              FractionTo = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : r.FractionTo,
                              Fractions = a.Award.ByFraction == (int)ByFractionEnum.S ? 1 : r.FractionTo - r.FractionFrom + 1,
                              Production = r.IdentifyBach.Raffle.Prospect.Production,
                              RaffleId = r.IdentifyBach.Raffle.Id,
                              RaffleDesc = a.Raffle.Name,
                              IdentifyBachId = r.IdentifyBachId,
                              IdentifyDate = r.IdentifyBach.CreateDate.ToString(),
                              Status = r.Status,
                              StatusDesc = context.Catalogs.FirstOrDefault(u => u.Id == r.Status).NameDetail,
                          }).ToList();

            foreach (var number in identifyNumbers)
            {
                bool wfrac = true;
                var fFrom = number.FractionFrom;
                var fTo = number.FractionTo;

                foreach (var a in awards.Where(w => w.Byfrac == (int)Tickets.Models.Enums.ByFractionEnum.S))
                {
                    for (int i = fFrom; i <= fTo; i++)
                    {
                        if (i == a.Frac)
                        {
                            wfrac = true;
                            break;
                        }
                        else
                        {
                            wfrac = false;
                        }
                    }
                }
                if (wfrac == false)
                {
                    awards.RemoveAll(w => w.Number == number.TicketAllocationNumber.Number && w.Byfrac == (int)Tickets.Models.Enums.ByFractionEnum.S && w.ff == fFrom && w.ft == fTo);
                }
            }

            return new { awards };
        }
    }
}
