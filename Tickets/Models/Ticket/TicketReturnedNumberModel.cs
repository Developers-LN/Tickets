using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketReturnedNumberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "production")]
        public int Production { get; set; }

        [JsonProperty(PropertyName = "returnedGroup")]
        public string ReturnedGroup { get; set; }

        [JsonProperty(PropertyName = "returnedSubGroup")]
        public string ReturnedSubGroup { get; set; }

        [JsonProperty(PropertyName = "returnedDate")]
        public DateTime ReturnedDate { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "userDesc")]
        public string UserDesc { get; set; }

        [JsonProperty(PropertyName = "fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty(PropertyName = "fractionTo")]
        public int FractionTo { get; set; }

        [JsonProperty(PropertyName = "statuId")]
        public int Status { get; set; }

        [JsonProperty(PropertyName = "statusDesc")]
        public string StatusDesc { get; set; }

        [JsonProperty(PropertyName = "numberId")]
        public int NumberId { get; set; }

        [JsonProperty(PropertyName = "numberDesc")]
        public long NumberDesc { get; set; }

        internal TicketReturnedNumberModel ToObject(TicketReturn ticketReturned)
        {
            var context = new TicketsEntities();
            var model = new TicketReturnedNumberModel()
            {
                Id = ticketReturned.Id,
                RaffleId = ticketReturned.RaffleId,
                RaffleDesc = context.Raffles.FirstOrDefault(r => r.Id == ticketReturned.RaffleId).Name,
                Production = ticketReturned.Raffle.Prospect.Production,
                ReturnedGroup = /*int.Parse(Regex.Match(*/ticketReturned.ReturnedGroup,/*@"\d+").Value),*/
                ReturnedSubGroup = ticketReturned.ReturnedGroup,
                ReturnedDate = ticketReturned.ReturnedDate,
                ClientId = ticketReturned.ClientId,
                ClientDesc = context.Clients.Where(r => r.Id == ticketReturned.ClientId).Select(c => c.Name).FirstOrDefault(),
                CreateDate = ticketReturned.CreateDate,
                CreateUser = ticketReturned.CreateUser,
                UserDesc = context.Users.FirstOrDefault(u => u.Id == ticketReturned.CreateUser).Name,
                FractionTo = ticketReturned.FractionTo,
                FractionFrom = ticketReturned.FractionFrom,
                Status = ticketReturned.Statu,
                StatusDesc = context.Catalogs.FirstOrDefault(c => c.Id == ticketReturned.Statu).NameDetail,
                NumberId = ticketReturned.TicketAllocationNumber.Id,
                NumberDesc = ticketReturned.TicketAllocationNumber.Number
            };
            return model;
        }

        internal object GetReturnedDetail(int numberId)
        {
            var context = new TicketsEntities();
            /*var number = context.TicketAllocationNumbers.AsEnumerable()
                .Where(n => n.Id == numberId).Select(n => TicketReturnedToObject(n)).FirstOrDefault();
            return number;*/
            return new object();
        }

        internal object GetReturnedGroupList(int raffleId)
        {
            var context = new TicketsEntities();

            var groups = context.TicketReturns.AsEnumerable().Where(g => g.RaffleId == raffleId).GroupBy(g => g.ReturnedGroup).Select(g => new
            {
                text = g.FirstOrDefault().ReturnedGroup,
                value = g.FirstOrDefault().ReturnedGroup
            }).ToList();

            var clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                r.Id,
                r.Name
            }).ToList();

            return new { groups, clients };
        }

        internal object GetReturnedAwardList(int raffleId)
        {
            var context = new TicketsEntities();
            var returneds = new List<object>();

            var raffle = context.Raffles.FirstOrDefault(d => d.Id == raffleId);
            if (raffle != null)
            {
                returneds = (from r in raffle.TicketReturns
                             join a in raffle.RaffleAwards on r.TicketAllocationNumber.Number equals a.ControlNumber
                             select new
                             {
                                 RaffleAwardId = a.Id,
                                 certificationId = a.CertificationNumbers.Any() ? a.CertificationNumbers.FirstOrDefault().Id : 0,
                                 NumberId = r.TicketAllocationNimberId,
                                 Number = r.TicketAllocationNumber.Number,
                                 ClientDesc = r.Client.Name,
                                 AwardName = a.Award.Name,
                                 FractionFrom = r.FractionFrom,
                                 FractionTo = r.FractionTo,
                                 Production = r.Raffle.Prospect.Production,
                                 RaffleId = r.Raffle.Id,
                                 RaffleDesc = a.Raffle.Name,
                                 ReturnedDate = r.ReturnedDate.ToString()
                             }).GroupBy(r => r.Number)
                                .Select(n => new
                                {
                                    NumberId = n.FirstOrDefault().NumberId,
                                    RaffleAwardId = n.FirstOrDefault().RaffleAwardId,
                                    Number = n.FirstOrDefault().Number,
                                    ClientDesc = n.FirstOrDefault().ClientDesc,
                                    AwardName = n.FirstOrDefault().AwardName,
                                    Fractions = n.Select(f => f.FractionTo - f.FractionFrom + 1).Aggregate((k, l) => k + l),
                                    Production = n.FirstOrDefault().Production,
                                    RaffleId = n.FirstOrDefault().RaffleId,
                                    RaffleDesc = n.FirstOrDefault().RaffleDesc,
                                    ReturnedDate = n.FirstOrDefault().ReturnedDate,
                                    certificationId = n.FirstOrDefault().certificationId
                                }).ToList<object>();
            }
            return new { returneds };
        }

        internal object GetReturnedList(int raffleId, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = context.Raffles.Where(s =>
                ((s.Statu == (int)RaffleStatusEnum.Active
                || s.Statu == (int)RaffleStatusEnum.Planned)
                && s.EndReturnDate >= DateTime.Now)
                || s.ReturnedOpens.Any(r => r.EndReturnedDate >= DateTime.Now)
                ).Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList();

            var clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                r.Id,
                r.Name
            }).ToList();

            var returneds = context.TicketReturns.AsEnumerable().Where(d =>
                (d.RaffleId == raffleId || raffleId == 0)
                && (d.ClientId == clientId || clientId == 0)
                && d.Statu != (int)TicketReturnedStatuEnum.Invoiced
                && (
                ((d.Raffle.Statu == (int)RaffleStatusEnum.Active
                || d.Raffle.Statu == (int)RaffleStatusEnum.Planned)
                && d.Raffle.EndReturnDate >= DateTime.Now)
                || d.Raffle.ReturnedOpens.Any(r => r.EndReturnedDate >= DateTime.Now))
                ).GroupBy(d => d.ReturnedGroup).Select(d => new
                {
                    RaffleName = d.FirstOrDefault().Raffle.Id + " - " + d.FirstOrDefault().Raffle.Name,
                    ClientName = d.FirstOrDefault().ClientId + " - " + d.FirstOrDefault().Client.Name,
                    ClientId = d.FirstOrDefault().ClientId,
                    Group = d.FirstOrDefault().ReturnedGroup,
                    Quantity = d.Select(f => f.FractionTo - f.FractionFrom + 1).Aggregate((f, l) => f + l),
                    FractionPerSheet = d.FirstOrDefault().Raffle.Prospect.LeafFraction,
                    ReturnedDate = d.FirstOrDefault().ReturnedDate.ToUnixTime(),
                    RaffleId = d.FirstOrDefault().Raffle.Id,
                    isValidated = d.FirstOrDefault().Statu == (int)TicketReturnedStatuEnum.Invoiced
                });
            return new { returneds, clients, raffles };
        }

        internal object ReturnedDetailsGroup(string group, int raffleId)
        {
            var context = new TicketsEntities();
            var returns = context.TicketReturns.AsEnumerable().Where(tr =>
                tr.ReturnedGroup == group
                && tr.RaffleId == raffleId
                ).Select(tr => this.ToObject(tr));
            return returns;
        }

        internal object ReturnedAwardReportData(int raffleId = 0)
        {
            var context = new TicketsEntities();

            var raffles = context.Raffles.Select(r => new
            {
                value = r.Id,
                text = r.Name
            }).ToList();

            var groups = context.TicketReturns.Where(w => w.RaffleId == raffleId).AsEnumerable().Select(o => int.Parse(Regex.Match(o.ReturnedGroup, @"\d+").Value))
                .OrderBy(o => o).GroupBy(g => g).Select(g => new
                {
                    text = Utils.AddZeroToNumber(3, g.FirstOrDefault()),
                    value = g.FirstOrDefault(),

                }).ToList();

            return new
            {
                raffles,
                groups
            };
        }

        #region Private Method
        /*private List<NumberTransactionModel> GetReturnedTransactions(TicketAllocationNumber n)
        {
            List<NumberTransactionModel> transactions = new List<NumberTransactionModel>();
            using (var context = new TicketsEntities())
            {
                context.TicketReturns.Where(a => a.TicketAllocationNumber.Number == n.Number && a.RaffleId == n.TicketAllocation.RaffleId).ToList().ForEach(
                tn => transactions.Add(new NumberTransactionModel
                {
                    Description = "Devolución",
                    ClientDesc = tn.ClientId + " - " + tn.Client.Name,
                    Date = tn.CreateDate.ToUnixTime(),
                    FractionFrom = tn.FractionFrom,
                    FractionTo = tn.FractionTo,
                    Group = tn.ReturnedGroup,
                    UserDesc = context.Users.FirstOrDefault(u => u.Id == tn.CreateUser).Name,
                }));
            }

            return transactions;
        }*/

        #endregion

        internal RequestResponseModel Delete(TicketReturnedNumberModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tm = context.Database.BeginTransaction())
                {
                    try
                    {
                        var returned = context.TicketReturns.FirstOrDefault(n => n.Id == model.Id);
                        if (returned == null)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "No se encontro la devolucion"
                            };
                        }
                        context.TicketReturns.Remove(returned);
                        context.SaveChanges();

                        tm.Commit();
                    }
                    catch (Exception e)
                    {
                        tm.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "No es posible borrar esta debolución!",
                            Object = new
                            {
                                e.StackTrace
                            }
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Borando  devoluciones", model);
            return new RequestResponseModel()
            {
                Result = true,
                Message = "Devolucion borrado correctamente!"
            };
        }
    }
}