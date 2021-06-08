using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;
using Tickets.Models.Prospects;
using WebMatrix.WebData;

namespace Tickets.Models.Raffles
{
    public class RaffleModel
    {

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "ticketProspectId")]
        public int TicketProspectId { get; set; }

        [JsonProperty(PropertyName = "ticketProspectDesc")]
        public string TicketProspectDesc { get; set; }

        [JsonProperty(PropertyName = "poolsProspectId")]
        public int PoolsProspectId { get; set; }

        [JsonProperty(PropertyName = "poolsProspectDesc")]
        public string PoolsProspectDesc { get; set; }

        [JsonProperty(PropertyName = "raffleDate")]
        public DateTime RaffleDate { get; set; }

        [JsonProperty(PropertyName = "raffleDateLong")]
        public long RaffleDateLong { get; set; }

        [JsonProperty(PropertyName = "commodityId")]
        public int CommodityId { get; set; }

        [JsonProperty(PropertyName = "commodityDesc")]
        public string CommodityDesc { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "startReturnDate")]
        public DateTime StartReturnDate { get; set; }

        [JsonProperty(PropertyName = "startReturnDateLong")]
        public long StartReturnDateLong { get; set; }

        [JsonProperty(PropertyName = "endReturnDate")]
        public DateTime EndReturnDate { get; set; }

        [JsonProperty(PropertyName = "endReturnDateLong")]
        public long EndReturnDateLong { get; set; }

        [JsonProperty(PropertyName = "endAllocationDate")]
        public DateTime EndAllocationDate { get; set; }

        [JsonProperty(PropertyName = "endAllocationDateLong")]
        public long EndAllocationDateLong { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        [JsonProperty(PropertyName = "ticketProspect")]
        public ProspectModel TicketProspect { get; set; }

        [JsonProperty(PropertyName = "poolProspect")]
        public ProspectModel PoolProspect { get; set; }

        [JsonProperty(PropertyName = "raffleAwards")]
        public List<RaffleAwardModel> RaffleAwards { get; set; }

        internal RaffleModel ListaSorteosGenerados(Raffle raffle, bool hasTicketProspect = false, bool hasPoolProspect = false, bool hasAwards = false)
        {
            var context = new TicketsEntities();
            var raffleModel = new RaffleModel()
            {
                Id = raffle.Id,
                EndAllocationDate = raffle.EndAllocationDate.Value,
                EndReturnDate = raffle.EndReturnDate,
                Name = raffle.Name,
                RaffleDate = raffle.DateSolteo,
                StartReturnDate = raffle.StartReturnDate,
                Statu = raffle.Statu,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == raffle.Statu).NameDetail,
                TicketProspectDesc = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId).Name
            };

            return raffleModel;
        }

        internal RaffleModel ToObject(Raffle raffle, bool hasTicketProspect = false, bool hasPoolProspect = false, bool hasAwards = false)
        {
            var context = new TicketsEntities();
            var raffleModel = new RaffleModel()
            {
                Id = raffle.Id,
                CommodityId = raffle.Commodity,
                CommodityDesc = raffle.Commodity > 0 ? context.Catalogs.FirstOrDefault(c => c.Id == raffle.Commodity).NameDetail : "",
                EndAllocationDate = raffle.EndAllocationDate.Value,
                EndAllocationDateLong = raffle.EndAllocationDate.Value.ToUnixTime(),
                EndReturnDate = raffle.EndReturnDate,
                EndReturnDateLong = raffle.EndReturnDate.ToUnixTime(),
                Name = raffle.Name,
                Note = raffle.Note,
                //PoolsProspectId = raffle.PoolsProspectId.HasValue? raffle.PoolsProspectId.Value : 0,
                //PoolsProspectDesc = raffle.PoolsProspectId.HasValue? context.Prospects.FirstOrDefault( p=> p.Id == raffle.PoolsProspectId.Value).Name : "",
                RaffleDate = raffle.DateSolteo,
                RaffleDateLong = raffle.DateSolteo.ToUnixTime(),
                StartReturnDate = raffle.StartReturnDate,
                StartReturnDateLong = raffle.StartReturnDate.ToUnixTime(),
                Statu = raffle.Statu,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == raffle.Statu).NameDetail,
                TicketProspectId = raffle.ProspectId,
                TicketProspectDesc = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId).Name
            };
            var prospectModel = new ProspectModel();
            if (hasTicketProspect == true)
            {
                raffleModel.TicketProspect = prospectModel.ToObject(context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId), true, true);
            }
            if (hasPoolProspect == true)
            {
                raffleModel.TicketProspect = prospectModel.ToObject(context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId), true, true);
            }
            if (hasAwards == true)
            {
                raffleModel.RaffleAwards = new List<RaffleAwardModel>();
            }

            return raffleModel;
        }

        internal RequestResponseModel GetRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.Where(r => r.Id == raffleId).AsEnumerable()
                .Select(r => this.ToObject(r)).FirstOrDefault();
            if (raffle == null)
            {
                raffle = new RaffleModel()
                {
                    RaffleDate = DateTime.Now
                };
            }
            return new RequestResponseModel()
            {
                Object = raffle,
                Result = true
            };
        }

        internal RequestResponseModel GetRaffleList(int statu)
        {
            var context = new TicketsEntities();
            var raffles = context.Raffles.AsEnumerable()
                .Where(r => r.Statu == statu || (statu == 0 && r.Statu != (int)RaffleStatusEnum.Suspended))
                .Select(r => this.ListaSorteosGenerados(r)).ToList();

            return new RequestResponseModel()
            {
                Object = raffles,
                Result = true
            };
        }

        internal RequestResponseModel GetRaffleListSP()
        {
            var context = new TicketsEntities();
            var raffles = context.Raffles.AsEnumerable()
                .Where(r => r.Statu != (int)RaffleStatusEnum.Generated)
                .Select(r => this.ToObject(r)).ToList();

            return new RequestResponseModel()
            {
                Object = raffles,
                Result = true
            };
        }

        internal object GetRaffleList(string status)
        {
            var statuList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(status);
            var context = new TicketsEntities();

            var raffles = context.Raffles.AsEnumerable().Where(s => statuList.Contains(s.Statu) || status.Count() == 0).Select(r => new
            {
                r.Id,
                r.Name,
                r.Prospect.Production,
                ProspectName = r.Prospect.Name,
                FractionCount = r.Prospect.LeafFraction * r.Prospect.LeafNumber
            }).ToList();

            return new { raffles };
        }

        internal RequestResponseModel SuspendExpiredRaffles()
        {
            var context = new TicketsEntities();

            var expiredRaffles = context.Raffles.AsEnumerable().Where(p =>
                    (p.Statu != (int)RaffleStatusEnum.Generated || p.Statu != (int)RaffleStatusEnum.Suspended)
                    && p.Prospect.ExpirateDate.Date < DateTime.Now.Date).ToList();

            foreach (var p in expiredRaffles)
            {
                p.Statu = (int)RaffleStatusEnum.Suspended;
                context.SaveChanges();
            }

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Sorteos suspendido correctamente!"
            };
        }

        internal object RaffleAwardDetails(TypesAwardCreationEnum? typesAwardCreation, RaffleAwardTypeEnum? raffleAwardType)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.Where(r => r.Statu == (int)RaffleStatusEnum.Active).FirstOrDefault();
            if (raffle == null)
            {
                return new
                {
                    result = false,
                    message = "No existe sorteo activo para ingresar los premios."
                };
            }

            var raffleData = new
            {
                awardList = raffle.Prospect.Awards.Where(a => typesAwardCreation.HasValue == false || a.TypesAward.Creation == (int)typesAwardCreation.Value).Select(a => new { a.Id, a.Name }),
                RaffleId = raffle.Id,
                RaffleName = raffle.Name,
                ProspectName = raffle.Prospect.Name,
                raffleAwards = raffle.RaffleAwards.Where(ra => raffleAwardType.HasValue == false || ra.RaffleAwardType == (int)raffleAwardType.Value).Select(r => new
                {
                    r.Id,
                    r.AwardId,
                    AwardName = r.Award.Name,
                    r.ControlNumber,
                    r.Fraction
                })
            };
            return raffleData;
        }

        internal RequestResponseModel SaveRaffle(RaffleModel model)
        {
            Raffle raffle;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Statu == (int)RaffleStatusEnum.Active && context.Raffles.Any(r => r.Statu == (int)RaffleStatusEnum.Active && r.Id != model.Id))
                        {
                            return new RequestResponseModel
                            {
                                Result = false,
                                Message = "Ya existe un sorteo activo, cambie el estado del sorteo para guardar."
                            };
                        }
                        if (model.Id <= 0)
                        {
                            raffle = new Raffle
                            {
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId
                            };
                        }
                        else
                        {
                            raffle = context.Raffles.FirstOrDefault(c => c.Id == model.Id);
                        }
                        raffle.Note = string.IsNullOrEmpty(model.Note) ? "" : model.Note;
                        raffle.Name = model.Name;
                        raffle.ProspectId = model.TicketProspectId;
                        //raffle.PoolsProspectId = model.PoolsProspectId;
                        raffle.Commodity = model.CommodityId;
                        raffle.Statu = model.Statu;
                        raffle.StartReturnDate = model.StartReturnDate;
                        raffle.EndReturnDate = model.EndReturnDate;
                        raffle.EndAllocationDate = model.EndAllocationDate;
                        raffle.Note = string.IsNullOrEmpty(model.Note) ? "" : model.Note;
                        raffle.DateSolteo = model.RaffleDate;

                        if (model.Id == 0)
                        {
                            context.Raffles.Add(raffle);
                        }
                        context.SaveChanges();
                        if (model.Id == 0)
                        {
                            var prospect = context.Prospects.Where(p => p.Id == raffle.ProspectId).FirstOrDefault();

                            var suscribers = context.TicketSuscribers.Where(s => s.Statu == (int)GeneralStatusEnum.Active).Select(a => new
                            {
                                a.ClientId,
                                a.FractionFrom,
                                a.FractionTo,
                                Numbers = a.TicketSuscriberNumbers
                            }).ToList();

                            var maxFraction = prospect.LeafFraction * prospect.LeafNumber;
                            foreach (var suscriber in suscribers)
                            {
                                var suscriberNumbers = new List<TicketAllocationNumber>();
                                foreach (var number in suscriber.Numbers)
                                {
                                    if (number.Number < prospect.Production)
                                    {
                                        var ticketAllocationNumber = new TicketAllocationNumber()
                                        {
                                            Number = number.Number,
                                            Printed = false,
                                            Invoiced = false,
                                            ControlNumber = "",
                                            FractionFrom = 1,
                                            FractionTo = maxFraction,
                                            Statu = (int)TicketStatusEnum.Alloated,
                                            CreateDate = DateTime.Now,
                                            CreateUser = WebSecurity.CurrentUserId
                                        };
                                        suscriberNumbers.Add(ticketAllocationNumber);
                                    }
                                }

                                if (suscriberNumbers.Count > 0)
                                {
                                    var ticketAllocation = new TicketAllocation()
                                    {
                                        ClientId = suscriber.ClientId,
                                        RaffleId = raffle.Id,
                                        CreateUser = WebSecurity.CurrentUserId,
                                        CreateDate = DateTime.Now,
                                        Type = (int)AllocationTypeEnum.Tickets,
                                        Statu = (int)AllocationStatuEnum.Created
                                    };

                                    context.TicketAllocations.Add(ticketAllocation);
                                    context.SaveChanges();

                                    suscriberNumbers.ForEach(s => s.TicketAllocationId = ticketAllocation.Id);
                                    context.TicketAllocationNumbers.AddRange(suscriberNumbers);
                                }
                            }
                            context.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message,
                            Object = new
                            {
                                e.InnerException,
                                e.StackTrace,
                                e.Source
                            }
                        };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, raffle.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Solteo", this.ToObject(raffle));

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Sorteo guardada correctamente!"
            };
        }

        internal RequestResponseModel GetRaffleSelect(int statu)
        {
            var context = new TicketsEntities();
            var prospectModel = new ProspectModel();
            var raffles = context.Raffles.AsEnumerable()
                .Where(r => r.Statu == statu || (statu == 0 && r.Statu != (int)RaffleStatusEnum.Suspended))
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    value = r.Id,
                    text = r.Name + " " + r.DateSolteo.ToShortDateString(),
                    ticketProspectId = r.ProspectId,
                    //poolProspectId = r.PoolsProspectId,
                    isActive = DateTime.Now <= r.EndAllocationDate

                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = raffles
            };
        }

        internal RequestResponseModel GetRaffleForReturnedSelect()
        {
            var context = new TicketsEntities();
            var prospectModel = new ProspectModel();
            var raffles = context.Raffles.AsEnumerable()
                .Where(s =>
                    ((s.Statu == (int)RaffleStatusEnum.Active
                    || s.Statu == (int)RaffleStatusEnum.Planned)
                    && s.EndReturnDate >= DateTime.Now)
                    || s.ReturnedOpens.Any(r => r.EndReturnedDate >= DateTime.Now))
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    value = r.Id,
                    text = r.Name + " " + r.DateSolteo.ToShortDateString(),
                    maxFraction = r.Prospect.LeafFraction * r.Prospect.LeafNumber,
                    production = r.Prospect.Production
                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = raffles
            };
        }

        internal RequestResponseModel GetRaffleForAllocationSelect()
        {
            var context = new TicketsEntities();
            var prospectModel = new ProspectModel();
            var raffles = context.Raffles.AsEnumerable()
                .Where(s =>
                    ((s.Statu == (int)RaffleStatusEnum.Active
                    || s.Statu == (int)RaffleStatusEnum.Planned)
                    && s.EndAllocationDate >= DateTime.Now))
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    value = r.Id,
                    text = r.Name + " " + r.DateSolteo.ToShortDateString(),
                    maxFraction = r.Prospect.LeafFraction * r.Prospect.LeafNumber,
                    production = r.Prospect.Production
                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = raffles
            };
        }


        internal RequestResponseModel Suspend(RaffleModel model)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(m => m.Id == model.Id);
            if (raffle != null)
            {
                raffle.Statu = (int)RaffleStatusEnum.Suspended;
                context.SaveChanges();
            }
            else
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "El sorto " + model.Id + " no existe."
                };
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Sorteo", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Sorteo borrado correctamente."
            };
        }

        internal RequestResponseModel GetActive()
        {
            var context = new TicketsEntities();

            var activeRaffles = context.Raffles.AsEnumerable().Where(s =>
                (s.Statu == (int)RaffleStatusEnum.Active
                || s.Statu == (int)RaffleStatusEnum.Planned)
                && s.DateSolteo.Date == DateTime.Now.Date).ToList();

            if (activeRaffles.Count() == 0)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "No hay sorteos para esta fecha"
                };
            }

            var activeRaffle = activeRaffles.OrderBy(r => r.Id).FirstOrDefault();
            activeRaffle.Statu = (int)RaffleStatusEnum.Active;
            context.SaveChanges();

            return new RequestResponseModel()
            {
                Result = true,
                Object = this.ToObject(activeRaffle)
            };
        }
    }
}