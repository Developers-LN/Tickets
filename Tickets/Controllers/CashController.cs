﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Procedures;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class CashController : Controller
    {
        //  GET: /Cash/InvoiceDetailCashReports
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceDetailCashReports()
        {
            return View();
        }

        //  GET: /Cash/InvoiceDetailList
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceDetailList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult InvoicePaymentHistory()
        {
            return View();
        }

        //
        // GET: /Cash/OpenCash
        [Authorize]
        [HttpGet]
        public ActionResult OpenCash()
        {
            return View();
        }

        //
        // GET: /Cash/CashReports
        [Authorize]
        [HttpGet]
        public ActionResult CashReports()
        {
            return View();
        }

        //
        // GET: /Cash/AccountsReceivablesReports
        [Authorize]
        [HttpGet]
        public ActionResult AccountsReceivablesReports()
        {
            return View();
        }

        //
        // GET: /Cash/AccountsReceivablesReportsByPeriod
        [Authorize]
        [HttpGet]
        public ActionResult AccountsReceivablesReportsByPeriod()
        {
            return View();
        }

        //
        // GET: /Cash/PayedAwardByPeriod
        [Authorize]
        [HttpGet]
        public ActionResult PayedAwardByPeriod()
        {
            return View();
        }

        //
        // GET: /Cash/ElectronicAwardPayed
        [Authorize]
        [HttpGet]
        public ActionResult ElectronicAwardPayed()
        {
            return View();
        }

        //
        // GET: /Cash/InvoiceByPeriod
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceByPeriod()
        {
            return View();
        }

        //
        // GET: /Cash/AccountsReceivablesReportsExcel
        [Authorize]
        [HttpGet]
        public ActionResult AccountsReceivablesReportsExcel()
        {
            return View();
        }

        //
        // GET: /Cash/PayedBachByPeriodExcel
        [Authorize]
        [HttpGet]
        public ActionResult PayedBachByPeriodExcel()
        {
            return View();
        }

        //
        // GET: /Cash/AccountsReceivablesCloseReportsExcel
        [Authorize]
        [HttpGet]
        public ActionResult AccountsReceivablesCloseReportsExcel()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult PayableAwardsReportsExcel()
        {
            return View();
        }

        //
        // GET: /Cash/ClientGeneralBalance
        [Authorize]
        [HttpGet]
        public ActionResult ClientGeneralBalance()
        {
            return View();
        }

        //
        // GET: /Cash/AccountsPaymentsReports
        [Authorize]
        [HttpGet]
        public ActionResult AccountsPaymentsReports()
        {
            return View();
        }

        //
        //  GET: Cash/CashReportListData
        [Authorize]
        [HttpGet]
        public ActionResult CashReportListData(int raffleId = 0, int clientId = 0)
        {
            var context = new TicketsEntities();
            List<object> clients = new List<object>();
            List<object> raffles = new List<object>();
            List<object> invoiceDetails = new List<object>();
            if (clientId == 0 && raffleId == 0)
            {
                clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList<object>();

                var raffles1 = context.Raffles.Select(r => new
                {
                    r.Id,
                    r.SequenceNumber,
                    r.Name,
                    raffleNomenclature = r.Symbol + r.Separator + r.SequenceNumber,
                    text = r.Symbol + r.Separator + r.SequenceNumber + " " + r.Name,
                    r.DateSolteo
                }).ToList();

                raffles = raffles1.Select(s => new
                {
                    s.Id,
                    s.SequenceNumber,
                    s.Name,
                    s.raffleNomenclature,
                    text = s.text + " " + s.DateSolteo.ToString("dd/MM/yyyy")
                }).ToList<object>();
            }
            else
            {
                invoiceDetails = context.InvoiceDetails.Where(i =>
                   (i.RaffleId == raffleId || raffleId == 0)
                   && (i.ClientId == clientId || clientId == 0)
                ).AsEnumerable().Select(invoiceDetail => InvoiceDetailToObject(invoiceDetail)).ToList();
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de detalle de facturas", null);

            return new JsonResult() { Data = new { clients, raffles, invoiceDetails }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object InvoiceDetailToObject(InvoiceDetail invoiceDetail)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.PayingFund).Select(s => new
            {
                s.Id,
                s.NameDetail,
                s.IdGroup
            }).ToList();

            return new
            {
                invoiceDetail.Id,
                PayingFund = invoiceDetail.PayingFund.HasValue && invoiceDetail.PayingFund > 0 ? catalog.FirstOrDefault(f => f.Id == invoiceDetail.PayingFund.Value).NameDetail : "N/A",
                invoiceDetail.SequenceNumber,
                EndDate = invoiceDetail.EndDate.ToUnixTime(),
                StartDate = invoiceDetail.StartDate.ToUnixTime(),
                invoiceDetail.RaffleId,
                //RaffleDesc = invoiceDetail.Raffle.Name,
                RaffleDesc = invoiceDetail.Raffle.Symbol + invoiceDetail.Raffle.Separator + invoiceDetail.Raffle.SequenceNumber + " " + invoiceDetail.Raffle.Name + " " + invoiceDetail.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                invoiceDetail.ClientId,
                ClientDesc = invoiceDetail.Client.Name,
                UserDesc = invoiceDetail.User.Name,
                UserId = invoiceDetail.CreateUser,
                clientType = invoiceDetail.Client.GroupId
            };
        }

        //
        // POST: /Cash/InvoiceDetailCreate
        [Authorize]
        [HttpPost]
        public JsonResult InvoiceDetailCreate(InvoiceDetail model)
        {
            object invoiceDetailsObject;
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var invoiceDetailExist = context.InvoiceDetails.Any(a => a.RaffleId == model.RaffleId && a.ClientId == model.ClientId
                        && a.StartDate == model.StartDate.Date && a.EndDate == model.EndDate.Date && a.PayingFund == model.PayingFund);

                        if (invoiceDetailExist == true)
                        {
                            tx.Rollback();
                            return new JsonResult() { Data = new { result = false, message = "Ya existe una factura con estos datos" } };
                        }

                        var awards = context.RaffleAwards.Where(a => a.RaffleId == model.RaffleId).ToList();

                        List<IdentifyNumber> identifyNumbers = new List<IdentifyNumber>();

                        context.IdentifyBaches.AsEnumerable().Where(i =>
                            i.RaffleId == model.RaffleId && i.ClientId == model.ClientId &&
                            (i.IdentifyBachPayments.Any(p => p.CreateDate.Date >= model.StartDate.Date && p.CreateDate.Date <= model.EndDate.Date) == true)
                            && (Utils.IdentifyBachIsPayedMinor(i, awards) || Utils.IdentifyBachIsPayedMayor(i, awards))
                        ).ToList().ForEach(i => identifyNumbers.AddRange(i.IdentifyNumbers));

                        if (identifyNumbers.Count <= 0)
                        {
                            tx.Rollback();
                            return new JsonResult() { Data = new { result = false, message = "No se encontro pago para este rango de fecha" } };
                        }

                        var invoiceDetail = new InvoiceDetail()
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            EndDate = model.EndDate,
                            RaffleId = model.RaffleId,
                            ClientId = model.ClientId,
                            StartDate = model.StartDate,
                            PayingFund = model.PayingFund
                        };

                        context.InvoiceDetails.Add(invoiceDetail);
                        context.SaveChanges();

                        invoiceDetailsObject = new
                        {
                            invoiceDetail.Id,
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            model.EndDate,
                            model.RaffleId,
                            model.ClientId,
                            model.StartDate,
                            model.PayingFund,
                        };

                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Generando reporte de detailles de facturas", invoiceDetailsObject);
            return new JsonResult() { Data = new { result = true, invoiceDetailsObject, message = "Detaille de factura creada Corractamente." } };
        }

        //
        //  GET: Cash/CashReportData
        [Authorize]
        [HttpGet]
        public ActionResult CashReportData()
        {
            var context = new TicketsEntities();

            var users = context.Users.Where(c => c.Statu == (int)UserStatusEnum.Active && c.Employee.Department == (int)DepartmentEnum.GeneralCashier).Select(r => new
            {
                value = r.Id,
                text = r.Employee.Name + " " + r.Employee.LastName
            }).ToList();

            var clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                value = r.Id,
                text = r.Name
            }).ToList();

            var accountReceivableTypes = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.AccountReceivableType && w.Statu == true).OrderBy(o => o.IdDetail).Select(s => new
            {
                value = s.Id,
                text = s.NameDetail
            });

            var raffles1 = context.Raffles.Select(r => new
            {
                value = r.Id,
                raffleSequence = r.SequenceNumber,
                raffleNomenclature = r.Symbol + r.Separator + r.SequenceNumber,
                text = r.Symbol + r.Separator + r.SequenceNumber + " " + r.Name,
                r.DateSolteo
            }).ToList();

            var raffles = raffles1.Select(s => new
            {
                s.value,
                s.raffleSequence,
                s.raffleNomenclature,
                text = s.text + " " + s.DateSolteo.ToString("dd/MM/yyyy")
            }).ToList();

            var payingFund = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.PayingFund && w.Statu == true).OrderBy(o => o.IdDetail).Select(s => new
            {
                value = s.Id,
                text = s.NameDetail
            });

            return new JsonResult() { Data = new { clients, raffles, users, accountReceivableTypes, payingFund }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Cash/CashClose
        [Authorize]
        [HttpGet]
        public ActionResult CashClose()
        {
            return View();
        }

        //  GET:    /Cash/GetCashCloseData
        //
        [HttpGet]
        [Authorize]
        public JsonResult GetCashCloseData()
        {
            var context = new TicketsEntities();
            var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
            var totalCash = 0.00m;
            var totalCheck = 0.00m;
            var totalCard = 0.00m;

            foreach (var recipient in context.ReceiptPayments.Where(r => r.CashId == cash.Id))
            {
                totalCash += recipient.TotalCash;
                totalCheck += recipient.TotalCheck;
                totalCard += recipient.TotalCredit;
            }

            return new JsonResult()
            {
                Data = new
                {
                    result = true,
                    totalCash,
                    totalCheck,
                    totalCard
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        // POST: /Cash/CashClose
        [Authorize]
        [HttpPost]
        public JsonResult CashClose(CashClose cashClose)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);

                        cashClose.StartDate = cash.CraeteDate;
                        cashClose.CashId = cash.Id;
                        cashClose.TotalStart = cash.OpenValue;
                        cashClose.UserStartId = cash.CreateUser;
                        cashClose.EndDate = DateTime.Now;
                        cashClose.UserEndId = WebSecurity.CurrentUserId;
                        cashClose.Statu = (int)GeneralStatusEnum.Active;
                        context.CashCloses.Add(cashClose);
                        context.SaveChanges();

                        cash.Statu = (int)CashStatusEnum.Close;
                        context.SaveChanges();
                        tx.Commit();

                        return new JsonResult() { Data = new { result = true, message = "Caja cerrada correctamente." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // POST: /Cash/VerifyCashOpen
        [HttpGet]
        [Authorize]
        public JsonResult VerifyCashOpen()
        {
            var context = new TicketsEntities();
            var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
            if (cash == null)
            {
                return new JsonResult() { Data = new { result = 1, message = "No hay caja abierta. ¿ Desea abrir la caja ?" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                return new JsonResult() { Data = new { result = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //
        // POST: /Cash/VerifyCashClose
        [HttpGet]
        [Authorize]
        public JsonResult VerifyCashClose()
        {
            var context = new TicketsEntities();
            var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
            if (cash == null)
            {
                return new JsonResult() { Data = new { result = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            else
            {
                if (cash.CraeteDate.Date > DateTime.Now.Date)
                {
                    return new JsonResult() { Data = new { result = false, message = "Existe un caja abierta del dia anterior, debe hacer el cuadre de caja." }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    return new JsonResult() { Data = new { result = false, message = "Existe un caja abierta," }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }
        }

        // POST: /Cash/OpenCash
        [HttpPost]
        [Authorize]
        public JsonResult OpenCash(Cash cash)
        {
            try
            {
                var context = new TicketsEntities();
                var user = context.Users.AsEnumerable().Where(u => u.Id == WebSecurity.CurrentUserId).FirstOrDefault();

                cash.CraeteDate = DateTime.Now;
                cash.CreateUser = WebSecurity.CurrentUserId;
                cash.AgencyId = user.Employee.AgencyId;
                cash.Statu = (int)CashStatusEnum.Open;
                cash.Name = DateTime.Now.Date.ToString();

                context.Cashes.Add(cash);

                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, cash.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Cliente", CashToObject(cash));

                return new JsonResult() { Data = new { result = true, message = "Caja Abierta correctamente!" } };
            }
            catch
            {
                return new JsonResult() { Data = new { result = false, message = "Error abriendo caja." } };
            }
        }

        private object CashToObject(Cash cash)
        {
            var context = new TicketsEntities();
            return new
            {
                cash.Id,
                cash.Name,
                cash.OpenValue,
                cash.Statu,
                StatuDesc = context.Catalogs.Where(c => c.Id == cash.Statu).FirstOrDefault().NameDetail,
                cash.AgencyId,
                AgencyDesc = context.Agencies.FirstOrDefault(a => a.Id == cash.AgencyId).Name,
                cash.CreateUser,
                CraeteDate = cash.CraeteDate.ToString()
            };
        }

        #region Receivable

        //
        // GET: /Cash/Receivable
        [Authorize]
        [HttpGet]
        public ActionResult Receivable()
        {
            return View();
        }

        //
        //[HttpPost]
        //[Authorize]
        //public JsonResult Receivable(ReceiptPayment receiptPayment)
        //{
        //    using (var context = new TicketsEntities())
        //    {
        //        using (var tx = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);

        //                var invoice = context.Invoices.FirstOrDefault(i => i.Id == receiptPayment.InvoiceId);

        //                var noteCreditReceiptPayments = receiptPayment.NoteCreditReceiptPayments;
        //                var totalCash = receiptPayment.TotalCash;
        //                receiptPayment.CashId = cash.Id;
        //                receiptPayment.ClientId = invoice.ClientId;
        //                receiptPayment.CreateDate = DateTime.Now;
        //                receiptPayment.CreateUser = WebSecurity.CurrentUserId;
        //                receiptPayment.TotalCheck = 0;
        //                receiptPayment.TotalCredit = 0;
        //                receiptPayment.TotalCash = 0;

        //                receiptPayment.NoteCreditReceiptPayments = new List<NoteCreditReceiptPayment>();

        //                if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.Cash)
        //                {
        //                    receiptPayment.TotalCash = totalCash;
        //                }
        //                else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.CreditCard)
        //                {
        //                    receiptPayment.TotalCredit = totalCash;
        //                }
        //                else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.Checks)
        //                {
        //                    receiptPayment.TotalCheck = totalCash;
        //                }
        //             //   context.ReceiptPayments.Add(receiptPayment);
        //             //   context.SaveChanges();

        //                if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.CreditNote)
        //                {
        //                    var creditNotePaymentList = new List<NoteCreditReceiptPayment>();
        //                    foreach (var credit in noteCreditReceiptPayments)
        //                    {
        //                        var noteCredit = context.NoteCredits.FirstOrDefault(n => n.Id == credit.NoteCreditId);
        //                        var noteCreditTotalCash = 0.00m;
        //                        if (totalCash >= noteCredit.TotalRest)
        //                        {
        //                            noteCreditTotalCash = noteCredit.TotalRest;
        //                            noteCredit.Statu = (int)GeneralStatusEnum.Delete;
        //                            noteCredit.TotalRest = 0;
        //                            totalCash -= noteCreditTotalCash;
        //                        }
        //                        else
        //                        {
        //                            noteCreditTotalCash = totalCash;
        //                            noteCredit.TotalRest = noteCredit.TotalRest - totalCash;
        //                            totalCash = 0;
        //                        }
        //                        var cp = new NoteCreditReceiptPayment()
        //                        {
        //                            NoteCreditId = noteCredit.Id,
        //                            ReceiptPaymentId = receiptPayment.Id,
        //                            TotalCash = noteCreditTotalCash
        //                        };
        //                        creditNotePaymentList.Add(cp);
        //                    }
        //                    context.NoteCreditReceiptPayments.AddRange(creditNotePaymentList);
        //                    context.SaveChanges();
        //                }

        //                var payment = GetPaymentCash(invoice);
        //                if (( payment.totalRestant - payment.discount) <= 0)
        //                {
        //                    invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Payed;
        //                    context.SaveChanges();
        //                }

        //                tx.Commit();
        //                return new JsonResult() { Data = new { result = true, message = "Recibo de Efectivo Guardado." } };
        //            }
        //            catch (Exception e)
        //            {
        //                tx.Rollback();
        //                return new JsonResult() { Data = new { result = false, message = e.Message } };
        //            }
        //        }
        //    }
        //}

        // POST: Cash/Receivable
        [HttpPost]
        [Authorize]
        public JsonResult Receivable(ReceiptPayment receiptPayment, int includeCashAdvance, decimal totalCashAdvance, string noteCashAdvance)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.TransDepDirect && receiptPayment.Recibo == null)
                        {
                            return new JsonResult() { Data = new { result = false, message = "No ha ingresado el número de referencia." } };
                        }

                        var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);

                        var invoice = context.Invoices.FirstOrDefault(i => i.Id == receiptPayment.InvoiceId);

                        var ClientType = invoice.Client.GroupId;

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.TransDepDirect)
                        {
                            if (context.ReceiptPayments.FirstOrDefault(r => r.ClientId == invoice.ClientId && r.Recibo == receiptPayment.Recibo
                            && r.InvoiceId == invoice.Id) != null)
                            {
                                return new JsonResult() { Data = new { result = false, message = "Este pago fue efectuado anteriormente." } };
                            }
                        }

                        var noteCreditReceiptPayments = receiptPayment.NoteCreditReceiptPayments;
                        var totalCash = receiptPayment.TotalCash;
                        var Recibo = receiptPayment.Recibo;
                        var Cedula = receiptPayment.Cedula;
                        var Nombre = receiptPayment.Nombre;
                        var Observaciones = receiptPayment.Observaciones;
                        var Telefono = receiptPayment.Telefono;
                        var Codigo = receiptPayment.Codigo;
                        receiptPayment.CashId = cash.Id;
                        receiptPayment.ClientId = invoice.ClientId;
                        receiptPayment.CreateDate = DateTime.Now;
                        receiptPayment.CreateUser = WebSecurity.CurrentUserId;
                        receiptPayment.TotalCheck = 0;
                        receiptPayment.TotalCredit = 0;
                        receiptPayment.TotalCash = 0;
                        receiptPayment.Recibo = "";
                        receiptPayment.Cedula = "";
                        receiptPayment.Nombre = "";
                        receiptPayment.Observaciones = "";
                        receiptPayment.Telefono = "";
                        receiptPayment.Codigo = "";

                        receiptPayment.NoteCreditReceiptPayments = new List<NoteCreditReceiptPayment>();

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.Cash)
                        {
                            receiptPayment.TotalCash = totalCash;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.CreditCard)
                        {
                            receiptPayment.TotalCredit = totalCash;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.Checks)
                        {
                            receiptPayment.TotalCheck = totalCash;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.TransDepDirect)
                        {
                            receiptPayment.TotalCheck = totalCash;
                            receiptPayment.Recibo = Recibo;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.DescuentoNomina)
                        {
                            receiptPayment.TotalCheck = totalCash;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.DescuentoPrestaciones)
                        {
                            receiptPayment.TotalCheck = totalCash;
                        }
                        else if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.Otros)
                        {
                            receiptPayment.TotalCheck = totalCash;
                        }
                        if (ClientType == (int)ClientGroupEnum.CajaDespachoExpress)
                        {
                            receiptPayment.Cedula = Cedula;
                            receiptPayment.Nombre = Nombre;
                            receiptPayment.Telefono = Telefono;
                            receiptPayment.Codigo = Codigo;
                            receiptPayment.Observaciones = Observaciones;
                        }

                        context.ReceiptPayments.Add(receiptPayment);
                        context.SaveChanges();

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.CreditNote)
                        {
                            var creditNotePaymentList = new List<NoteCreditReceiptPayment>();
                            foreach (var credit in noteCreditReceiptPayments)
                            {
                                var noteCredit = context.NoteCredits.FirstOrDefault(n => n.Id == credit.NoteCreditId);
                                var noteCreditTotalCash = 0.00m;
                                if (totalCash >= noteCredit.TotalRest)
                                {
                                    noteCreditTotalCash = noteCredit.TotalRest;
                                    noteCredit.Statu = (int)GeneralStatusEnum.Delete;
                                    noteCredit.TotalRest = 0;
                                    totalCash -= noteCreditTotalCash;
                                }
                                else
                                {
                                    noteCreditTotalCash = totalCash;
                                    noteCredit.TotalRest -= totalCash;
                                    totalCash = 0;
                                }
                                var cp = new NoteCreditReceiptPayment()
                                {
                                    NoteCreditId = noteCredit.Id,
                                    ReceiptPaymentId = receiptPayment.Id,
                                    TotalCash = noteCreditTotalCash
                                };
                                creditNotePaymentList.Add(cp);
                            }
                            context.NoteCreditReceiptPayments.AddRange(creditNotePaymentList);
                            context.SaveChanges();
                        }

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.CashAdvance)
                        {
                            var creditNotePaymentList = new List<NoteCreditReceiptPayment>();
                            foreach (var credit in noteCreditReceiptPayments)
                            {
                                var noteCredit = context.NoteCredits.FirstOrDefault(n => n.Id == credit.NoteCreditId);
                                var noteCreditTotalCash = 0.00m;
                                if (totalCash >= noteCredit.TotalRest)
                                {
                                    noteCreditTotalCash = noteCredit.TotalRest;
                                    noteCredit.Statu = (int)GeneralStatusEnum.Delete;
                                    noteCredit.TotalRest = 0;
                                    totalCash -= noteCreditTotalCash;
                                }
                                else
                                {
                                    noteCreditTotalCash = totalCash;
                                    noteCredit.TotalRest -= totalCash;
                                    totalCash = 0;
                                }
                                var cp = new NoteCreditReceiptPayment()
                                {
                                    NoteCreditId = noteCredit.Id,
                                    ReceiptPaymentId = receiptPayment.Id,
                                    TotalCash = noteCreditTotalCash
                                };
                                creditNotePaymentList.Add(cp);
                            }
                            context.NoteCreditReceiptPayments.AddRange(creditNotePaymentList);
                            context.SaveChanges();
                        }

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.PositiveBalance)
                        {
                            var creditNotePaymentList = new List<NoteCreditReceiptPayment>();
                            foreach (var credit in noteCreditReceiptPayments)
                            {
                                var noteCredit = context.NoteCredits.FirstOrDefault(n => n.Id == credit.NoteCreditId);
                                var noteCreditTotalCash = 0.00m;
                                if (totalCash >= noteCredit.TotalRest)
                                {
                                    noteCreditTotalCash = noteCredit.TotalRest;
                                    noteCredit.Statu = (int)GeneralStatusEnum.Delete;
                                    noteCredit.TotalRest = 0;
                                    totalCash -= noteCreditTotalCash;
                                }
                                else
                                {
                                    noteCreditTotalCash = totalCash;
                                    noteCredit.TotalRest -= totalCash;
                                    totalCash = 0;
                                }
                                var cp = new NoteCreditReceiptPayment()
                                {
                                    NoteCreditId = noteCredit.Id,
                                    ReceiptPaymentId = receiptPayment.Id,
                                    TotalCash = noteCreditTotalCash
                                };
                                creditNotePaymentList.Add(cp);
                            }
                            context.NoteCreditReceiptPayments.AddRange(creditNotePaymentList);
                            context.SaveChanges();
                        }

                        if (receiptPayment.ReceiptType == (int)PaymentTypeEnum.TaxReceiptNoteCredit)
                        {
                            var creditNotePaymentList = new List<NoteCreditReceiptPayment>();
                            foreach (var credit in noteCreditReceiptPayments)
                            {
                                var noteCredit = context.NoteCredits.FirstOrDefault(n => n.Id == credit.NoteCreditId);
                                var noteCreditTotalCash = 0.00m;
                                if (totalCash >= noteCredit.TotalRest)
                                {
                                    noteCreditTotalCash = noteCredit.TotalRest;
                                    noteCredit.Statu = (int)GeneralStatusEnum.Delete;
                                    noteCredit.TotalRest = 0;
                                    totalCash -= noteCreditTotalCash;
                                }
                                else
                                {
                                    noteCreditTotalCash = totalCash;
                                    noteCredit.TotalRest -= totalCash;
                                    totalCash = 0;
                                }
                                var cp = new NoteCreditReceiptPayment()
                                {
                                    NoteCreditId = noteCredit.Id,
                                    ReceiptPaymentId = receiptPayment.Id,
                                    TotalCash = noteCreditTotalCash
                                };
                                creditNotePaymentList.Add(cp);
                            }
                            context.NoteCreditReceiptPayments.AddRange(creditNotePaymentList);
                            context.SaveChanges();
                        }

                        var payment = GetPaymentCash(invoice);
                        if ((payment.totalRestant - payment.discount) <= 0)
                        {
                            invoice.PaymentStatu = (int)InvoicePaymentStatuEnum.Payed;
                            context.SaveChanges();
                        }

                        if (includeCashAdvance == (int)ReceiptPaymentAdd.PositiveBalance || includeCashAdvance == (int)ReceiptPaymentAdd.CashAdvance)
                        {
                            NoteCredit cashAdvance = new NoteCredit
                            {
                                ClientId = receiptPayment.ClientId,
                                TotalCash = totalCashAdvance,
                                TotalRest = totalCashAdvance,
                                NoteDate = DateTime.Now.Date,
                                Statu = (int)GeneralStatusEnum.Active,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now,
                                Concepts = noteCashAdvance,
                                DiscountPercent = 0,
                                TypeNote = includeCashAdvance == (int)ReceiptPaymentAdd.CashAdvance ? (int)NoteCreditEnum.CashAdvance : (int)NoteCreditEnum.PositiveBalance,
                                ReceiptPaymentId = receiptPayment.Id
                            };
                            context.NoteCredits.Add(cashAdvance);
                            context.SaveChanges();
                        }

                        tx.Commit();

                        /*try
                        {
                            Procedure_Accounting_ReceiptPayment procedure_Accounting_ReceiptPayment = new Procedure_Accounting_ReceiptPayment();
                            var Upload = procedure_Accounting_ReceiptPayment.Upload_Accounting_ReceiptPayment(receiptPayment.Id);
                        }
                        catch (Exception e)
                        {
                            return new JsonResult() { Data = new { result = true, message = "Recibo de Efectivo Guardado.", Pago = receiptPayment.Id, clientType = ClientType } };
                        }*/

                        return new JsonResult() { Data = new { result = true, message = "Recibo de Efectivo Guardado.", Pago = receiptPayment.Id, clientType = ClientType } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // GET: Ticket/GetReceivableData
        [HttpGet]
        [Authorize]
        public JsonResult GetReceivableData(int invoiceId)
        {
            var context = new TicketsEntities();
            var invoice = context.Invoices.Where(i => i.Id == invoiceId).FirstOrDefault();

            var clientGroup = context.Clients.Where(w => w.Id == invoice.ClientId).Select(s => s.GroupId).FirstOrDefault();

            var paymentTypes = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.PaymentType && c.Id != (int)PaymentTypeEnum.DescuentoNomina && c.Id != (int)PaymentTypeEnum.DescuentoPrestaciones)
            .Select(c => new
            {
                c.Id,
                c.NameDetail
            }).ToList();

            if (clientGroup == (int)ClientGroupEnum.Empreados)
            {
                paymentTypes = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.PaymentType).Select(c => new
                {
                    c.Id,
                    c.NameDetail
                }).ToList();
            }

            ///    var ticketController = new TicketAllocationController();  // esta instancia de clase no presenta ninguna funcionalidad
            if (invoice == null)
            {
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { } };
            }
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    paymentTypes,
                    clientName = invoice.Client.Name,
                    invoiceId = invoice.Id,
                    sequenceNumberInvoice = invoice.SequenceNumber,
                    clientId = invoice.ClientId,
                    clientType = invoice.Client.GroupId,
                    invoiceDiscount = invoice.Discount,
                    invoiceDate = invoice.CreateDate.ToString("dd/MM/yyyy"),
                    invoice.InvoiceTickets.FirstOrDefault().TicketAllocationNumber.TicketAllocation.Agente,
                    paymentsHistory = invoice.ReceiptPayments.AsEnumerable()
                    .Where(w => w.InvoiceId == invoice.Id)
                    .Select(s => new
                    {
                        s.Id,
                        SequenceNumberReceiptPaymnet = s.Nomenclature == null ? s.SequenceNumber.Value.ToString() : string.Concat(s.Nomenclature, s.SequenceNumber.Value.ToString().PadLeft((s.Digits != null ? (int)s.Digits : 0), '0')),
                        SequenceNumberInvoice = s.Invoice.SequenceNumber,
                        s.Nomenclature,
                        s.Nombre,
                        s.Observaciones,
                        receiptType = context.Catalogs.Where(w => w.Id == s.ReceiptType).Select(f => f.NameDetail).FirstOrDefault(),
                        paymentDate = s.CreateDate.ToString("dd/MM/yy")
                    }).ToList(),
                    creditNotes = invoice.Client.NoteCredits.AsEnumerable().Where(c => c.TypeNote == (int)NoteCreditEnum.NoteCredit
                       && c.Statu == (int)GeneralStatusEnum.Active
                       && (c.RaffleId.HasValue == false || c.RaffleId.Value == invoice.RaffleId)
                       && c.TotalRest > 0).Select(c => CreditNoteToObject(c)).ToList(),
                    cashAdvances = invoice.Client.NoteCredits.AsEnumerable().Where(w => w.TypeNote == (int)NoteCreditEnum.CashAdvance
                        && w.Statu == (int)GeneralStatusEnum.Active
                        && (w.RaffleId.HasValue == false || w.RaffleId.Value == invoice.RaffleId)
                        && w.TotalRest > 0).Select(s => CreditNoteToObject(s)).ToList(),
                    positiveBalances = invoice.Client.NoteCredits.AsEnumerable().Where(w => w.TypeNote == (int)NoteCreditEnum.PositiveBalance
                        && w.Statu == (int)GeneralStatusEnum.Active
                        && (w.RaffleId.HasValue == false || w.RaffleId.Value == invoice.RaffleId)
                        && w.TotalRest > 0).Select(s => CreditNoteToObject(s)).ToList(),
                    typeCashAdvances = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.CashAdvances && w.Statu == true).Select(s => new
                    {
                        s.Id,
                        s.NameDetail
                    }),
                    payment = GetPaymentCashByProcedure(invoice),
                    raffleId = invoice.RaffleId,
                    sequenceNumberRaffle = invoice.Raffle.SequenceNumber,
                    //raffleName = invoice.Raffle.Name
                    raffleName = invoice.Raffle.Symbol + invoice.Raffle.Separator + invoice.Raffle.SequenceNumber + " " + invoice.Raffle.Name + " " + invoice.Raffle.DateSolteo.ToString("dd/MM/yyyy")
                }
            };
        }

        //
        // GET: /Cash/ReceivableList
        [Authorize]
        [HttpGet]
        public ActionResult ReceivableList()
        {
            return View();
        }

        //
        // GET: /Cash/GetReceivableList
        [Authorize]
        [HttpGet]
        public JsonResult GetReceivableList(int raffleId, int clientId = 0)
        {
            var context = new TicketsEntities();

            var raffles = new List<object>();
            if (raffleId == 0)
            {
                var raffles1 = context.Raffles.OrderByDescending(s => s.Id).Where(s => s.Statu != (int)RaffleStatusEnum.Suspended).Select(r => new
                {
                    r.Id,
                    r.SequenceNumber,
                    r.Name,
                    raffleNomenclature = r.Symbol + r.Separator + r.SequenceNumber,
                    text = r.Symbol + r.Separator + r.SequenceNumber + " " + r.Name,
                    r.DateSolteo
                }).ToList();

                raffles = raffles1.Select(s => new
                {
                    s.Id,
                    s.SequenceNumber,
                    s.Name,
                    s.raffleNomenclature,
                    text = s.text + " " + s.DateSolteo.ToString("dd/MM/yyyy")
                }).ToList<object>();
            }
            var clients = new List<object>();
            if (clientId == 0)
            {
                clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList<object>();
            }
            var invoices = new List<object>();
            if (raffleId != 0 || clientId != 0)
            {
                Procedure_InvoiceList invoiceListProcedure = new Procedure_InvoiceList();
                invoices = invoiceListProcedure.ListaFacturas(raffleId, clientId);

                /*invoices = context.Invoices.AsEnumerable().Where(i =>
                    (i.RaffleId == raffleId || raffleId == 0)
                    && (i.ClientId == clientId || clientId == 0)
                    && i.Condition == (int)InvoiceConditionEnum.Credit
                    //&& i.PaymentStatu == (int)InvoicePaymentStatuEnum.Pendient
                    //&& VerifyReceivable(i)
                    ).Select(i => InvoiceTicketsMainToObject(i)).ToList<object>();*/
            }
            return new JsonResult() { Data = new { invoices, clients, raffles }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /*        private bool VerifyReceivable(Invoice invoice)
                {
                    var totalInvoiceTicketPrice = 0.0M;
                    foreach (var ticketPrice in invoice.InvoiceTickets.Select(i => i.Quantity * i.PricePerFraction))
                    {
                        totalInvoiceTicketPrice += ticketPrice;
                    }
                    var totalRequestCash = 0.0M;
                    foreach (var requestCash in invoice.ReceiptPayments.Select(r => r.TotalCash + r.TotalCheck + r.TotalCredit))
                    {
                        totalRequestCash += requestCash;
                    }
                    return totalInvoiceTicketPrice > totalRequestCash;
                }*/

        public InvoicePaymentModel GetPaymentCashByProcedure(Invoice invoice)
        {
            var totalReturned = 0.0M;
            var totalCreditNote = 0.0M;

            Procedure_InvoiceListProcedureByList invoiceListProcedureByList = new Procedure_InvoiceListProcedureByList();
            var Payments = invoiceListProcedureByList.ListaFacturas(invoice.RaffleId, invoice.ClientId);

            return new InvoicePaymentModel
            {
                totalInvoice = Payments.FirstOrDefault(w => w.Id == invoice.Id).totalInvoice,
                totalPayment = Payments.FirstOrDefault(w => w.Id == invoice.Id).totalPayer,
                totalReturned = totalReturned,
                totalCreditNote = totalCreditNote,
                totalRestant = Payments.FirstOrDefault(w => w.Id == invoice.Id).totalRestant,
                discount = Payments.FirstOrDefault(w => w.Id == invoice.Id).discount
            };
        }

        public InvoicePaymentModel GetPaymentCash(Invoice invoice)
        {
            var context = new TicketsEntities();
            var clientDiscount = invoice.Discount;
            var totalInvoiceTicketPrice = 0.0M;
            var totalReturned = 0.0M;

            foreach (var invoiceTickets in invoice.InvoiceTickets)
            {
                totalInvoiceTicketPrice += (invoiceTickets.Quantity * invoiceTickets.PricePerFraction);
            }

            var totalCreditNote = 0.0M;
            var totalRequestCash = 0.0M;

            invoice.ReceiptPayments.ToList().ForEach(r => r.NoteCreditReceiptPayments.ToList().ForEach(rn => totalCreditNote += rn.TotalCash));

            invoice.ReceiptPayments.Select(r => r.TotalCash + r.TotalCheck + r.TotalCredit).ToList().ForEach(t => totalRequestCash += t);
            var discount = (totalInvoiceTicketPrice * clientDiscount) / 100;
            return new InvoicePaymentModel
            {
                totalInvoice = totalInvoiceTicketPrice,
                totalPayment = totalRequestCash,
                totalReturned = totalReturned,
                totalCreditNote = totalCreditNote,
                totalRestant = totalInvoiceTicketPrice - (totalRequestCash + totalReturned + totalCreditNote),
                discount = discount
            };
        }

        //
        // POST: /Cash/ReceivablePayment
        [Authorize]
        [HttpPost]
        public JsonResult ReceivablePayment(int invoiceId)
        {
            object receibableObject;
            int receivableId = 0;
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var receibable = context.ReceivablePayments.FirstOrDefault(r => r.InvoiceId == invoiceId);
                        if (receibable == null)
                        {
                            var receivablePayment = new ReceivablePayment()
                            {
                                InvoiceId = invoiceId,
                                CreateUser = WebSecurity.CurrentUserId,
                                CreateDate = DateTime.Now
                            };
                            context.ReceivablePayments.Add(receivablePayment);
                            context.SaveChanges();
                            receivableId = receivablePayment.Id;
                        }
                        else
                        {
                            receivableId = receibable.Id;
                        }
                        receibableObject = new
                        {
                            InvoiceId = invoiceId,
                            CreateUser = WebSecurity.CurrentUserId,
                            CreateDate = DateTime.Now,
                            Id = receivableId
                        };
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Recibo de cuenta por cobrar", receibableObject);

            return new JsonResult() { Data = new { result = true, receibableId = receivableId, message = "Recibo de pagos a factura generado." } };
        }

        #endregion

        private object InvoiceTicketsMainToObject(Invoice invoice)
        {
            var context = new TicketsEntities();
            var xpiredDate = invoice.InvoiceDate.AddDays(invoice.InvoiceExpredDay.Value);
            var ClientData = context.Clients.Where(r => r.Id == invoice.ClientId).FirstOrDefault();
            return new
            {
                invoice.Id,
                SequenceNumberInvoice = invoice.SequenceNumber,
                SequenceNumberRaffle = invoice.Raffle.SequenceNumber,
                invoice.RaffleId,
                ClientDesc = ClientData.Name,
                InvoiceDate = invoice.InvoiceDate.ToUnixTime(),
                invoice.PaymentStatu,
                clientType = ClientData.GroupId,
                Agente = invoice.InvoiceTickets.Where(i => i.InvoiceId == invoice.Id).Select(t => t.TicketAllocationNumber.TicketAllocation.Agente).FirstOrDefault(),
                xpiredDate = xpiredDate.ToUnixTime(),
                xpiredDay = (xpiredDate.Date < DateTime.Now.Date && invoice.PaymentStatu == 2082) ? "Caducada" : "",
                PaymentStatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == invoice.PaymentStatu).NameDetail,
                Payment = GetPaymentCash(invoice)
            };
        }

        /*        private object InvoiceTicketsToObject(Invoice invoice)
                {
                    var context = new TicketsEntities();
                    return new
                    {
                        invoice.Id,
                        invoice.RaffleId,
                        RaffleDesc = context.Raffles.Where(r => r.Id == invoice.RaffleId).Select(c => c.Name).FirstOrDefault(),
                        invoice.AgencyId,
                        AgencyDesc = context.Agencies.Where(r => r.Id == invoice.AgencyId).Select(c => c.Name).FirstOrDefault(),
                        invoice.ClientId,
                        ClientDesc = context.Clients.Where(r => r.Id == invoice.ClientId).Select(c => c.Name).FirstOrDefault(),
                        CraeteDate = invoice.CreateDate.ToString(),
                        InvoiceDate = invoice.InvoiceDate.ToUnixTime(),
                        invoice.Condition,
                        ConditionDesc = context.Catalogs.Where(r => r.Id == invoice.Condition).Select(c => c.NameDetail).FirstOrDefault(),
                        invoice.PaymentStatu,
                        PaymentStatuDesc = context.Catalogs.Where(r => r.Id == invoice.PaymentStatu).Select(c => c.NameDetail).FirstOrDefault(),
                        invoice.Statu,
                        StatuDesc = context.Catalogs.Where(r => r.Id == invoice.Statu).Select(c => c.NameDetail).FirstOrDefault(),
                        invoice.CreateUser,
                        Payment = GetPaymentCash(invoice),
                        InvoiceTickets = invoice.InvoiceTickets.Select(i => new
                        {
                            i.Id,
                            i.InvoiceId,
                            i.PricePerFraction,
                            i.Quantity,
                            i.TicketNumberAllocationId,
                            TicketAllocationNumber = i.TicketAllocationNumber.Number
                        })
                    };
                }*/

        #region Credit Note
        //
        // GET: /Cash/CreditNote
        [Authorize]
        [HttpGet]
        public ActionResult CreditNote()
        {
            return View();
        }

        //
        // GET: /Cash/CreditNoteTaxReceipts
        [Authorize]
        [HttpGet]
        public ActionResult CreditNoteTaxReceipts()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult CashAdvance()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult PositiveBalance()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetCashAdvanceInfo()
        {
            var context = new TicketsEntities();
            var clients = context.Clients.Where(w => w.Statu == (int)ClientStatuEnum.Approbed).Select(s => new { s.Id, s.Name }).ToList();
            var raffles1 = context.Raffles.Where(w => w.Statu == (int)RaffleStatusEnum.Planned)
                .Select(s => new
                {
                    s.Id,
                    s.SequenceNumber,
                    s.Name,
                    raffleNomenclature = s.Symbol + s.Separator + s.SequenceNumber,
                    text = s.Symbol + s.Separator + s.SequenceNumber + " " + s.Name,
                    s.DateSolteo
                }).ToList();

            var raffles = raffles1.Select(s => new
            {
                s.Id,
                s.SequenceNumber,
                s.Name,
                s.raffleNomenclature,
                text = s.text + " " + s.DateSolteo.ToString("dd/MM/yyyy")
            }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    clients,
                    raffles
                }
            };
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetPositiveBalanceInfo()
        {
            var context = new TicketsEntities();
            var clients = context.Clients.Where(w => w.Statu == (int)ClientStatuEnum.Approbed).Select(s => new { s.Id, s.Name }).ToList();
            var raffles1 = context.Raffles.Where(w => w.Statu == (int)RaffleStatusEnum.Planned)
                .Select(s => new
                {
                    s.Id,
                    s.SequenceNumber,
                    s.Name,
                    raffleNomenclature = s.Symbol + s.Separator + s.SequenceNumber,
                    text = s.Symbol + s.Separator + s.SequenceNumber + " " + s.Name,
                    s.DateSolteo
                }).ToList();

            var raffles = raffles1.Select(s => new
            {
                s.Id,
                s.SequenceNumber,
                s.Name,
                text = s.text + " " + s.DateSolteo.ToString("dd/MM/yyyy")
            }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    clients,
                    raffles
                }
            };
        }

        //
        //GET: /Cash/CreditNote
        [Authorize]
        [HttpGet]
        public JsonResult GetCreditNoteData(int creditNoteId, int identifyBachId)
        {
            var context = new TicketsEntities();
            var clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
            {
                c.Id,
                c.Name
            }).ToList();

            object creditNote;
            if (creditNoteId > 0)
            {
                creditNote = CreditNoteToObject(context.NoteCredits.FirstOrDefault(r => r.Id == creditNoteId));
            }
            else
            {
                if (identifyBachId == 0)
                {
                    creditNote = new object();
                }
                else
                {
                    var identifyBach = context.IdentifyBaches.Where(i => i.Id == identifyBachId).FirstOrDefault();
                    var identifyBaches = new List<IdentifyBach>();
                    creditNote = new
                    {
                        Id = 0,
                        identifyBach.ClientId,
                        TotalCash = ""
                    };
                }
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    clients,
                    creditNote
                }
            };
        }

        //
        //GET: /Cash/CreditNote
        [Authorize]
        [HttpGet]
        public JsonResult GetCreditNoteTaxReceiptData(int creditNoteId, int identifyBachId)
        {
            var context = new TicketsEntities();
            /*var clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
            {
                c.Id,
                c.Name
            }).ToList();*/

            var clients = context.TaxReceiptNumbersHistories.Where(w => w.Invoice.PaymentStatu == (int)InvoicePaymentStatuEnum.Pendient)
                .Select(s => new
                {
                    s.InvoiceId,
                    s.Invoice.SequenceNumber,
                    s.Invoice.Client.Name,
                    s.TaxReceiptNumber.TaxReceipt.Catalog.Description2,
                    s.TaxReceiptNumber.Number,
                    s.Invoice.CreateDate
                }).ToList().Where(w => w.CreateDate.Date.AddDays(30) >= DateTime.Now.Date).Select(s2 => new
                {
                    Id = s2.InvoiceId,
                    Name = String.Concat(s2.Description2, s2.Number.ToString().PadLeft(8, '0'), " - ", s2.Name)
                }).ToList();

            object creditNote;
            if (creditNoteId > 0)
            {
                creditNote = CreditNoteToObject(context.NoteCredits.FirstOrDefault(r => r.Id == creditNoteId));
            }
            else
            {
                if (identifyBachId == 0)
                {
                    creditNote = new object();
                }
                else
                {
                    var identifyBach = context.IdentifyBaches.Where(i => i.Id == identifyBachId).FirstOrDefault();
                    var identifyBaches = new List<IdentifyBach>();
                    creditNote = new
                    {
                        Id = 0,
                        identifyBach.ClientId,
                        TotalCash = ""
                    };
                }
            }

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    clients,
                    creditNote
                }
            };
        }

        private object CreditNoteToObject(NoteCredit noteCredit)
        {
            return new
            {
                noteCredit.Id,
                noteCredit.SequenceNumber,
                noteCredit.Nomenclature,
                noteCredit.ClientId,
                CreateDate = noteCredit.CreateDate.ToString("dd/MM/yyyy"),
                noteCredit.CreateUser,
                NoteDate = noteCredit.NoteDate.ToShortDateString(),
                noteCredit.Statu,
                noteCredit.TotalCash,
                noteCredit.TotalRest,
                Concepts = noteCredit.IdentifyBaches.Count > 0 ? noteCredit.Concepts + " Lote " + noteCredit.IdentifyBaches.FirstOrDefault().SequenceNumber :
                noteCredit.RaffleId.HasValue ? noteCredit.Concepts + " del Sorteo " + (noteCredit.Raffle.Symbol + noteCredit.Raffle.Separator + noteCredit.Raffle.SequenceNumber) : noteCredit.Concepts
            };
        }

        //
        // POST: /Cash/CashAdvance
        [Authorize]
        [HttpPost]
        public JsonResult CashAdvance(NoteCredit creditNote)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (creditNote.Id == 0)
                        {
                            var identifyBachId = 0;
                            Client client;
                            if (creditNote.IdentifyBaches.Where(r => r.Id > 0).Any())
                            {
                                identifyBachId = creditNote.IdentifyBaches.FirstOrDefault().Id;
                                client = context.IdentifyBaches.FirstOrDefault(i => i.Id == identifyBachId).Client;
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = client.GroupId == (int)ClientGroupEnum.Mayorista ? 0 : 0;
                            }
                            else
                            {
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = 0;
                            }
                            creditNote.NoteDate = DateTime.Now.Date;
                            creditNote.TotalRest = creditNote.TotalCash;
                            creditNote.CreateDate = DateTime.Now;
                            creditNote.CreateUser = WebSecurity.CurrentUserId;
                            creditNote.Statu = (int)GeneralStatusEnum.Active;
                            creditNote.TypeNote = (int)NoteCreditEnum.CashAdvance;
                            context.NoteCredits.Add(creditNote);
                            context.SaveChanges();
                            if (identifyBachId > 0)
                            {
                                creditNote.IdentifyBaches.Add(context.IdentifyBaches.FirstOrDefault(r => r.Id == identifyBachId));
                            }
                            context.SaveChanges();
                        }
                        else
                        {
                            var mCreditNote = context.NoteCredits.FirstOrDefault(n => n.Id == creditNote.Id);
                            mCreditNote.ClientId = creditNote.ClientId;
                            mCreditNote.NoteDate = creditNote.CreateDate;
                            mCreditNote.TotalCash = creditNote.TotalCash;
                            mCreditNote.TotalRest = creditNote.TotalRest;
                            mCreditNote.Concepts = creditNote.Concepts;
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", mCreditNote);
                        }
                        tx.Commit();
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Avance de efectivo creada", creditNote);

                        return new JsonResult() { Data = new { result = true, message = "Avance de efectivo completado.", cashAdvance = creditNote.Id } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // POST: /Cash/PositiveBalance
        [Authorize]
        [HttpPost]
        public JsonResult PositiveBalance(NoteCredit creditNote)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (creditNote.Id == 0)
                        {
                            var identifyBachId = 0;
                            Client client;
                            if (creditNote.IdentifyBaches.Where(r => r.Id > 0).Any())
                            {
                                identifyBachId = creditNote.IdentifyBaches.FirstOrDefault().Id;
                                client = context.IdentifyBaches.FirstOrDefault(i => i.Id == identifyBachId).Client;
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = client.GroupId == (int)ClientGroupEnum.Mayorista ? 0 : 0;
                            }
                            else
                            {
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = 0;
                            }
                            creditNote.NoteDate = DateTime.Now.Date;
                            creditNote.TotalRest = creditNote.TotalCash;
                            creditNote.CreateDate = DateTime.Now;
                            creditNote.CreateUser = WebSecurity.CurrentUserId;
                            creditNote.Statu = (int)GeneralStatusEnum.Active;
                            creditNote.TypeNote = (int)NoteCreditEnum.PositiveBalance;
                            context.NoteCredits.Add(creditNote);
                            context.SaveChanges();
                            if (identifyBachId > 0)
                            {
                                creditNote.IdentifyBaches.Add(context.IdentifyBaches.FirstOrDefault(r => r.Id == identifyBachId));
                            }
                            context.SaveChanges();
                        }
                        else
                        {
                            var mCreditNote = context.NoteCredits.FirstOrDefault(n => n.Id == creditNote.Id);
                            mCreditNote.ClientId = creditNote.ClientId;
                            mCreditNote.NoteDate = creditNote.CreateDate;
                            mCreditNote.TotalCash = creditNote.TotalCash;
                            mCreditNote.TotalRest = creditNote.TotalRest;
                            mCreditNote.Concepts = creditNote.Concepts;
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", mCreditNote);
                        }
                        tx.Commit();
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Saldo a favor creado", creditNote);

                        return new JsonResult() { Data = new { result = true, message = "Saldo a favor completado.", positiveBalance = creditNote.Id } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // GET: /Cash/CreditNote
        [Authorize]
        [HttpPost]
        public JsonResult CreditNote(NoteCredit creditNote)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (creditNote.Id == 0)
                        {
                            var identifyBachId = 0;
                            Client client;
                            if (creditNote.IdentifyBaches.Where(r => r.Id > 0).Any())
                            {
                                identifyBachId = creditNote.IdentifyBaches.FirstOrDefault().Id;
                                client = context.IdentifyBaches.FirstOrDefault(i => i.Id == identifyBachId).Client;
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = client.GroupId == (int)ClientGroupEnum.Mayorista || client.GroupId == (int)ClientGroupEnum.DistribuidorElectronico ? 2 : 0;
                            }
                            else
                            {
                                creditNote.IdentifyBaches = new List<IdentifyBach>();
                                creditNote.DiscountPercent = 0;
                            }
                            creditNote.NoteDate = DateTime.Now.Date;
                            creditNote.TotalRest = creditNote.TotalCash;
                            creditNote.CreateDate = DateTime.Now;
                            creditNote.CreateUser = WebSecurity.CurrentUserId;
                            creditNote.Statu = (int)GeneralStatusEnum.Active;
                            creditNote.TypeNote = (int)NoteCreditEnum.NoteCredit;
                            context.NoteCredits.Add(creditNote);
                            context.SaveChanges();
                            if (identifyBachId > 0)
                            {
                                creditNote.IdentifyBaches.Add(context.IdentifyBaches.FirstOrDefault(r => r.Id == identifyBachId));
                            }
                            context.SaveChanges();
                        }
                        else
                        {
                            var mCreditNote = context.NoteCredits.FirstOrDefault(n => n.Id == creditNote.Id);
                            mCreditNote.ClientId = creditNote.ClientId;
                            mCreditNote.NoteDate = creditNote.CreateDate;
                            mCreditNote.TotalCash = creditNote.TotalCash;
                            mCreditNote.TotalRest = creditNote.TotalRest;
                            mCreditNote.Concepts = creditNote.Concepts;
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", mCreditNote);
                        }
                        tx.Commit();
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", creditNote);

                        return new JsonResult() { Data = new { result = true, message = "Nota de Credito Completada." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // GET: /Cash/CreditNote
        [Authorize]
        [HttpPost]
        public JsonResult CreditNoteTaxReceipt(NoteCredit creditNote)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (creditNote.Id == 0)
                        {
                            var user = context.Users.AsEnumerable().Where(u => u.Id == WebSecurity.CurrentUserId).FirstOrDefault();
                            var cashInfo = new Cash
                            {
                                CraeteDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                AgencyId = user.Employee.AgencyId,
                                Statu = (int)CashStatusEnum.Close,
                                Name = DateTime.Now.Date.ToString()
                            };

                            context.Cashes.Add(cashInfo);
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, cashInfo.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Cliente", CashToObject(cashInfo));

                            var InvoiceInfo = context.Invoices.Where(w => w.Id == creditNote.ClientId).FirstOrDefault();
                            var TotalInvoice = InvoiceInfo.InvoiceTickets.Sum(s => s.Quantity * s.PricePerFraction);

                            var TaxReceiptNoteCredit = context.TaxReceiptNumbers
                                .FirstOrDefault(w => w.TaxReceipt.Type == (int)TaxReceiptTypeEnum.NotaCredito &&
                                       w.TaxReceipt.DueDate >= DateTime.Now &&
                                       w.TaxReceipt.Statu == (int)TaxReceiptStatuEnum.Activo &&
                                       w.Status == (int)TaxReceiptNumberStatuEnum.Disponible);

                            if (TaxReceiptNoteCredit == null)
                            {
                                tx.Rollback();
                                return new JsonResult() { Data = new { result = false, message = "No hay números de comprobante de nota de crédito" } };
                            }

                            creditNote.ClientId = InvoiceInfo.ClientId;
                            creditNote.IdentifyBaches = new List<IdentifyBach>();
                            creditNote.TotalRest = TotalInvoice >= creditNote.TotalCash ? 0 : creditNote.TotalCash - TotalInvoice;
                            creditNote.NoteDate = DateTime.Now.Date;
                            creditNote.CreateUser = WebSecurity.CurrentUserId;
                            creditNote.CreateDate = DateTime.Now;
                            creditNote.DiscountPercent = 0;
                            creditNote.Statu = TotalInvoice >= creditNote.TotalCash ? (int)GeneralStatusEnum.Delete : (int)GeneralStatusEnum.Active;
                            creditNote.RaffleId = InvoiceInfo.RaffleId;
                            creditNote.TypeNote = (int)NoteCreditEnum.NoteTaxReceipt;
                            creditNote.TaxReceiptNumberId = TaxReceiptNoteCredit.Id;

                            context.NoteCredits.Add(creditNote);
                            context.SaveChanges();

                            var receiptPayment = new ReceiptPayment
                            {
                                CashId = cashInfo.Id,
                                ClientId = InvoiceInfo.ClientId,
                                InvoiceId = InvoiceInfo.Id,
                                ReceiptDate = DateTime.Now,
                                TotalCash = 0,
                                TotalCredit = 0,
                                TotalCheck = 0,
                                ReceiptType = (int)PaymentTypeEnum.TaxReceiptNoteCredit,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId
                            };
                            context.ReceiptPayments.Add(receiptPayment);
                            context.SaveChanges();

                            var noteCreditReceiptPayment = new NoteCreditReceiptPayment
                            {
                                NoteCreditId = creditNote.Id,
                                ReceiptPaymentId = receiptPayment.Id,
                                TotalCash = TotalInvoice >= creditNote.TotalCash ? creditNote.TotalCash : creditNote.TotalCash - TotalInvoice
                            };
                            context.NoteCreditReceiptPayments.Add(noteCreditReceiptPayment);
                            context.SaveChanges();

                            TaxReceiptNoteCredit.Status = (int)TaxReceiptNumberStatuEnum.Ocupado;
                            context.SaveChanges();
                        }

                        tx.Commit();
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", creditNote);

                        return new JsonResult() { Data = new { result = true, message = "Nota de crédito fiscal completada." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // GET: /Cash/CreditNoteList
        [Authorize]
        [HttpGet]
        public ActionResult CreditNoteList()
        {
            return View();
        }

        //
        // GET: /Cash/CreditNoteListTaxReceipt
        [Authorize]
        [HttpGet]
        public ActionResult CreditNoteListTaxReceipt()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult CashAdvanceList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult PositiveBalanceList()
        {
            return View();
        }

        //
        // GET: /Cash/GetCreditNoteList
        [Authorize]
        [HttpGet]
        public JsonResult GetCreditNoteList(int clientId)
        {
            var context = new TicketsEntities();
            List<object> clients = new List<object>();
            List<object> creditNotes = new List<object>();
            if (clientId == 0)
            {
                clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList<object>();

                creditNotes = context.NoteCredits.Where(w => w.TypeNote == (int)NoteCreditEnum.NoteCredit)
                    .AsEnumerable().OrderByDescending(n => n.Id).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.Concepts,
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }
            else
            {
                creditNotes = context.NoteCredits.AsEnumerable().OrderByDescending(n => n.Id)
                    .Where(c => c.ClientId == clientId && c.TypeNote == (int)NoteCreditEnum.NoteCredit).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }

            return new JsonResult() { Data = new { clients, creditNotes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Cash/GetCreditNoteTaxReceiptList
        [Authorize]
        [HttpGet]
        public JsonResult GetCreditNoteTaxReceiptList(int clientId)
        {
            var context = new TicketsEntities();
            List<object> clients = new List<object>();
            List<object> creditNotes = new List<object>();
            if (clientId == 0)
            {
                clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList<object>();

                creditNotes = context.NoteCredits.Where(w => w.TypeNote == (int)NoteCreditEnum.NoteTaxReceipt)
                    .AsEnumerable().OrderByDescending(n => n.Id).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.Concepts,
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }
            else
            {
                creditNotes = context.NoteCredits.AsEnumerable().OrderByDescending(n => n.Id)
                    .Where(c => c.ClientId == clientId && (c.TypeNote == (int)NoteCreditEnum.NoteTaxReceipt)).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }

            return new JsonResult() { Data = new { clients, creditNotes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetCashAdvancesList(int clientId)
        {
            var context = new TicketsEntities();
            List<object> clients = new List<object>();
            List<object> creditNotes = new List<object>();
            if (clientId == 0)
            {
                clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList<object>();

                creditNotes = context.NoteCredits.Where(w => w.TypeNote == (int)NoteCreditEnum.CashAdvance).AsEnumerable().OrderByDescending(n => n.Id).Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                    ClientDesc = c.Client.Name,
                    NoteDate = c.NoteDate.ToShortDateString(),
                    c.Concepts,
                    c.TotalCash,
                    c.TotalRest
                }).ToList<object>();
            }
            else
            {
                creditNotes = context.NoteCredits.AsEnumerable().OrderByDescending(n => n.Id)
                    .Where(c => c.ClientId == clientId && c.TypeNote == (int)NoteCreditEnum.CashAdvance).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }

            return new JsonResult() { Data = new { clients, creditNotes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetPositiveBalanceList(int clientId)
        {
            var context = new TicketsEntities();
            List<object> clients = new List<object>();
            List<object> creditNotes = new List<object>();
            if (clientId == 0)
            {
                clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
                {
                    c.Id,
                    c.Name
                }).ToList<object>();

                creditNotes = context.NoteCredits.Where(w => w.TypeNote == (int)NoteCreditEnum.PositiveBalance).AsEnumerable().OrderByDescending(n => n.Id).Select(c => new
                {
                    c.Id,
                    c.ClientId,
                    SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                    ClientDesc = c.Client.Name,
                    NoteDate = c.NoteDate.ToShortDateString(),
                    c.Concepts,
                    c.TotalCash,
                    c.TotalRest
                }).ToList<object>();
            }
            else
            {
                creditNotes = context.NoteCredits.AsEnumerable().OrderByDescending(n => n.Id)
                    .Where(c => c.ClientId == clientId && c.TypeNote == (int)NoteCreditEnum.PositiveBalance).Select(c => new
                    {
                        c.Id,
                        c.ClientId,
                        SequenceNumberNoteCredit = c.Nomenclature == null ? c.SequenceNumber.Value.ToString() : string.Concat(c.Nomenclature, "-", c.SequenceNumber.Value.ToString().PadLeft(5, '0')),
                        ClientDesc = c.Client.Name,
                        NoteDate = c.NoteDate.ToShortDateString(),
                        c.TotalCash,
                        c.TotalRest
                    }).ToList<object>();
            }

            return new JsonResult() { Data = new { clients, creditNotes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // POST: /Cash/CreditNoteGroupApply
        [Authorize]
        [HttpPost]
        public JsonResult CreditNoteGroupApply(string group, int raffleId, int clientId)
        {
            var groups = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(group);
            //object note;
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    context.Database.CommandTimeout = (10 * 60);
                    try
                    {
                        //decimal totalPrice = 0.0M;
                        var clientReturneds = context.Clients.FirstOrDefault(c => c.Id == clientId)
                            .TicketReturns.AsEnumerable().Where(r => r.RaffleId == raffleId && r.Statu == (int)TicketReturnedStatuEnum.Created);

                        //decimal discountValue = clientReturneds.FirstOrDefault().Discount;

                        /*var price = clientReturneds.Where(r => groups.Where(g => g == r.ReturnedGroup).Any())
                            .FirstOrDefault().TicketAllocationNumber.InvoiceTickets.FirstOrDefault().PricePerFraction;*/

                        var returnesToChanged = clientReturneds
                            .Where(r =>
                            groups.Where(g => g == r.ReturnedGroup).Any()
                            ).ToList();

                        foreach (var ret in returnesToChanged)
                        {
                            //totalPrice += (ret.FractionTo - ret.FractionFrom + 1) * price;
                            ret.Statu = (int)TicketReturnedStatuEnum.Invoiced;
                        }
                        context.SaveChanges();

                        /*decimal discountTotal = totalPrice * discountValue / 100;
                        var creditNote = new NoteCredit()
                        {
                            ClientId = clientId,
                            Concepts = "Nota de credito por devolución de billetes",
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            NoteDate = DateTime.Now,
                            Statu = (int)GeneralStatusEnum.Active,
                            TotalCash = (totalPrice - discountTotal),
                            TotalRest = (totalPrice - discountTotal),
                            RaffleId = raffleId
                        };
                        context.NoteCredits.Add(creditNote);
                        context.SaveChanges();
                        note = CreditNoteToObject(context.NoteCredits.FirstOrDefault(n => n.Id == creditNote.Id));*/
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
            //Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito", note);
            return new JsonResult() { Data = new { result = true, message = "Devoluciones guardadas correctamente!" } };
        }

        //
        //  GET: Cash/GetCreditNoteReturneds
        [Authorize]
        [HttpGet]
        public JsonResult GetCreditNoteReturneds()
        {
            var context = new TicketsEntities();
            var hasRole = false;
            var user = context.Users.FirstOrDefault(u => u.Id == WebSecurity.CurrentUserId);
            user.webpages_Roles.ToList().ForEach(r => r.Rol_Module.ToList().ForEach(m =>
            {
                if (m.Module.Name == "app.cashReceivableCreate" && m.CanDelete)
                {
                    hasRole = true;
                }
            }));
            var creditNotes = context.NoteCredits.Where(n => n.TotalRest > 0 && n.RaffleId.HasValue && hasRole).Select(n => new
            {
                clientname = n.Client.Name,
                raffleId = n.RaffleId.Value
            });
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = creditNotes
            };
        }
        #endregion
        #region Previous Debt
        //
        // GET: /Cash/PreviousDebt
        [Authorize]
        [HttpGet]
        public ActionResult PreviousDebt()
        {
            return View();
        }

        //
        // GET: /Cash/GetPreviousDebt
        [Authorize]
        [HttpGet]
        public ActionResult GetPreviousDebt()
        {
            var context = new TicketsEntities();

            var clients = context.Clients.AsEnumerable().Where(c =>
                c.Statu == (int)ClientStatuEnum.Approbed
                && c.GroupId == (int)ClientGroupEnum.Mayorista
                && c.PreviousDebt > 0)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.PreviousDebt,
                    PrevieusPayment = r.PreviousDebtPayments.Count > 0 ? r.PreviousDebtPayments.Select(p => p.PaimentValue).Aggregate((i, s) => i + s) : 0
                }).ToList();

            return new JsonResult() { Data = clients, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        #endregion
        #region Payment
        //
        // GET: /Cash/CheckPayment
        [Authorize]
        [HttpGet]
        public ActionResult CheckPayment()
        {
            return View();
        }

        //
        // GET: /Cash/NaturePayment
        [Authorize]
        [HttpGet]
        public ActionResult NaturePayment()
        {
            return View();
        }

        //
        // GET: /Cash/Payment
        [Authorize]
        [HttpGet]
        public ActionResult Payment()
        {
            return View();
        }

        //
        //GET: /Cash/GetPaymentData
        [Authorize]
        [HttpGet]
        public JsonResult GetPaymentData()
        {
            var context = new TicketsEntities();

            var clients = context.Clients.Where(c => c.Statu == (int)ClientStatuEnum.Approbed).Select(c => new
            {
                c.Id,
                c.Name
            }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    clients
                }
            };
        }

        //
        // POST: /Cash/CheckPayment
        [Authorize]
        [HttpPost]
        public JsonResult CheckPayment(IdentifyBachPayment payment)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        payment.Note = string.IsNullOrEmpty(payment.Note) ? "" : payment.Note;
                        var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
                        var identifyBach = context.IdentifyBaches.Where(n => n.Id == payment.IdentifyBachId).FirstOrDefault();

                        payment.DiscountPercent = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista ? 2 : 0;
                        if (payment.Id == 0)
                        {
                            payment.CreateDate = DateTime.Now;
                            payment.CreateUser = WebSecurity.CurrentUserId;
                            payment.ClientId = identifyBach.ClientId;
                            payment.CashId = cash.Id;
                            payment.PaymentType = (int)BachPaymentTypeEnum.Check;
                            context.IdentifyBachPayments.Add(payment);
                        }
                        else
                        {
                            var mPayment = context.IdentifyBachPayments.FirstOrDefault(n => n.Id == payment.Id);
                            mPayment.ClientId = payment.ClientId;
                            mPayment.Note = payment.Note;
                            mPayment.Value = payment.Value;
                            mPayment.PaymentType = (int)BachPaymentTypeEnum.Check;
                        }
                        context.SaveChanges();
                        tx.Commit();
                        return new JsonResult() { Data = new { result = true, message = "Pago Completado." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // POST: /Cash/NaturePayment
        [Authorize]
        [HttpPost]
        public JsonResult NaturePayment(IdentifyBachPayment payment)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        payment.Note = string.IsNullOrEmpty(payment.Note) ? "" : payment.Note;
                        var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
                        var identifyBach = context.IdentifyBaches.Where(n => n.Id == payment.IdentifyBachId).FirstOrDefault();

                        if (payment.Id == 0)
                        {
                            payment.CreateDate = DateTime.Now;
                            payment.CreateUser = WebSecurity.CurrentUserId;
                            payment.ClientId = identifyBach.ClientId;
                            payment.CashId = cash.Id;
                            payment.PaymentType = (int)BachPaymentTypeEnum.Nature;
                            context.IdentifyBachPayments.Add(payment);
                        }
                        else
                        {
                            var mPayment = context.IdentifyBachPayments.FirstOrDefault(n => n.Id == payment.Id);
                            mPayment.ClientId = payment.ClientId;
                            mPayment.Note = payment.Note;
                            mPayment.Value = payment.Value;
                            mPayment.PaymentType = (int)BachPaymentTypeEnum.Nature;
                        }
                        context.SaveChanges();
                        tx.Commit();
                        return new JsonResult() { Data = new { result = true, message = "Pago Completado." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        //
        // POST: /Cash/Payment
        [Authorize]
        [HttpPost]
        public JsonResult Payment(IdentifyBachPayment payment)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        payment.Note = string.IsNullOrEmpty(payment.Note) ? "" : payment.Note;
                        var cash = context.Cashes.FirstOrDefault(c => c.Statu == (int)CashStatusEnum.Open && c.CreateUser == WebSecurity.CurrentUserId);
                        var identifyBach = context.IdentifyBaches.Where(n => n.Id == payment.IdentifyBachId).FirstOrDefault();

                        payment.DiscountPercent = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista ? 2 : 0;
                        if (payment.Id == 0)
                        {
                            payment.CreateDate = DateTime.Now;
                            payment.CreateUser = WebSecurity.CurrentUserId;
                            payment.ClientId = identifyBach.ClientId;
                            payment.CashId = cash.Id;
                            payment.PaymentType = (int)BachPaymentTypeEnum.Cash;
                            context.IdentifyBachPayments.Add(payment);
                        }
                        else
                        {
                            var mPayment = context.IdentifyBachPayments.FirstOrDefault(n => n.Id == payment.Id);
                            mPayment.ClientId = payment.ClientId;
                            mPayment.Note = payment.Note;
                            mPayment.Value = payment.Value;
                            mPayment.PaymentType = (int)BachPaymentTypeEnum.Cash;
                        }
                        context.SaveChanges();
                        tx.Commit();
                        return new JsonResult() { Data = new { result = true, message = "Pago Completado." } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }
        #endregion
    }
}
