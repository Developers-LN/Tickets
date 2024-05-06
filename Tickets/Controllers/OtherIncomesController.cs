using System;
using System.Linq;
using System.Web.Mvc;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    public class OtherIncomesController : Controller
    {
        // GET: OtherIncomes
        public ActionResult OtherIncomesList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult OtherIncomesPaymentHistory()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetPaymentHistory(int otherIncomeId)
        {
            var context = new TicketsEntities();

            var otherIncomeData = context.OtherIncomes.FirstOrDefault(f => f.Id == otherIncomeId);
            var catalog = context.Catalogs.Select(s => new { s.Id, s.NameDetail }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    otherIncomeData.Id,
                    otherIncomeData.NoCatalogAccount,
                    otherIncomeData.AccountName,
                    origin = catalog.FirstOrDefault(f => f.Id == otherIncomeData.Origin).NameDetail,
                    accountType = catalog.FirstOrDefault(f => f.Id == otherIncomeData.AccountType).NameDetail,
                    otherIncomeData.Description,
                    periodicity = catalog.FirstOrDefault(f => f.Id == otherIncomeData.Periodicity).NameDetail,
                    status = catalog.FirstOrDefault(f => f.Id == otherIncomeData.Status).NameDetail,
                    paymentHistory = otherIncomeData.OtherIncomeDetails.Select(s => new
                    {
                        s.Id,
                        s.Total,
                        s.Description,
                        PaymentDate = s.PaymentDate.Value.ToShortDateString(),
                        s.OtherIncomeId,
                        s.SequenceNumber
                    })
                }
            };
        }

        public ActionResult OtherIncomesPaymentList()
        {
            return View();
        }

        // GET: /Client/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var otherIncomes = context.OtherIncomes.AsEnumerable().Where(w => w.Status == (int)GeneralStatusEnum.Active).Select(s => OtherIncomeList(s)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de otros ingresos");

            return new JsonResult() { Data = new { otherIncomes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetOtherIncomePaymentList(int otherIncomeId)
        {
            var context = new TicketsEntities();
            var otherIncomesPayments = context.OtherIncomeDetails.Where(w => w.OtherIncomeId == otherIncomeId).AsEnumerable().Select(s => OtherIncomeDetailList(s)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de pagos por otros ingresos");

            return new JsonResult() { Data = new { otherIncomesPayments }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object OtherIncomeList(OtherIncome o)
        {
            return new
            {
                o.Id,
                o.NoCatalogAccount,
                o.AccountName,
                Origin = o.Catalog.NameDetail,
                AccountType = o.Catalog1.NameDetail,
                Periodicity = o.Catalog2.NameDetail,
                HasPayment = o.OtherIncomeDetails.Any() ? 1 : 0,
            };
        }

        private object OtherIncomeDetailList(OtherIncomeDetail o)
        {
            return new
            {
                o.Id,
                o.OtherIncome.NoCatalogAccount,
                o.OtherIncome.AccountName,
                o.Total,
                o.Description,
                o.CreateDate,
                PaymentDate = o.PaymentDate.Value.ToShortDateString(),
                o.OtherIncomeId,
                o.SequenceNumber
            };
        }

        // GET: OtherIncomes/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CreatePayment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePayment(OtherIncomeDetail otherIncomePayement)
        {
            if (otherIncomePayement.Id <= 0)
            {
                using (var context = new TicketsEntities())
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            otherIncomePayement.Total = otherIncomePayement.Total;
                            otherIncomePayement.Description = otherIncomePayement.Description;
                            otherIncomePayement.CreateDate = DateTime.Now;
                            otherIncomePayement.CreateUser = WebSecurity.CurrentUserId;
                            otherIncomePayement.PaymentDate = otherIncomePayement.PaymentDate;
                            otherIncomePayement.OtherIncomeId = otherIncomePayement.OtherIncomeId;
                            otherIncomePayement.BankAccountCatalogId = otherIncomePayement.BankAccountCatalogId;
                            context.OtherIncomeDetails.Add(otherIncomePayement);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            tx.Rollback();
                            return new JsonResult()
                            {
                                Data = new
                                {
                                    result = false,
                                    message = e.Message
                                }
                            };
                        }
                        tx.Commit();
                    }
                }
            }
            else
            {
                try
                {
                    using (var context = new TicketsEntities())
                    {
                        var motherIncomeDetail = context.OtherIncomeDetails.FirstOrDefault(c => c.Id == otherIncomePayement.Id);
                        motherIncomeDetail.Total = otherIncomePayement.Total;
                        motherIncomeDetail.Description = otherIncomePayement.Description;
                        motherIncomeDetail.PaymentDate = otherIncomePayement.PaymentDate;
                        motherIncomeDetail.OtherIncomeId = otherIncomePayement.OtherIncomeId;
                        context.Entry(motherIncomeDetail).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            result = false,
                            message = "Error al crear la cuenta de ingresos"
                        }
                    };
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, otherIncomePayement.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Pago por otros ingresos", OtherIncomePaymentToObject(otherIncomePayement));
            return new JsonResult() { Data = new { result = true, paymentId = otherIncomePayement.Id, } };
        }

        // POST: OtherIncomes/Create
        [HttpPost]
        public ActionResult Create(OtherIncome otherIncome)
        {
            if (otherIncome.Id <= 0)
            {
                using (var context = new TicketsEntities())
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            otherIncome.NoCatalogAccount = otherIncome.NoCatalogAccount;
                            otherIncome.AccountName = otherIncome.AccountName;
                            otherIncome.Origin = otherIncome.Origin;
                            otherIncome.AccountType = otherIncome.AccountType;
                            otherIncome.Description = otherIncome.Description;
                            otherIncome.Periodicity = otherIncome.Periodicity;
                            otherIncome.CreateDate = DateTime.Now;
                            otherIncome.CreateUser = WebSecurity.CurrentUserId;
                            otherIncome.Status = otherIncome.Status;
                            context.OtherIncomes.Add(otherIncome);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            tx.Rollback();
                            return new JsonResult()
                            {
                                Data = new
                                {
                                    result = false,
                                    message = e.Message
                                }
                            };
                        }
                        tx.Commit();
                    }
                }
            }
            else
            {
                try
                {
                    using (var context = new TicketsEntities())
                    {
                        var motherIncome = context.OtherIncomes.FirstOrDefault(c => c.Id == otherIncome.Id);
                        motherIncome.NoCatalogAccount = otherIncome.NoCatalogAccount;
                        motherIncome.AccountName = otherIncome.AccountName;
                        motherIncome.Origin = otherIncome.Origin;
                        motherIncome.AccountType = otherIncome.AccountType;
                        motherIncome.Description = otherIncome.Description;
                        motherIncome.Periodicity = otherIncome.Periodicity;
                        motherIncome.Status = otherIncome.Status;
                        context.Entry(motherIncome).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    return new JsonResult()
                    {
                        Data = new
                        {
                            result = false,
                            message = "Error al crear la cuenta de ingresos"
                        }
                    };
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, otherIncome.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Otros ingresos", OtherIncomeToObject(otherIncome));
            return new JsonResult() { Data = true };
        }

        // GET: OtherIncomes/GetOtherIncomeData
        [HttpGet]
        [Authorize]
        public JsonResult GetOtherIncomeData(int otherincomeId)
        {
            var context = new TicketsEntities();
            object otherIncome = null;
            if (otherincomeId > 0)
            {
                otherIncome = context.OtherIncomes.AsEnumerable().Where(a => a.Id == otherincomeId).Select(e => OtherIncomeToObject(e)).FirstOrDefault();
            }

            var origins = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.OriginAccount).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            var otherIncomeTypes = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.AccountType).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            var periodicities = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.PaymentPeriodicity).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            var status = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.GeneralStatus).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            return new JsonResult() { Data = new { otherIncome, origins, otherIncomeTypes, periodicities, status }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetOtherIncomeList()
        {
            var context = new TicketsEntities();
            var otherIncome = context.OtherIncomes.AsEnumerable().Where(a => a.Status == (int)GeneralStatusEnum.Active).Select(e => OtherIncomeToObject(e)).ToList();

            var bankAccount = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.BankAccountCatalog).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            return new JsonResult() { Data = new { otherIncome, bankAccount }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object OtherIncomeToObject(OtherIncome c)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs.Select(s => new { s.Id, s.NameDetail }).ToList();
            return new
            {
                c.Id,
                c.NoCatalogAccount,
                c.AccountName,
                c.Origin,
                OriginDesc = catalog.FirstOrDefault(s => s.Id == c.Origin).NameDetail,
                c.AccountType,
                OtherIncomeTypeDesc = catalog.FirstOrDefault(s => s.Id == c.AccountType).NameDetail,
                c.Description,
                c.Periodicity,
                PeriodicityDesc = catalog.FirstOrDefault(s => s.Id == c.Periodicity).NameDetail,
                c.CreateDate,
                c.CreateUser,
                c.Status,
                StatusDesc = catalog.FirstOrDefault(s => s.Id == c.Status).NameDetail,
            };
        }

        private object OtherIncomePaymentToObject(OtherIncomeDetail c)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs.Select(s => new { s.Id, s.NameDetail }).ToList();
            var OtherIncomeInfo = context.OtherIncomeDetails.FirstOrDefault(f => f.Id == c.Id);
            return new
            {
                OtherIncomeInfo.Id,
                OtherIncomeInfo.Total,
                OtherIncomeInfo.Description,
                CreateDate = OtherIncomeInfo.CreateDate.Value,
                OtherIncomeInfo.CreateUser,
                PaymentDate = OtherIncomeInfo.PaymentDate.Value,
                OtherIncomeInfo.OtherIncomeId,
                OtherIncomeInfo.SequenceNumber,
                OtherIncomeInfo.OtherIncome.NoCatalogAccount,
                OtherIncomeInfo.OtherIncome.AccountName,
                OtherIncomeInfo.OtherIncome.Origin,
                OriginDesc = catalog.FirstOrDefault(s => s.Id == OtherIncomeInfo.OtherIncome.Origin).NameDetail,
                OtherIncomeInfo.OtherIncome.AccountType,
                OtherIncomeTypeDesc = catalog.FirstOrDefault(s => s.Id == OtherIncomeInfo.OtherIncome.AccountType).NameDetail,
                OtherIncomeInfo.OtherIncome.Periodicity,
                PeriodicityDesc = catalog.FirstOrDefault(s => s.Id == OtherIncomeInfo.OtherIncome.Periodicity).NameDetail,
                OtherIncomeInfo.OtherIncome.Status,
                StatusDesc = catalog.FirstOrDefault(s => s.Id == OtherIncomeInfo.OtherIncome.Status).NameDetail,
                OtherIncomeDescription = OtherIncomeInfo.OtherIncome.Description,
                OtherIncomeCreateDate = OtherIncomeInfo.OtherIncome.CreateDate,
                OtherIncomeCreateUser = OtherIncomeInfo.OtherIncome.CreateUser,
            };
        }
    }
}
