using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;

namespace Tickets.Models.AuxModels
{
    public class AuxIdentifyBach
    {
        public int Id { get; set; }

        public int RaffleId { get; set; }

        public int ClientId { get; set; }

        public int DocumentType { get; set; }

        public string DocumentNumber { get; set; }

        public string WinnerName { get; set; }

        public string WinnerPhone { get; set; }

        public string Notes { get; set; }

        public int Type { get; set; }

        public int GenderId { get; set; }

        public int SequenceNumber { get; set; }

        public Nullable<int> WinnerId { get; set; }

        public string StatusDesc { get; set; }

        public int Status { get; set; }

        public decimal Percent { get; set; }

        public string ClientDesc { get; set; }

        public int ProductionLength { get; set; }

        public string CreateUser { get; set; }

        public int CreateUserId { get; set; }

        public DateTime CreateDate { get; set; }

        public string RaffleDesc { get; set; }

        public virtual ICollection<IdentifyNumber> IdentifyNumbers { get; set; }

        public virtual ICollection<object> IdentifyNumbersObject { get; set; }

        internal AuxIdentifyBach ToObject(IdentifyBach identifyBach)
        {
            var context = new TicketsEntities();
            var identifybachtModel = new AuxIdentifyBach()
            {
                Id = identifyBach.Id,
                SequenceNumber = identifyBach.SequenceNumber.HasValue ? identifyBach.SequenceNumber.Value : 0,
                ClientId = identifyBach.ClientId,
                Percent = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista || identifyBach.Client.GroupId == (int)ClientGroupEnum.DistribuidorElectronico ? 2 : 0,
                ClientDesc = identifyBach.Client.Name,
                DocumentNumber = identifyBach.Winner.DocumentNumber,
                WinnerName = identifyBach.Winner.WinnerName,
                WinnerPhone = identifyBach.Winner.Phone,
                WinnerId = identifyBach.WinnerId,
                RaffleId = identifyBach.RaffleId,
                RaffleDesc = identifyBach.Raffle.Symbol + identifyBach.Raffle.Separator + identifyBach.Raffle.SequenceNumber + " " + identifyBach.Raffle.Name + " " + identifyBach.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                CreateDate = identifyBach.CreateDate,
                CreateUser = identifyBach.User.Name,
                CreateUserId = identifyBach.CreateUser,
                Status = identifyBach.Statu,
                StatusDesc = context.Catalogs.FirstOrDefault(c => c.Id == identifyBach.Statu).NameDetail,
                ProductionLength = identifyBach.Raffle.Prospect.Production,
                Notes = identifyBach.Notas,
                IdentifyNumbersObject = context.IdentifyNumbers.AsEnumerable().Where(n => n.IdentifyBachId == identifyBach.Id)
                .Select(n => IdentifyNumberToObejct(n)).ToList()
            };
            return identifybachtModel;
        }

        private object IdentifyNumberToObejct(IdentifyNumber n)
        {
            var context = new TicketsEntities();
            var identifyObject = new
            {
                n.Id,
                n.NumberId,
                NumberDesc = n.TicketAllocationNumber.Number,
                n.IdentifyBachId,
                n.FractionFrom,
                n.FractionTo,
                n.IdentifyBachNumberType,
                Fractions = (n.FractionTo - n.FractionFrom) + 1,
                n.Status,
                AwardName = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Name).FirstOrDefault(),

                Value = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo)
                || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)).FirstOrDefault(),

                Total = context.RaffleAwards.Where(ra =>
                ra.RaffleId == n.IdentifyBach.RaffleId && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived
                && ra.ControlNumber == n.TicketAllocationNumber.Number
                && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)).Select(s => (s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)) * ((n.FractionTo - n.FractionFrom) + 1)).FirstOrDefault(),

                RaffleAwards = context.RaffleAwards.Where(ra =>
                    ra.RaffleId == n.IdentifyBach.RaffleId
                    && ra.ControlNumber == n.TicketAllocationNumber.Number
                    && ((ra.Fraction >= n.FractionFrom && ra.Fraction <= n.FractionTo) || ra.Award.ByFraction == (int)ByFractionEnum.N)
                    && ra.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived).Select(
                ra => new
                {
                    AwardName = ra.Award.Name,
                    AwardValue = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value : ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber),
                    FractionFrom = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionFrom,
                    FractionTo = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionTo,
                    ra.Id,
                    LawDiscount = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor : 0,
                    Total = ra.Award.ByFraction == (int)ByFractionEnum.S || ra.Award.TypesAwardId == (int)AwardTypeEnum.Mayors ? (ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber) * (n.FractionTo - n.FractionFrom + 1)) * (context.SystemConfigs.FirstOrDefault().LawDiscountPercentMayor / 100)) : (ra.Award.Value / (ra.Raffle.Prospect.LeafFraction * ra.Raffle.Prospect.LeafNumber)),
                }).ToList()
            };
            return identifyObject;
        }
    }
}
