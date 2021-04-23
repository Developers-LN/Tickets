using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;
using Tickets.Models.XML;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    public class IntegrationController : Controller
    {
        //GET: /Integration/ElectronicTicketXml
        [Authorize]
        [HttpGet]
        public ActionResult ElectronicTicketXml()
        {
            return View();
        }

        //NUEVO CODIGO PARA GENERAR XML DE LAS ASIGNACIONES
        //GET: /Integration/AllocationNumbers
        [Authorize]
        [HttpGet]
        public JsonResult AllocationNumbers(int id)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var allocation = context.TicketAllocations.FirstOrDefault(i => i.Id == id);
                        var raffle = context.Raffles.Where(r => r.Id == allocation.RaffleId).FirstOrDefault();

                        var allocationXML = new Models.XML.TicketAllocateXML();

                        if (raffle.Prospect.ImpresionType == (int)PrintTypeProspect.Ordinario)
                        {
                            allocationXML = new Models.XML.TicketAllocateXML()
                            {
                                RaffleDate = allocation.Raffle.DateSolteo.ToShortDateString(),
                                RaffleId = allocation.RaffleId,
                                /*User = WebSecurity.CurrentUserName,*/
                                CreateDate = DateTime.Now.ToString(),
                                Allocation = id,
                                TicketAllocationNumbers = new List<Models.XML.TicketAllocationNumber>()
                            };

                            raffle.TicketAllocations.ToList().ForEach(a => a.TicketAllocationNumbers.Where(n => n.TicketAllocationId == id).ToList().ForEach(t =>
                                allocationXML.TicketAllocationNumbers.Add(new Models.XML.TicketAllocationNumber()
                                {
                                    TiketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number),
                                    FractionFrom = t.FractionFrom,
                                    FractionTo = t.FractionTo
                                })
                            ));
                        }
                        else
                        {
                            allocationXML = new Models.XML.TicketAllocateXML()
                            {
                                RaffleDate = allocation.Raffle.DateSolteo.ToShortDateString(),
                                RaffleId = allocation.RaffleId,
                                /*User = WebSecurity.CurrentUserName,*/
                                CreateDate = DateTime.Now.ToString(),
                                Allocation = id,
                                ticketAllocationNumberExtraordinarios = new List<Models.XML.TicketAllocationNumberExtraordinario>()
                            };

                            raffle.TicketAllocations.ToList().ForEach(a => a.TicketAllocationNumbers.Where(n => n.TicketAllocationId == id).ToList().ForEach(t =>
                                allocationXML.ticketAllocationNumberExtraordinarios.Add(new Models.XML.TicketAllocationNumberExtraordinario()
                                {
                                    TiketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number) + raffle.Separator + raffle.Symbol
                                })
                            ));
                        }

                        XmlDocument xmlDoc = new XmlDocument();
                        //Represents an XML document,

                        XmlSerializer xmlSerializer = new XmlSerializer(allocationXML.GetType());
                        // Creates a stream whose backing store is memory.
                        using (MemoryStream xmlStream = new MemoryStream())
                        {
                            xmlSerializer.Serialize(xmlStream, allocationXML);
                            xmlStream.Position = 0;
                            xmlDoc.Load(xmlStream);
                        }
                        string patch = System.Web.Hosting.HostingEnvironment.MapPath("~") + "generalRaffle";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        patch += "/allocationXML";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        var fileName = Guid.NewGuid().ToString() + ".xml";

                        xmlDoc.Save(patch + "/" + fileName);

                        return new System.Web.Mvc.JsonResult()
                        {
                            JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = true,
                                message = "Asignaciones descargado en XML Correctamente.",
                                path = "generalRaffle/allocationXML/" + fileName
                            }
                        };
                    }
                    catch (Exception e)
                    {
                        return new System.Web.Mvc.JsonResult()
                        {
                            JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = false,
                                message = e.Message
                            }
                        };
                    }
                }
            }
        }

        //GET: /Integration/AllocationNumberToXML
        [Authorize]
        [HttpGet]
        public JsonResult AllocationNumberToXML(int raffleId)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                        var allocationXML = new Models.XML.TicketAllocateXML()
                        {
                            RaffleDate = raffle.DateSolteo.ToShortDateString(),
                            RaffleId = raffle.Id,
                            User = WebSecurity.CurrentUserName,
                            CreateDate = DateTime.Now.ToString(),
                            TicketAllocationNumbers = new List<Models.XML.TicketAllocationNumber>()
                        };

                        raffle.TicketAllocations.ToList().ForEach(a => a.TicketAllocationNumbers.ToList().ForEach(t =>
                            allocationXML.TicketAllocationNumbers.Add(new Models.XML.TicketAllocationNumber()
                            {
                                TiketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number),
                                FractionFrom = t.FractionFrom,
                                FractionTo = t.FractionTo
                            })
                        ));

                        XmlDocument xmlDoc = new XmlDocument();
                        //Represents an XML document,

                        XmlSerializer xmlSerializer = new XmlSerializer(allocationXML.GetType());
                        // Creates a stream whose backing store is memory.
                        using (MemoryStream xmlStream = new MemoryStream())
                        {
                            xmlSerializer.Serialize(xmlStream, allocationXML);
                            xmlStream.Position = 0;
                            xmlDoc.Load(xmlStream);
                        }
                        string patch = HttpContext.Server.MapPath("~") + "generalRaffle";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        patch += "/allocationXML";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        var fileName = Guid.NewGuid().ToString() + ".xml";

                        xmlDoc.Save(patch + "/" + fileName);

                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = true,
                                message = "Asignaciones descargado en XML Correctamente.",
                                path = "generalRaffle/allocationXML/" + fileName
                            }
                        };
                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = false,
                                message = e.Message
                            }
                        };
                    }
                }
            }
        }


        ////GET: /Integration/AwardNumberToXML
        //[Authorize]
        //[HttpGet]
        //public JsonResult AwardNumberToXML(int raffleId)
        //{

        //    using (var context = new TicketsEntities())
        //    {
        //        using (var tx = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
        //                var awardNumbesXML = new Models.XML.AwardNumbesXML()
        //                {
        //                    RaffleDate = raffle.DateSolteo.ToShortDateString(),
        //                    RaffleId = raffle.Id,
        //                    User = WebSecurity.CurrentUserName,
        //                    CreateDate = DateTime.Now.ToString(),
        //                    TicketNumbers = new List<Models.XML.TicketNumber>()
        //                };
        //                var ticketNumbers = context.TicketAllocationNumbers.Where(n => n.TicketAllocation.RaffleId == raffle.Id).ToList();
        //                (from a in raffle.RaffleAwards.ToList()
        //                 join n in ticketNumbers on a.ControlNumber equals n.Number
        //                 select new
        //                 {
        //                     Number = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)n.Number),
        //                     FractionFrom = n.FractionFrom,
        //                     FractionTo = n.FractionTo,
        //                     Award = new
        //                     {
        //                         AwardId = a.Id,
        //                         AwardName = a.Award.Name,
        //                         AwardFractionPrice = (a.Award.Value / (raffle.Prospect.LeafFraction * raffle.Prospect.LeafNumber)),
        //                         AwardPrice = a.Award.Value
        //                     }
        //                 }).GroupBy(r => r.Number).ToList().ForEach(r => awardNumbesXML.TicketNumbers.Add(new Models.XML.TicketNumber()
        //                 {
        //                     TiketNumber = r.FirstOrDefault().Number,
        //                     FractionFrom = r.FirstOrDefault().FractionFrom,
        //                     FractionTo = r.FirstOrDefault().FractionTo,
        //                     Awards = r.Select(a => new Models.XML.Award()
        //                     {
        //                         AwardFractionPrice = a.Award.AwardFractionPrice,
        //                         AwardId = a.Award.AwardId,
        //                         AwardName = a.Award.AwardName,
        //                         AwardPrice = a.Award.AwardPrice
        //                     }).ToList()
        //                 }));

        //                XmlDocument xmlDoc = new XmlDocument();
        //                //Represents an XML document,

        //                XmlSerializer xmlSerializer = new XmlSerializer(awardNumbesXML.GetType());
        //                // Creates a stream whose backing store is memory.
        //                using (MemoryStream xmlStream = new MemoryStream())
        //                {
        //                    xmlSerializer.Serialize(xmlStream, awardNumbesXML);
        //                    xmlStream.Position = 0;
        //                    xmlDoc.Load(xmlStream);
        //                }
        //                string patch = HttpContext.Server.MapPath("~") + "generalRaffle";
        //                if (!System.IO.Directory.Exists(patch))
        //                {
        //                    System.IO.Directory.CreateDirectory(patch);
        //                }
        //                patch += "/awardsXML";
        //                if (!System.IO.Directory.Exists(patch))
        //                {
        //                    System.IO.Directory.CreateDirectory(patch);
        //                }
        //                var fileName = Guid.NewGuid().ToString() + ".xml";

        //                xmlDoc.Save(patch + "/" + fileName);

        //                return new JsonResult()
        //                {
        //                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //                    Data = new
        //                    {
        //                        result = true,
        //                        message = "Numeros premiados descargado en XML Correctamente.",
        //                        path = "generalRaffle/awardsXML/" + fileName
        //                    }
        //                };
        //            }
        //            catch (Exception e)
        //            {
        //                return new JsonResult()
        //                {
        //                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        //                    Data = new
        //                    {
        //                        result = false,
        //                        message = e.Message
        //                    }
        //                };
        //            }
        //        }
        //    }
        //}


        [Authorize]
        [HttpGet]
        public JsonResult AwardNumberToXML(int raffleId)
        {

            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                        var awardNumbesXML = new Models.XML.AwardNumbesXML()
                        {
                            RaffleDate = raffle.DateSolteo.ToShortDateString(),
                            RaffleId = raffle.Id,
                            User = WebSecurity.CurrentUserName,
                            CreateDate = DateTime.Now.ToString(),
                            TicketNumbers = new List<Models.XML.TicketNumber>()
                        };
                        var FractionTo = raffle.Prospect.LeafFraction * raffle.Prospect.LeafNumber;
                        (from a in raffle.RaffleAwards.ToList()

                         select new
                         {
                             Number = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)a.ControlNumber),
                             FractionFrom = 1,
                             FractionTo = FractionTo,
                             Award = new
                             {
                                 AwardId = a.Id,
                                 AwardName = a.Award.Name,
                                 AwardFractionPrice = (a.Award.Value / (raffle.Prospect.LeafFraction * raffle.Prospect.LeafNumber)),
                                 AwardPrice = a.Award.Value
                             }
                         }).GroupBy(r => r.Number).ToList().ForEach(r => awardNumbesXML.TicketNumbers.Add(new Models.XML.TicketNumber()
                         {
                             TiketNumber = r.FirstOrDefault().Number,
                             FractionFrom = r.FirstOrDefault().FractionFrom,
                             FractionTo = r.FirstOrDefault().FractionTo,
                             Awards = r.Select(a => new Models.XML.Award()
                             {
                                 AwardFractionPrice = a.Award.AwardFractionPrice,
                                 AwardId = a.Award.AwardId,
                                 AwardName = a.Award.AwardName,
                                 AwardPrice = a.Award.AwardPrice
                             }).ToList()
                         }));

                        XmlDocument xmlDoc = new XmlDocument();
                        //Represents an XML document,

                        XmlSerializer xmlSerializer = new XmlSerializer(awardNumbesXML.GetType());
                        // Creates a stream whose backing store is memory.
                        using (MemoryStream xmlStream = new MemoryStream())
                        {
                            xmlSerializer.Serialize(xmlStream, awardNumbesXML);
                            xmlStream.Position = 0;
                            xmlDoc.Load(xmlStream);
                        }
                        string patch = HttpContext.Server.MapPath("~") + "generalRaffle";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        patch += "/awardsXML";
                        if (!System.IO.Directory.Exists(patch))
                        {
                            System.IO.Directory.CreateDirectory(patch);
                        }
                        var fileName = Guid.NewGuid().ToString() + ".xml";

                        xmlDoc.Save(patch + "/" + fileName);

                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = true,
                                message = "Numeros premiados descargado en XML Correctamente.",
                                path = "generalRaffle/awardsXML/" + fileName
                            }
                        };
                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = false,
                                message = e.Message
                            }
                        };
                    }
                }
            }
        }

        //POST: /Integration/CreateInvoiceXML
        [Authorize]
        [HttpPost]
        public JsonResult CreateInvoiceXML(int raffleId, string path)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);

                        InvoiceXML invoiceXml = null;

                        XmlSerializer serializer = new XmlSerializer(typeof(InvoiceXML));

                        StreamReader reader = new StreamReader(path);
                        invoiceXml = (InvoiceXML)serializer.Deserialize(reader);
                        reader.Close();

                        if (raffleId != invoiceXml.RaffleId)
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    result = false,
                                    message = "El archivo no pertenece a este Sorteo."
                                }
                            };
                        }
                        var actived = context.Raffles.Where(r => r.Id == invoiceXml.RaffleId &&
                        (r.Statu == (int)RaffleStatusEnum.Generated || r.Statu == (int)RaffleStatusEnum.Suspended)).Select(r => r).FirstOrDefault();
                        if (actived != null)
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    result = false,
                                    message = "El sorteo ha sido generado o se ha suspendido, La facturacion se ha cancelado!"
                                }
                            };
                        }

                        int clientId = int.Parse(ConfigurationManager.AppSettings["IntegrationClientId"].ToString());

                        var allocationNumbers = context.TicketAllocationNumbers.Where(t => t.TicketAllocation.RaffleId == raffleId).Select(t => t.Number);
                        string duplicateNumber = "";
                        foreach (var number in allocationNumbers)
                        {
                            if (invoiceXml.InvoiceTicketNumbers.AsEnumerable().Where(t => long.Parse(t.TicketNumber) == number).Any())
                            {
                                duplicateNumber += number + ", ";
                            }
                        }
                        if (duplicateNumber != "")
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    result = false,
                                    message = "Los numeros ( " + duplicateNumber + " ) ya están asignados."
                                }
                            };
                        }

                        /*  START ALLOCATIONS */
                        var ticketAllocation = new TicketAllocation()
                        {
                            ClientId = clientId,
                            RaffleId = invoiceXml.RaffleId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now,
                            Type = 5821,
                            Statu = (int)AllocationStatuEnum.Invoiced,

                        };
                        context.TicketAllocations.Add(ticketAllocation);
                        context.SaveChanges();

                        List<Tickets.Models.TicketAllocationNumber> ticketAllocations = new List<Tickets.Models.TicketAllocationNumber>();
                        foreach (var number in invoiceXml.InvoiceTicketNumbers)
                        {
                            var ticket = new Tickets.Models.TicketAllocationNumber()
                            {
                                Number = long.Parse(number.TicketNumber),
                                Invoiced = true,
                                Printed = true,
                                ControlNumber = "",
                                FractionFrom = number.FractionFrom,
                                FractionTo = number.FractionTo,
                                TicketAllocationId = ticketAllocation.Id,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now,
                                Statu = (int)TicketStatusEnum.Printed,


                            };
                            ticketAllocations.Add(ticket);
                        }
                        context.TicketAllocationNumbers.AddRange(ticketAllocations);
                        context.SaveChanges();

                        /*  END ALLOCATIONS */

                        var user = context.Users.Where(u => u.Id == WebSecurity.CurrentUserId).FirstOrDefault();
                        var invoiceModel = new TicketInvoiceModel()
                        {
                            TicketAllocations = new List<TicketAllocationModel>() { new TicketAllocationModel() { Id = ticketAllocation.Id } },
                            ClientId = clientId,
                            Condition = (int)InvoiceConditionEnum.Credit,
                            InvoiceDate = DateTime.Now,
                            InvoiceExpredDay = 14,
                            PaymentType = (int)PaymentTypeEnum.Ninguno,
                            RaffleId = raffleId,

                        };

                        var client = context.Clients.FirstOrDefault(c => c.Id == invoiceModel.ClientId);
                        var invoice = new Invoice()
                        {
                            PaymentType = invoiceModel.PaymentType,
                            InvoiceDate = invoiceModel.InvoiceDate,
                            Condition = invoiceModel.Condition,
                            Discount = client.Discount,
                            InvoiceExpredDay = invoiceModel.InvoiceExpredDay <= 0 ? 30 : invoiceModel.InvoiceExpredDay,
                            ClientId = invoiceModel.ClientId,
                            RaffleId = invoiceModel.RaffleId,
                            AgencyId = user.Employee.AgencyId,
                            Statu = (int)GeneralStatusEnum.Active,
                            OutstandingBalance = 0.0M,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now
                        };
                        if (invoiceModel.Condition == (int)InvoiceConditionEnum.Credit)
                        {
                            invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Pendient;
                        }
                        else
                        {
                            invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Payed;
                        }

                        context.Invoices.Add(invoice);
                        context.SaveChanges();

                        var allocations = context.TicketAllocations.AsEnumerable().Where(a => invoiceModel.TicketAllocations.Any(ta => ta.Id == a.Id)).ToList();
                        var prospect = context.Raffles.Where(r => r.Id == raffleId).Select(r => r.Prospect).FirstOrDefault();
                        var fractionPrice = context.Prospect_Price.Where(pp => pp.PriceId == client.PriceId && pp.ProspectId == raffle.ProspectId).Select(pp => pp.FactionPrice).FirstOrDefault();
                        List<InvoiceTicket> invoiceTicketList = new List<InvoiceTicket>();
                        foreach (var allocation in allocations)
                        {
                            foreach (var number in allocation.TicketAllocationNumbers)
                            {
                                invoiceTicketList.Add(new InvoiceTicket()
                                {
                                    Invoice = invoice,
                                    InvoiceId = invoice.Id,
                                    PricePerFraction = fractionPrice,
                                    Quantity = (number.FractionTo - number.FractionFrom) + 1,
                                    TicketNumberAllocationId = number.Id,

                                });
                                number.Invoiced = true;
                                number.Statu = (int)TicketStatusEnum.Factured;
                            }
                            allocation.Statu = (int)AllocationStatuEnum.Invoiced;
                        }
                        context.InvoiceTickets.AddRange(invoiceTicketList);


                        context.SaveChanges();
                        tx.Commit();

                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new RequestResponseModel()
                            {
                                Result = true,
                                Message = "Entrega de Billetes Completada.",
                                Object = null
                            }
                        };
                        /*  END INVOICE */
                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = false,
                                message = e.Message
                            }
                        };
                    }
                }
            }
        }


        //POST: /Integration/CreateBachXML
        [Authorize]
        [HttpPost]
        public JsonResult CreateBachXML(int raffleId, string path)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);

                        TicketPayedXML identifyBach = null;

                        XmlSerializer serializer = new XmlSerializer(typeof(TicketPayedXML));

                        StreamReader reader = new StreamReader(path);
                        identifyBach = (TicketPayedXML)serializer.Deserialize(reader);
                        reader.Close();

                        if (raffleId != identifyBach.RaffleId)
                        {
                            return new JsonResult()
                            {
                                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                Data = new
                                {
                                    result = false,
                                    messages = new List<string>() { "El archivo no pertenece a este Sorteo." }
                                }
                            };
                        }

                        foreach (var identifyNumber in identifyBach.TicketNumbers)
                        {
                            var number = new AwardTicketModel()
                            {
                                FractionFrom = identifyNumber.FractionFrom,
                                FractionTo = identifyNumber.FractionTo,
                                NumberId = int.Parse(identifyNumber.TicketNumber),
                                RaffleId = identifyBach.RaffleId,
                                Type = (int)IdentifyBachTypeEnum.Menor
                            };
                            var result = (ReultRequestModel)new TicketIdentifyModel().ValidateNumberAward(number, 0);
                            if (result.result == false)
                            {
                                return new JsonResult()
                                {
                                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                                    Data = result
                                };
                            }
                        }
                        int clientId = int.Parse(ConfigurationManager.AppSettings["IntegrationClientId"].ToString());
                        var newIdentifyBach = new IdentifyBach()
                        {
                            ClientId = clientId,
                            RaffleId = identifyBach.RaffleId,
                            Type = (int)IdentifyBachTypeEnum.Menor,
                            IdentifyNumbers = identifyBach.TicketNumbers.Select(n => new IdentifyNumber()
                            {
                                FractionFrom = n.FractionFrom,
                                FractionTo = n.FractionTo,
                                NumberId = int.Parse(n.TicketNumber)
                            }).ToList()
                        };
                        var saveResult = new TicketIdentifyModel().IdentifyAward(newIdentifyBach);

                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = saveResult
                        };
                    }
                    catch (Exception e)
                    {
                        return new JsonResult()
                        {
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = false,
                                messages = new List<string>() { e.Message }
                            }
                        };
                    }
                }
            }
        }
    }
}