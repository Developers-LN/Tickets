using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Models.Raffles
{
    public class TicketReprintModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "ticketReprintNumbers")]
        public List<TicketReprintNumberModel> TicketReprintNumbers { get; set; }

        [JsonProperty(PropertyName = "numberCount")]
        public int NumberCount { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

        [JsonProperty(PropertyName = "isPrint")]
        public bool IsPrint { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createDateLong")]
        public long CreateDateLong { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "createUserDesc")]
        public string CreateUserDesc { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        internal object GetTicketReprintList(int raffleId = 0)
        {
            var context = new TicketsEntities();
            var ticketReprints = context.TicketRePrints.AsEnumerable()
                .Where(t => t.RaffleId == raffleId
                    && (t.Statu == (int)TicketReprintStatuEnum.Created
                    || t.Statu == (int)TicketReprintStatuEnum.Improcess
                    || t.Statu == (int)TicketReprintStatuEnum.Rejected)
                    )
                .Select(t => this.ToObject(t)).ToList();

            List<object> raffles = null;
            if (raffleId <= 0)
            {
                raffles = context.Raffles.AsEnumerable().Where(r => r.Statu != (int)RaffleStatusEnum.Suspended)
                    .Select(r => new
                    {
                        r.Name,
                        r.Id
                    }).ToList<object>();
            }
            return new { ticketReprints, raffles };
        }

        internal object GetAllocationByClient(int raffleId, int clientId)
        {
            var context = new TicketsEntities();
            var allocations = context.TicketAllocations.AsEnumerable().Where(a =>
                a.ClientId == clientId
                && a.RaffleId == raffleId
                && a.Statu == (int)AllocationStatuEnum.Printed
                ).Select(a => new
                {
                    a.Id,
                    Numbers = a.TicketAllocationNumbers.Select(tn => new
                    {
                        tn.Id,
                        tn.Number
                    })
                }).ToList();

            return new { allocations };
        }

        private object ProccessToObject(WorkflowProccess proccess)
        {
            return new
            {
                proccess.WorkFlowId,
                proccess.Statu,
                proccess.Id,
                proccess.Comment,
                proccess.CreateUser,
                proccess.CreateDate
            };
        }

        internal TicketReprintModel ToObject(TicketRePrint reprint, bool hasNumbers = false)
        {
            var context = new TicketsEntities();

            var model = new TicketReprintModel()
            {
                Id = reprint.Id,
                Note = reprint.Note,
                RaffleId = reprint.RaffleId,
                RaffleDesc = context.Raffles.FirstOrDefault( r=> r.Id == reprint.RaffleId).Name,
                IsPrint = false,
                CreateDate = reprint.CreateDate,
                CreateDateLong = reprint.CreateDate.ToUnixTime(),
                CreateUser = reprint.CreateUser,
                CreateUserDesc = context.Users.FirstOrDefault( u=>u.Id == reprint.CreateUser).Name,
                Statu = reprint.Statu,
                StatuDesc = context.Catalogs.FirstOrDefault(u => u.Id == reprint.Statu).NameDetail,
                NumberCount = reprint.TicketRePrintNumbers.Count()
            };
            if (hasNumbers == true)
            {
                var reprintNumberModel = new TicketReprintNumberModel();
                model.TicketReprintNumbers = reprint.TicketRePrintNumbers.Select(r => reprintNumberModel.ToObject(r)).ToList();
            }
            return model;
        }

        internal RequestResponseModel GetReprint(int id)
        {
            var context = new TicketsEntities();
            var ticketReprint = context.TicketRePrints.Where(t => t.Id == id ).AsEnumerable()
                .Select(t => this.ToObject(t)).FirstOrDefault();
            if (ticketReprint == null)
            {
                ticketReprint = new TicketReprintModel()
                {
                    Id = 0,
                    IsPrint = false,
                    TicketReprintNumbers = new List<TicketReprintNumberModel>()
                };
            }
            return new RequestResponseModel (){ 
                Result = true,
                Object = ticketReprint 
            };
        }

        internal RequestResponseModel GetReprintList(int raffleId = 0, int status = 0)
        {
            var context = new TicketsEntities();
            var reprints = context.TicketRePrints.AsEnumerable()
                .Where(t => (t.RaffleId == raffleId || raffleId == 0)
                    && (status == t.Statu || status == 0))
                .Select(t => this.ToObject(t)).ToList();

            return new RequestResponseModel ()
            {
                Result = true,
                Object = reprints
            };
        }

        internal RequestResponseModel Save(TicketReprintModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.Id == 0)
                        {
                            var ticketRePrint = new TicketRePrint()
                            {
                                RaffleId = model.RaffleId,
                                Note = model.Note,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now
                            };
                            if (model.IsPrint == true)
                            {
                                ticketRePrint.Statu = (int)TicketReprintStatuEnum.Approved;
                            }
                            else
                            {
                                ticketRePrint.Statu = (int)TicketReprintStatuEnum.Created;
                            }
                            context.TicketRePrints.Add(ticketRePrint);
                            context.SaveChanges();

                            List<TicketRePrintNumber> ticketAllocations = new List<TicketRePrintNumber>();
                            foreach (var number in model.TicketReprintNumbers)
                            {
                                var allocationNumber = context.TicketAllocationNumbers
                                    .FirstOrDefault(a => a.TicketAllocation.RaffleId == model.RaffleId && (int)a.Number == number.Number);
                                var ticket = new TicketRePrintNumber()
                                {
                                    TicketAllocationNumberId = allocationNumber.Id,
                                    TicketRePrintId = ticketRePrint.Id
                                };
                                ticketAllocations.Add(ticket);
                            }
                            context.TicketRePrintNumbers.AddRange(ticketAllocations);
                            context.SaveChanges();
                        }
                        else
                        {
                            List<TicketRePrintNumber> ticketAllocations = new List<TicketRePrintNumber>();
                            foreach (var number in model.TicketReprintNumbers)
                            {
                                var allocationNumber = context.TicketAllocationNumbers
                                    .FirstOrDefault(a => a.TicketAllocation.RaffleId == model.RaffleId && (int)a.Number == number.Number);
                                var ticket = new TicketRePrintNumber()
                                {
                                    TicketAllocationNumberId = allocationNumber.Id,
                                    TicketRePrintId = model.Id
                                };
                                ticketAllocations.Add(ticket);
                            }
                            context.TicketRePrintNumbers.AddRange(ticketAllocations);
                            context.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel ()
                        { 
                            Result = false, 
                            Message = e.Message 
                        };
                    }
                }
            }
            
            Utils.SaveLog(WebSecurity.CurrentUserName, model.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Reimpresión de Billetes", model);
            
            return new RequestResponseModel()
            {
                Result = true,
                Message = "Reimpresión guardada correctamente."
            };
        }

        internal RequestResponseModel Verify(TicketReprintModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    if (model.Id > 0)
                    {
                        var allocation = context.TicketAllocations.FirstOrDefault(a => a.Id == model.Id);
                        model.RaffleId = allocation.RaffleId;
                    }

                    var allNumber = context.TicketAllocationNumbers.AsEnumerable().Where(t =>
                        t.TicketAllocation.RaffleId == model.RaffleId).ToList();

                    string number = "";
                    foreach (var num in model.TicketReprintNumbers)
                    {
                        if (!allNumber.Where(d => (int)d.Number == num.Number && d.Printed == true).Any())
                        {
                            number += num + ", ";
                        }
                    }

                    if (number != "")
                    {
                        number = number.Substring(0, number.Length - 2);
                        return new RequestResponseModel()
                        { 
                            Result = false, 
                            Message = "Los numeros ( " + number + " ) no fueron impreso." 
                        };
                    }
                    else
                    {
                        return new RequestResponseModel ()
                        { 
                            Result = true, 
                            Message = "Nummero agregado correctamente" 
                        };
                    }
                }
            }
        }

        internal RequestResponseModel Delete(TicketReprintModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tm = context.Database.BeginTransaction())
                {
                    try
                    {
                        var reprint = context.TicketRePrints.FirstOrDefault(t => t.Id == model.Id);
                        if (reprint != null)
                        {
                            var reprintTikects = reprint.TicketRePrintNumbers.ToList();
                            context.TicketRePrintNumbers.RemoveRange(reprintTikects);
                            context.SaveChanges();

                            var workflows = context.Workflows.Where(w => w.ProcessId == model.Id).ToList();
                            if (workflows.Any())
                            {
                                context.Workflows.RemoveRange(workflows);
                                context.SaveChanges();
                            }

                            context.TicketRePrints.Remove(reprint);
                            context.SaveChanges();
                            tm.Commit();
                        }
                    }
                    catch (Exception e)
                    {
                        tm.Rollback();
                        return new RequestResponseModel (){ 
                            Result = false, 
                            Message = e.Message 
                        };
                    }
                }
            }

            return new RequestResponseModel (){ 
                Result = true ,
                Message = "Reimpresion borrada correctamente."
            };
        }

    }
}