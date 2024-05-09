using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.ModelsProcedures.Raffle;
using Tickets.Models.Procedures;
using Tickets.Models.Procedures.Allocations;
using Tickets.Models.Procedures.PayableAward;
using Tickets.Models.Procedures.Raffle;
using Tickets.Models.Procedures.Receivables;
using Tickets.Models.Procedures.Returns;

namespace Tickets.Controllers
{
    
    [InitializeSimpleMembership]
    public class ReportsController : Controller
    {
        //
        //  GET: Reports/ReportPaymenHistory
        [HttpGet]
        
        public ActionResult ReportPaymenHistory(int raffleId = 0, string fecha = null, int clientId = 0)
        {
            if (!String.IsNullOrEmpty(fecha))
            {
                DateTime FechaConvert = DateTime.ParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var context = new TicketsEntities();
                var invoices = context.Invoices.Where(i =>
                   (i.RaffleId == raffleId || raffleId == 0)
                   && (i.ClientId == clientId || clientId == 0)
                   && (i.InvoiceDate == FechaConvert)
                   && i.ReceiptPayments.Count > 0).ToList();
                return View(invoices);
            }
            else
            {
                var context = new TicketsEntities();
                var invoices = context.Invoices.Where(i =>
                   (i.RaffleId == raffleId || raffleId == 0)
                   && (i.ClientId == clientId || clientId == 0)
                   && i.ReceiptPayments.Count > 0).ToList();
                return View(invoices);
            }
        }

        //
        //  GET: Reports/DebtReport
        [HttpGet]
        
        public ActionResult DebtReport(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            var indentifybatchs = context.IdentifyBaches.Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)).ToList();
            return View(indentifybatchs);
        }

        [HttpGet]
        
        public ActionResult DataByDay(int allocationId, string dateSale)
        {
            var context = new TicketsEntities();
            var date = DateTime.Parse(dateSale);
            ViewBag.dateSale = date;
            var ticketAllocation = context.TicketAllocations.FirstOrDefault(w => w.Id == allocationId);
            return View(ticketAllocation);
        }

        // 
        //  GET: Reports/IndentifyReceivableReport
        [HttpGet]
        
        public ActionResult IndentifyReceivableReport(int creditNoteId = 0, int paymentId = 0)
        {
            var context = new TicketsEntities();
            PaymentReceivableReportModel payment;
            if (paymentId > 0)
            {
                payment = context.IdentifyBachPayments.Where(p => p.Id == paymentId).Select(p => new PaymentReceivableReportModel
                {
                    ClientName = p.ClientId + " - " + p.Client.Name,
                    PaymentType = "PAGO EN EFECTIVO ",// + p.Id,
                    SequenceNumberPayment = p.SequenceNumber,
                    Value = p.Value,
                    Id = p.Id,
                    UserId = p.CreateUser,
                    RaffleId = p.IdentifyBach.RaffleId,
                    CreateDate = p.CreateDate,
                    IdentifyBach = p.IdentifyBach,
                    ReceivablePercent = p.DiscountPercent,
                    Production = p.IdentifyBach.Raffle.Prospect.Production - 1,
                }).FirstOrDefault();
            }
            else
            {
                payment = context.NoteCredits.AsEnumerable().Where(c => c.Id == creditNoteId).Select(n => new PaymentReceivableReportModel
                {
                    ClientName = n.ClientId + " - " + n.Client.Name,
                    PaymentType = "PAGO CON NOTA DE CREDITO " + n.Nomenclature == null ? n.SequenceNumber.Value.ToString() : string.Concat(n.Nomenclature, "-", n.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                    SequenceNumberPayment = n.SequenceNumber,
                    Value = n.TotalCash,
                    Id = n.Id,
                    Nomenclature = n.Nomenclature,
                    UserId = n.CreateUser,
                    RaffleId = n.IdentifyBaches.FirstOrDefault().RaffleId,
                    CreateDate = n.CreateDate,
                    IdentifyBach = n.IdentifyBaches.FirstOrDefault(),
                    ReceivablePercent = n.DiscountPercent,
                    CreditNoteId = creditNoteId,
                    Concept = n.Concepts,
                    Production = n.IdentifyBaches.FirstOrDefault().Raffle.Prospect.Production - 1,
                }).FirstOrDefault();
            }
            return View(payment);
        }

        // 
        //  GET: Reports/GenerarRaffle
        [HttpGet]
        
        public ActionResult GenerarRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/PreviousDebtPaymentReport
        [HttpGet]
        
        public ActionResult PreviousDebtPaymentReport(int paymentId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            PreviousDebtPayment previousDebtPayment;
            if (paymentId > 0)
            {
                previousDebtPayment = context.PreviousDebtPayments.FirstOrDefault(r => r.Id == paymentId);
            }
            else
            {
                previousDebtPayment = context.PreviousDebtPayments.FirstOrDefault(r => r.ClientId == clientId);
            }
            if (previousDebtPayment == null)
            {
                return RedirectToAction("Error", new { message = "Este cliente no tiene pagos por deudas anterior." });
            }
            return View(previousDebtPayment);
        }

        //
        //  GET: Reports/CXCReport
        
        [HttpGet]
        public ActionResult CXCReport(int clientId, DateTime startDate, DateTime endDate)
        {
            var context = new TicketsEntities();
            var invoices = context.Invoices.AsEnumerable().Where(i =>
                    clientId == i.ClientId
                    && i.CreateDate.Date >= startDate.Date && i.CreateDate.Date <= endDate.Date
                ).ToList();
            return View(invoices);
        }

        //
        //  GET: Reports/ReceivableReport
        
        [HttpGet]
        public ActionResult ReceivableReport(int invoiceId)
        {
            var context = new TicketsEntities();
            var invoice = context.Invoices.FirstOrDefault(r => r.Id == invoiceId);
            return View(invoice);
        }

        // 
        //  GET: Reports/SpecialAwardInline
        [HttpGet]
        
        public ActionResult SpecialAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/IntermAwardInline
        [HttpGet]
        
        public ActionResult IntermAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/MinorAwardInline
        [HttpGet]
        
        public ActionResult MinorAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/ShowDocument
        [HttpGet]
        
        public ActionResult ShowDocument(int clientId, string type)
        {
            var context = new TicketsEntities();
            var client = context.Clients.FirstOrDefault(c => c.Id == clientId);
            List<string> base64String;
            string fileName = "";
            if (type == "admin")
            {
                base64String = client.AdminDocument.Split(",".ToCharArray()).ToList();
                fileName = "Documento Administrativo";
            }
            else
            {
                base64String = client.AdminDocument.Split(",".ToCharArray()).ToList();
                fileName = "Documento de Fianza";
            }
            byte[] byteArray = Convert.FromBase64String(base64String[1]);
            return File(byteArray, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName + ".pdf");
        }

        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        // 
        //  GET: Reports/PrintImage
        [HttpGet]
        
        public JsonResult PrintImage(int raffleId)
        {
            var path = Server.MapPath("~") + "/generalRaffle";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, raffleId + ".jpeg");
            if (System.IO.File.Exists(path))
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        result = true,
                        fileName = raffleId + ".jpeg"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            try
            {
                var context = new TicketsEntities();
                Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
                var stringHtml = RenderRazorViewToString("GenerarRaffle", raffle);

                var htmlToImageConv = new NReco.ImageGenerator.HtmlToImageConverter
                {
                    Zoom = 5.0f
                };
                var bytes = htmlToImageConv.GenerateImage(stringHtml, ImageFormat.Jpeg.ToString());

                Image img = System.Drawing.Image.FromStream(new MemoryStream(bytes));
                img.Save(path);

                return new JsonResult()
                {
                    Data = new
                    {
                        result = true,
                        fileName = raffleId + ".jpeg"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        result = false,
                        message = e.InnerException + " --- " + e.Message + " --- " + e.StackTrace
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        //
        //  GET: Reports/ProspectFormat
        
        [HttpGet]
        public ActionResult ProspectFormat(int prospectId)
        {
            var context = new TicketsEntities();
            var prospect = context.Prospects.FirstOrDefault(p => p.Id == prospectId);
            return View(prospect);
        }

        //
        //  GET: Reports/BilletesAnulados
        
        [HttpGet]
        public ActionResult BilletesAnulados(int raffleId)
        {
            var context = new TicketsEntities();
            var tickets = context.TicketAllocationNumbers.Where(w => w.RaffleId == raffleId && w.Statu == (int)TicketStatusEnum.Anulated);
            return View(tickets);
        }

        //
        //  GET: Reports/PrintedNumbesForRaffle
        
        [HttpGet]
        public ActionResult PrintedNumbesForRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/PrintedNumbersNotInvoiced
        
        [HttpGet]
        public ActionResult PrintedNumbersNotInvoiced(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/NoPrintedNumbesForRaffle
        
        [HttpGet]
        public ActionResult NoPrintedNumbesForRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        
        [HttpGet]
        public ActionResult BilletesVendidos(int raffleId)
        {
            Procedure_InvoicedTickets invoicedTicketsProcedure = new Procedure_InvoicedTickets();
            var Resultado = invoicedTicketsProcedure.ConsultaBilletesVendidos(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult BilletesDevueltos(int raffleId)
        {
            Procedure_ReturnedTickets returnedTicketsProcedure = new Procedure_ReturnedTickets();
            var Resultado = returnedTicketsProcedure.ConsultaBilletesDevueltos(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult ClientDetails(int ClientId)
        {
            var context = new TicketsEntities();
            var client = context.Clients.FirstOrDefault(r => r.Id == ClientId);
            return View(client);
        }

        
        [HttpGet]
        public ActionResult DatosPorCliente(int raffleId)
        {
            Procedure_NetSalesByClient netSalesByClientProcedure = new Procedure_NetSalesByClient();
            var Resultado = netSalesByClientProcedure.ConsultaVentaNetaPorCliente(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult PayableAwardsByClient(int raffleId)
        {
            Procedure_PayableAward payableAwardProcedure = new Procedure_PayableAward();
            var Resultado = payableAwardProcedure.ConsultaBilletesPagables(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult PayableAwards(int raffleId)
        {
            Procedure_PayableAwards payableAwardsProcedure = new Procedure_PayableAwards();
            var Resultado = payableAwardsProcedure.ConsultaBilletesPagables(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult AllPayableAwards(int raffleId)
        {
            Procedure_AllPayableAwards allPatyableAwardsProcedure = new Procedure_AllPayableAwards();
            var Resultado = allPatyableAwardsProcedure.ConsultaTodosBilletesPagables(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult PayNotPay(int raffleId)
        {
            Procedure_PayableAwardSummary procedurePayableAwardSummary = new Procedure_PayableAwardSummary();
            var Resultado = procedurePayableAwardSummary.payableAwardSummary(raffleId);
            return View(Resultado);
        }

        public ActionResult AllocatedSummary(int raffleId)
        {
            Procedure_AllocatedSummary allocatedSummaryProcedure = new Procedure_AllocatedSummary();
            var Resultado = allocatedSummaryProcedure.AllocatedSummary(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult ApprovedBach(int bachId)
        {
            var context = new TicketsEntities();

            using (var dbContextTransaction = context.Database.BeginTransaction())
            {
                try
                {
                    var bach = context.IdentifyBaches.FirstOrDefault(r => r.Id == bachId);
                    bach.Statu = (int)BachIdentifyStatuEnum.Approved;
                    context.SaveChanges();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                }

                dbContextTransaction.Commit();
            }

            var bachApproved = context.IdentifyBaches.FirstOrDefault(r => r.Id == bachId);

            return View(bachApproved);
        }

        
        [HttpGet]
        public ActionResult ElectronicSaleDetails(int allocationId)
        {
            Procedure_AvailableTickets availableTicketsProcedure = new Procedure_AvailableTickets();
            var Resultado = availableTicketsProcedure.ConsultaBilletesDisponible(allocationId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult BilletesCirculacion(int raffleId)
        {
            Procedure_AvailableTickets availableTicketsProcedure = new Procedure_AvailableTickets();
            var Resultado = availableTicketsProcedure.ConsultaBilletesDisponible(raffleId);
            return View(Resultado);
        }

        
        [HttpGet]
        public ActionResult AllocationConsigNumberList(int allocationId)
        {
            var context = new TicketsEntities();
            var allocation = context.TicketAllocations.FirstOrDefault(r => r.Id == allocationId);
            return View(allocation);
        }

        //
        //  GET: Reports/NoPrintedNumbesAward
        
        [HttpGet]
        public ActionResult NoPrintedNumbesAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/InvoicedNumbersAward
        
        [HttpGet]
        public ActionResult InvoicedNumbersAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbersAward
        
        [HttpGet]
        public ActionResult ReturnedNumbersAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //GET: Reports/ReturnedGroupAwards
        
        [HttpGet]
        public ActionResult ReturnedGroupAwards(int raffleId, string groupId = "")
        {
            Procedure_DevolucionesPremiadas devolucionesPremiadasProcedure = new Procedure_DevolucionesPremiadas();
            var Resultado = devolucionesPremiadasProcedure.ConsultaDevolucionesPremiadas(raffleId, groupId);
            return View(Resultado);

            /*var context = new TicketsEntities();
            var returnedList = context.TicketReturns.Where(i =>
                i.RaffleId == raffleId).ToList();
            if (groupId != "")
            {
                var returned = returnedList.Where(i => (int.Parse(Regex.Match(i.ReturnedGroup, @"\d+").Value) == int.Parse(groupId))
            ).ToList();
                if (returned.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron datos para los criterios seleccionados." });
                }
                return View(returned);
            }
            else
            {
                var returned = returnedList.Where(i =>
                i.RaffleId == raffleId
                || groupId == "")
                .ToList();
                if (returned.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron datos para los criterios seleccionados." });
                }
                return View(returned);
            }*/
        }

        //
        //  GET: Reports/NumbesAward
        
        [HttpGet]
        public ActionResult NumbesAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ClientNumbersAward
        
        [HttpGet]
        public ActionResult ClientNumbersAward(int? raffleId, int clientId)
        {
            var context = new TicketsEntities();
            ViewBag.clientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/NumbesAwardExpired
        
        [HttpGet]
        public ActionResult NumbesAwardExpired(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/MajorAwardInline
        // 
        [AllowAnonymous]
        [HttpGet]
        public ActionResult MajorAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/RaffleGeneralOver
        
        [HttpGet]
        public ActionResult RaffleGeneralOver(int raffleId)
        {
            Procedure_CuadreSorteoResumido cuadreSorteoResumidoProcedure = new Procedure_CuadreSorteoResumido();
            var resultado = cuadreSorteoResumidoProcedure.CuadreSorteo(raffleId);
            return View(resultado);

            /*var context = new TicketsEntities();
            var raffle = context.Raffles.Where(r => r.Id == raffleId).Select(r => new RaffleModel()
            {
                Id = r.Id,
                RaffleDate = r.DateSolteo,
                TicketProspectId = r.ProspectId,
                TicketProspect = new ProspectModel()
                {
                    LeafFraction = r.Prospect.LeafFraction,
                    LeafNumber = r.Prospect.LeafNumber
                }
            }).FirstOrDefault();

            return View(raffle);*/
        }

        //
        //  GET: Reports/RaffleGeneralOverR
        
        [HttpGet]
        public ActionResult RaffleGeneralOverR(int raffleId)
        {
            Procedure_CuadreSorteoResumido cuadreSorteoResumidoProcedure = new Procedure_CuadreSorteoResumido();
            var resultado = cuadreSorteoResumidoProcedure.CuadreSorteo(raffleId);
            return View(resultado);

            /*var context = new TicketsEntities();
            var raffle = context.Raffles.Where(r => r.Id == raffleId).Select(r => new RaffleModel()
            {
                Id = r.Id,
                RaffleDate = r.DateSolteo,
                TicketProspectId = r.ProspectId,
                TicketProspect = new ProspectModel()
                {
                    LeafFraction = r.Prospect.LeafFraction,
                    LeafNumber = r.Prospect.LeafNumber
                }
            }).FirstOrDefault();
            return View(raffle);*/
        }

        //
        //  GET: Reports/ReturnedNumbers
        
        [HttpGet]
        public ActionResult ReturnedNumbers(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/SpecificReturnedFractions
        
        [HttpGet]
        public ActionResult SpecificReturnedFractions(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbersGroup
        
        [HttpGet]
        public ActionResult ReturnedNumbersGroup(int raffleId = 0, int clientId = 0)
        {
            Procedure_ReturnedByGroup returnedByGroupProcedure = new Procedure_ReturnedByGroup();
            var Resultado = returnedByGroupProcedure.ConsultarBilletesDevueltosPorGrupo(raffleId);
            return View(Resultado);
        }

        //
        //  GET: Reports/ReturnedNumbersClient
        
        [HttpGet]
        public ActionResult ReturnedNumbersClient(int raffleId)
        {
            Procedure_ReturnedNumbersByClient returnedNumbersByClient = new Procedure_ReturnedNumbersByClient();
            var Resultado = returnedNumbersByClient.ConsultarBilletesDevueltosPorCliente(raffleId);
            return View(Resultado);
        }

        //
        //  GET: Reports/AllocatinosNumbers
        
        [HttpGet]
        public ActionResult AllocatinosNumbers(int raffleId = 0, int clientId = 0, int allocationId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            ViewBag.AllocationId = allocationId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/AllocationSummary
        
        [HttpGet]
        public ActionResult AllocationSummary(int raffleId = 0)
        {
            Procedure_AllocationSummary allocationSummaryProcedure = new Procedure_AllocationSummary();
            var Resultado = allocationSummaryProcedure.ConsultaAsignacionesSorteo(raffleId);
            return View(Resultado);
        }

        //
        //  GET: Reports/ReturnsSummary
        
        [HttpGet]
        public ActionResult ReturnsSummary(int raffleId = 0)
        {
            Procedure_ReturnsSummary returnsSummaryProcedure = new Procedure_ReturnsSummary();
            var Resultado = returnsSummaryProcedure.ReturnedSummary(raffleId);
            return View(Resultado);
        }

        //
        //  GET: Reports/AllocatinosNumberList
        
        [HttpGet]
        public ActionResult AllocatinosNumberList(int allocationId)
        {
            var context = new TicketsEntities();
            var ticketAllocation = context.TicketAllocations.FirstOrDefault(r => r.Id == allocationId);
            return View(ticketAllocation);
        }

        //
        //  GET: Reports/InvoiceListPrint
        
        [HttpGet]
        public ActionResult InvoicesByRaffle(int raffleId = 0, int clientId = 0, int invoiceId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            ViewBag.invoiceId = context.Invoices.Any(a => a.Id == invoiceId) ? context.Invoices.Find(invoiceId).SequenceNumber : invoiceId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/InvoicesByRaffleAllocation
        
        [HttpGet]
        public ActionResult InvoicesByRaffleAllocation(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/Error
        
        [HttpGet]
        public ActionResult Error(string message)
        {
            ViewBag.errorMessage = message;
            return View();
        }

        //
        //  GET: Reports/InvoiceDetail
        
        [HttpGet]
        public ActionResult InvoiceDetail(int invoiceId)
        {
            var context = new TicketsEntities();
            var invoice = context.Invoices.FirstOrDefault(r => r.Id == invoiceId);
            return View(invoice);
        }

        //
        //  GET: Reports/InvoiceDetail
        
        [HttpGet]
        public ActionResult InvoiceDetailUpdate(int invoiceId)
        {
            var context = new TicketsEntities();
            var invoice = context.Invoices.FirstOrDefault(r => r.Id == invoiceId);
            return View(invoice);
        }

        
        [HttpGet]
        public ActionResult InvoicePaymentInfo(int paymentId)
        {
            var context = new TicketsEntities();
            var payment = context.ReceiptPayments.FirstOrDefault(f => f.Id == paymentId);
            return View(payment);
        }

        
        [HttpGet]
        public ActionResult OtherPaymentReceipt(int paymentId)
        {
            var context = new TicketsEntities();
            var payment = context.OtherIncomeDetails.FirstOrDefault(f => f.Id == paymentId);
            return View(payment);
        }

        [HttpGet]
        public ActionResult OtherPaymentGroup(int groupId)
        {
            var context = new TicketsEntities();
            var payment = context.OtherIncomesGroups.FirstOrDefault(f => f.Id == groupId);
            return View(payment);
        }

        
        [HttpGet]
        public ActionResult CashAdvanceReport(int cashAdvance)
        {
            var context = new TicketsEntities();
            var cash = context.NoteCredits.FirstOrDefault(f => f.Id == cashAdvance);
            return View(cash);
        }

        
        [HttpGet]
        public ActionResult PositiveBalanceReport(int positiveBalance)
        {
            var context = new TicketsEntities();
            var cash = context.NoteCredits.FirstOrDefault(f => f.Id == positiveBalance);
            return View(cash);
        }

        
        [HttpGet]
        public ActionResult InvoicePayment(int paymentId)
        {
            var context = new TicketsEntities();
            var payment = context.ReceiptPayments.FirstOrDefault(f => f.Id == paymentId);
            return View(payment);
        }

        
        [HttpGet]
        public ActionResult NoteCreditDetail(int noteCreditId)
        {
            var context = new TicketsEntities();
            var notecredit = context.NoteCredits.FirstOrDefault(r => r.Id == noteCreditId);
            return View(notecredit);
        }

        
        [HttpGet]
        public ActionResult NoteCreditTaxReceiptDetail(int noteCreditId)
        {
            var context = new TicketsEntities();
            var notecredit = context.NoteCredits.FirstOrDefault(r => r.Id == noteCreditId);
            return View(notecredit);
        }

        //
        //  GET: Reports/ReturnedDeatils
        
        [HttpGet]
        public ActionResult ReturnedDeatils(int raffleId, string group = "", int clientId = 0, int statu = (int)TicketReturnedStatuEnum.Created)
        {
            var context = new TicketsEntities();
            var returneds = context.TicketReturns.Where(r =>
                r.RaffleId == raffleId
                && (r.ReturnedGroup == group || group == "")
                && r.Statu == statu
                && (r.ClientId == clientId || clientId == 0)).ToList();
            return View(returneds);
        }

        //
        //  GET: Reports/Tickets
        
        [HttpGet]
        public ActionResult Tickets(int allocationId)
        {
            var context = new TicketsEntities();
            var ticketAllocation = context.TicketAllocations.FirstOrDefault(r => r.Id == allocationId);
            if (ticketAllocation.Statu == (int)AllocationStatuEnum.Printed)
            {
                return RedirectToAction("Error", new { message = "Los Billetes de la asignación " + allocationId + " fueron impreso." });
            }

            var allocation = context.TicketAllocations.Where(a => a.Id == allocationId).FirstOrDefault();
            allocation.Statu = (int)AllocationStatuEnum.Printed;
            context.SaveChanges();

            return View(ticketAllocation);
        }

        //
        //  GET: Reports/TicketsChristmas
        
        [HttpGet]
        public ActionResult TicketsChristmas(int allocationId)
        {
            var context = new TicketsEntities();
            var ticketAllocation = context.TicketAllocations.FirstOrDefault(r => r.Id == allocationId);
            if (ticketAllocation.Statu == (int)AllocationStatuEnum.Printed)
            {
                return RedirectToAction("Error", new { message = "Los Billetes de la asignación " + allocationId + " fueron impreso." });
            }

            var allocation = context.TicketAllocations.Where(a => a.Id == allocationId).FirstOrDefault();
            allocation.Statu = (int)AllocationStatuEnum.Printed;
            context.SaveChanges();

            return View(ticketAllocation);
        }

        //
        //  GET: Reports/CashReport
        
        [HttpGet]
        public ActionResult CashReport(int IdentifyBachId)
        {
            var context = new TicketsEntities();
            var identifyBatch = context.IdentifyBaches.FirstOrDefault(i => i.Id == IdentifyBachId);
            return View(identifyBatch);
        }

        //
        //  GET: Reports/NumberCertification
        
        [HttpGet]
        public ActionResult NumberCertification(int CertificationNumberId)
        {
            var context = new TicketsEntities();
            var certification = context.CertificationNumbers.FirstOrDefault(i => i.Id == CertificationNumberId);
            return View(certification);
        }

        //
        //  GET: Reports/AwardCertification
        
        [HttpGet]
        public ActionResult AwardCertification(int CertificationNumberId)
        {
            var context = new TicketsEntities();
            var certification = context.AwardCertification.FirstOrDefault(i => i.Id == CertificationNumberId);
            return View(certification);
        }

        //
        //  GET: Reports/ReturnedGroupClientDetails
        
        [HttpGet]
        public ActionResult ReturnedGroupClientDetails(int raffleId, int clientId, string group)
        {
            Procedure_ReturnedGroupByClient procedure_ReturnedGroupByClient = new Procedure_ReturnedGroupByClient();
            var Resultado = procedure_ReturnedGroupByClient.ReturnedGroupByClient(raffleId, clientId, group);
            return View(Resultado);
        }

        //
        //  GET: Reports/GetCashReport
        
        [HttpGet]
        public ActionResult GetCashReport(int userId, string Fecha)
        {
            var context = new TicketsEntities();

            DateTime FechaConvert = DateTime.Parse(Fecha);

            List<ReceiptPayment> receiptPayment = new List<ReceiptPayment>();

            if (userId == 0)
            {
                receiptPayment = context.ReceiptPayments.AsEnumerable().Where(i => i.ReceiptDate.Date == FechaConvert.Date).ToList();
            }
            else
            {
                receiptPayment = context.ReceiptPayments.AsEnumerable().Where(i => i.CreateUser == userId && i.ReceiptDate.Date == FechaConvert.Date).ToList();
            }

            if (receiptPayment.Count <= 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron pagos de este usuario." });
            }

            return View(receiptPayment);
        }

        //
        //  GET: Reports/InvoiceDeatilCashReport
        
        [HttpGet]
        public ActionResult InvoiceDeatilCashReport(int invoiceDetailId)//int raffleId, int userId, DateTime startDate, DateTime endDate)
        {
            var context = new TicketsEntities();
            var invoiceDetail = context.InvoiceDetails.Where(i => i.Id == invoiceDetailId).FirstOrDefault();

            var awards = context.RaffleAwards.Where(a => a.RaffleId == invoiceDetail.RaffleId).ToList();
            List<IdentifyNumber> identifyNumbers = new List<IdentifyNumber>();
            context.IdentifyBaches.AsEnumerable().Where(i =>
                i.RaffleId == invoiceDetail.RaffleId &&
                i.ClientId == invoiceDetail.ClientId &&
                (i.IdentifyBachPayments.Any(p => p.CreateDate.Date >= invoiceDetail.StartDate.Date && p.CreateDate.Date <= invoiceDetail.EndDate.Date) == true)
                && (Utils.IdentifyBachIsPayedMinor(i, awards) || Utils.IdentifyBachIsPayedMayor(i, awards))
            ).ToList().ForEach(i => identifyNumbers.AddRange(i.IdentifyNumbers));

            if (identifyNumbers.Count <= 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron pagos del sorteo " + invoiceDetail.Raffle.Symbol + invoiceDetail.Raffle.Separator + invoiceDetail.Raffle.SequenceNumber + "." });
            }
            ViewBag.invoiceDetailId = invoiceDetail.SequenceNumber;
            ViewBag.invoiceDetailDate = invoiceDetail.CreateDate.ToShortDateString();
            ViewBag.invoiceDetailTime = invoiceDetail.CreateDate.ToShortTimeString();
            return View(identifyNumbers);
        }

        //
        //  GET: Reports/ReprintTickets
        
        [HttpGet]
        public ActionResult ReprintTickets(int reprintId)
        {
            var context = new TicketsEntities();
            var reprint = context.TicketRePrints.FirstOrDefault(r => r.Id == reprintId);

            /*if (reprint.Statu == (int)TicketReprintStatuEnum.Printed)
            {
                return RedirectToAction("Error", new { message = "Los Billetes de la reimpresion " + reprintId + " fueron impreso." });
            }

            reprint.Statu = (int)AllocationStatuEnum.Printed;
            context.SaveChanges();*/

            return View(reprint);
        }

        //
        //  GET: Reports/ReprintTicketsChristmas
        
        [HttpGet]
        public ActionResult ReprintTicketsChristmas(int reprintId)
        {
            var context = new TicketsEntities();
            var reprint = context.TicketRePrints.FirstOrDefault(r => r.Id == reprintId);
            if (reprint.Statu == (int)TicketReprintStatuEnum.Printed)
            {
                return RedirectToAction("Error", new { message = "Los Billetes de la reimpresion " + reprintId + " fueron impreso." });
            }

            reprint.Statu = (int)AllocationStatuEnum.Printed;
            context.SaveChanges();

            return View(reprint);
        }

        
        [HttpGet]
        public ActionResult TaxReceiptInfo(int? receiptId)
        {
            var context = new TicketsEntities();
            if (receiptId == 0 || receiptId == null)
            {
                return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
            }
            else
            {
                var receipt = context.TaxReceipts.FirstOrDefault(f => f.Id == receiptId);
                return View(receipt);
            }
        }

        
        [HttpGet]
        public ActionResult InvoicesDetail(int raffleId = 0, int taxtReceipt = 0)
        {
            var context = new TicketsEntities();
            if (raffleId == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
            }
            else
            {
                if (taxtReceipt != 0)
                {
                    var taxreceiptlist = context.TaxReceipts.Where(w => w.Type == taxtReceipt).Select(s => s.Id).ToList();
                    var taxreceiptnumberlist = context.TaxReceiptNumbers.Where(w => taxreceiptlist.Contains(w.TaxReceiptId)).Select(s => s.Id).ToList();
                    var invoices = context.Invoices.Where(w => w.RaffleId == raffleId && taxreceiptnumberlist.Contains((int)w.TaxReceipt)).ToList();

                    if (invoices.Count == 0)
                    {
                        return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                    }
                    ViewBag.TaxReceipt = taxtReceipt;
                    return View(invoices);
                }
                else
                {
                    var invoices = context.Invoices.Where(w => w.RaffleId == raffleId).ToList();

                    if (invoices.Count == 0)
                    {
                        return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                    }
                    ViewBag.TaxReceipt = taxtReceipt;
                    return View(invoices);
                }
            }
        }

        //
        //GET: Reports/AccountsReceivables
        
        [HttpGet]
        public ActionResult AccountsReceivables(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                var invoiceslist = context.Invoices.ToList();
                var invoices = invoiceslist.Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)).ToList();

                if (invoices.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(invoices);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                var invoices = context.Invoices.AsEnumerable().Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)
                && (i.InvoiceDate.Date >= startD.Date && i.InvoiceDate.Date <= endD.Date)
            ).ToList();

                if (invoices.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(invoices);
            }
        }

        //
        //GET: Reports/AccountsReceivablesByPeriod
        
        [HttpGet]
        public ActionResult AccountsReceivablesByPeriod(string startDate = "undefined", string endDate = "undefined", int raffleId = 0, int receivableType = 0)
        {
            Procedure_ReceivableClose receivableClose = new Procedure_ReceivableClose();
            ViewBag.startDate = Convert.ToDateTime(startDate).ToString("dd/MM/yyyy");
            ViewBag.endDate = Convert.ToDateTime(endDate).ToString("dd/MM/yyyy");
            var Resultado = receivableClose.ConsultaVentasCierre(startDate, endDate, raffleId, receivableType);
            return View(Resultado);
        }

        //
        //GET: Reports/InvoiceByPeriod
        
        [HttpGet]
        public ActionResult InvoiceByPeriod(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                var invoiceslist = context.Invoices.ToList();
                var invoices = invoiceslist.Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)).ToList();

                if (invoices.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(invoices);
            }
            else
            {
                ViewBag.startDate = Convert.ToDateTime(startDate).ToString("dd/MM/yyyy");
                ViewBag.endDate = Convert.ToDateTime(endDate).ToString("dd/MM/yyyy");
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                var invoices = context.Invoices.AsEnumerable().Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)
                && (i.InvoiceDate.Date >= startD.Date && i.InvoiceDate.Date <= endD.Date)).ToList();

                if (invoices.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(invoices);
            }
        }

        //
        //GET: Reports/PayedAwardByPeriod
        
        [HttpGet]
        public ActionResult PayedAwardByPeriod(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            var startD = DateTime.Parse(startDate);
            var endD = DateTime.Parse(endDate);

            var raffleData = context.Raffles.Find(raffleId);

            List<IdentifyNumber> identifyNumbers = new List<IdentifyNumber>();

            context.IdentifyBaches.AsEnumerable().Where(i =>
                (i.RaffleId == raffleId || raffleId == 0) && (i.ClientId == clientId || clientId == 0) &&
                ((i.IdentifyBachPayments.Any(p => p.CreateDate.Date >= startD.Date && p.CreateDate.Date <= endD.Date) == true) || (i.NoteCredits.Any(a => a.IdentifyBaches.Any(a2 => a2.Id == i.Id) && a.CreateDate.Date >= startD.Date && a.CreateDate.Date <= endD.Date) == true))
             ).ToList().ForEach(i => identifyNumbers.AddRange(i.IdentifyNumbers));

            if (identifyNumbers.Count <= 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron pagos del sorteo " + raffleData.SequenceNumber + "." });
            }

            return View(identifyNumbers);
        }

        //
        //GET: Reports/PayedElectronicAward
        
        [HttpGet]
        public ActionResult PayedElectronicAward(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            var startD = DateTime.Parse(startDate);
            var endD = DateTime.Parse(endDate);

            var electronicAwards = context.ElectronicAwardPayeds.AsEnumerable().Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)
                && (i.PayedDate.Value.Date >= startD.Date && i.PayedDate.Value.Date <= endD.Date)
                ).ToList();

            if (electronicAwards.Count == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron premios pagados para los criterios seleccionados." });
            }

            return View(electronicAwards);
        }

        //
        //GET: Reports/AccountsPayments
        
        [HttpGet]
        public ActionResult AccountsPayments(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var rafflesList = context.Raffles.ToList();
                var raffles = rafflesList.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (i.DateSolteo.Date >= startD.Date && i.DateSolteo.Date <= endD.Date)
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        
        [HttpGet]
        public ActionResult ReporteSorteo(string startDate = "undefined", string endDate = "undefined")
        {
            var context = new TicketsEntities();

            if (startDate == "undefined" && endDate == "undefined")
            {
                return RedirectToAction("Error", new { message = "La fecha de inicio y la fecha de fin son requeridas." });
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);

                var raffles = context.Raffles.AsEnumerable().Where(r => r.DateSolteo.Date >= startD.Date && r.DateSolteo.Date <= endD.Date).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron datos para las fechas seleccionadas." });
                }

                List<ModelProcedure_RaffleSales> modelProcedure_RaffleSales = new List<ModelProcedure_RaffleSales>();

                foreach (var raffle in raffles)
                {
                    Procedure_RaffleSales procedure_RaffleSales = new Procedure_RaffleSales();
                    var results = procedure_RaffleSales.RaffleSales(raffle.Id);
                    foreach (var result in results)
                    {
                        var data = new ModelProcedure_RaffleSales()
                        {
                            RaffleId = result.RaffleId,
                            RaffleDate = result.RaffleDate,
                            RaffleName = result.RaffleName,
                            Award = result.Award,
                            GrossSales = result.GrossSales,
                            NetSales = result.NetSales,
                            Order = result.Order
                        };
                        modelProcedure_RaffleSales.Add(data);
                    }
                }

                ViewBag.StartDate = startD.ToShortDateString();
                ViewBag.EndDate = endD.ToShortDateString();

                return View(modelProcedure_RaffleSales);
            }
        }

        //
        //GET: Reports/ClientGeneralReport
        
        [HttpGet]
        public ActionResult ClientGeneralReport(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (DbFunctions.TruncateTime(i.DateSolteo) >= DbFunctions.TruncateTime(startD) && DbFunctions.TruncateTime(i.DateSolteo) <= DbFunctions.TruncateTime(endD))
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        //
        //Get: ClientBalanceGeneralReport
        
        [HttpGet]
        public ActionResult ClientBalanceGeneralReport(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (DbFunctions.TruncateTime(i.DateSolteo) >= DbFunctions.TruncateTime(startD)
                && DbFunctions.TruncateTime(i.DateSolteo) <= DbFunctions.TruncateTime(endD))
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        //
        //GET: Reports/GeneralClientReport
        
        [HttpGet]
        public ActionResult GeneralClientReport(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var rafflesList = context.Raffles.ToList();
                var raffles = rafflesList.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (i.DateSolteo.Date >= startD.Date && i.DateSolteo.Date <= endD.Date)
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        //
        //GET: Reports/IdentifyAwardReportResumen
        
        [HttpGet]
        public ActionResult IdentifyAwardsReportResumen(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var rafflesList = context.Raffles.ToList();
                var raffles = rafflesList.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (i.DateSolteo.Date >= startD.Date && i.DateSolteo.Date <= endD.Date)
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        //
        //GET: Reports/IdentifyAwardReport
        
        [HttpGet]
        public ActionResult IdentifyAwardsReport(string startDate = "undefined", string endDate = "undefined", int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            if (startDate == "undefined" || endDate == "undefined")
            {
                ViewBag.clientId = clientId;
                var rafflesList = context.Raffles.ToList();
                var raffles = rafflesList.Where(i => (i.Id == raffleId || raffleId == 0)).ToList();
                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
            else
            {
                var startD = DateTime.Parse(startDate);
                var endD = DateTime.Parse(endDate);
                ViewBag.clientId = clientId;
                var raffles = context.Raffles.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (i.DateSolteo.Date >= startD.Date && i.DateSolteo.Date <= endD.Date)
            ).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
                }
                return View(raffles);
            }
        }

        //GET: Reports/VentaLoteria365
        
        [HttpGet]
        public ActionResult VentasLoteria365(int clientId, int raffleId)
        {
            var context = new TicketsEntities();
            var invoiceList = context.Invoices.AsEnumerable().Where(i =>
            i.RaffleId == raffleId
            && (i.ClientId == clientId || clientId == 0)
            ).ToList();
            if (invoiceList.Count == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron cuentas por cobrar para los criterios seleccionados." });
            }
            return View(invoiceList);
        }

        //
        //GET: Reports/RaffleDashboardReport
        
        [HttpGet]
        public ActionResult RaffleDashboardReport(DateTime startDate, DateTime endDate, int clientId = 0, int raffleId = 0)
        {
            var context = new TicketsEntities();
            var tickets = context.TicketAllocations.AsEnumerable().Where(i =>
                (i.Id == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)
                && (i.CreateDate.Date >= startDate.Date && i.CreateDate.Date <= endDate.Date)
            ).ToList();
            if (tickets.Count == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron datos para los criterios seleccionados." });
            }
            return View(tickets);
        }

        //
        //GET: Reports/AllocationSummaryBy
        
        [HttpGet]
        public ActionResult AllocationSummaryBy(int clientId, int raffleId)
        {
            var context = new TicketsEntities();
            var allocations = context.TicketAllocations.AsEnumerable().Where(i =>
                i.RaffleId == raffleId
                && (i.ClientId == clientId || clientId == 0)
            ).ToList();

            if (allocations.Count == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron asignaciones para los criterios seleccionados." });
            }

            return View(allocations);
        }

        //
        //GET: Reports/DeliveredAllocation
        
        [HttpGet]
        public ActionResult DeliveredAllocation(int clientId, int raffleId)
        {
            var context = new TicketsEntities();
            var allocations = context.TicketAllocations.AsEnumerable().Where(i => i.RaffleId == raffleId
                && (i.ClientId == clientId || clientId == 0) &&
                (i.Statu == (int)AllocationStatuEnum.Consigned || i.Statu == (int)AllocationStatuEnum.Delivered ||
                 i.Statu == (int)AllocationStatuEnum.Invoiced || i.Statu == (int)AllocationStatuEnum.Returned)
                 && (i.Client.GroupId != (int)ClientGroupEnum.DistribuidorElectronico || i.Client.GroupId != (int)ClientGroupEnum.DistribuidorXML))
                .OrderBy(o => o.Id).ToList();

            if (allocations.Count == 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron asignaciones para los criterios seleccionados." });
            }

            return View(allocations);
        }

        //
        //GET: Reports/TicketReprinted
        
        [HttpGet]
        public ActionResult TicketReprinted(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.Where(r => r.Id == raffleId).FirstOrDefault();
            if (raffle.TicketRePrints.Count == 0)
            {
                return RedirectToAction("Error", new { message = "El sorteo seleccionao no tiene numeros reimpresos." });
            }
            return View(raffle);
        }

        public ActionResult ProfitabilityReport(int raffleId)
        {
            //int raffleId = 3742;
            var context = new TicketsEntities();
            context.sp_ReporteRentabilidadProspecto(raffleId);
            var costos = context.ProductionCosts.Where(c => c.RaffleId == raffleId && c.Status == true).ToList();
            int currentRaffleIdCostos = 0;
            if (costos.Count == 0)
            {
                var lastRaffle = context.ProductionCosts.OrderByDescending(c => c.RaffleId).FirstOrDefault();
                currentRaffleIdCostos = lastRaffle.RaffleId;
                costos = context.ProductionCosts.Where(c => c.RaffleId == lastRaffle.RaffleId && c.Status == true).ToList();
                ViewBag.CostoTotal = context.sp_GETCostos(currentRaffleIdCostos).FirstOrDefault();
                ViewBag.Costos = costos;
            }
            else
            {
                ViewBag.Costos = costos;
                ViewBag.CostoTotal = context.sp_GETCostos(raffleId).FirstOrDefault();
            }

            var raffle = context.Raffles.Where(r => r.Id == raffleId).FirstOrDefault();
            Prospect current_p = null;
            if (raffle != null)
            {
                current_p = context.Prospects.Where(p => p.Id == raffle.ProspectId).FirstOrDefault();
                ViewBag.Prospecto = current_p;
            }
            var prospectMoney = context.sp_GetRentabilidadProspecto(raffleId).FirstOrDefault();
            ViewBag.ProspectMoney = prospectMoney;
            RRentabilidadSorteo report = context.RRentabilidadSorteos.Where(p => p.RaffleId == raffleId).FirstOrDefault();

            return View(report);
        }
    }
}
