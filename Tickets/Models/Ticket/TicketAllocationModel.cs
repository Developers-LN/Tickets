using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketAllocationModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

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

        [JsonProperty(PropertyName = "numberCount")]
        public int NumberCount { get; set; }

        [JsonProperty(PropertyName = "fractionCount")]
        public int FractionCount { get; set; }

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

        private object TicketAllocationMainToObject(TicketAllocation ticket)
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
        }

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
                isActive = (ticket.Raffle.Statu == (int)RaffleStatusEnum.Active || ticket.Raffle.Statu == (int)RaffleStatusEnum.Planned) && DateTime.Now <= ticket.Raffle.EndAllocationDate,
                CraeteuserDesc = context.Users.FirstOrDefault(u => u.Id == ticket.CreateUser).Name,
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
                    CraeteuserDesc = context.Users.FirstOrDefault(u => u.Id == t.CreateUser).Name,
                    t.Invoiced,
                    FractionFrom = t.FractionFrom,
                    FractionTo = t.FractionTo,
                    t.Printed,
                    t.ControlNumber,
                    TicketAllocationId = t.TicketAllocationId
                })
            };
        }

        #endregion

        internal TicketAllocationModel ToObject(TicketAllocation model, bool hasNumber = true, bool hasProspectProperty = true)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(c => c.Id == model.RaffleId);
            var client = context.Clients.FirstOrDefault(c => c.Id == model.ClientId);
            var allocation = new TicketAllocationModel()
            {
                Id = model.Id,
                ClientId = model.ClientId,
                ClientDesc = client.Name,
                RaffleDesc = model.RaffleId + " - " + raffle.Name,
                RaffleId = model.RaffleId,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Statu).NameDetail,
                StatuId = model.Statu,
                //TypeDesc = context.Catalogs.FirstOrDefault(c => c.Id == model.Type).NameDetail,
                TypeId = model.Type,
                CreateDate = model.CreateDate,
                CreateDateLong = model.CreateDate.ToUnixTime(),
                CanAllocate = DateTime.Now <= raffle.EndAllocationDate,
                NumberCount = model.TicketAllocationNumbers.Count,
                FractionCount = model.TicketAllocationNumbers.Select(a => a.FractionTo - a.FractionFrom + 1).Sum()
            };
            if (hasNumber)
            {
                var numberModel = new TicketAllocationNumberModel();
                allocation.TicketAllocationNumbers = model.TicketAllocationNumbers.Select(n => numberModel.ToObject(n)).ToList();
            }
            if (hasProspectProperty)
            {
                Prospect prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.ProspectId);

                /*else
                {
                    //prospect = context.Prospects.FirstOrDefault(p => p.Id == raffle.PoolsProspectId);
                }*/
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

        internal RequestResponseModel GetTicketAllocationList(int raffleId, int clientId = 0, int statu = 0, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations
                .Where(a =>
                    (a.Statu == statu || statu == 0)
                    && a.RaffleId == raffleId
                    && (a.ClientId == clientId || clientId == 0)).AsEnumerable()
                .Select(a => this.ToObject(a, hasNumber)).ToList();

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
                                Statu = (int)AllocationStatuEnum.Created
                            };
                            context.TicketAllocations.Add(ticketAllocation);
                            context.SaveChanges();

                            List<TicketAllocationNumber> ticketAllocations = new List<TicketAllocationNumber>();
                            
                            foreach (var number in model.TicketAllocationNumbers)
                            {
                                var ticket = new TicketAllocationNumber()
                                {
                                    Number = number.Number,
                                    Invoiced = false,
                                    Printed = false,
                                    ControlNumber = "NA",
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