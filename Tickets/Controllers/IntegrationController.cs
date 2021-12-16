using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Procedures;
using Tickets.Models.Ticket;
using Tickets.Models.XML;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    public class IntegrationController : Controller
    {
        private static Random random = new Random();

        //GET: /Integration/ElectronicTicketXml
        [Authorize]
        [HttpGet]
        public ActionResult ElectronicTicketXml()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult PayableAwardsToExcel(int raffleId)
        {
            AllPatyableAwardsProcedure allPatyableAwardsProcedure = new AllPatyableAwardsProcedure();
            var Resultado = allPatyableAwardsProcedure.ConsultaTodosBilletesPagables(raffleId);

            var context = new TicketsEntities();

            using (var workBook = new XLWorkbook())
            {
                var workSheet = workBook.Worksheets.Add("Billetes pagables");
                var curretRow = 1;

                workSheet.Cell(curretRow, 1).Value = "Billete";
                workSheet.Cell(curretRow, 2).Value = "Estatus_De_Pago";
                workSheet.Cell(curretRow, 3).Value = "Fracciones";
                workSheet.Cell(curretRow, 4).Value = "Tipo_De_Premio";
                workSheet.Cell(curretRow, 5).Value = "Monto_Del_Premio";
                workSheet.Cell(curretRow, 6).Value = "Monto_A_Pagar";

                var IdentifyTickets = context.IdentifyBaches.Where(w => w.RaffleId == raffleId).Select(s => new { s.IdentifyNumbers }).ToList();

                foreach (var item in Resultado)
                {
                    curretRow++;
                    workSheet.Cell(curretRow, 1).Value = item.number;

                    if (IdentifyTickets.Any(a => a.IdentifyNumbers.Any(a2 => a2.NumberId == item.tanId)))
                    {
                        workSheet.Cell(curretRow, 2).Value = "Pagado";
                    }
                    else
                    {
                        workSheet.Cell(curretRow, 2).Value = "Pendiente";
                    }
                    workSheet.Cell(curretRow, 3).Value = item.fracciones;
                    workSheet.Cell(curretRow, 4).Value = item.nameaward;
                    workSheet.Cell(curretRow, 5).Value = item.value;
                    workSheet.Cell(curretRow, 6).Value = item.valorpagar;
                }

                var range = workSheet.RangeUsed();
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleLight9;
                workSheet.Columns().AdjustToContents();
                workSheet.Column(5).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(6).Style.NumberFormat.Format = "$ #,##0.00";

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();

                    string Nombre = ("Billetes pagables del sorteo " + raffleId + ".xlsx").ToString();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Nombre);
                }
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ExportToExcel(string FechaInicio = null, string FechaFin = null, int raffleId = 0)
        {
            SalesAndPendingPayments salesAndPendingPayments = new SalesAndPendingPayments();
            var Resultado = salesAndPendingPayments.ConsultaVentasCuentasPendientes(FechaInicio, FechaFin, raffleId);

            using (var workBook = new XLWorkbook())
            {
                var workSheet = workBook.Worksheets.Add("Cuentas");
                var curretRow = 1;

                workSheet.Cell(curretRow, 1).Value = "ID_Cliente";
                workSheet.Cell(curretRow, 2).Value = "Nombre_Cliente";
                workSheet.Cell(curretRow, 3).Value = "Tipo_Cliente";
                workSheet.Cell(curretRow, 4).Value = "ID_Sorteo";
                workSheet.Cell(curretRow, 5).Value = "Nombre_Sorteo";
                workSheet.Cell(curretRow, 6).Value = "ID_Factura";
                workSheet.Cell(curretRow, 7).Value = "Fecha_Factura";
                workSheet.Cell(curretRow, 8).Value = "Estado_Factura";
                workSheet.Cell(curretRow, 9).Value = "Total_Billetes";
                workSheet.Cell(curretRow, 10).Value = "Precio_Billete";
                workSheet.Cell(curretRow, 11).Value = "Total_Factura";
                workSheet.Cell(curretRow, 12).Value = "Descuento";
                workSheet.Cell(curretRow, 13).Value = "Total_Descuento";
                workSheet.Cell(curretRow, 14).Value = "Total_A_Pagar";
                workSheet.Cell(curretRow, 15).Value = "Pagos_Efectivo";
                workSheet.Cell(curretRow, 16).Value = "Pagos_Nota_Credito";
                workSheet.Cell(curretRow, 17).Value = "Billetes_Devueltos";
                workSheet.Cell(curretRow, 18).Value = "Fracciones_Devueltas";
                workSheet.Cell(curretRow, 19).Value = "Total_Pagado";
                workSheet.Cell(curretRow, 20).Value = "Total_Faltante";

                foreach (var item in Resultado)
                {
                    curretRow++;
                    workSheet.Cell(curretRow, 1).Value = item.IdClient;
                    workSheet.Cell(curretRow, 2).Value = item.NameClient;
                    workSheet.Cell(curretRow, 3).Value = item.TypeClient;
                    workSheet.Cell(curretRow, 4).Value = item.IdRaffle;
                    workSheet.Cell(curretRow, 5).Value = item.NameRaffle;
                    workSheet.Cell(curretRow, 6).Value = item.IdInvoice;
                    workSheet.Cell(curretRow, 7).Value = item.DateInvoice;
                    workSheet.Cell(curretRow, 8).Value = item.StatusInvoice;
                    workSheet.Cell(curretRow, 9).Value = item.TotalTickets;
                    workSheet.Cell(curretRow, 10).Value = item.PriceTicket;
                    workSheet.Cell(curretRow, 11).Value = item.TotalInvoice;
                    workSheet.Cell(curretRow, 12).Value = item.DiscountPercent;
                    workSheet.Cell(curretRow, 13).Value = item.TotalDiscount;
                    workSheet.Cell(curretRow, 14).Value = item.TotalToPay;
                    workSheet.Cell(curretRow, 15).Value = item.CashPayment;
                    workSheet.Cell(curretRow, 16).Value = item.NoteCreditPayment;
                    workSheet.Cell(curretRow, 17).Value = item.TicketReturn;
                    workSheet.Cell(curretRow, 18).Value = item.FractionReturn;
                    workSheet.Cell(curretRow, 19).Value = item.TotalPayed;
                    workSheet.Cell(curretRow, 20).Value = item.TotalPending;
                }

                var range = workSheet.RangeUsed();
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleLight9;
                workSheet.Columns().AdjustToContents();
                workSheet.Column(9).Style.NumberFormat.Format = "#,##0";
                workSheet.Column(10).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(11).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(13).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(14).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(15).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(16).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(17).Style.NumberFormat.Format = "#,##0";
                workSheet.Column(19).Style.NumberFormat.Format = "$ #,##0.00";
                workSheet.Column(20).Style.NumberFormat.Format = "$ #,##0.00";

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    var content = stream.ToArray();

                    string Nombre;

                    if (FechaInicio == "undefined" && FechaInicio == "undefined")
                    {
                        Nombre = ("Ventas y cuentas por cobrar del sorteo " + raffleId + ".xlsx").ToString();
                    }
                    else if (FechaInicio != "undefined" && FechaInicio != "undefined" && raffleId != 0)
                    {
                        DateTime FI = Convert.ToDateTime(FechaInicio);
                        DateTime FF = Convert.ToDateTime(FechaFin);

                        Nombre = ("Ventas y cuentas por cobrar del sorteo " + raffleId + " desde " + FI.ToString("dd-MM-yyyy") + " hasta " + FF.ToString("dd-MM-yyyy") + ".xlsx").ToString();
                    }
                    else
                    {
                        DateTime FI = Convert.ToDateTime(FechaInicio);
                        DateTime FF = Convert.ToDateTime(FechaFin);

                        Nombre = ("Ventas y cuentas por cobrar desde " + FI.ToString("dd-MM-yyyy") + " hasta " + FF.ToString("dd-MM-yyyy") + ".xlsx").ToString();
                    }

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Nombre);
                }
            }
        }

        //NUEVO CODIGO PARA GENERAR XML DE LOS PREMIOS
        //GET: /Integration/AllocationNumbers
        [Authorize]
        [HttpGet]
        public JsonResult RaffleAwards(int ClientId, int RaffleId)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        PayableAwardByClientProcedure payableAwardByClientProcedure = new PayableAwardByClientProcedure();
                        var Resultado = payableAwardByClientProcedure.ConsultaBilletesPagablesPorCliente(RaffleId);

                        var DataPremios = Resultado.Where(w => w.ClientId == ClientId).ToList();
                        var PremiosCliente = Resultado.Where(w => w.ClientId == ClientId).Select(s => s.Number).ToList();

                        var raffleData = context.Raffles.Where(w => w.Id == RaffleId).FirstOrDefault();

                        var awardNumbesXML = new Models.XML.AwardNumbesXML()
                        {
                            RaffleId = RaffleId,
                            RaffleName = raffleData.Name,
                            RaffleDate = raffleData.DateSolteo.ToShortDateString(),
                            CreateDate = DateTime.Now.ToString(),
                            TicketNumbers = new List<Models.XML.TicketNumber>()
                        };
                        var FractionTo = raffleData.Prospect.LeafFraction * raffleData.Prospect.LeafNumber;
                        (from a in raffleData.RaffleAwards.Where(w => PremiosCliente.Contains((int)w.ControlNumber)).ToList()
                         select new
                         {
                             TicketNumber = Utils.AddZeroToNumber((raffleData.Prospect.Production - 1).ToString().Length, (int)a.ControlNumber),
                             Allocation = DataPremios.FirstOrDefault(f => f.Number == a.ControlNumber).TaId,
                             IdNumber = DataPremios.FirstOrDefault(f => f.Number == a.ControlNumber).TanId,
                             FractionFrom = 1,
                             FractionTo,
                             AvailableFractions = DataPremios.FirstOrDefault(f => f.Number == a.ControlNumber).Fracciones,
                             Award = new
                             {
                                 AwardId = a.Id,
                                 AwardName = a.Award.Name,
                                 AwardFractionPrice = (a.Award.Value / (raffleData.Prospect.LeafFraction * raffleData.Prospect.LeafNumber)),
                                 AwardPrice = a.Award.Value,
                                 ValueToPay = DataPremios.FirstOrDefault(f => f.Number == a.ControlNumber).ValorPagar,
                                 AvailableFractions = DataPremios.FirstOrDefault(f => f.Number == a.ControlNumber).Fracciones
                             }
                         }).GroupBy(r => r.TicketNumber).ToList().ForEach(r => awardNumbesXML.TicketNumbers.Add(new Models.XML.TicketNumber()
                         {
                             TiketNumber = r.FirstOrDefault().TicketNumber,
                             Allocation = r.FirstOrDefault().Allocation,
                             IdNumber = r.FirstOrDefault().IdNumber,
                             FractionFrom = r.FirstOrDefault().FractionFrom,
                             FractionTo = r.FirstOrDefault().FractionTo,
                             Awards = r.Select(a => new Models.XML.Award()
                             {
                                 AwardId = a.Award.AwardId,
                                 AwardName = a.Award.AwardName,
                                 AwardValue = a.Award.AwardPrice,
                                 AwardPerFraction = a.Award.AwardFractionPrice,
                                 AvailableFractions = a.AvailableFractions,
                                 AwardToPay = a.Award.ValueToPay
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

                        //var fileName = Guid.NewGuid().ToString() + ".xml";

                        var fileName = "Numeros_ganadores_del_sorteo_" + RaffleId + "_del_cliente_numero_" + ClientId + ".xml";

                        xmlDoc.Save(patch + "/" + fileName);

                        return new System.Web.Mvc.JsonResult()
                        {
                            JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                            Data = new
                            {
                                result = true,
                                message = "Números ganadores descargado en XML Correctamente.",
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
                        //var raffle = context.Raffles.Where(r => r.Id == allocation.RaffleId).FirstOrDefault();
                        //var ClientControlNumber = context.Clients.Where(c => c.Id == allocation.ClientId).Select(c => c.ControlNumber).FirstOrDefault();
                        var Production = allocation.Raffle.Prospect.Production.ToString().Length - 1;

                        var allocationXML = new Models.XML.TicketAllocateXML();

                        allocationXML = new Models.XML.TicketAllocateXML()
                        {
                            RaffleId = allocation.RaffleId,
                            RaffleName = allocation.Raffle.Name,
                            FractionPrice = allocation.Raffle.Prospect.Price,
                            TicketPrice = ((allocation.Raffle.Prospect.LeafFraction * allocation.Raffle.Prospect.LeafNumber) * allocation.Raffle.Prospect.Price),
                            RaffleDate = allocation.Raffle.DateSolteo.ToShortDateString(),
                            StopSales = allocation.Raffle.EndReturnDate.AddHours(-1).ToString(),
                            CreateDate = DateTime.Now.ToString(),
                            Allocation = id,
                            TicketAllocationNumbers = new List<Models.XML.TicketAllocationNumber>()
                        };

                        DateTime actualDate = DateTime.Now;

                        /*if (allocation.Statu != (int)AllocationStatuEnum.Generated)
                        {*/
                        //const int length = 8;
                        //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                        context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == id).OrderBy(o => o.Number)
                        .Select(s => new { s.Id, s.Number, s.ControlNumber, s.FractionFrom, s.FractionTo }).ToList()
                        .ForEach(f => allocationXML.TicketAllocationNumbers.Add(new Models.XML.TicketAllocationNumber()
                        {
                            IdNumber = f.Id,
                            TicketNumber = f.Number.ToString().PadLeft(Production, '0'),
                            ControlNumber = f.ControlNumber,
                            //ControlNumber = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                            FractionFrom = f.FractionFrom,
                            FractionTo = f.FractionTo
                        }));

                        /*allocation.TicketAllocationNumbers.ToList().ForEach(f =>
                        {
                            f.ControlNumber = allocationXML.TicketAllocationNumbers.Where(w => w.IdNumber == f.Id).FirstOrDefault().ControlNumber;
                            f.PrintedDate = actualDate;
                        });*/
                        
                        if(allocation.Statu != (int)AllocationStatuEnum.Generated)
                        {
                            allocation.Statu = (int)AllocationStatuEnum.Generated;
                            context.SaveChanges();
                            tx.Commit();
                        }

                        //}
                        /*else
                        {
                            context.TicketAllocationNumbers.OrderBy(o => o.Number).Where(w => w.TicketAllocationId == id)
                            .Select(s => new { s.Id, s.Number, s.ControlNumber, s.FractionFrom, s.FractionTo }).ToList()
                            .ForEach(f => allocationXML.TicketAllocationNumbers.Add(new Models.XML.TicketAllocationNumber()
                            {
                                IdNumber = f.Id,
                                TicketNumber = f.Number.ToString().PadLeft(Production, '0'),
                                ControlNumber = f.ControlNumber,
                                FractionFrom = f.FractionFrom,
                                FractionTo = f.FractionTo
                            }));
                        }*/

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

                        var fileName = "Asignacion_" + id + "_" + actualDate.Year + actualDate.Month + actualDate.Day + "_" + allocation.ClientId + ".xml";

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
                                TicketNumber = Utils.AddZeroToNumber((raffle.Prospect.Production - 1).ToString().Length, (int)t.Number),
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
                             FractionTo,
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
                                 AwardPerFraction = a.Award.AwardFractionPrice,
                                 AwardId = a.Award.AwardId,
                                 AwardName = a.Award.AwardName,
                                 AwardValue = a.Award.AwardPrice
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
                                Statu = (int)TicketStatusEnum.Printed
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
