using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Prospects;
using Tickets.Models.Raffles;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ReportsController : Controller
    {
        //
        //  GET: Reports/ReportPaymenHistory
        [HttpGet]
        [Authorize]
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
        [Authorize]
        public ActionResult DebtReport(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            var indentifybatchs = context.IdentifyBaches.Where(i =>
                (i.RaffleId == raffleId || raffleId == 0)
                && (i.ClientId == clientId || clientId == 0)).ToList();
            return View(indentifybatchs);
        }

        // 
        //  GET: Reports/IndentifyReceivableReport
        [HttpGet]
        [Authorize]
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
                    Value = p.Value,
                    Id = p.Id,
                    UserId = p.CreateUser,
                    RaffleId = p.IdentifyBach.RaffleId,
                    CreateDate = p.CreateDate,
                    IdentifyBach = p.IdentifyBach,
                    ReceivablePercent = p.DiscountPercent
                }).FirstOrDefault();
            }
            else
            {
                payment = context.NoteCredits.AsEnumerable().Where(c => c.Id == creditNoteId).Select(n => new PaymentReceivableReportModel
                {
                    ClientName = n.ClientId + " - " + n.Client.Name,
                    PaymentType = "PAGO CON NOTA DE CREDITO #" + n.Id,
                    Value = n.TotalCash,
                    Id = n.Id,
                    UserId = n.CreateUser,
                    RaffleId = n.IdentifyBaches.FirstOrDefault().RaffleId,
                    CreateDate = n.CreateDate,
                    IdentifyBach = n.IdentifyBaches.FirstOrDefault(),
                    ReceivablePercent = n.DiscountPercent,
                    CreditNoteId = creditNoteId,
                    Concept = n.Concepts
                }).FirstOrDefault();
            }
            return View(payment);
        }

        // 
        //  GET: Reports/GenerarRaffle
        [HttpGet]
        [Authorize]
        public ActionResult GenerarRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/PreviousDebtPaymentReport
        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public ActionResult SpecialAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/IntermAwardInline
        [HttpGet]
        [Authorize]
        public ActionResult IntermAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/MinorAwardInline
        [HttpGet]
        [Authorize]
        public ActionResult MinorAwardInline(int raffleId)
        {
            var context = new TicketsEntities();
            Raffle raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        // 
        //  GET: Reports/ShowDocument
        [HttpGet]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        public ActionResult ProspectFormat(int prospectId)
        {
            var context = new TicketsEntities();
            var prospect = context.Prospects.FirstOrDefault(p => p.Id == prospectId);
            return View(prospect);
        }

        //
        //  GET: Reports/PrintedNumbesForRaffle
        [Authorize]
        [HttpGet]
        public ActionResult PrintedNumbesForRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/PrintedNumbersNotInvoiced
        [Authorize]
        [HttpGet]
        public ActionResult PrintedNumbersNotInvoiced(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/NoPrintedNumbesForRaffle
        [Authorize]
        [HttpGet]
        public ActionResult NoPrintedNumbesForRaffle(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        [Authorize]
        [HttpGet]
        public ActionResult BilletesVendidos(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        [Authorize]
        [HttpGet]
        public ActionResult BilletesDevueltos(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        [Authorize]
        [HttpGet]
        public ActionResult BilletesCirculacion(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/NoPrintedNumbesAward
        [Authorize]
        [HttpGet]
        public ActionResult NoPrintedNumbesAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/InvoicedNumbersAward
        [Authorize]
        [HttpGet]
        public ActionResult InvoicedNumbersAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbersAward
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedNumbersAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //GET: Reports/ReturnedGroupAwards
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedGroupAwards(int raffleId, string groupId = "")
        {
            var context = new TicketsEntities();
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
            }

        }

        //
        //  GET: Reports/NumbesAward
        [Authorize]
        [HttpGet]
        public ActionResult NumbesAward(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ClientNumbersAward
        [Authorize]
        [HttpGet]
        public ActionResult ClientNumbersAward(int raffleId, int clientId)
        {
            var context = new TicketsEntities();
            ViewBag.clientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/NumbesAwardExpired
        [Authorize]
        [HttpGet]
        public ActionResult NumbesAwardExpired(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/MajorAwardInline
        // [Authorize]
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
        [Authorize]
        [HttpGet]
        public ActionResult RaffleGeneralOver(int raffleId)
        {
            var context = new TicketsEntities();
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

            return View(raffle);
        }

        //
        //  GET: Reports/RaffleGeneralOverR
        [Authorize]
        [HttpGet]
        public ActionResult RaffleGeneralOverR(int raffleId)
        {
            var context = new TicketsEntities();
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
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbers
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedNumbers(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbersGroup
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedNumbersGroup(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/ReturnedNumbersClient
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedNumbersClient(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/AllocatinosNumbers
        [Authorize]
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
        [Authorize]
        [HttpGet]
        public ActionResult AllocationSummary(int raffleId = 0)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/AllocatinosNumberList
        [Authorize]
        [HttpGet]
        public ActionResult AllocatinosNumberList(int allocationId)
        {
            var context = new TicketsEntities();
            var ticketAllocation = context.TicketAllocations.FirstOrDefault(r => r.Id == allocationId);
            return View(ticketAllocation);
        }

        //
        //  GET: Reports/InvoiceListPrint
        [Authorize]
        [HttpGet]
        public ActionResult InvoicesByRaffle(int raffleId = 0, int clientId = 0, int invoiceId = 0)
        {
            var context = new TicketsEntities();
            ViewBag.ClientId = clientId;
            ViewBag.invoiceId = invoiceId;
            var raffle = context.Raffles.FirstOrDefault(r => raffleId == 0 || r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/InvoicesByRaffleAllocation
        [Authorize]
        [HttpGet]
        public ActionResult InvoicesByRaffleAllocation(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.FirstOrDefault(r => r.Id == raffleId);
            return View(raffle);
        }

        //
        //  GET: Reports/Error
        [Authorize]
        [HttpGet]
        public ActionResult Error(string message)
        {
            ViewBag.errorMessage = message;
            return View();
        }

        //
        //  GET: Reports/InvoiceDetail
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceDetail(int invoiceId)
        {
            var context = new TicketsEntities();
            var invoice = context.Invoices.FirstOrDefault(r => r.Id == invoiceId);
            return View(invoice);
        }

        [Authorize]
        [HttpGet]
        public ActionResult NoteCreditDetail(int noteCreditId)
        {
            var context = new TicketsEntities();
            var notecredit = context.NoteCredits.FirstOrDefault(r => r.Id == noteCreditId);
            return View(notecredit);
        }

        //
        //  GET: Reports/ReturnedDeatils
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        [HttpGet]
        public ActionResult CashReport(int IdentifyBachId)
        {
            var context = new TicketsEntities();
            var identifyBatch = context.IdentifyBaches.FirstOrDefault(i => i.Id == IdentifyBachId);
            return View(identifyBatch);
        }

        //
        //  GET: Reports/NumberCertification
        [Authorize]
        [HttpGet]
        public ActionResult NumberCertification(int CertificationNumberId)
        {
            var context = new TicketsEntities();
            var certification = context.CertificationNumbers.FirstOrDefault(i => i.Id == CertificationNumberId);
            return View(certification);
        }

        //
        //  GET: Reports/AwardCertification
        [Authorize]
        [HttpGet]
        public ActionResult AwardCertification(int CertificationNumberId)
        {
            var context = new TicketsEntities();
            var certification = context.AwardCertification.FirstOrDefault(i => i.Id == CertificationNumberId);
            return View(certification);
        }

        //
        //  GET: Reports/GetCashReport
        [Authorize]
        [HttpGet]
        public ActionResult GetCashReport(int userId, string Fecha)
        {
            var context = new TicketsEntities();

            DateTime FechaConvert = DateTime.Parse(Fecha);

            var receiptPayment = context.ReceiptPayments.Include(i => i.Invoice)
                .Include(c => c.Client).AsEnumerable()
                .Where(i => i.CreateUser == userId && i.CreateDate.Date == FechaConvert.Date)
                .ToList();

            if (receiptPayment.Count <= 0)
            {
                return RedirectToAction("Error", new { message = "No se encontraron pagos de este usuario." });
            }

            return View(receiptPayment);
        }

        //
        //  GET: Reports/InvoiceDeatilCashReport
        [Authorize]
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
                return RedirectToAction("Error", new { message = "No se encontraron pagos del sorteo #" + invoiceDetail.RaffleId + "." });
            }
            ViewBag.invoiceDetailId = invoiceDetailId;
            return View(identifyNumbers);
        }

        //
        //  GET: Reports/ReprintTickets
        [Authorize]
        [HttpGet]
        public ActionResult ReprintTickets(int reprintId)
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

        //
        //  GET: Reports/ReprintTicketsChristmas
        [Authorize]
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

        //
        //GET: Reports/AccountsReceivables
        [Authorize]
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
        //GET: Reports/AccountsPayments
        [Authorize]
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

        [Authorize]
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

                var raffles = context.Raffles.AsEnumerable()
                                     .Where(r => r.DateSolteo.Date >= startD.Date && r.DateSolteo.Date <= endD.Date && 
                                     r.Prospect.ImpresionType != (int)ProspectImpresionTypeEnum.Extraordinario).ToList();

                if (raffles.Count == 0)
                {
                    return RedirectToAction("Error", new { message = "No se encontraron datos para las fechas seleccionadas." });
                }

                ViewBag.StartDate = startD.ToShortDateString();
                ViewBag.EndDate = endD.ToShortDateString();

                return View(raffles);
            }
        }

        //
        //GET: Reports/ClientGeneralReport
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        //GET: Reports/AllocationSummaryby
        [Authorize]
        [HttpGet]
        public ActionResult AllocationSummaryby(int clientId, int raffleId)
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
        //GET: Reports/TicketReprinted
        [Authorize]
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
