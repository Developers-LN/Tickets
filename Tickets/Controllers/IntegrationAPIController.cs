//using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    public class IntegrationAPIController : ApiController
    {
    }

    [RoutePrefix("integration/electronicTicketApi")]
    [AllowAnonymous]
    public class ElectronicTicketAPIController : ApiController
    {
        //
        //  GET: integration/electronicTicketApi/allocations
        [HttpGet]
        [ActionName("allocations")]
        [AllowAnonymous]
        public Models.JSON.TicketAllocationJSON Allocations(int raffleId, string u, string p)
        {
            var allocationJSON = new Models.JSON.TicketAllocationJSON()
            {
                Result = true,
                ErrorMessage = "",
                TicketNumbers = new List<Models.JSON.TicketNumber>()
            };
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (WebSecurity.Login(u, p, false) == false)
                        {
                            allocationJSON.Result = false;
                            allocationJSON.ErrorMessage = "Error en el usuario o la contraseña.";
                        }
                        else
                        {

                            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                            if (raffle == null)
                            {
                                allocationJSON.Result = false;
                                allocationJSON.ErrorMessage = "El sorteo " + raffleId + " no es valido.";
                            }
                            else
                            {
                                allocationJSON.RaffleDate = raffle.DateSolteo.ToString("dd/MM/yyyy");
                                allocationJSON.RaffleId = raffle.Id;

                                raffle.TicketAllocations.ToList().ForEach(a => a.TicketAllocationNumbers.ToList().ForEach(t =>
                                    allocationJSON.TicketNumbers.Add(new Models.JSON.TicketNumber()
                                    {
                                        TiketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number),
                                        FractionFrom = t.FractionFrom,
                                        FractionTo = t.FractionTo
                                    })
                                ));
                            }
                        }
                        return allocationJSON;
                    }
                    catch
                    {
                        allocationJSON.Result = false;
                        allocationJSON.ErrorMessage = "Ha ocurrido un error en el Servidor, Comuníquese con el administrador.";
                        return allocationJSON;
                    }
                }
            }
        }

        //
        //  GET: integration/electronicTicketApi/allocation
        [HttpGet]
        [ActionName("allocation")]
        [AllowAnonymous]
        public Models.JSON.TicketAllocationJSON Allocation(int raffleId, int clientId, string u, string p)
        {
            var allocationJSON = new Models.JSON.TicketAllocationJSON()
            {
                Result = true,
                ErrorMessage = "",
                TicketNumbers = new List<Models.JSON.TicketNumber>()
            };
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (WebSecurity.Login(u, p, false) == false)
                        {
                            allocationJSON.Result = false;
                            allocationJSON.ErrorMessage = "Error en el usuario o la contraseña.";
                        }
                        else
                        {

                            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                            if (raffle == null)
                            {
                                allocationJSON.Result = false;
                                allocationJSON.ErrorMessage = "El sorteo " + raffleId + " no es valido.";
                            }
                            else
                            {
                                allocationJSON.RaffleDate = raffle.DateSolteo.ToString("dd/MM/yyyy");
                                allocationJSON.RaffleId = raffle.Id;

                                raffle.TicketAllocations.Where(t => t.ClientId == clientId).ToList().ForEach(a => a.TicketAllocationNumbers.ToList().ForEach(t =>
                                    allocationJSON.TicketNumbers.Add(new Models.JSON.TicketNumber()
                                    {
                                        TiketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number),
                                        FractionFrom = t.FractionFrom,
                                        FractionTo = t.FractionTo
                                    })
                                ));
                            }
                        }
                        return allocationJSON;
                    }
                    catch
                    {
                        allocationJSON.Result = false;
                        allocationJSON.ErrorMessage = "Ha ocurrido un error en el Servidor, Comuníquese con el administrador.";
                        return allocationJSON;
                    }
                }
            }
        }

        //
        //  GET: integration/electronicTicketApi/numberAwards
        [HttpGet]
        [ActionName("numberAwards")]
        [AllowAnonymous]
        public Models.JSON.AwardNumbesJSON NumberAwards(int raffleId, string u, string p)
        {
            var numberAwardsJSON = new Models.JSON.AwardNumbesJSON()
            {
                Result = true,
                ErrorMessage = "",
                TicketNumbers = new List<Models.JSON.TicketNumberAward>()
            };
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (WebSecurity.Login(u, p, false) == false)
                        {
                            numberAwardsJSON.Result = false;
                            numberAwardsJSON.ErrorMessage = "Error en el usuario o la contraseña.";
                        }
                        else
                        {
                            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                            if (raffle == null)
                            {
                                numberAwardsJSON.Result = false;
                                numberAwardsJSON.ErrorMessage = "El sorteo " + raffleId + " no es valido.";
                            }
                            else
                            {
                                numberAwardsJSON.RaffleDate = raffle.DateSolteo.ToString("dd/MM/yyyy");
                                numberAwardsJSON.RaffleId = raffle.Id;
                                numberAwardsJSON.TicketNumbers = new List<Models.JSON.TicketNumberAward>();

                                var ticketNumbers = context.TicketAllocationNumbers.Where(n => n.TicketAllocation.RaffleId == raffle.Id).ToList();
                                (from a in raffle.RaffleAwards.ToList()
                                 join n in ticketNumbers on a.ControlNumber equals n.Number
                                 select new
                                 {
                                     Number = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)n.Number),
                                     n.FractionFrom,
                                     n.FractionTo,
                                     Award = new
                                     {
                                         AwardId = a.Id,
                                         AwardName = a.Award.Name,
                                         AwardFractionPrice = (a.Award.Value / (raffle.Prospect.LeafFraction * raffle.Prospect.LeafNumber)),
                                         AwardPrice = a.Award.Value
                                     }
                                 }).GroupBy(r => r.Number).ToList().ForEach(r => numberAwardsJSON.TicketNumbers.Add(new Models.JSON.TicketNumberAward()
                                 {
                                     TiketNumber = r.FirstOrDefault().Number,
                                     FractionFrom = r.FirstOrDefault().FractionFrom,
                                     FractionTo = r.FirstOrDefault().FractionTo,
                                     Awards = r.Select(a => new Models.JSON.Award()
                                     {
                                         AwardFractionPrice = a.Award.AwardFractionPrice,
                                         AwardId = a.Award.AwardId,
                                         AwardName = a.Award.AwardName,
                                         AwardPrice = a.Award.AwardPrice
                                     }).ToList()
                                 }));
                            }
                        }
                        return numberAwardsJSON;
                    }
                    catch
                    {
                        numberAwardsJSON.Result = false;
                        numberAwardsJSON.ErrorMessage = "Ha ocurrido un error en el Servidor, Comuníquese con el administrador.";
                        return numberAwardsJSON;
                    }
                }
            }
        }

        //
        //  POST: integration/electronicTicketApi/invoiceTicket
        [HttpPost]
        [ActionName("invoiceTicket")]
        [AllowAnonymous]
        public object InvoiceTicket(Models.JSON.InvoiceJSON invoice)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (WebSecurity.Login(invoice.UserName, invoice.Password, false) == false)
                        {
                            return new
                            {
                                result = false,
                                message = "Error en el usuario o la contraseña."
                            };
                        }
                        else
                        {
                            var raffle = context.Raffles.FirstOrDefault(r => r.Id == invoice.RaffleId);
                            if (raffle == null)
                            {
                                return new
                                {
                                    result = false,
                                    message = "El sorteo " + invoice.RaffleId + " no es valido."
                                };
                            }
                            else
                            {
                                var allocationNumbers = context.TicketAllocationNumbers.Where(t => t.TicketAllocation.RaffleId == invoice.RaffleId).Select(t => t.Number);
                                string duplicateNumber = "";
                                foreach (var number in allocationNumbers)
                                {
                                    if (invoice.TicketNumbers.AsEnumerable().Where(t => long.Parse(t.TiketNumber) == number).Any())
                                    {
                                        duplicateNumber += number + ", ";
                                    }
                                }
                                if (duplicateNumber != "")
                                {
                                    return new
                                    {
                                        result = false,
                                        message = "Los numeros ( " + duplicateNumber + " ) ya están asignados."
                                    };
                                }

                                /*  START ALLOCATIONS */
                                var ticketAllocation = new TicketAllocation()
                                {
                                    ClientId = invoice.ClientId,
                                    RaffleId = invoice.RaffleId,
                                    CreateUser = WebSecurity.CurrentUserId,
                                    CreateDate = DateTime.Now,
                                    Type = 1,
                                    Statu = (int)AllocationStatuEnum.Invoiced
                                };
                                context.TicketAllocations.Add(ticketAllocation);
                                context.SaveChanges();

                                List<Tickets.Models.TicketAllocationNumber> ticketAllocations = new List<Tickets.Models.TicketAllocationNumber>();
                                foreach (var number in invoice.TicketNumbers)
                                {
                                    var ticket = new Tickets.Models.TicketAllocationNumber()
                                    {
                                        Number = long.Parse(number.TiketNumber),
                                        Invoiced = true,
                                        Printed = true,
                                        ControlNumber = "",
                                        FractionFrom = number.FractionFrom,
                                        FractionTo = number.FractionTo,
                                        TicketAllocationId = ticketAllocation.Id,
                                        CreateUser = WebSecurity.CurrentUserId,
                                        CreateDate = DateTime.Now,
                                        Statu = (int)TicketStatusEnum.Printed
                                    };
                                    ticketAllocations.Add(ticket);
                                }
                                context.TicketAllocationNumbers.AddRange(ticketAllocations);
                                context.SaveChanges();

                                /*  END ALLOCATIONS */
                                tx.Commit();
                                /*  START INVOICE */
                                var invoiceModel = new TicketInvoiceModel()
                                {
                                    TicketAllocations = new List<TicketAllocationModel>() { new TicketAllocationModel() { Id = ticketAllocation.Id } },
                                    ClientId = invoice.ClientId,
                                    Condition = (int)InvoiceConditionEnum.Counted,
                                    InvoiceDate = DateTime.Now,
                                    InvoiceExpredDay = 14,
                                    PaymentType = (int)PaymentTypeEnum.Cash,
                                    RaffleId = invoice.RaffleId
                                };

                                var result = new TicketInvoiceModel().Save(invoiceModel);
                                return result;
                            }
                        }
                    }
                    catch
                    {
                        return new
                        {
                            result = false,
                            message = "Ha ocurrido un error en el Servidor, Comuníquese con el administrador."
                        };
                    }
                }
            }
        }

        //
        //  POST: integration/electronicTicketApi/identifyticket
        [HttpPost]
        [ActionName("identifyticket")]
        [AllowAnonymous]
        public object Identifyticket(Models.JSON.IdentifyTicketJSON identify)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (WebSecurity.Login(identify.UserName, identify.Password, false) == false)
                        {
                            return new
                            {
                                result = false,
                                message = "Error en el usuario o la contraseña."
                            };
                        }
                        else
                        {
                            var raffle = context.Raffles.FirstOrDefault(r => r.Id == identify.RaffleId);
                            if (raffle == null)
                            {
                                return new
                                {
                                    result = false,
                                    message = "El sorteo " + identify.RaffleId + " no es valido."
                                };
                            }
                            else
                            {
                                foreach (var identifyNumber in identify.TicketNumbers)
                                {
                                    var number = new AwardTicketModel()
                                    {
                                        FractionFrom = identifyNumber.FractionFrom,
                                        FractionTo = identifyNumber.FractionTo,
                                        NumberId = int.Parse(identifyNumber.TiketNumber),
                                        RaffleId = identify.RaffleId,
                                        Type = (int)IdentifyBachTypeEnum.Menor
                                    };
                                    var result = new TicketIdentifyModel().ValidateNumberAward(number, 0);
                                    return result;
                                }

                                var newIdentifyBach = new IdentifyBach()
                                {
                                    ClientId = identify.ClientId,
                                    RaffleId = identify.RaffleId,
                                    Type = (int)IdentifyBachTypeEnum.Menor,
                                    IdentifyNumbers = identify.TicketNumbers.Select(n => new IdentifyNumber()
                                    {
                                        FractionFrom = n.FractionFrom,
                                        FractionTo = n.FractionTo,
                                        NumberId = int.Parse(n.TiketNumber)
                                    }).ToList()
                                };
                                var saveResult = new TicketIdentifyModel().IdentifyAward(newIdentifyBach);
                                return saveResult;
                            }
                        }
                    }
                    catch
                    {
                        return new
                        {
                            result = false,
                            message = "Ha ocurrido un error en el Servidor, Comuníquese con el administrador."
                        };
                    }
                }
            }
        }
    }
}
