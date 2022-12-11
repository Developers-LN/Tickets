using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    public class TaxReceiptController : Controller
    {
        // GET: TaxReceipt
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        // GET: TaxReceipt/Details/5
        public ActionResult TaxReceiptList()
        {
            return View();
        }

        // GET: TaxReceipt/Create
        public ActionResult TaxReceipt()
        {
            return View();
        }

        // POST: TaxReceipt/Create
        [Authorize]
        [HttpPost]
        public JsonResult TaxReceipt(TaxReceipt taxReceipt)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (taxReceipt.Id == 0)
                        {
                            taxReceipt.Statu = (int)TaxReceiptStatuEnum.Activo;
                            taxReceipt.CreateUser = WebSecurity.CurrentUserId;
                            taxReceipt.CreateDate = DateTime.Now.Date;
                            taxReceipt.Type = taxReceipt.Type;
                            taxReceipt.Notas = taxReceipt.Notas;
                            taxReceipt.DueDate = taxReceipt.DueDate;
                            taxReceipt.SequenceFrom = taxReceipt.SequenceFrom;
                            taxReceipt.SequenceTo = taxReceipt.SequenceTo;
                            context.TaxReceipts.Add(taxReceipt);
                            context.SaveChanges();

                            var Start = taxReceipt.SequenceFrom;
                            List<TaxReceiptNumber> taxReceiptNumbers = new List<TaxReceiptNumber>();

                            while (Start <= taxReceipt.SequenceTo)
                            {
                                TaxReceiptNumber number = new TaxReceiptNumber()
                                {
                                    Number = Start,
                                    TaxReceiptId = taxReceipt.Id,
                                    Status = (int)TaxReceiptNumberStatuEnum.Disponible
                                };
                                taxReceiptNumbers.Add(number);
                                Start++;
                            }
                            context.TaxReceiptNumbers.AddRange(taxReceiptNumbers);
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Comprobante fiscal agregado", taxReceipt);
                        }
                        else
                        {
                            /*
                            var mCreditNote = context.NoteCredits.FirstOrDefault(n => n.Id == creditNote.Id);
                            mCreditNote.ClientId = creditNote.ClientId;
                            mCreditNote.NoteDate = creditNote.CreateDate;
                            mCreditNote.TotalCash = creditNote.TotalCash;
                            mCreditNote.TotalRest = creditNote.TotalRest;
                            mCreditNote.Concepts = creditNote.Concepts;
                            context.SaveChanges();
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Nota de Credito Editada", mCreditNote);
                            */
                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Comprobante fiscal editado", taxReceipt);
                        }
                        tx.Commit();

                        return new JsonResult() { Data = new { result = true, message = "Secuencia de comprobante fiscal agregada" } };
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetTaxReceiptList()
        {
            var context = new TicketsEntities();
            List<object> taxReceipts = new List<object>();

            if (context.TaxReceipts.Any(a => a.DueDate < DateTime.Now))
            {
                var dueTax = context.TaxReceipts.Where(a => a.DueDate < DateTime.Now).ToList();
                foreach (var tax in dueTax)
                {
                    tax.Statu = (int)TaxReceiptStatuEnum.Caducado;
                }
                context.SaveChanges();
            }

            taxReceipts = context.TaxReceipts.AsEnumerable().OrderByDescending(o => o.Id).Select(s => new
            {
                Id = s.Id,
                type = context.Catalogs.Where(w => w.Id == s.Type).FirstOrDefault().NameDetail,
                CreateDate = s.CreateDate.ToString("dd/MM/yyyy"),
                DueDate = s.DueDate.ToString("dd/MM/yyyy"),
                from = s.SequenceFrom,
                to = s.SequenceTo,
                statu = context.Catalogs.Where(w => w.Id == s.Statu).FirstOrDefault().NameDetail,
                total = s.TaxReceiptNumbers.Count,
                available = s.TaxReceiptNumbers.Where(w => w.Status != (int)TaxReceiptNumberStatuEnum.Ocupado).Count()
            }).ToList<object>();

            return new JsonResult() { Data = new { taxReceipts }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpGet]
        public JsonResult CheckSequenceRange(int From, int To, int Type)
        {
            var context = new TicketsEntities();
            var result = false;
            var message = "";
            var Min = context.TaxReceipts.Any() == false ? 0 : context.TaxReceipts.Where(w => w.Type == Type).Min(mi => mi.SequenceFrom);
            var Max = context.TaxReceipts.Any() == false ? 0 : context.TaxReceipts.Where(w => w.Type == Type).Max(ma => ma.SequenceTo);

            if (From < To)
            {
                if (Min != 0 || Max != 0)
                {
                    if ((From <= Min || From <= Max) || (To <= Min || To <= Max))
                    {
                        result = true;
                        message = "Este rango de secuencia ya esta en uso";
                    }
                }
            }
            else
            {
                result = true;
                message = "La secuencia desde no debe ser mayor que la secuencia hasta";
            }
            return new JsonResult() { Data = new { result, message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetTaxReceiptInfo()
        {
            var context = new TicketsEntities();
            var taxType = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.TaxReceiptType && w.Statu == true).Select(s => new { s.Id, s.NameDetail }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    taxType
                }
            };
        }

        // GET: TaxReceipt/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TaxReceipt/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TaxReceipt/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TaxReceipt/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Client/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();

            var taxreceipts = context.Catalogs.Select(r => new
            {
                r.Id,
                r.NameDetail,
                r.IdDetail,
                r.IdGroup
            }).Where(w => w.IdGroup == (int)CatalogGroupEnum.TaxReceiptType && w.Id != (int)TaxReceiptTypeEnum.NotReceipt).OrderBy(o => o.IdDetail).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de facturas");

            return new JsonResult() { Data = new { taxreceipts }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
