using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketAllocationNumberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "number")]
        public long Number { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "invoiced")]
        public bool Invoiced { get; set; }

        [JsonProperty(PropertyName = "printed")]
        public bool Printed { get; set; }

        [JsonProperty(PropertyName = "fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty(PropertyName = "fractionTo")]
        public int FractionTo { get; set; }

        [JsonProperty(PropertyName = "controlNumber")]
        public string ControlNumber { get; set; }

        [JsonProperty(PropertyName = "ticketAllocationId")]
        public int TicketAllocationId { get; set; }

        [JsonProperty(PropertyName = "sequenceNumberTicketAllocation")]
        public int? SequenceNumberTicketAllocation {  get; set; }

        [JsonProperty(PropertyName = "serie")]
        public List<TicketAllocationSerieModel> Serie { get; set; }

        internal TicketAllocationNumberModel ToObject(TicketAllocationNumber model)
        {
            var number = new TicketAllocationNumberModel()
            {
                Id = model.Id,
                ControlNumber = model.ControlNumber,
                FractionFrom = model.FractionFrom,
                FractionTo = model.FractionTo,
                Invoiced = model.Invoiced,
                Number = model.Number,
                Printed = model.Printed,
                Statu = model.Statu,
                TicketAllocationId = model.TicketAllocationId,
                SequenceNumberTicketAllocation = model.TicketAllocation.SequenceNumber.Value,
                Serie = new List<TicketAllocationSerieModel>()
            };

            return number;
        }

        internal TicketAllocationNumberModel ToObjectDelete(TicketAllocationNumber_Delete model)
        {
            var number = new TicketAllocationNumberModel()
            {
                Id = model.Id,
                ControlNumber = model.ControlNumber,
                FractionFrom = model.FractionFrom,
                FractionTo = model.FractionTo,
                Invoiced = model.Invoiced,
                Number = model.Number,
                Printed = model.Printed,
                Statu = model.Statu,
                TicketAllocationId = model.TicketAllocationId,
                SequenceNumberTicketAllocation = model.TicketAllocation.SequenceNumber.Value,
                Serie = new List<TicketAllocationSerieModel>()
            };

            return number;
        }

        internal RequestResponseModel DeleteAllocationNumber(TicketAllocationNumberModel model)
        {
            var context = new TicketsEntities();

            using (var Trans = context.Database.BeginTransaction())
            {
                try
                {
                    var number = context.TicketAllocationNumbers.FirstOrDefault(n => n.Id == model.Id);

                    if (number == null)
                    {
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El numero no fue encontrado!"
                        };
                    }

                    context.TicketAllocationNumbers.Remove(number);
                    context.SaveChanges();

                    Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Borando Asignación de Billetes", model);

                    Trans.Commit();

                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "El Numero fue borrado correctamente!"
                    };
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "El Numero fue borrado correctamente!"
                    };
                }
            }
        }

        internal RequestResponseModel UnConsignateNumber(TicketAllocationNumberModel model)
        {
            var context = new TicketsEntities();

            using (var Trans = context.Database.BeginTransaction())
            {
                try
                {
                    var number = context.TicketAllocationNumbers.FirstOrDefault(n => n.Id == model.Id);
                    var Raffle = number.RaffleId;
                    var generalAllocation = context.TicketAllocations.Where(w => w.RaffleId == Raffle && w.ClientId == (int)GeneralClientEnum.CajaGeneral)
                        .Select(s => s.Id).FirstOrDefault();

                    if (number == null)
                    {
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El numero no fue encontrado!"
                        };
                    }

                    if (generalAllocation == 0)
                    {
                        var ticketAllocation = new TicketAllocation()
                        {
                            ClientId = (int)GeneralClientEnum.CajaGeneral,
                            RaffleId = Raffle.Value,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now,
                            Type = (int)AllocationTypeEnum.Tickets,
                            Statu = (int)AllocationStatuEnum.Printed,
                            Agente = null
                        };
                        context.TicketAllocations.Add(ticketAllocation);
                        context.SaveChanges();

                        generalAllocation = ticketAllocation.Id;
                    }

                    number.TicketAllocationId = generalAllocation;

                    context.SaveChanges();

                    Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Borando Asignación de Billetes", model);

                    Trans.Commit();

                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "El Numero fue eliminado de la consignacion correctamente!"
                    };
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "Ocurrio un error al intentar eliminar el billete"
                    };
                }
            }
        }

        internal RequestResponseModel RemoveAllocation(int id)
        {
            var context = new TicketsEntities();

            using (var Trans = context.Database.BeginTransaction())
            {
                try
                {
                    var allocation = context.TicketAllocations.FirstOrDefault(n => n.Id == id && n.Statu == (int)AllocationStatuEnum.Created);

                    if (allocation == null)
                    {
                        Trans.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El numero de asignación no fue encontrado!"
                        };
                    }

                    var allocationNumber = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == id);

                    if (allocation.Client.GroupId != (int)ClientGroupEnum.DistribuidorXML || allocation.Client.GroupId != (int)ClientGroupEnum.DistribuidorElectronico)
                    {
                        if (allocationNumber.Any(a => a.Printed == true))
                        {
                            Trans.Rollback();
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "Hay billetes impresos!"
                            };
                        }
                    }

                    List<TicketAllocationNumber_Delete> ticketAllocations_delete = new List<TicketAllocationNumber_Delete>();

                    foreach (var number in allocationNumber)
                    {
                        var ticket = new TicketAllocationNumber_Delete()
                        {
                            Id_Origin = number.Id,
                            Number = number.Number,
                            Printed = number.Printed,
                            ControlNumber = number.ControlNumber,
                            FractionFrom = number.FractionFrom,
                            FractionTo = number.FractionTo,
                            RaffleId = number.RaffleId,
                            TicketAllocationId = number.TicketAllocationId,
                            CreateUser = number.CreateUser,
                            CreateDate = number.CreateDate,
                            Statu = number.Statu
                        };
                        ticketAllocations_delete.Add(ticket);
                    }
                    context.TicketAllocationNumber_Delete.AddRange(ticketAllocations_delete);
                    context.SaveChanges();

                    context.TicketAllocationNumbers.RemoveRange(allocation.TicketAllocationNumbers);
                    allocation.Statu = (int)AllocationStatuEnum.Deleted;
                    context.SaveChanges();

                    Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Asignación eliminada", id);

                    Trans.Commit();

                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "La asingación fue eliminada correctamente!"
                    };
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return new RequestResponseModel()
                    {
                        Result = false,
                        Message = "Error al intentar realizar la eliminación de la asignación!"
                    };
                }
            }
        }

        internal RequestResponseModel ConsignateAllocation(int id)
        {
            var context = new TicketsEntities();

            using (var Trans = context.Database.BeginTransaction())
            {
                try
                {
                    var allocation = context.TicketAllocations.FirstOrDefault(n => n.Id == id);

                    if (allocation == null)
                    {
                        Trans.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El numero de asignación no fue encontrado!"
                        };
                    }

                    var expiredInvoices = (from i in context.Invoices
                                           join it in context.InvoiceTickets on i.Id equals it.InvoiceId
                                           where i.ClientId == allocation.ClientId
                                             && i.Condition == (int)InvoiceConditionEnum.Credit
                                             && i.PaymentStatu == (int)InvoicePaymentStatuEnum.Pendient
                                           select new
                                           {
                                               i.ClientId,
                                               i.Condition,
                                               i.Id,
                                               i.PaymentStatu,
                                               i.InvoiceDate,
                                               i.InvoiceExpredDay,
                                               it.Quantity,
                                               it.PricePerFraction
                                           }).AsEnumerable().GroupBy(i => i.Id).Select(ig => new
                                           {
                                               ig.FirstOrDefault().ClientId,
                                               ig.FirstOrDefault().Condition,
                                               ig.FirstOrDefault().Id,
                                               ig.FirstOrDefault().PaymentStatu,
                                               ig.FirstOrDefault().InvoiceDate,
                                               ig.FirstOrDefault().InvoiceExpredDay,
                                               InvoiceTickets = ig.Select(it => new
                                               {
                                                   it.Quantity,
                                                   it.PricePerFraction
                                               }).ToList()
                                           }).ToList();

                    if (expiredInvoices.Where(i => DateTime.Now > i.InvoiceDate.AddDays(i.InvoiceExpredDay ?? 45)).ToList().Count > 0)
                    {
                        Trans.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El cliente tiene facturas pendiente."
                        };
                    }

                    var allocationNumber = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == id);

                    var LastAllocations = context.TicketAllocations.Where(w => w.ClientId == allocation.ClientId
                                                                          && w.RaffleId == allocation.RaffleId
                                                                          && w.Statu == (int)AllocationStatuEnum.Consigned);

                    var CashAdvances = context.NoteCredits.Where(w => w.ClientId == allocation.ClientId
                                                                 && w.TypeNote == (int)NoteCreditEnum.CashAdvance
                                                                 && w.RaffleId == allocation.RaffleId
                                                                 && w.Statu == (int)GeneralStatusEnum.Active
                                                                 && w.TotalRest > 0);
                    var TotalEnConsignacion = 0.0m;
                    var totalCashAdvances = 0.0m;
                    //var TotalEnFacturas = 0.0m;
                    var TicketPrice = context.Prospect_Price.Where(w => w.ProspectId == allocation.Raffle.ProspectId && w.PriceId == allocation.Client.PriceId).FirstOrDefault().TicketPrice;

                    CashAdvances.ToList().ForEach(f => totalCashAdvances += f.TotalRest);

                    LastAllocations.ToList().ForEach(f => TotalEnConsignacion += f.TicketAllocationNumbers.Sum(s => ((s.FractionTo - s.FractionFrom) + 1) * TicketPrice));

                    if ((((TotalEnConsignacion /*+ TotalEnFacturas*/) - ((TotalEnConsignacion * allocation.Client.Discount) / 100)) - totalCashAdvances) > allocation.Client.CreditLimit)
                    {
                        Trans.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El cliente ha agotado su limite de credito!"
                        };
                    }

                    if (allocation.Client.GroupId != (int)ClientGroupEnum.DistribuidorXML || allocation.Client.GroupId != (int)ClientGroupEnum.DistribuidorElectronico)
                    {
                        if (allocationNumber.Any(a => a.Printed == false))
                        {
                            Trans.Rollback();
                            return new RequestResponseModel()
                            {
                                Result = false,
                                Message = "Hay billetes sin imprimir!"
                            };
                        }
                    }

                    var CurrentUser = WebSecurity.CurrentUserId;

                    allocationNumber.ToList().ForEach(f =>
                    {
                        f.UserConsigned = CurrentUser;
                        f.Consignated = true;
                        f.Statu = (int)TicketStatusEnum.Delivered;
                    });

                    allocation.Statu = (int)AllocationStatuEnum.Consigned;

                    context.SaveChanges();

                    Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Consignado de asignación", id);

                    Trans.Commit();

                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "La asingación fue consignada correctamente!"
                    };
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return new RequestResponseModel()
                    {
                        Result = false,
                        Message = "Error al intentar realizar la consignacion!"
                    };
                }
            }
        }

        internal RequestResponseModel DeliverAllocation(int id)
        {
            var context = new TicketsEntities();

            using (var Trans = context.Database.BeginTransaction())
            {
                try
                {
                    var allocation = context.TicketAllocations.FirstOrDefault(n => n.Id == id);

                    if (allocation == null)
                    {
                        Trans.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "El numero de asignación no fue encontrado!"
                        };
                    }

                    allocation.Statu = (int)AllocationStatuEnum.Delivered;

                    context.SaveChanges();

                    Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Entrega de asignación", id);

                    Trans.Commit();

                    return new RequestResponseModel()
                    {
                        Result = true,
                        Message = "La asingación fue entregada correctamente!"
                    };
                }
                catch (Exception)
                {
                    Trans.Rollback();
                    return new RequestResponseModel()
                    {
                        Result = false,
                        Message = "Error al intentar realizar la entrega de la asignación!"
                    };
                }
            }
        }

        internal RequestResponseModel AwardNumberDetails(int number, int raffleId, int fractionFrom, int fractionTo)
        {
            var context = new TicketsEntities();
            var n = context.TicketAllocationNumbers.Where(tn => tn.TicketAllocation.RaffleId == raffleId &&
                                                          tn.Number == number &&
                                                          tn.TicketType == (int)TicketsTypeEnum.AvailableTicket).FirstOrDefault();
            if (n == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "El numero no fue encontrado!"
                };
            }

            var awards = context.RaffleAwards.Where(r => r.RaffleId == n.TicketAllocation.RaffleId &&
                                                    r.ControlNumber == n.Number &&
                                                    r.Award.TypesAward.Creation != (int)TypesAwardCreationEnum.SameAwardDerived).ToList();
            bool wfrac = true;
            var fFrom = fractionFrom;
            var fTo = fractionTo;
            foreach (var a in awards.Where(w => w.Award.ByFraction == (int)ByFractionEnum.S))
            {
                for (int i = fFrom; i <= fTo; i++)
                {
                    if (i == a.Fraction)
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
                awards.RemoveAll(w => w.Award.ByFraction == (int)ByFractionEnum.S);
            }

            var numberDetails = awards.Select(s => new
            {
                AwardNumber = s.ControlNumber,
                FractionFrom = fFrom,
                FractionTo = fTo,
                Fractions = s.Award.ByFraction == (int)ByFractionEnum.S ? 1 : fTo - fFrom + 1,
                AwardName = s.Award.ByFraction == (int)ByFractionEnum.S ? s.Award.Name + "- NO." + s.Fraction : s.Award.Name,
                AwardValue = s.Award.ByFraction == (int)ByFractionEnum.S ? s.Award.Value : s.Award.Value / (n.TicketAllocation.Raffle.Prospect.LeafNumber * n.TicketAllocation.Raffle.Prospect.LeafFraction),
                TotalValue = s.Award.ByFraction == (int)ByFractionEnum.S ? 1 * s.Award.Value : (fTo - fFrom + 1) * (s.Award.Value / (n.TicketAllocation.Raffle.Prospect.LeafNumber * n.TicketAllocation.Raffle.Prospect.LeafFraction)),
                s.Id
            }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = numberDetails
            };
        }

        internal RequestResponseModel AwardSellerNumberDetails(int number, int raffleId, int fractionFrom, int fractionTo)
        {
            var context = new TicketsEntities();
            var n = context.TicketAllocationNumbers.Where(tn => tn.TicketAllocation.RaffleId == raffleId &&
                                                          tn.Number == number &&
                                                          tn.TicketType == (int)TicketsTypeEnum.AvailableTicket).FirstOrDefault();
            if (n == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "El numero no fue encontrado!"
                };
            }

            var awards = context.RaffleAwards.Where(r => r.RaffleId == n.TicketAllocation.RaffleId &&
                                                    r.ControlNumber == n.Number &&
                                                    r.Award.TypesAward.Creation == (int)TypesAwardCreationEnum.SameAwardDerived).ToList();

            bool wfrac = true;
            var fFrom = fractionFrom;
            var fTo = fractionTo;
            foreach (var a in awards.Where(w => w.Award.ByFraction == (int)ByFractionEnum.S))
            {
                for (int i = fFrom; i <= fTo; i++)
                {
                    if (i == a.Fraction)
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
                awards.RemoveAll(w => w.Award.ByFraction == (int)ByFractionEnum.S);
            }

            var numberDetails = awards.Select(s => new
            {
                AwardNumber = s.ControlNumber,
                FractionFrom = fFrom,
                FractionTo = fTo,
                Fractions = s.Award.ByFraction == (int)ByFractionEnum.S ? 1 : fTo - fFrom + 1,
                AwardName = s.Award.ByFraction == (int)ByFractionEnum.S ? s.Award.Name + "- NO." + s.Fraction : s.Award.Name,
                AwardValue = s.Award.ByFraction == (int)ByFractionEnum.S ? s.Award.Value : s.Award.Value / (n.TicketAllocation.Raffle.Prospect.LeafNumber * n.TicketAllocation.Raffle.Prospect.LeafFraction),
                TotalValue = s.Award.ByFraction == (int)ByFractionEnum.S ? 1 * s.Award.Value : (fTo - fFrom + 1) * (s.Award.Value / (n.TicketAllocation.Raffle.Prospect.LeafNumber * n.TicketAllocation.Raffle.Prospect.LeafFraction)),
                s.Id
            }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = numberDetails
            };
        }

        internal RequestResponseModel GetTicketNumberDetails(int numberId)
        {
            var context = new TicketsEntities();
            var n = context.TicketAllocationNumbers.Where(tn => tn.Id == numberId).FirstOrDefault();
            if (n == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "El numero no fue encontrado!"
                };
            }

            var awards = context.RaffleAwards.Where(r => r.RaffleId == n.TicketAllocation.RaffleId && r.ControlNumber == n.Number).ToList();
            var numberDetails = new
            {
                n.Id,
                n.TicketAllocationId,
                SequenceNumberTicketAllocation = n.TicketAllocation.SequenceNumber,
                n.ControlNumber,
                n.Invoiced,
                MaxFaction = n.TicketAllocation.Raffle.Prospect.LeafFraction * n.TicketAllocation.Raffle.Prospect.LeafNumber,
                n.Number,
                n.TicketAllocation.Raffle.Prospect.Production,
                n.Printed,
                n.Statu,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == n.Statu).NameDetail,
                Caducado = n.TicketAllocation.Raffle.DueRaffleDate < DateTime.Now ? true : false,
                //Caducado = n.TicketAllocation.Raffle.DateSolteo.AddDays(90) < DateTime.Now ? true : false,
                Allocation = new
                {
                    n.TicketAllocation.Id,
                    n.FractionFrom,
                    n.FractionTo,
                    n.TicketAllocation.ClientId,
                    ClientDesc = context.Clients.FirstOrDefault(c => c.Id == n.TicketAllocation.ClientId).Name,
                    n.TicketAllocation.RaffleId,
                    SequenceNumberTicketAllocation = n.TicketAllocation.SequenceNumber,
                    SequenceNumberRaffle = n.TicketAllocation.Raffle.SequenceNumber,
                    //RaffleDesc = context.Raffles.FirstOrDefault(c => c.Id == n.TicketAllocation.RaffleId).Name,
                    RaffleNomenclature = n.TicketAllocation.Raffle.Symbol + n.TicketAllocation.Raffle.Separator + n.TicketAllocation.Raffle.SequenceNumber,
                    RaffleDesc = n.TicketAllocation.Raffle.Symbol + n.TicketAllocation.Raffle.Separator + n.TicketAllocation.Raffle.SequenceNumber + " " + n.TicketAllocation.Raffle.Name + " " + n.TicketAllocation.Raffle.DateSolteo.ToShortDateString(),
                    Cedula = n.ElectronicTicketSales.Any(a => a.TicketAllocationNimberId == n.Id) ? n.ElectronicTicketSales.FirstOrDefault().Cedula : null,
                    Telefono = n.ElectronicTicketSales.Any(a => a.TicketAllocationNimberId == n.Id) ? n.ElectronicTicketSales.FirstOrDefault().PhoneNumber : null
                },
                Transactions = GetNumberTransactions(n),
                returnes = context.TicketReturns.Where(r => r.RaffleId == n.TicketAllocation.RaffleId && r.TicketAllocationNumber.Number == n.Number).Select(r => new
                {
                    r.FractionFrom,
                    r.FractionTo
                }),
                awards = awards.Select(ra => new
                {
                    FractionFrom = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionFrom,
                    FractionTo = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Fraction : n.FractionTo,
                    AwardName = ra.Award.Name,
                    AwardValue = ra.Award.ByFraction == (int)ByFractionEnum.S ? ra.Award.Value : ra.Award.Value / (n.TicketAllocation.Raffle.Prospect.LeafNumber * n.TicketAllocation.Raffle.Prospect.LeafFraction),
                    ra.Id
                }).ToList(),
                identifyNumbersMinor = context.IdentifyNumbers
                .Where(i => i.IdentifyBach.RaffleId == n.TicketAllocation.RaffleId && i.IdentifyBach.Type == (int)IdentifyBachTypeEnum.Menor)
                .AsEnumerable().Join(awards.Where(a => a.Award.TypesAwardId != (int)AwardTypeEnum.Mayors && a.Award.TypesAwardId != (int)AwardTypeEnum.WinFraction).AsEnumerable(), i => i.TicketAllocationNumber.Number, a => a.ControlNumber, (i, a) => new
                {
                    i.IdentifyBachId,
                    i.IdentifyBach.SequenceNumber,
                    i.FractionTo,
                    i.FractionFrom,
                    AwardName = a.Award.Name,
                    AwardValue = a.Award.Value / (a.Raffle.Prospect.LeafNumber * a.Raffle.Prospect.LeafFraction),
                    IsPayed = Utils.IdentifyBachIsPayedMinor(i.IdentifyBach, awards)
                }).ToList(),
                identifyNumbersMayor = context.IdentifyNumbers
                .Where(i => i.IdentifyBach.RaffleId == n.TicketAllocation.RaffleId && i.IdentifyBach.Type == (int)IdentifyBachTypeEnum.Mayor)
                .AsEnumerable().Join(awards
                .Where(a => a.Award.TypesAwardId == (int)AwardTypeEnum.Mayors || a.Award.TypesAwardId == (int)AwardTypeEnum.WinFraction).AsEnumerable(), i => i.TicketAllocationNumber.Number, a => a.ControlNumber, (i, a) => new
                {
                    i.IdentifyBachId,
                    i.IdentifyBach.SequenceNumber,
                    FractionTo = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionTo,
                    FractionFrom = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionFrom,
                    AwardName = a.Award.Name,
                    AwardValue = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Award.Value : a.Award.Value / (a.Raffle.Prospect.LeafNumber * a.Raffle.Prospect.LeafFraction),
                    IsPayed = Utils.IdentifyBachIsPayedMinor(i.IdentifyBach, awards)
                }).ToList()
            };

            return new RequestResponseModel()
            {
                Result = true,
                Object = numberDetails
            };
        }

        private List<NumberTransactionModel> GetNumberTransactions(TicketAllocationNumber n)
        {
            List<NumberTransactionModel> transactions = new List<NumberTransactionModel>();
            using (var context = new TicketsEntities())
            {
                var allocations = context.TicketAllocationNumbers.Where(a => a.Number == n.Number && a.TicketAllocation.RaffleId == n.TicketAllocation.RaffleId).ToList();
                allocations.ForEach(
                    tn =>
                    {
                        transactions.Add(new NumberTransactionModel
                        {
                            Id = tn.TicketAllocation.Id,
                            Description = "Asignación",
                            ClientDesc = tn.TicketAllocation.ClientId + " - " + tn.TicketAllocation.Client.Name,
                            Date = tn.TicketAllocation.CreateDate.ToUnixTime(),
                            FractionFrom = tn.FractionFrom,
                            UserDesc = tn.TicketAllocation.User.Name,
                            FractionTo = tn.FractionTo,
                            Group = "No hay datos"
                        });
                        if (n.Printed == true)
                        {
                            transactions.Add(new NumberTransactionModel
                            {
                                Id = tn.TicketAllocation.Id,
                                Description = "Impresión",
                                ClientDesc = tn.TicketAllocation.ClientId + " - " + tn.TicketAllocation.Client.Name,
                                Date = Convert.ToDateTime(tn.PrintedDate).ToUnixTime(),
                                FractionFrom = tn.FractionFrom,
                                FractionTo = tn.FractionTo,
                                Group = "No hay datos",
                                UserDesc = tn.TicketAllocation.User.Name,
                            });
                        }
                    });
                var tickets = context.TicketAllocationNumbers.Where(w => w.Number == n.Number && w.RaffleId == n.RaffleId && w.TicketType == (int)TicketsTypeEnum.AvailableTicket).Select(s => s.Id).ToList();
                context.InvoiceTickets.Where(a => tickets.Contains(a.TicketNumberAllocationId) && a.Invoice.RaffleId == n.TicketAllocation.RaffleId).ToList().ForEach(
                    tn => transactions.Add(new NumberTransactionModel
                    {
                        Description = "Facturación",
                        ClientDesc = tn.Invoice.ClientId + " - " + tn.Invoice.Client.Name,
                        Date = tn.Invoice.CreateDate.ToUnixTime(),
                        FractionFrom = n.FractionFrom,
                        FractionTo = n.FractionTo,
                        Group = "No hay datos",
                        UserDesc = tn.Invoice.User.Name,
                    }));
                context.TicketReturns.Where(a => a.TicketAllocationNimberId == n.Id).ToList().ForEach(
                    tn => transactions.Add(new NumberTransactionModel
                    {
                        Description = "Devolución",
                        ClientDesc = tn.ClientId + " - " + tn.Client.Name,
                        Date = tn.CreateDate.ToUnixTime(),
                        FractionFrom = tn.FractionFrom,
                        FractionTo = tn.FractionTo,
                        Group = tn.ReturnedGroup,
                        UserDesc = tn.User.Name,
                    }));
                context.TicketAllocationNumbers.Where(w => w.Number == n.Number && w.RaffleId == n.RaffleId && w.TicketType == (int)TicketsTypeEnum.AvailableTicket && n.Statu == (int)TicketStatusEnum.Anulated)
                    .ToList().ForEach(
                    tn => transactions.Add(new NumberTransactionModel
                    {
                        Description = "Anulación",
                        ClientDesc = tn.InvoiceTickets.FirstOrDefault().Invoice.Client.Id + " - " + tn.InvoiceTickets.FirstOrDefault().Invoice.Client.Name,
                        Date = tn.CreateDate.ToUnixTime(),
                        FractionFrom = tn.FractionFrom,
                        FractionTo = tn.FractionTo,
                        Group = "No hay datos",
                        UserDesc = tn.User.Name,
                    }));
            }
            return transactions;
        }
    }
}
