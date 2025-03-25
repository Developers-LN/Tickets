using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tickets.Models.Enums;
using WebMatrix.WebData;

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

        [JsonProperty(PropertyName = "sequenceNumberRaffle")]
        public string SequenceNumberRaffle { get; set; }

        /*internal TicketReturnedModel ListaDevoluciones(List<TicketReturn> ticketReturneds, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            var subGroups = "";
            ticketReturneds.GroupBy(r => r.ReturnedGroup).AsEnumerable().Select(r => Regex.Replace(r.FirstOrDefault().ReturnedGroup, @"[\d-]", string.Empty).ToString()).ToList().ForEach(r => subGroups += r + ", ");
            var model = new TicketReturnedModel()
            {
                RaffleId = ticketReturneds.FirstOrDefault().RaffleId,
                RaffleDesc = ticketReturneds.FirstOrDefault().Raffle.Name,
                ReturnedGroup = ticketReturneds.FirstOrDefault().ReturnedGroup,
                ReturnedSubGroup = ticketReturneds.Count() == 1 ? ticketReturneds.FirstOrDefault().ReturnedGroup : ticketReturneds.Select(e => e.ReturnedGroup).Distinct().Aggregate((s, e) => s + ", " + e),
                ReturnedDate = ticketReturneds.FirstOrDefault().ReturnedDate,
                ClientId = ticketReturneds.FirstOrDefault().ClientId,
                ClientDesc = ticketReturneds.FirstOrDefault().Client.Name,
                FractionQuantity = ticketReturneds.Select(t => t.FractionTo - t.FractionFrom + 1).Sum(),
                StatusId = ticketReturneds.FirstOrDefault().Statu,
                NumberCount = ticketReturneds.Count
            };

            if (hasNumber == true)
            {
                var ticketReturnedNumberModel = new TicketReturnedNumberModel();
                model.TicketReturnedNumbers = ticketReturneds.Select(t => ticketReturnedNumberModel.ToObject(t)).ToList();
            }
            return model;
        }*/

        internal TicketReturnedModel ToObject(List<TicketReturn> ticketReturneds, bool hasNumber = false)
        {
            var context = new TicketsEntities();
            //string group = Regex.Match(ticketReturneds.FirstOrDefault().ReturnedGroup, @"\d+").Value.ToString();
            var subGroups = "";
            ticketReturneds.GroupBy(r => r.ReturnedGroup).AsEnumerable().Select(r => Regex.Replace(r.FirstOrDefault().ReturnedGroup, @"[\d-]", string.Empty).ToString()).ToList().ForEach(r => subGroups += r + ", ");
            var raffleData = ticketReturneds.FirstOrDefault().Raffle;
            var model = new TicketReturnedModel()
            {
                RaffleId = ticketReturneds.FirstOrDefault().RaffleId,
                SequenceNumberRaffle = raffleData.Symbol + raffleData.Separator + raffleData.SequenceNumber.Value,
                //RaffleDesc = ticketReturneds.FirstOrDefault().Raffle.Name,
                RaffleDesc = raffleData.Symbol + raffleData.Separator + raffleData.SequenceNumber + " " + raffleData.Name + " " + raffleData.DateSolteo.ToString("dd/MM/yyyy"),
                ReturnedGroup = ticketReturneds.FirstOrDefault().ReturnedGroup,
                ReturnedSubGroup = ticketReturneds.Count() == 1 ? ticketReturneds.FirstOrDefault().ReturnedGroup : ticketReturneds.Select(e => e.ReturnedGroup).Distinct().Aggregate((s, e) => s + ", " + e),
                ReturnedDate = ticketReturneds.FirstOrDefault().ReturnedDate,
                ClientId = ticketReturneds.FirstOrDefault().ClientId,
                ClientDesc = ticketReturneds.FirstOrDefault().Client.Name,
                FractionQuantity = ticketReturneds.Select(t => t.FractionTo - t.FractionFrom + 1).Sum(),
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

            var returneds = context.TicketReturns.Where(d => (d.RaffleId == raffleId) && (d.ClientId == clientId || clientId == 0) && (d.Statu == status || status == 0)
                ).ToList()
                .GroupBy(d => d.ReturnedGroup)
                .Select(r => this.ToObject(r.ToList()))
                .ToList();
            return new RequestResponseModel()
            {
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
            bool state = false;
            using (var context = new TicketsEntities())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Los boletos: ");

                var TicketNumber = model.TicketReturnedNumbers.Select(n => n.NumberId).FirstOrDefault();

                var allocationId = context.TicketAllocationNumbers
                    .Where(r => r.RaffleId == model.RaffleId && r.Number == TicketNumber)
                    .Select(t => t.TicketAllocationId).FirstOrDefault();

                var Client = context.Clients.Where(c => c.Id == model.ClientId).Select(c => new { clientId = c.Id, clientName = c.Name }).FirstOrDefault();

                var raffleData = context.Raffles.Where(f => f.Id == model.RaffleId).Select(s => new { Production = (s.Prospect.Production - 1).ToString().Length }).FirstOrDefault();

                var ticketAllocationNumbers = context.TicketAllocationNumbers
                    .Where(t => t.TicketAllocation.RaffleId == model.RaffleId && t.TicketAllocationId == allocationId).Select(a => new
                    {
                        a.Id,
                        a.TicketAllocation.ClientId,
                        a.Number,
                        a.FractionFrom,
                        a.FractionTo,
                        a.Statu
                    }).ToList();

                foreach (var item in model.TicketReturnedNumbers)
                {
                    var tan = ticketAllocationNumbers.FirstOrDefault(t => t.Number == item.NumberId);
                    if (tan != null)
                    {
                        if (tan.ClientId != item.ClientId)
                        {
                            sb.Append(item.NumberId.ToString().PadLeft((raffleData.Production), '0'));
                            sb.Append(",");
                            state = true;
                        }
                    }
                }
                if (state)
                {
                    sb.Append(" no pertenecen al cliente ");
                    var client = context.Clients.Where(a => a.Id == model.ClientId).FirstOrDefault();
                    sb.Append(client != null ? client.Name : "");

                    return new RequestResponseModel()
                    {
                        Result = false,
                        Object = new
                        {
                            Client.clientId,
                            Client.clientName,
                        },
                        Message = sb.ToString()
                    };
                }

                using (var tx = context.Database.BeginTransaction())
                {
                    var raffle = context.Raffles.Where(p => p.Id == model.RaffleId).FirstOrDefault();

                    if (((raffle.Statu == (int)RaffleStatusEnum.Active || raffle.Statu == (int)RaffleStatusEnum.Planned)
                        && raffle.EndReturnDate >= DateTime.Now)
                        || raffle.ReturnedOpens.Any(r => r.EndReturnedDate >= DateTime.Now))
                    {
                        foreach (var ticketReturn in model.TicketReturnedNumbers)
                        {
                            var allocationNumbers = ticketAllocationNumbers.Where(t => t.Number == ticketReturn.NumberId);

                            if (allocationNumbers.Any() == false)
                            {
                                return new RequestResponseModel()
                                {
                                    Result = false,
                                    Message = "El billete " + ticketReturn.NumberId + " no se ha asignado."
                                };
                            }

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

                                    if (context.Clients.FirstOrDefault(f => f.Id == model.ClientId).GroupId != (int)ClientGroupEnum.CajasOficinaPrincipal)
                                    {
                                        if (allocationNumber.Statu == (int)TicketStatusEnum.Printed)
                                        {
                                            for (var fraction = ticketReturn.FractionFrom; fraction <= ticketReturn.FractionTo; fraction++)
                                            {
                                                if (fraction >= allocationNumber.FractionFrom && fraction <= allocationNumber.FractionTo)
                                                {
                                                    messageList.Add("La fracción " + fraction + " del billete " + allocationNumber.Number + " fue impresa pero no fue entregado al cliente.");
                                                }
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
                                return new RequestResponseModel()
                                {
                                    Result = false,
                                    Message = "",
                                    Object = messageList
                                };
                            }
                        }

                        return new RequestResponseModel()
                        {
                            Result = true,
                            Object = new
                            {
                                Client.clientId,
                                Client.clientName,
                            },
                            Message = "billete agregado correctamente!"
                        };
                    }
                    else
                    {
                        return new RequestResponseModel()
                        {
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
            bool state = false;
            StringBuilder sb = new StringBuilder();
            List<string> messageList = new List<string>();
            var ticketNumberList = new List<TicketReturn>();

            if (model.ReturnedGroup.Length > 10)
            {
                return new RequestResponseModel()
                {
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
                        var fraccionMinima = 1;
                        var fraccionMaxima = 0;
                        var clientId = model.TicketReturnedNumbers.Select(r => r.ClientId).FirstOrDefault();

                        var raffleData = context.Raffles.Where(r => r.Id == model.RaffleId).Select(s => new
                        {
                            s.Prospect.MaxReturnTickets,
                            s.Statu,
                            s.EndReturnDate,
                            Production = (s.Prospect.Production - 1).ToString().Length,
                            MaxFraction = s.Prospect.LeafNumber * s.Prospect.LeafFraction,
                            returnedOpens = s.ReturnedOpens.Select(r => r.EndReturnedDate)
                        }).FirstOrDefault();

                        //Validacion de Fraccion Minima y Maxima
                        fraccionMaxima = raffleData.MaxFraction;

                        var ticketAllocationNumbers = context.TicketAllocationNumbers
                            .Where(t => t.RaffleId == model.RaffleId &&
                                  (t.TicketAllocation.Statu == (int)AllocationStatuEnum.Consigned ||
                                   t.TicketAllocation.Statu == (int)AllocationStatuEnum.Invoiced ||
                                   t.TicketAllocation.Statu == (int)AllocationStatuEnum.Printed ||
                                   t.TicketAllocation.Statu == (int)AllocationStatuEnum.Review ||
                                   t.TicketAllocation.Statu == (int)AllocationStatuEnum.Returned ||
                                   t.TicketAllocation.Statu == (int)AllocationStatuEnum.Delivered) &&
                                   t.TicketAllocation.ClientId == clientId).Select(t => new
                                   {
                                       t.Id,
                                       t.RaffleId,
                                       t.TicketAllocation.Statu,
                                       t.TicketAllocation.ClientId,
                                       t.FractionTo,
                                       t.FractionFrom,
                                       t.Number,
                                       t.TicketAllocation.Client.Name,
                                       InvoiceCondition = t.InvoiceTickets.Select(s => s.Invoice.Condition).FirstOrDefault(),
                                       Payment = t.InvoiceTickets.Select(s => s.Invoice.ReceiptPayments).Count(),
                                       clientDiscount = t.InvoiceTickets.Select(s => s.Invoice.Discount).FirstOrDefault()
                                   }).ToList();

                        var TicketsFromOtherClient = model.TicketReturnedNumbers.Select(s => s.NumberId).Where(w => !ticketAllocationNumbers.Select(s => s.Number).Contains(w)).ToList();

                        if (TicketsFromOtherClient.Any())
                        {
                            state = true;
                            var DataFromOtherClients = context.TicketAllocationNumbers.Where(w => w.RaffleId == model.RaffleId && TicketsFromOtherClient.Contains((int)w.Number)).Select(S => new
                            {
                                S.Number,
                                S.TicketAllocation.Client.Name
                            }).ToList();

                            var ClientGroup = DataFromOtherClients.GroupBy(g => g.Name).Select(s => s.Key).ToList();

                            foreach (var client in ClientGroup)
                            {
                                sb.Append(" Los billetes ");
                                foreach (var ticket in DataFromOtherClients.Where(w => w.Name == client))
                                {
                                    sb.Append(ticket.Number.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(", ");
                                }
                                sb.Append(" pertenecen al cliente ");
                                sb.Append(client);
                                sb.Append(".");
                                sb.AppendLine();
                            }
                        }
                        if (state)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = sb.ToString()
                            };
                        }

                        var TicketsGroupList = model.TicketReturnedNumbers.Select(s => s.NumberId).GroupBy(g => g).ToList();

                        foreach (var g in TicketsGroupList)
                        {
                            var TicketsRecords = model.TicketReturnedNumbers.Select(s => new { s.FractionFrom, s.FractionTo, s.NumberId }).Where(w => w.NumberId == g.Key).OrderBy(o => o.FractionFrom).ToArray();

                            for (int b = 0; (b + 1) < TicketsRecords.Length; b++)
                            {
                                var reference = TicketsRecords[b];
                                var test = TicketsRecords[b + 1].FractionFrom;

                                if (test >= reference.FractionFrom && test <= reference.FractionTo)
                                {
                                    state = true;
                                    sb.Append(TicketsRecords[b].NumberId.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(", ");
                                    b = TicketsRecords.Length;
                                }
                            }
                        }
                        if (state)
                        {
                            sb.Insert(0, " Los billetes ");
                            sb.Append(" presentan superposición de fracciones.");
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = sb.ToString()
                            };
                        }

                        if (ticketAllocationNumbers.Any(a => a.InvoiceCondition == (int)InvoiceConditionEnum.Counted) || ticketAllocationNumbers.Any(a => a.Payment != 0))
                        {
                            state = true;
                            var countedTickets = ticketAllocationNumbers.Where(w => w.InvoiceCondition == (int)InvoiceConditionEnum.Counted && model.TicketReturnedNumbers.Select(s => s.NumberId).Contains((int)w.Number)).ToList();
                            var payedTickets = ticketAllocationNumbers.Where(w => w.Payment != 0 && model.TicketReturnedNumbers.Select(s => s.NumberId).Contains((int)w.Number)).ToList();

                            if (countedTickets.Any())
                            {
                                sb.Append(" Los billetes ");
                                foreach (var ticket in countedTickets)
                                {
                                    sb.Append(ticket.Number.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(", ");
                                }
                                sb.Append(" y estan al contado.");
                                sb.AppendLine();
                            }
                            if (payedTickets.Any())
                            {
                                sb.Append(" Los billetes ");
                                foreach (var ticket in payedTickets)
                                {
                                    sb.Append(ticket.Number.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(", ");
                                }
                                sb.Append(" y estan pagos.");
                                sb.AppendLine();
                            }
                        }
                        if (state)
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = sb.ToString()
                            };
                        }

                        var totalTickets = 0;
                        ticketAllocationNumbers.ForEach(t => totalTickets += (t.FractionTo - t.FractionFrom + 1));

                        var returnTickets = 0;

                        var returneds = context.TicketReturns.Where(r => r.RaffleId == model.RaffleId && r.ClientId == clientId).Select(r => new { r.FractionTo, r.FractionFrom, r.ClientId, r.TicketAllocationNimberId }).ToList();

                        returneds.Where(r => r.ClientId == clientId).ToList().ForEach(a => returnTickets += (a.FractionTo - a.FractionFrom + 1));

                        model.TicketReturnedNumbers.ForEach(a => returnTickets += (a.FractionTo - a.FractionFrom + 1));

                        if (returnTickets > (totalTickets * (raffleData.MaxReturnTickets / 100)))
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "No puedes devolver mas del " + raffleData.MaxReturnTickets + " % de los billetes asignados."
                            };
                        }

                        if (((raffleData.Statu == (int)RaffleStatusEnum.Active || raffleData.Statu == (int)RaffleStatusEnum.Planned) && raffleData.EndReturnDate >= DateTime.Now) || raffleData.returnedOpens.Any())
                        {
                            sb.Append("No se pudo realizar el guardado debido a una superposición de fracciones. <br/>");
                            sb.Append("Resultados del conflicto: <br/>");

                            foreach (var ticketReturn in model.TicketReturnedNumbers)
                            {
                                var allocationNumber = ticketAllocationNumbers.FirstOrDefault(t => t.Number == ticketReturn.NumberId);

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
                                    Discount = ticketAllocationNumbers.Where(n => n.Number == ticketReturn.NumberId).Select(t => t.clientDiscount).FirstOrDefault()
                                };

                                //Validacion fraccion minima
                                if (ticketReturn.FractionFrom < fraccionMinima || ticketReturn.FractionFrom > fraccionMaxima)
                                {
                                    return new RequestResponseModel()
                                    {
                                        Result = false,
                                        Message = "La fracción " + ticketReturn.FractionFrom + " no es valida."
                                    };
                                }

                                //Validacion fraccion maxima
                                if (ticketReturn.FractionTo < fraccionMinima || ticketReturn.FractionTo > fraccionMaxima)
                                {
                                    return new RequestResponseModel()
                                    {
                                        Result = false,
                                        Message = "La fracción " + ticketReturn.FractionFrom + " no es valida."
                                    };
                                }

                                //Validacion si existe una fraccion ya devuelta
                                var exists = returneds.Where(t => t.TicketAllocationNimberId == allocationNumber.Id && (t.FractionFrom <= ticketReturn.FractionTo && t.FractionTo >= ticketReturn.FractionFrom)).FirstOrDefault();

                                if (exists == null)
                                {
                                    ticketNumberList.Add(returnedTicket);
                                }
                                else
                                {
                                    state = true;
                                    sb.Append("Ya existe el Billete " + allocationNumber.Number.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(" desde " + exists.FractionFrom);
                                    sb.Append(" hasta " + exists.FractionTo);
                                    sb.Append("<br/>");
                                    sb.Append("No se puede insertar el Billete " + allocationNumber.Number.ToString().PadLeft((raffleData.Production), '0'));
                                    sb.Append(" desde " + returnedTicket.FractionFrom);
                                    sb.Append(" hasta " + returnedTicket.FractionTo);
                                    sb.Append("<br/>");
                                    notAllReturnet = true;
                                }
                            }
                            if (state)
                            {
                                return new RequestResponseModel()
                                {
                                    Result = false,
                                    Message = sb.ToString()
                                };
                            }
                            else
                            {
                                context.TicketReturns.AddRange(ticketNumberList);
                                context.SaveChanges();
                                tx.Commit();
                            }
                        }
                        else
                        {
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "La fecha de devolucion ya expiro."
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        context.Dispose();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.InnerException.InnerException.Message.ToString() == null ? e.Message : e.InnerException.InnerException.Message.ToString()
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Devolución de Billetes", model);
            string message = notAllReturnet ? sb.ToString() : "Devolución de Billetes Completada.";
            return new RequestResponseModel()
            {
                Result = true,
                Message = message
            };
        }

        internal RequestResponseModel GetList(int raffleId, string group)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d => (d.RaffleId == raffleId) && (d.ReturnedGroup == group)
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

        internal RequestResponseModel GetListByClient(int raffleId, int clientId, string group)
        {
            var context = new TicketsEntities();

            var returneds = context.TicketReturns.Where(d => (d.RaffleId == raffleId) && (d.ReturnedGroup == group) && (d.ClientId == clientId)
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
                        return new RequestResponseModel()
                        {
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

            var returneds = context.TicketReturns.Where(d => (d.RaffleId == raffleId)).ToList()
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

            //var t = context.TicketReturns.Where(r => r.RaffleId == raffleId && (r.ClientId == clientId || clientId == 0)).ToList().GroupBy(r => r.ClientId);

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
                Object = returneds
            };
        }
    }
}
