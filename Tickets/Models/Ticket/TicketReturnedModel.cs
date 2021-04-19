using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Tickets.Models.Enums;
using WebMatrix.WebData;
using System.Text;
using Tickets.Models.Raffles;

namespace Tickets.Models.Ticket
{
    public class TicketReturnedModel
    {
        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        [JsonProperty(PropertyName = "raffleDesc")]
        public string RaffleDesc { get; set; }

        [JsonProperty(PropertyName = "returnedGroup")]
        public string ReturnedGroup { get; set; }

        [JsonProperty(PropertyName = "returnedSubGroups")]
        public string ReturnedSubGroup { get; set; }

        [JsonProperty(PropertyName = "returnedDate")]
        public DateTime ReturnedDate { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "fractionQuantity")]
        public int FractionQuantity { get; set; }

        [JsonProperty(PropertyName = "ticketReturnedNumbers")]
        public List<TicketReturnedNumberModel> TicketReturnedNumbers { get; set; }

        [JsonProperty(PropertyName = "statusId")]
        public int StatusId { get; set; }

        [JsonProperty(PropertyName = "numberCount")]
        public int NumberCount { get; set; }

        internal TicketReturnedModel ToObject(List<TicketReturn> ticketReturneds, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            //string group = Regex.Match(ticketReturneds.FirstOrDefault().ReturnedGroup, @"\d+").Value.ToString();
            var subGroups = "";
            ticketReturneds.GroupBy(r => r.ReturnedGroup).AsEnumerable().Select(r => Regex.Replace(r.FirstOrDefault().ReturnedGroup, @"[\d-]", string.Empty).ToString()).ToList().ForEach( r=> subGroups += r + ", ");
            var model = new TicketReturnedModel()
            {
                RaffleId = ticketReturneds.FirstOrDefault().RaffleId,
                RaffleDesc = ticketReturneds.FirstOrDefault().Raffle.Name,
                ReturnedGroup = ticketReturneds.FirstOrDefault().ReturnedGroup,
                ReturnedSubGroup = ticketReturneds.Count() == 1 ? ticketReturneds.FirstOrDefault().ReturnedGroup : ticketReturneds.Select(e => e.ReturnedGroup).Distinct().Aggregate((s, e) => s + ", " + e),
                ReturnedDate = ticketReturneds.FirstOrDefault().ReturnedDate,
                ClientId = ticketReturneds.FirstOrDefault().ClientId,
                ClientDesc = ticketReturneds.FirstOrDefault().Client.Name,
                FractionQuantity = ticketReturneds.Select( t=> t.FractionTo - t.FractionFrom + 1).Sum(),
                StatusId = ticketReturneds.FirstOrDefault().Statu,
                NumberCount = ticketReturneds.Count
            };
            if (hasNumber == true)
            {
                var ticketReturnedNumberModel = new TicketReturnedNumberModel();
                model.TicketReturnedNumbers = ticketReturneds.Select(t => ticketReturnedNumberModel.ToObject(t)).ToList();
            }
            return model;
        }

        internal RequestResponseModel GetList(int raffleId, int clientId = 0, int status = 0)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d =>
                    (d.RaffleId == raffleId)
                    && (d.ClientId == clientId || clientId == 0)
                    && (d.Statu == status || status == 0)
                ).ToList()
                .GroupBy(d => d.ReturnedGroup)
                .Select( r => this.ToObject(r.ToList()) )
                .ToList();
            return new RequestResponseModel (){
                Result = true,
                Object = returneds
            };
        }

        internal RequestResponseModel GetGroup(int group, int raffleId)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.AsEnumerable().Where(d => d.RaffleId == raffleId &&
                    int.Parse(Regex.Match(d.ReturnedGroup, @"\d+").Value) == group
                ).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = this.ToObject(returneds)
            };
        }

        internal RequestResponseModel GetSubGroup(string subGroup, int raffleId)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d => d.ReturnedGroup == subGroup && d.RaffleId == raffleId).ToList();
            return new RequestResponseModel()
            {
                Result = true,
                Object = this.ToObject(returneds)
            };
        }

        internal RequestResponseModel ValidateTicketReturned(TicketReturnedModel model)
        {
            int clientId = 0;
            string clientName = "";
            bool state = false;
            using (var context = new TicketsEntities())
            {
                
                StringBuilder sb = new StringBuilder();
                sb.Append("Los boletos: ");
                var ticketAllocationNumbers = context.TicketAllocationNumbers.Where(t => t.TicketAllocation.RaffleId == model.RaffleId).Select(a => new
                {
                    a.TicketAllocation.ClientId,
                    a.Number,
                    a.FractionFrom,
                    a.FractionTo,
                    a.Statu,
                    a.Id
                }).ToList();
                foreach (var item in model.TicketReturnedNumbers)
                {
                    var tan = ticketAllocationNumbers.FirstOrDefault( t => t.Number == item.NumberId);
                    if(tan != null)
                    {
                    
                            if(tan.ClientId != item.ClientId)
                            {
                                sb.Append(item.NumberId);
                                sb.Append(",");
                                state = true;
                            }
                    }
                }
                if(state)
                {
                    sb.Append(" no pertenecen al cliente ");
                    var client = context.Clients.Where(a => a.Id == model.ClientId).FirstOrDefault();
                    sb.Append(client != null ? client.Name : "");

                    return new RequestResponseModel()
                    {
                        Result = false,
                        Object = new
                        {
                            clientId,
                            clientName,
                        },
                        Message = sb.ToString()
                    };

                }
            

                using (var tx = context.Database.BeginTransaction())
                {
                    var raffle = context.Raffles.Where(p => p.Id == model.RaffleId).FirstOrDefault();

                    if (((raffle.Statu == (int)RaffleStatusEnum.Active
                        || raffle.Statu == (int)RaffleStatusEnum.Planned)
                        && raffle.EndReturnDate >= DateTime.Now)
                        || raffle.ReturnedOpens.Any(r => r.EndReturnedDate >= DateTime.Now))
                    {
                        foreach (var ticketReturn in model.TicketReturnedNumbers)
                        {
                            var allocationNumbers = ticketAllocationNumbers.Where(t => t.Number == ticketReturn.NumberId);

                            if (allocationNumbers.Any() == false)
                            {
                                return new RequestResponseModel (){ 
                                    Result = false, 
                                    Message = "El billete " + ticketReturn.NumberId + " no se ha asignado." 
                                };
                            }

                            clientId = ticketReturn.ClientId;
                            clientName = context.Clients.FirstOrDefault(r => r.Id == clientId).Name;

                            var messageList = new List<string>();

                            for (var fraction = ticketReturn.FractionFrom; fraction <= ticketReturn.FractionTo; fraction++)
                            {
                                if (!allocationNumbers.Where(a => fraction >= a.FractionFrom && fraction <= a.FractionTo).Any())
                                {
                                    messageList.Add("La fracción " + fraction + " del billete " + ticketReturn.NumberId + " no fue asignada.");
                                }
                            }
                            if (messageList.Count == 0)
                            {
                                foreach (var allocationNumber in allocationNumbers)
                                {
                                    if (allocationNumber.Statu == (int)TicketStatusEnum.Alloated)
                                    {
                                        for (var fraction = ticketReturn.FractionFrom; fraction <= ticketReturn.FractionTo; fraction++)
                                        {
                                            if (fraction >= allocationNumber.FractionFrom && fraction <= allocationNumber.FractionTo)
                                            {
                                                messageList.Add("La fracción " + fraction + " del billete " + allocationNumber.Number + " fue asignada pero no impresa.");
                                            }
                                        }
                                    }

                                    if (allocationNumber.Statu == (int)TicketStatusEnum.Printed)
                                    {
                                        for (var fraction = ticketReturn.FractionFrom; fraction <= ticketReturn.FractionTo; fraction++)
                                        {
                                            if (fraction >= allocationNumber.FractionFrom && fraction <= allocationNumber.FractionTo)
                                            {
                                                messageList.Add("La fracción " + fraction + " del billete " + allocationNumber.Number + " fue impresa pero no facturada.");
                                            }
                                        }
                                    }

                                    var invoiceFraction = (allocationNumber.FractionTo - allocationNumber.FractionFrom + 1);

                                    var fractionQuantity = (ticketReturn.FractionTo - ticketReturn.FractionFrom + 1);

                                    var returneds = context.TicketReturns.Where(r => r.TicketAllocationNimberId == allocationNumber.Id).ToList();
                                    for (var fraction = ticketReturn.FractionFrom; fraction <= ticketReturn.FractionTo; fraction++)
                                    {
                                        if (returneds.Where(r => fraction >= r.FractionFrom && fraction <= r.FractionTo).Any())
                                        {
                                            messageList.Add("La fracción " + fraction + " del billete " + allocationNumber.Number + " fue devuelta.");
                                        }
                                    }
                                }
                            }
                            if (messageList.Count > 0)
                            {
                                return new RequestResponseModel (){ 
                                    Result = false, 
                                    Message = "",
                                    Object = messageList 
                                };
                            }
                        }
                        return new RequestResponseModel(){ 
                            Result = true,
                            Object = new { 
                                clientId, 
                                clientName,
                            },
                            Message = "billete agregado correctamente!" 
                        };
                    }
                    else
                    {
                        return new RequestResponseModel (){ 
                            Result = false, 
                            Message = "La fecha de devolucion ya expiro." 
                        };
                    }
                }
            }
        }

        internal RequestResponseModel Save(TicketReturnedModel model)
        {
            bool notAllReturnet = false;
            StringBuilder sb = new StringBuilder();
            List<string> messageList = new List<string>();
            var ticketNumberList = new List<TicketReturn>();

            if (model.ReturnedGroup.Length > 10)
            {
                return new RequestResponseModel (){ 
                    Result = false, 
                    Message = "El campo Grupo contiene muchos caracteres." 
                };
            }
            
        

            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {

                        var clientId = model.TicketReturnedNumbers.Select(r => r.ClientId).FirstOrDefault();
                        var raffleData = (from r in context.Raffles
                            join p in context.Prospects on r.ProspectId equals p.Id
                            where r.Id == model.RaffleId
                            select new
                            {
                                MaxReturnTickets = p.MaxReturnTickets,
                                Statu = r.Statu,
                                EndReturnDate = r.EndReturnDate
                            }).FirstOrDefault();

                        var returnedOpens = context.ReturnedOpens.Where(r => 
                            r.RaffleId == model.RaffleId &&
                            r.EndReturnedDate >= DateTime.Now).Select(ro => new
                        {
                            ro.EndReturnedDate
                        }).ToList();

                        var ticketAllocationNumbers = (from tn in context.TicketAllocationNumbers
                                                      join a in context.TicketAllocations on tn.TicketAllocationId equals a.Id
                                                      where a.RaffleId == model.RaffleId
                                                        && a.ClientId == clientId
                                                        && a.Statu == (int)AllocationStatuEnum.Invoiced
                                                      select new{
                                                          tn.FractionTo,
                                                          tn.FractionFrom,
                                                          tn.Number,
                                                          tn.Id
                                                      }).ToList();

                        var clientDiscount = context.Clients.Where(c => c.Id == clientId).Select( a => a.Discount).FirstOrDefault();

                        var totalTickets = 0;
                        ticketAllocationNumbers.ForEach(t => totalTickets += (t.FractionTo - t.FractionFrom + 1));

                        var returnTickets = 0;
                        var returneds = (from r in context.TicketReturns
                                         where r.RaffleId == model.RaffleId
                                         select new
                                         {
                                             r.FractionTo,
                                             r.FractionFrom,
                                             r.ClientId,
                                             r.TicketAllocationNimberId
                                         }).ToList();
                        returneds.Where( r => r.ClientId == clientId).ToList().ForEach(a => returnTickets += (a.FractionTo - a.FractionFrom + 1));

                        model.TicketReturnedNumbers.ForEach(a => returnTickets += (a.FractionTo - a.FractionFrom + 1));


                        if (returnTickets >= (totalTickets * (raffleData.MaxReturnTickets / 100)))
                        {
                            return new RequestResponseModel(){ 
                                Result = false, 
                                Message = "No puedes devolver mas del " + raffleData.MaxReturnTickets + " % de los billetes asignados." 
                            };
                        }

                        if (((raffleData.Statu == (int)RaffleStatusEnum.Active
                            || raffleData.Statu == (int)RaffleStatusEnum.Planned)
                            && raffleData.EndReturnDate >= DateTime.Now)
                            || returnedOpens.Any())
                        {
                            sb.Append("Las siguientes fracciones se encuentran devueltas\n ");

                            foreach (var ticketReturn in model.TicketReturnedNumbers)
                            {
                                var allocationNumber = ticketAllocationNumbers.FirstOrDefault(t => t.Number == ticketReturn.NumberId );

                                var returnedTicket = new TicketReturn()
                                {
                                    RaffleId = model.RaffleId,
                                    ClientId = ticketReturn.ClientId,
                                    CreateDate = DateTime.Now,
                                    ReturnedDate = DateTime.Now,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    FractionFrom = ticketReturn.FractionFrom,
                                    FractionTo = ticketReturn.FractionTo,
                                    ReturnedGroup = model.ReturnedGroup,
                                    TicketAllocationNimberId = allocationNumber.Id,
                                    Statu = (int)TicketReturnedStatuEnum.Created,
                                    Discount = clientDiscount
                                };
                                var exists = returneds.Where(t => t.TicketAllocationNimberId == allocationNumber.Id && t.FractionFrom == ticketReturn.FractionFrom && t.FractionTo == t.FractionTo).FirstOrDefault();
                                if (exists == null)
                                {
                                    ticketNumberList.Add(returnedTicket);
                                }else
                                {
                                    sb.Append("desde "+returnedTicket.FractionFrom);
                                    sb.Append("hasta " + returnedTicket.FractionTo);
                                //    sb.Append("del billete " + returnedTicket.);
                                    sb.Append(",");
                                    notAllReturnet = true;
                                }
                                
                            }
                            if(ticketNumberList.Count> 0)
                            {
                                context.TicketReturns.AddRange(ticketNumberList);
                                context.SaveChanges();
                                tx.Commit();
                            }
                        }
                        else
                        {
                            return new RequestResponseModel(){ 
                                Result = false, 
                                Message = "La fecha de devolucion ya expiro."
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new RequestResponseModel(){ 
                            Result = false,
                            Message = e.Message 
                        };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Devolución de Billetes", model);
            string message = notAllReturnet ? sb.ToString() : "Devolución de Billetes Completada.";
            return new RequestResponseModel(){ 
                Result = true, 
                Message = message
            };
        }

        internal RequestResponseModel GetList(int raffleId, string group)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d =>
                    (d.RaffleId == raffleId)
                    && (d.ReturnedGroup == group )
                ).ToList()
                .GroupBy(d => d.ReturnedGroup)
                .Select(r => this.ToObject(r.ToList(), true))
                .ToList();
            return new RequestResponseModel()
            {
                Result = true,
                Object = returneds
            };
        }

        internal RequestResponseModel Delete(TicketReturnedModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var tm = context.Database.BeginTransaction())
                {
                    try
                    {
                        var returneds = context.TicketReturns.Where(n => n.ReturnedGroup == model.ReturnedGroup && n.RaffleId == model.RaffleId).ToList();

                        context.TicketReturns.RemoveRange(returneds);
                        context.SaveChanges();

                        tm.Commit();
                    }
                    catch
                    {
                        tm.Rollback();
                        return new RequestResponseModel(){ 
                            Result = false, 
                            Message = "No es posible borrar esta debolución!" 
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Borando Grupo de devoluciones", model);

            return new RequestResponseModel()
            { 
                Result = true, 
                Message = "Grupo de devolucion borrado correctamente!" 
            };
        }

        internal RequestResponseModel GetGroupListSelect(int raffleId, TicketReturnedStatuEnum ticketReturnedStatuEnum)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d =>(d.RaffleId == raffleId)).ToList()
                .GroupBy(d => d.ReturnedGroup)
                .Select(r => new
                {
                    value = r.FirstOrDefault().ReturnedGroup,
                    text = r.FirstOrDefault().ReturnedGroup
                })
                .ToList();
            return new RequestResponseModel()
            {
                Result = true,
                Object = returneds
            };
        }

        internal RequestResponseModel GeListtByClient(int raffleId, int clientId = 0, bool propertys = true)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d =>
                    (d.RaffleId == raffleId)
                    && (d.ClientId == clientId || clientId == 0)
                ).ToList()
                .GroupBy(d => d.ClientId)
                .Select(r => this.ToObject(r.ToList(), propertys))
                .ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = returneds
            };
        }

        //Arreglo para validación de devoluciones.
        
        internal RequestResponseModel GetListtByValidation(int raffleId, int clientId = 0, bool propertys = true)
        {
            var context = new TicketsEntities();

            var returneds = new
            {
                validates = context.TicketReturns.Where(d =>
                            (d.RaffleId == raffleId)
                            && (d.ClientId == clientId || clientId == 0)
                            && (d.Statu != (int)TicketReturnedStatuEnum.Created)
                            ).ToList()
                            .GroupBy(d => d.ClientId)
                            .Select(r => this.ToObject(r.ToList(), propertys))
                            .ToList(),

                nonValidates = context.TicketReturns.Where(d =>
                    (d.RaffleId == raffleId)
                    && (d.ClientId == clientId || clientId == 0)
                    && (d.Statu == (int)TicketReturnedStatuEnum.Created)
                    ).ToList()
                    .GroupBy(d => d.ClientId)
                    .Select(r => this.ToObject(r.ToList(), propertys))
                    .ToList()
            };

            return new RequestResponseModel()
            {
                Result = true,
                Object =  returneds
            };
        }

    }
}