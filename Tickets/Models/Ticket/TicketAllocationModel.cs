﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tickets.Models.Enums;
using Tickets.Models.Procedures;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketAllocationModel
    {
        private static Random random = new Random();

        [JsonProperty(PropertyName = "id")]
        [ConcurrencyCheck]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "raffleNomenclature")]
        public string RaffleNomenclature { get; set; }

        [JsonProperty(PropertyName = "typeId")]
        public int TypeId { get; set; }

        [JsonProperty(PropertyName = "typeDesc")]
        public string TypeDesc { get; set; }

        [JsonProperty(PropertyName = "statuId")]
        public int StatuId { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        [JsonProperty(PropertyName = "createDateLong")]
        public long CreateDateLong { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "canAllocate")]
        public bool CanAllocate { get; set; }

        [JsonProperty(PropertyName = "fractionPrice")]
        public decimal FractionPrice { get; set; }

        [JsonProperty(PropertyName = "production")]
        public int Production { get; set; }

        [JsonProperty(PropertyName = "ticketAllocationNumbers")]
        public List<TicketAllocationNumberModel> TicketAllocationNumbers { get; set; }

        [JsonProperty(PropertyName = "ticketAllocationSeries")]
        public List<TicketAllocationSerieModel> TicketAllocationSeries { get; set; }

        [JsonProperty(PropertyName = "numberCount")]
        public int NumberCount { get; set; }

        [JsonProperty(PropertyName = "fractionCount")]
        public int FractionCount { get; set; }

        [JsonProperty(PropertyName = "fractionRest")]
        public int FractionRest { get; set; }

        [JsonProperty(PropertyName = "ticketFraction")]
        public int TicketFraction { get; set; }

        [JsonProperty(PropertyName = "returnFractions")]
        public int ReturnFractions { get; set; }

        [JsonProperty(PropertyName = "returnTickets")]
        public int ReturnTickets { get; set; }

        [JsonProperty(PropertyName = "restReturnFractions")]
        public int RestReturnFractions { get; set; }

        [JsonProperty(PropertyName = "Agente")]
        public string Agente { get; set; }

        [JsonProperty(PropertyName = "grupo")]
        public int Grupo { get; set; }

        [JsonProperty(PropertyName = "allocationId")]
        public int AllocationId { get; set; }

        [JsonProperty(PropertyName = "ticketsQuantity")]
        public int TicketsQuantity { get; set; }

        [JsonProperty(PropertyName = "fractionQuantity")]
        public int FractionQuantity { get; set; }

        [JsonProperty(PropertyName = "prospectFraction")]
        public int ProspectFraction { get; set; }

        [JsonProperty(PropertyName = "allocationFractionQuantity")]
        public int AllocationFractionQuantity { get; set; }

        [JsonProperty(PropertyName = "allocationTicketsQuantity")]
        public int AllocationTicketsQuantity { get; set; }

        [JsonProperty(PropertyName = "totalRest")]
        public int TotalRest { get; set; }

        [JsonProperty(PropertyName = "totalRestTickets")]
        public int TotalRestTickets { get; set; }

        [JsonProperty(PropertyName = "anyReturn")]
        public int AnyReturn { get; set; }

        [JsonProperty(PropertyName = "sequenceNumberRaffle")]
        public int? SequenceNumberRaffle { get; set; }

        [JsonProperty(PropertyName = "sequenceNumberTicketAllocation")]
        public int? SequenceNumberTicketAllocation { get; set; }

        #region Private Method
        private static List<TicketAllocation> CopyTicketAllocation(TicketsEntities context, int sourceId, int targetId, int type)
        {
            var source = context.Raffles.FirstOrDefault(m => m.Id == sourceId);
            var target = context.Raffles.FirstOrDefault(r => r.Id == targetId);

            int maxFraction = target.Prospect.LeafFraction * target.Prospect.LeafNumber;
            var targentNumbers = new List<long>();
            target.TicketAllocations.Where(a => a.Type == type).ToList()
                .ForEach(a => targentNumbers.AddRange(a.TicketAllocationNumbers.Select(n => n.Number).ToList()));

            List<TicketAllocation> allocations = new List<TicketAllocation>();
            foreach (var a in source.TicketAllocations.Where(a => a.Type == type).ToList())
            {
                var allocationNumbers = new List<TicketAllocationNumber>();
                foreach (var n in a.TicketAllocationNumbers.ToList())
                {
                    if (targentNumbers.Any(tn => tn == n.Number) == false)
                    {
                        int fractionTo = n.FractionTo > maxFraction ? maxFraction : n.FractionTo;
                        var number = new TicketAllocationNumber()
                        {
                            Number = n.Number,
                            ControlNumber = "",
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            FractionTo = fractionTo,
                            FractionFrom = n.FractionFrom,
                            Invoiced = false,
                            Printed = false,
                            Statu = (int)TicketStatusEnum.Alloated
                        };
                        allocationNumbers.Add(number);
                    }
                }
                if (allocationNumbers.Count > 0)
                {
                    if (allocationNumbers[0].FractionFrom < maxFraction)
                    {
                        var allocation = new TicketAllocation()
                        {
                            ClientId = a.ClientId,
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            RaffleId = target.Id,
                            Statu = (int)AllocationStatuEnum.Created,
                            Type = a.Type
                        };
                        allocation.TicketAllocationNumbers = allocationNumbers;
                        allocations.Add(allocation);
                    }
                }
            }
            return allocations;
        }

        /*        private object TicketAllocationMainToObject(TicketAllocation ticket)
                {
                    var context = new TicketsEntities();
                    return new
                    {
                        ticket.Id,
                        ticket.RaffleId,
                        RaffleDesc = context.Raffles.FirstOrDefault(s => s.Id == ticket.RaffleId).Name,
                        RaffleDate = context.Raffles.FirstOrDefault(s => s.Id == ticket.RaffleId).DateSolteo.ToUnixTime(),
                        ticket.ClientId,
                        ClientDesc = context.Clients.FirstOrDefault(c => c.Id == ticket.ClientId).Name,
                        FractionFrom = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionFrom : 0,
                        FractionTo = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionTo : 0,
                        CreateDate = ticket.CreateDate.ToUnixTime(),
                        ticket.CreateUser,
                        ticket.Statu,
                        CraeteuserDesc = context.Users.FirstOrDefault(u => u.Id == ticket.CreateUser).Name
                    };
                }*/

        public object TicketAllocationForInvoiceToObject(TicketAllocation ticket)
        {
            var context = new TicketsEntities();
            return new
            {
                ticket.Id,
                ticket.RaffleId,
                FractionFrom = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionFrom : 0,
                FractionTo = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionTo : 0,
                Price = ticket.Raffle.Prospect.Prospect_Price.AsEnumerable().Where(p => p.PriceId == ticket.Client.PriceId).Select(p => new
                {
                    p.FactionPrice,
                    p.SeriePrice,
                    p.TicketPrice,
                    PriceDesc = p.Catalog.NameDetail
                }).FirstOrDefault(),
                numberCount = context.TicketAllocationNumbers.Where(tn => tn.TicketAllocationId == ticket.Id).Count()
            };
        }

        private object TicketAllocationToObject(TicketAllocation ticket)
        {
            var context = new TicketsEntities();
            return new
            {
                ticket.Id,
                sequenceNumberTicketAllocation = ticket.SequenceNumber,
                ticket.RaffleId,
                sequenceNumberRaffle = ticket.Raffle.SequenceNumber,
                //RaffleDesc = context.Raffles.FirstOrDefault(s => s.Id == ticket.RaffleId).Name,
                RaffleNomenclature = ticket.Raffle.Symbol + ticket.Raffle.Separator + ticket.Raffle.SequenceNumber,
                RaffleDesc = ticket.Raffle.Symbol + ticket.Raffle.Separator + ticket.Raffle.SequenceNumber + " " + ticket.Raffle.Name + " " + ticket.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                RaffleDate = ticket.Raffle.DateSolteo.ToUnixTime(),
                ticket.ClientId,
                ClientDesc = ticket.Client.Name,
                FractionFrom = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionFrom : 0,
                FractionTo = ticket.TicketAllocationNumbers.Count > 0 ? ticket.TicketAllocationNumbers.FirstOrDefault().FractionTo : 0,
                CreateDate = ticket.CreateDate.ToUnixTime(),
                ticket.CreateUser,
                ticket.Statu,
                isActive = (ticket.Raffle.Statu == (int)RaffleStatusEnum.Active || ticket.Raffle.Statu == (int)RaffleStatusEnum.Planned) && DateTime.Now <= ticket.Raffle.EndAllocationDate,
                CraeteuserDesc = ticket.User.Name,
                Price = ticket.Raffle.Prospect.Prospect_Price.AsEnumerable().Where(p => p.PriceId == ticket.Client.PriceId).Select(p => new
                {
                    p.FactionPrice,
                    p.SeriePrice,
                    p.TicketPrice,
                    PriceDesc = p.Catalog.NameDetail
                }).FirstOrDefault(),
                Printed = ticket.Statu == (int)AllocationStatuEnum.Printed,
                ticketNumbers = context.TicketAllocationNumbers.Where(tn => tn.TicketAllocationId == ticket.Id).Select(t => new
                {
                    t.Id,
                    t.Number,
                    t.Statu,
                    StatuDec = context.Catalogs.FirstOrDefault(c => c.Id == t.Statu).NameDetail,
                    CreateDate = t.CreateDate.ToString(),
                    t.CreateUser,
                    CraeteuserDesc = t.User.Name,
                    t.Invoiced,
                    t.FractionFrom,
                    t.FractionTo,
                    t.Printed,
                    t.ControlNumber,
                    t.TicketAllocationId
                })
            };
        }

        #endregion

        internal TicketAllocationModel ListadoAsignaciones(TicketAllocation model)
        {
            var context = new TicketsEntities();
            var allocation = new TicketAllocationModel()
            {
                Id = model.Id,
                SequenceNumberTicketAllocation = model.SequenceNumber,
                ClientId = model.ClientId,
                ClientDesc = model.Client.Name,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
                CreateDate = model.CreateDate,
                StatuId = model.Statu,
                Agente = model.Agente,
                Grupo = context.Clients.FirstOrDefault(c => c.Id == model.ClientId).GroupId
            };
            return allocation;
        }

        internal TicketAllocationModel InvoiceDetails(TicketAllocation model, bool hasNumber = true, bool hasProspectProperty = true)
        {
            var context = new TicketsEntities();

            Procedure_AvailableTicketToInvoice availableTicketToInvoice = new Procedure_AvailableTicketToInvoice();
            var Resultado = availableTicketToInvoice.AvailableTicketsToInvoice(model.RaffleId, model.Id);

            var TotalFracciones = Resultado.Select(s => s.AvailableFractions).Sum();

            var allocation = new TicketAllocationModel()
            {
                Id = model.Id,
                ClientId = model.ClientId,
                RaffleId = model.RaffleId,
                SequenceNumberTicketAllocation = model.SequenceNumber,
                SequenceNumberRaffle = model.Raffle.SequenceNumber,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
                StatuId = model.Statu,
                TypeId = model.Type,
                Agente = model.Agente,
                CreateDate = model.CreateDate,
                CreateDateLong = model.CreateDate.ToUnixTime(),
                NumberCount = TotalFracciones == 0 ? 0 : (TotalFracciones / Resultado.FirstOrDefault().TicketFraction),
                FractionCount = TotalFracciones,
                FractionRest = TotalFracciones == 0 ? 0 : (TotalFracciones % Resultado.FirstOrDefault().TicketFraction),
                TicketFraction = TotalFracciones == 0 ? model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber : model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber
            };

            if (hasNumber)
            {
                //var numberModel = new TicketAllocationNumberModel();
                //allocation.TicketAllocationNumbers = model.TicketAllocationNumbers.Select(n => numberModel.ToObject(n)).ToList();

                if (context.TicketReturns.Any(a => a.TicketAllocationNumber.TicketAllocationId == model.Id))
                {
                    allocation.ReturnFractions = context.TicketReturns.Where(w => w.TicketAllocationNumber.TicketAllocationId == model.Id).Select(s => s.FractionTo - s.FractionFrom + 1).Sum();

                    if (TotalFracciones != 0)
                    {
                        allocation.ReturnTickets = (allocation.ReturnFractions / Resultado.FirstOrDefault().TicketFraction);
                        allocation.RestReturnFractions = (allocation.ReturnFractions % Resultado.FirstOrDefault().TicketFraction);
                    }
                    else
                    {
                        allocation.ReturnTickets = (allocation.ReturnFractions / (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber));
                        allocation.RestReturnFractions = (allocation.ReturnFractions % (model.Raffle.Prospect.LeafFraction * model.Raffle.Prospect.LeafNumber));
                    }
                }
                else
                {
                    allocation.ReturnFractions = 0;
                    allocation.ReturnTickets = 0;
                    allocation.RestReturnFractions = 0;
                }
            }
            if (hasProspectProperty)
            {
                Prospect prospect = context.Prospects.FirstOrDefault(p => p.Id == model.Raffle.ProspectId);

                var price = prospect.Prospect_Price.FirstOrDefault(p => p.PriceId == model.Client.PriceId);
                if (price == null)
                {
                    allocation.FractionPrice = 16;
                }
                else
                {
                    allocation.FractionPrice = price.FactionPrice;
                }
                allocation.Production = prospect.Production;
            }
            return allocation;
        }

        internal TicketAllocationModel ToObject(TicketAllocation model, bool hasNumber = true, bool hasProspectProperty = true)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(c => c.Id == model.RaffleId);
            var client = context.Clients.FirstOrDefault(c => c.Id == model.ClientId);

            var allocation = new TicketAllocationModel()
            {
                Id = model.Id,
                SequenceNumberTicketAllocation = model.SequenceNumber,
                ClientId = model.ClientId,
                ClientDesc = client.Name,
                //RaffleDesc = model.Raffle.Id + " - " + raffle.Name,
                SequenceNumberRaffle = model.Raffle.SequenceNumber,
                RaffleNomenclature = model.Raffle.Symbol + model.Raffle.Separator + model.Raffle.SequenceNumber,
                RaffleDesc = model.Raffle.Symbol + model.Raffle.Separator + model.Raffle.SequenceNumber + " " + model.Raffle.Name + " " + model.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                RaffleId = model.RaffleId,
                StatuDesc = context.Catalogs.Where(c => c.Id == model.Statu).Select(s => s.NameDetail).FirstOrDefault(),
                StatuId = model.Statu,
                TypeDesc = context.Catalogs.Where(c => c.Id == model.Type).Select(s => s.NameDetail).FirstOrDefault(),
                TypeId = model.Type,
                CreateDate = model.CreateDate,
                CreateDateLong = model.CreateDate.ToUnixTime(),
                CanAllocate = DateTime.Now <= raffle.EndAllocationDate,
                NumberCount = model.TicketAllocationNumbers.Count,
                Agente = model.Agente,
                FractionCount = model.Statu == (int)AllocationStatuEnum.Deleted ? model.TicketAllocationNumber_Delete.Sum(s => (s.FractionTo - s.FractionFrom) + 1) : model.TicketAllocationNumbers.Sum(s => (s.FractionTo - s.FractionFrom) + 1)
            };
            if (hasNumber)
            {
                var numberModel = new TicketAllocationNumberModel();
                if (allocation.StatuId == (int)AllocationStatuEnum.Deleted)
                {
                    allocation.TicketAllocationNumbers = model.TicketAllocationNumber_Delete.Select(s => numberModel.ToObjectDelete(s)).ToList();
                }
                else
                {
                    allocation.TicketAllocationNumbers = model.TicketAllocationNumbers.Select(n => numberModel.ToObject(n)).ToList();
                }
            }
            if (hasProspectProperty)
            {
                Prospect prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId);

                /*else
                {
                    //prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.PoolsProspectId);
                }*/

                var ticketAllocationSerieModel = new List<TicketAllocationSerieModel>();
                for (int i = 1; i <= prospect.LeafNumber; i++)
                {
                    var serie = new TicketAllocationSerieModel()
                    {
                        Id = String.Concat("S", i),
                        Serie = String.Concat("S-", i)
                    };
                    ticketAllocationSerieModel.Add(serie);
                }
                allocation.TicketAllocationSeries = ticketAllocationSerieModel;

                var price = prospect.Prospect_Price.FirstOrDefault(p => p.PriceId == client.PriceId);
                if (price == null)
                {
                    allocation.FractionPrice = 16;
                }
                else
                {
                    allocation.FractionPrice = price.FactionPrice;
                }
                allocation.Production = prospect.Production;
            }
            return allocation;
        }

        internal RequestResponseModel GetTicketAllocationListForInvoice(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    (a.Statu == (int)AllocationStatuEnum.Consigned ||
                     a.Statu == (int)AllocationStatuEnum.Generated ||
                     a.Statu == (int)AllocationStatuEnum.Delivered)
                    && a.RaffleId == raffleId
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.InvoiceDetails(a)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel GetReturnedByGroup(int raffleId)
        {
            Procedure_ReturnedByGroup returnedByGroupProcedure = new Procedure_ReturnedByGroup();
            var Resultado = returnedByGroupProcedure.ConsultarBilletesDevueltosPorGrupo(raffleId);
            return new RequestResponseModel()
            {
                Result = true,
                Object = Resultado
            };
        }

        internal RequestResponseModel GetTicketAllocationList(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    (a.Statu == statu || statu == 0)
                    && a.RaffleId == raffleId
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ListadoAsignaciones(a)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel GetTicketAllocationToDeliverList(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    (a.Statu == (int)AllocationStatuEnum.Consigned || a.Statu == (int)AllocationStatuEnum.Delivered ||
                     a.Statu == (int)AllocationStatuEnum.Invoiced || a.Statu == (int)AllocationStatuEnum.Returned) &&
                    (a.Statu == statu || statu == 0)
                    && a.RaffleId == raffleId && (a.Client.GroupId != (int)ClientGroupEnum.DistribuidorElectronico || a.Client.GroupId != (int)ClientGroupEnum.DistribuidorXML)
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ListadoAsignaciones(a)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel GetTicketAllocationConsignateList(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a => (a.Statu == (int)AllocationStatuEnum.Consigned || a.Statu == (int)AllocationStatuEnum.Review ||
                       a.Statu == (int)AllocationStatuEnum.Printed || a.Statu == (int)AllocationStatuEnum.Delivered ||
                       a.Statu == (int)AllocationStatuEnum.Invoiced || a.Statu == (int)AllocationStatuEnum.Returned)
                    && a.RaffleId == raffleId &&
                    (//a.Client.GroupId != (int)ClientGroupEnum.CajaDespachoExpress && a.Client.GroupId != (int)ClientGroupEnum.CajasOficinaPrincipal &&
                    a.Client.GroupId != (int)ClientGroupEnum.ContenedorElectronico)
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ListadoAsignaciones(a)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel TicketAllocationListForPrint(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    ((a.Statu == statu || statu == 0) && a.Statu != (int)AllocationStatuEnum.Review)
                    && a.RaffleId == raffleId
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ToObject(a, hasNumber)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel GetPendintPrintedAllocations(int raffleId, int clientId = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    (a.Statu == (int)AllocationStatuEnum.PendientPrint || a.Statu == (int)AllocationStatuEnum.Printed)
                    && a.RaffleId == raffleId
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ToObject(a, hasNumber)).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel CopyAllocations(CopyAllocationModel model)
        {
            var context = new TicketsEntities();
            var tm = context.Database.BeginTransaction();
            try
            {
                List<TicketAllocation> allocations = new List<TicketAllocation>();
                if (model.SourceTicketRaffleId > 0)
                {
                    allocations.AddRange(CopyTicketAllocation(context, model.SourceTicketRaffleId, model.TargetRaffleId, (int)AllocationTypeEnum.Tickets));
                }
                if (model.SourcePoolRaffleId > 0)
                {
                    allocations.AddRange(CopyTicketAllocation(context, model.SourcePoolRaffleId, model.TargetRaffleId, (int)AllocationTypeEnum.Pools));
                }

                context.TicketAllocations.AddRange(allocations);
                allocations.ForEach(a =>
                {
                    a.TicketAllocationNumbers.ToList().ForEach(t => t.TicketAllocationId = a.Id);
                    context.TicketAllocationNumbers.AddRange(a.TicketAllocationNumbers);
                });
                context.SaveChanges();

                tm.Commit();
                return new RequestResponseModel()
                {
                    Result = true,
                    Message = "Copiado correctamente."
                };
            }
            catch (Exception e)
            {
                tm.Rollback();
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = e.Message,
                    Object = new
                    {
                        e.StackTrace,
                        e.InnerException
                    }
                };
            }
        }

        internal RequestResponseModel GetTicketAllocation(int id)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations.Where(a => a.Id == id).AsEnumerable()
                .Select(a => this.ToObject(a)).FirstOrDefault();
            if (allocation == null)
            {
                allocation = new TicketAllocationModel()
                {
                    Id = 0,
                    ClientId = 0,
                    RaffleId = 0,
                    TypeId = (int)Tickets.Models.Enums.AllocationTypeEnum.Tickets,
                    TicketAllocationNumbers = new List<TicketAllocationNumberModel>()
                };
            }
            return new RequestResponseModel()
            {
                Result = true,
                Object = allocation
            };
        }

        internal RequestResponseModel ValidateAllocation(TicketAllocationModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    if (model.Id > 0)
                    {
                        var allocation = context.TicketAllocations.Where(a => a.Id == model.Id)
                            .AsEnumerable().Select(m => this.ToObject(m)).FirstOrDefault();
                        model.RaffleId = allocation.RaffleId;
                    }
                    var numbers = model.TicketAllocationNumbers.Select(t => (int)t.Number);

                    var raffleNumbers = new List<TicketAllocationNumber>();
                    context.Raffles.FirstOrDefault(r => r.Id == model.RaffleId)
                        .TicketAllocations.Where(a => a.Type == model.TypeId).ToList().ForEach(a =>
                        raffleNumbers.AddRange(a.TicketAllocationNumbers));

                    var duplicateNumbers = raffleNumbers.Where(t =>
                         numbers.Where(n => n == t.Number).Any()).ToList();
                    string number = "";
                    if (duplicateNumbers.Count > 0)
                    {
                        var fractionFrom = model.TicketAllocationNumbers.FirstOrDefault().FractionFrom;
                        var fractionTo = model.TicketAllocationNumbers.FirstOrDefault().FractionTo;

                        foreach (var duplicateNumber in duplicateNumbers)
                        {
                            if (fractionFrom >= duplicateNumber.FractionFrom
                                && fractionTo <= duplicateNumber.FractionTo)
                            {
                                number += duplicateNumber.Number + " fracciones( " + fractionFrom + " a " + fractionTo + " ), ";
                            }
                        }
                    }

                    if (number != "")
                    {
                        number = number.Substring(0, number.Length - 2);
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "Los numeros ( " + number + " ) ya fueron asignado."
                        };
                    }
                    else
                    {
                        return new RequestResponseModel()
                        {
                            Result = true,
                            Message = "Nummero agregado correctamente"
                        };
                    }
                }
            }
        }

        internal RequestResponseModel SaveAllocation(TicketAllocationModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Id == 0)
                        {
                            var ticketAllocation = new TicketAllocation()
                            {
                                ClientId = model.ClientId,
                                RaffleId = model.RaffleId,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now,
                                Type = model.TypeId,
                                Statu = (int)AllocationStatuEnum.Created,
                                Agente = model.Agente
                            };
                            context.TicketAllocations.Add(ticketAllocation);
                            context.SaveChanges();

                            List<TicketAllocationNumber> ticketAllocations = new List<TicketAllocationNumber>();

                            const int length = 8;
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                            foreach (var number in model.TicketAllocationNumbers)
                            {
                                var ticket = new TicketAllocationNumber()
                                {
                                    Number = number.Number,
                                    Invoiced = false,
                                    Printed = false,
                                    ControlNumber = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    //ControlNumber = "N/A",
                                    FractionFrom = number.FractionFrom,
                                    FractionTo = number.FractionTo,
                                    TicketAllocationId = ticketAllocation.Id,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    CreateDate = DateTime.Now,
                                    Statu = (int)TicketStatusEnum.Alloated,
                                    RaffleId = ticketAllocation.RaffleId
                                };
                                ticketAllocations.Add(ticket);
                            }
                            context.TicketAllocationNumbers.AddRange(ticketAllocations);
                            context.SaveChanges();
                        }
                        else
                        {
                            List<TicketAllocationNumber> ticketAllocations = new List<TicketAllocationNumber>();
                            foreach (var number in model.TicketAllocationNumbers.Where(a => a.Id == 0))
                            {
                                var ticket = new TicketAllocationNumber()
                                {
                                    Number = number.Number,
                                    Invoiced = false,
                                    Printed = false,
                                    Consignated = false,
                                    ControlNumber = "",
                                    TicketAllocationId = model.Id,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    CreateDate = DateTime.Now,
                                    Statu = (int)TicketStatusEnum.Alloated
                                };
                                ticketAllocations.Add(ticket);
                            }
                            context.TicketAllocationNumbers.AddRange(ticketAllocations);
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
                                e.StackTrace
                            }
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, model.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Asignación de Billetes", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Asignación de ticket correctamente!"
            };
        }

        internal RequestResponseModel DeleteAllocation(TicketAllocationModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var allocation = context.TicketAllocations.Where(a => a.Id == model.Id).FirstOrDefault();
                        if (allocation == null)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "La asignación de billetes no existe."
                            };
                        }
                        if (allocation.TicketAllocationNumbers.Where(n => n.Printed == true).Any())
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "La asignación de billetes ya fue impresa."
                            };
                        }
                        if (allocation.Raffle.Statu == (int)RaffleStatusEnum.Generated)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "El sorteo de esta asignación de billetes fue generado."
                            };
                        }

                        context.TicketAllocationNumbers.RemoveRange(allocation.TicketAllocationNumbers);
                        context.SaveChanges();

                        context.TicketAllocations.Remove(allocation);
                        context.SaveChanges();
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
                                e.Message,
                                e.InnerException,
                                e.StackTrace
                            }
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Borando Asignación de Billetes", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Asignación de billetes borrada correctamente!"
            };
        }

        internal RequestResponseModel ChangeAllocationStatu(TicketAllocationModel model, int statu)
        {
            var allocationObject = new object();
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var allocation = context.TicketAllocations.Where(a => a.Id == model.Id).FirstOrDefault();
                        allocation.Statu = statu;
                        context.SaveChanges();
                        allocationObject = TicketAllocationToObject(allocation);
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
                                e.Message,
                                e.InnerException,
                                e.StackTrace
                            }
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Asignación enviada a impresión", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Asignación de billete enviada correctamente!"
            };
        }

        internal RequestResponseModel TicketAllocationReview(TicketAllocationModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var allocation = context.TicketAllocations.Where(a => a.Id == model.Id).FirstOrDefault();
                        if (allocation == null)
                        {
                            return new RequestResponseModel() { Result = true, Message = "La asignación de billetes no existe." };
                        }

                        if (allocation.Raffle.Statu == (int)RaffleStatusEnum.Generated)
                        {
                            return new RequestResponseModel() { Result = true, Message = "El sorteo de esta asignación de billetes fue generado." };
                        }

                        allocation.Statu = (int)AllocationStatuEnum.Review;
                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Revision de Asignación de Billetes", model);

            return new RequestResponseModel() { Result = true, Message = "Asignación de billetes revisada correctamente!" };
        }


        internal RequestResponseModel TicketPrintDetails(TicketAllocationModel model)
        {
            var allocationObject = new object();
            string ticketDesing = "";
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var numbers = context.TicketAllocationNumbers.Where(t => t.TicketAllocationId == model.Id);

                        foreach (var number in numbers)
                        {
                            if (number.Statu == (int)TicketStatusEnum.Alloated)
                            {
                                var randon = new Random();
                                var controlNumber = randon.Next(0, 10)
                                    + ((char)('a' + randon.Next(0, 26))).ToString().ToUpper()
                                    + randon.Next(0, 10)
                                    + ((char)('a' + randon.Next(0, 26))).ToString().ToUpper()
                                    + randon.Next(0, 10).ToString()
                                    + randon.Next(0, 10).ToString()
                                    + randon.Next(0, 10).ToString()
                                    + randon.Next(0, 10).ToString();
                                number.Printed = true;
                                number.ControlNumber = controlNumber;
                                number.PrintedDate = DateTime.Now;
                                number.Statu = (int)TicketStatusEnum.Printed;
                            }
                        }
                        context.SaveChanges();

                        var allocation = context.TicketAllocations.Where(a => a.Id == model.Id).FirstOrDefault();
                        allocationObject = this.ToObject(allocation, true, true);
                        dbContextTransaction.Commit();
                        var config = context.SystemConfigs.FirstOrDefault();
                        ticketDesing = context.Catalogs.FirstOrDefault(c => c.Id == config.TicketDesign).Description;
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = true,
                            Message = e.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Impreción de Billetes", allocationObject);

            return new RequestResponseModel()
            {
                Result = true,
                Object = ticketDesing,
                Message = "Impresion de Billete realizada correctamente!"
            };
        }
    }
}