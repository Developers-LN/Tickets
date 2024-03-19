using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tickets.Models.Enums;
using Tickets.Models.ModelsProcedures.RaffleAward;
using Tickets.Models.Procedures.RaffleAward;
using WebMatrix.WebData;

namespace Tickets.Models.Raffles
{
    public class VirtualRaffleModel
    {
        internal RequestResponseModel RaffleGenerate(List<RaffleAwardModel> raffleAwards)
        {
            var result = new GenericErrorResponse()
            {
                Result = false,
                Message = "error en la generación!"
            };

            int raffleId = 0;
            using (var context = new TicketsEntities())
            {
                using (var transManager = context.Database.BeginTransaction())
                {
                    try
                    {
                        /*SAVING DIGITED NUMBER*/
                        var raffleDigitedAwardList = new List<RaffleAward>();
                        var AvailableTickets = new List<ModelProcedure_AvailableTicketsAfterRaffle>();
                        HashSet<int> AvailableTicketsNumbers = new HashSet<int>();
                        foreach (var raffleAwardModel in raffleAwards)
                        {
                            raffleAwardModel.RaffleAwardType = (int)RaffleAwardTypeEnum.Digite;
                            raffleId = raffleAwardModel.RaffleId;
                            var raffleAward = new RaffleAward()
                            {
                                AwardId = raffleAwardModel.AwardId,
                                ControlNumber = raffleAwardModel.ControlNumber,
                                Fraction = raffleAwardModel.Fraction,
                                RaffleId = raffleAwardModel.RaffleId,
                                RaffleAwardType = raffleAwardModel.RaffleAwardType,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId
                            };

                            raffleDigitedAwardList.Add(raffleAward);
                        }
                        context.RaffleAwards.AddRange(raffleDigitedAwardList);
                        context.SaveChanges();
                        /*SAVING DIGITED NUMBER*/

                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                        var prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId);

                        if (prospect.Awards.Any(a => a.TypesAward.Creation == (int)TypesAwardCreationEnum.SalesAward))
                        {
                            Procedure_AvailableTicketAfterRaffle procedure_AvailableTicketAfterRaffle = new Procedure_AvailableTicketAfterRaffle();
                            AvailableTickets = procedure_AvailableTicketAfterRaffle.AvailableTicketsAfterRaffle(raffleId);

                            AvailableTickets.ForEach(f => AvailableTicketsNumbers.Add(f.TicketNumber));
                        }

                        var awardList = prospect.Awards.Where(a => a.TypesAward.Creation != (int)TypesAwardCreationEnum.Digited && a.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived).OrderBy(a => a.OrderAward);

                        Award currentAward;
                        List<RaffleAward> raffleAwardList = new List<RaffleAward>();
                        HashSet<int> controlNumberList = new HashSet<int>();

                        raffleAwards.ForEach(f => controlNumberList.Add(f.ControlNumber));

                        RaffleAward lastRaffleAward = null;
                        do
                        {
                            currentAward = awardList.FirstOrDefault(a => (raffle.RaffleAwards.Where(ra => ra.AwardId == a.Id).Count() + raffleAwardList.Where(ra => ra.AwardId == a.Id).Count()) < a.Quantity);

                            if (currentAward != null)
                            {
                                List<int> numbers = new List<int>();
                                Award sourceAward = null;
                                if (currentAward.SourceAward.HasValue)
                                {
                                    sourceAward = context.Awards.FirstOrDefault(a => a.Id == currentAward.SourceAward.Value);
                                }

                                switch (currentAward.TypesAward.Creation)
                                {
                                    case (int)TypesAwardCreationEnum.Aproximeted:
                                        numbers = GetAproximatedNumber(currentAward, sourceAward, raffle);
                                        break;
                                    case (int)TypesAwardCreationEnum.Generated:
                                        if (sourceAward != null)
                                        {
                                            int lastNumber = currentAward.Terminal.Value;
                                            int selectedNumber = (int)sourceAward.RaffleAwards.FirstOrDefault().ControlNumber;
                                            for (var n = 0; n < currentAward.Quantity; n++)
                                            {
                                                var selectedNumberString = Utils.AddZeroToNumber((prospect.Production - 1).ToString().Length, (int)selectedNumber);
                                                //var randon = new Random();
                                                string currentLastNumber = selectedNumberString.Substring(selectedNumberString.Length - lastNumber, lastNumber);
                                                int number = int.Parse(n + currentLastNumber);
                                                if (number == selectedNumber)
                                                {
                                                    int ln = int.Parse(currentAward.Quantity + currentLastNumber);
                                                    numbers.Add(ln);
                                                }
                                                else
                                                {
                                                    numbers.Add(number);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            numbers = GetGenerateNumber(currentAward, sourceAward, prospect.Production, controlNumberList);
                                        }
                                        break;
                                    case (int)TypesAwardCreationEnum.SalesAward:
                                        numbers = GetGenerateNumberFromSales(currentAward, prospect.Production, controlNumberList, AvailableTicketsNumbers);
                                        break;
                                    case (int)TypesAwardCreationEnum.Hundred:
                                        numbers = GetGenerateHundred(currentAward, sourceAward, prospect.Production);
                                        break;
                                    case (int)TypesAwardCreationEnum.LastBall:
                                        numbers = GetGenerateLastBall(sourceAward, raffleAwardList);
                                        break;
                                    default:
                                        break;
                                }

                                foreach (var number in numbers)
                                {
                                    controlNumberList.Add(number);

                                    var raffleAward = new RaffleAward()
                                    {
                                        AwardId = currentAward.Id,
                                        ControlNumber = number,
                                        CreateDate = DateTime.Now,
                                        CreateUser = WebSecurity.CurrentUserId,
                                        Fraction = 0,
                                        RaffleId = raffle.Id//,
                                        //RaffleAwardType = currentAward.TypesAwardId
                                    };
                                    raffleAwardList.Add(raffleAward);
                                    lastRaffleAward = raffleAward;
                                }
                            }
                        } while (currentAward != null);

                        var awardListDerived = prospect.Awards.Where(a => a.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived).OrderBy(a => a.OrderAward);

                        lastRaffleAward = null;
                        do
                        {
                            currentAward = awardListDerived.FirstOrDefault(a =>
                            (raffle.RaffleAwards.Where(ra => ra.AwardId == a.Id).Count()
                            + raffleAwardList.Where(ra => ra.AwardId == a.Id).Count()) < a.Quantity);

                            if (currentAward != null)
                            {
                                List<int> numbers = new List<int>();
                                Award sourceAward = null;
                                if (currentAward.SourceAward.HasValue)
                                {
                                    sourceAward = context.Awards.FirstOrDefault(a => a.Id == currentAward.SourceAward.Value);
                                }

                                switch (currentAward.TypesAward.Creation)
                                {
                                    case (int)TypesAwardCreationEnum.SameAwardDerived:
                                        if (!raffleAwardList.Any(a => a.AwardId == currentAward.SourceAward))
                                        {
                                            int selectedNumberD = selectedNumberD = (int)sourceAward.RaffleAwards.FirstOrDefault().ControlNumber;
                                            numbers.Add(selectedNumberD);
                                        }
                                        else
                                        {
                                            var ListOfSources = raffleAwardList.Where(w => w.AwardId == currentAward.SourceAward).ToArray();
                                            for (var n = 0; n < currentAward.Quantity; n++)
                                            {
                                                numbers.Add((int)ListOfSources[n].ControlNumber);
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }

                                foreach (var number in numbers)
                                {
                                    controlNumberList.Add(number);

                                    var raffleAward = new RaffleAward()
                                    {
                                        AwardId = currentAward.Id,
                                        ControlNumber = number,
                                        CreateDate = DateTime.Now,
                                        CreateUser = WebSecurity.CurrentUserId,
                                        Fraction = 0,
                                        RaffleId = raffle.Id
                                    };
                                    raffleAwardList.Add(raffleAward);
                                    lastRaffleAward = raffleAward;
                                }
                            }
                        }
                        while (currentAward != null);

                        context.RaffleAwards.AddRange(raffleAwardList);
                        context.SaveChanges();

                        /*Changing Virtual Raffle*/
                        raffle.Statu = (int)RaffleStatusEnum.Generated;
                        context.SaveChanges();
                        /*Changing Virtual Raffle*/

                        transManager.Commit();
                        //GUARDADO DE DATOS GENERADOS
                        //Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Generacion de Sorteo", raffleAwardList);
                        return new RequestResponseModel()
                        {
                            Result = true,
                            Message = "Sorteo virtual generado correctamente!"
                        };
                    }
                    catch (Exception e)
                    {
                        transManager.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message
                        };
                    }
                }
            }
        }

        internal RequestResponseModel VirtualRaffleDetails(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);

            if (raffle == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "No existe el sorteo requerido."
                };
            }

            var raffleModel = new RaffleModel();
            var raffleDetails = raffleModel.ToObject(raffle, true);
            var prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId);
            var digitedAwards = prospect.Awards.Where(r => r.TypesAward.Creation == (int)TypesAwardCreationEnum.Digited).OrderBy(r => r.SourceAward).Select(r => new
            {
                r.Id,
                AwardName = r.Name,
                HasAward = r.RaffleAwards.Any(ra => ra.RaffleId == raffle.Id),
                ControlNumber = r.RaffleAwards.Any(ra => ra.RaffleId == raffle.Id) ? r.RaffleAwards.Where(ra => ra.RaffleId == raffle.Id).FirstOrDefault().ControlNumber : 0,
                Fraction = r.RaffleAwards.Any(ra => ra.RaffleId == raffle.Id) ? r.RaffleAwards.Where(ra => ra.RaffleId == raffle.Id).FirstOrDefault().Fraction : 0,
                HasRaffleAward = r.RaffleAwards.Any(ra => ra.RaffleId == raffle.Id),
                RaffleId = raffle.Id,
                ChildAwardId = r.Award1.Select(s => s.Id).FirstOrDefault(),
                r.SourceAward,
                ByFraction = r.ByFraction == (int)ByFractionEnum.S,
                TotalValue = r.TotalValue.ToString("C", CultureInfo.CreateSpecificCulture("es-DO"))
            });

            var restantAwardCount = prospect.Awards
                .GroupBy(a => a.TypesAwardId).Select(a => new
                {
                    typeAwardId = a.FirstOrDefault().TypesAwardId,
                    quantity = a.Select(aq => aq.Quantity).Aggregate((e, i) => e + i) - raffle.RaffleAwards.Where(at => at.Award.TypesAwardId == a.FirstOrDefault().TypesAwardId).Count()
                });

            var raffleAwardListByAward = raffle.RaffleAwards.GroupBy(ag => ag.Award.TypesAwardId).Select(asd => new
            {
                typeAwardId = asd.FirstOrDefault().Award.TypesAwardId,
                awardList = asd.GroupBy(a => a.AwardId).Select(a => new
                {
                    awardName = a.FirstOrDefault().Award.Name,
                    awardList = a.Select(r => new
                    {
                        number = Utils.AddZeroToNumber((prospect.Production - 1).ToString().Length, (int)r.ControlNumber)
                    })
                })
            });

            var awardTypes = prospect.Awards.Where(a =>
                a.TypesAwardId == (int)AwardTypeEnum.Specials
                || a.TypesAwardId == (int)AwardTypeEnum.Intermediates
                || a.TypesAwardId == (int)AwardTypeEnum.Minors
                || a.TypesAwardId == (int)AwardTypeEnum.LastBall
                ).GroupBy(a => a.TypesAwardId)
                .Select(at => at.Select(a => new
                {
                    id = a.TypesAwardId,
                    name = a.TypesAward.Name
                }).FirstOrDefault());

            return new RequestResponseModel()
            {
                Result = true,
                Object = new
                {
                    raffleDetails,
                    digitedAwards,
                    awardTypes,
                    maxFraction = prospect.LeafNumber * prospect.LeafFraction,
                    restantAwardCount,
                    raffleAwardListByAward
                }
            };
        }

        #region Private Method
        private List<int> GetGenerateLastBall(Award sourceAward, List<RaffleAward> raffleAwardList)
        {
            var number = (int)raffleAwardList.Where(r => r.AwardId == sourceAward.Id).ToList().LastOrDefault().ControlNumber;
            var numbers = new List<int>();
            numbers.Add(number);
            return numbers;
        }

        private List<int> GetGenerateHundred(Award currentAward, Award sourceAward, int production)
        {
            var numbers = new List<int>();
            var number = Utils.AddZeroToNumber((production - 1).ToString().Length, (int)sourceAward.RaffleAwards.FirstOrDefault().ControlNumber);
            var numberRest = number.Substring(0, number.Length - 2);
            for (int i = 0; i < 100; i++)
            {
                string n = "";
                if (i.ToString().Length == 1)
                {
                    n = "0" + i.ToString();
                }
                else
                {
                    n = i.ToString();
                }
                var currentNumb = int.Parse(numberRest + n);
                string StringCurrectNum = currentNumb.ToString();
                string NewCurrectNum = StringCurrectNum.PadLeft(production.ToString().Length - 1, '0');
                if (number != NewCurrectNum)
                {
                    numbers.Add(currentNumb);
                }
            }
            return numbers;
        }

        private List<int> GetAproximatedNumber(Award currentAward, Award sourceAward, Raffle raffle)
        {
            var award = currentAward.Award2.RaffleAwards.FirstOrDefault();
            var numbers = GetAproximateNumbers((int)award.ControlNumber, raffle.Prospect.Production, award.Award.Quantity);

            return numbers;
        }

        private List<int> GetGenerateNumber(Award currentAward, Award sourceAward, int production, HashSet<int> controlNumberList)
        {
            int number = 0;
            if (sourceAward != null)
            {
                int terminal = currentAward.Terminal.Value;
                int selectedNumber = (int)sourceAward.RaffleAwards.FirstOrDefault().ControlNumber;
                number = GetRandomNumberToLastDigit(selectedNumber, controlNumberList, production, terminal);
            }
            else
            {
                number = GenerateRandonNumber(controlNumberList, production);
            }
            var numbers = new List<int>();
            numbers.Add(number);
            return numbers;
        }

        private List<int> GetGenerateNumberFromSales(Award currentAward, int production, HashSet<int> controlNumberList, HashSet<int> AvailableTicketsNumbers)
        {
            int number = 0;
            number = GenerateRandonNumberAvailable(currentAward, controlNumberList, production, AvailableTicketsNumbers);
            var numbers = new List<int>();
            numbers.Add(number);
            return numbers;
        }

        /*Generation Award Type*/
        private List<int> GetAproximateNumbers(int selectedNumber, int production, int count)
        {
            var numbers = new List<int>();
            for (var i = 1; i <= count; i++)
            {
                if (selectedNumber >= production)
                {
                    numbers.Add(0);
                    numbers.Add(selectedNumber - i);
                }
                else if (selectedNumber <= 0)
                {
                    numbers.Add(selectedNumber + i);
                    numbers.Add(production);
                }
                else
                {
                    numbers.Add(selectedNumber + i);
                    numbers.Add(selectedNumber - i);
                }
            }
            return numbers;
        }

        private int GetRandomNumberToLastDigit(int selectedNumber, HashSet<int> controlNumberList, int production, int lastNumber)
        {
            var selectedNumberString = Utils.AddZeroToNumber((production - 1).ToString().Length, selectedNumber);
            var randon = new Random();
            string currentLastNumber = selectedNumberString.Substring(selectedNumberString.Length - lastNumber, lastNumber);
            int number = 0;
            if (controlNumberList.Count > 0)
            {
                var lastNumberSelect = controlNumberList.ToList().Last().ToString();
                if (lastNumberSelect.Length == lastNumber)
                {
                    lastNumberSelect = "0" + lastNumberSelect;
                }
                var lastN = lastNumberSelect.Substring(0, lastNumberSelect.Length - lastNumber);

                int n = int.Parse(lastN) + 1;
                number = int.Parse(n + currentLastNumber);
            }
            else
            {
                number = int.Parse("0" + currentLastNumber);
            }

            return number;
        }

        private bool ValidateLastDigitNumber(int i, string currentLastNumber, int production)
        {
            var numberString = Utils.AddZeroToNumber((production - 1).ToString().Length, i);
            var restantNumber = numberString.Substring(numberString.Length - currentLastNumber.Length, currentLastNumber.Length);
            return restantNumber == currentLastNumber;
        }

        private int GenerateRandonNumber(HashSet<int> controlNumberList, int production)
        {
            var randon = new Random();
            int number = randon.Next(0, production);

            while (controlNumberList.Where(controlNumber => controlNumber == number).Any())
            {
                number = randon.Next(0, production);
            }
            return number;
        }

        private int GenerateRandonNumberAvailable(Award currentAward, HashSet<int> controlNumberList, int production, HashSet<int> AvailableTicketsNumbers)
        {
            var randon = new Random();
            int number = 0;

            if (AvailableTicketsNumbers.Count > 0 && currentAward.Quantity <= AvailableTicketsNumbers.Count && (AvailableTicketsNumbers.Count - controlNumberList.Count) >= currentAward.Quantity)
            {
                int randomIndex = 0;
                var MaxLenght = AvailableTicketsNumbers.Count - 1;
                randomIndex = randon.Next(0, MaxLenght);
                number = AvailableTicketsNumbers.ToArray()[(int)randomIndex];

                while (controlNumberList.Where(controlNumber => controlNumber == number).Any())
                {
                    randomIndex = randon.Next(0, MaxLenght);
                    number = AvailableTicketsNumbers.ToArray()[(int)randomIndex];
                }
            }
            else
            {
                number = randon.Next(0, production);

                while (controlNumberList.Where(controlNumber => controlNumber == number).Any())
                {
                    number = randon.Next(0, production);
                }
            }
            return number;
        }

        /*Generation Award Type*/
        #endregion
    }
}
