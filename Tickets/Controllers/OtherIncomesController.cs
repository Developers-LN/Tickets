using System;
using System.Linq;
using System.Web.Http.Results;
using System.Web.Mvc;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    public class OtherIncomesController : Controller
    {
        // GET: OtherIncomes
        public ActionResult OtherIncomesList()
        {
            return View();
        }

        [HttpGet]
        public ActionResult OtherIncomesPaymentHistory()
        {
            return View();
        }

        public ActionResult OtherIncomePaymentByGroup()
        {
            return View();
        }

        public ActionResult CreateGroupPayment()
        {
            return View();
        }

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
                        SequenceNumber = String.Concat(s.Symbol, s.SequenceNumber.ToString().PadLeft(s.LengthZero.Value, '0'))
                    })
                }
            };
        }

        [HttpGet]
        public JsonResult GetPaymentGroupHistory(int otherIncomeGroupId)
        {
            var context = new TicketsEntities();

            var otherIncomeGroupData = context.OtherIncomesGroups.FirstOrDefault(f => f.Id == otherIncomeGroupId);
            var catalog = context.Catalogs.Select(s => new { s.Id, s.NameDetail }).ToList();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    sequenceNumber = String.Concat(otherIncomeGroupData.Symbol, otherIncomeGroupData.SequenceNumber.ToString().PadLeft(otherIncomeGroupData.LengthZero.Value, '0')),
                    otherIncomeGroupData.Id,
                    otherIncomeGroupData.Description,
                    CreateDate = otherIncomeGroupData.CreateDate.Value.ToString(),
                    otherIncomeGroupData.Status,
                    statusDesc = catalog.FirstOrDefault(f => f.Id == otherIncomeGroupData.Status).NameDetail,
                    payments = otherIncomeGroupData.OtherIncomeDetails.Select(s => new
                    {
                        s.Id,
                        s.Total,
                        s.Description,
                        payAccount = String.Concat(s.OtherIncome.NoCatalogAccount, " - ", s.OtherIncome.AccountName),
                        PaymentDate = s.PaymentDate.Value.ToShortDateString(),
                        s.OtherIncomeId,
                        SequenceNumber = String.Concat(s.Symbol, s.SequenceNumber.ToString().PadLeft(s.LengthZero.Value, '0'))
                    })
                }
            };
        }

        [HttpPost]
        public JsonResult approveGroup(int id)
        {
            var context = new TicketsEntities();

            var otherIncomeGroupData = context.OtherIncomesGroups.FirstOrDefault(f => f.Id == id);

            if (otherIncomeGroupData == null)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        result = false,
                        message = "No se encontró el grupo de ingresos"
                    }
                };
            }

            otherIncomeGroupData.Status = (int)OtherIncomesStatusEnum.Approve;
            context.SaveChanges();

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    result = true,
                    message = "El grupo de ingresos fue aprobado"
                }
            };
        }

        public ActionResult OtherIncomesPaymentList()
        {
            return View();
        }

        // GET: /OtherIncomes/GetList
        [HttpGet]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var otherIncomes = context.OtherIncomes.AsEnumerable().Where(w => w.Status == (int)GeneralStatusEnum.Active).Select(s => OtherIncomeList(s)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de otros ingresos");

            return new JsonResult() { Data = new { otherIncomes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /OtherIncomes/GetList
        [HttpGet]
        public JsonResult GetOtherIncomeGroupsList()
        {
            var context = new TicketsEntities();
            var otherIncomesGroup = context.OtherIncomesGroups.AsEnumerable().Where(w => w.Status == (int)GeneralStatusEnum.Active).Select(s => OtherIncomeGroupList(s)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de otros ingresos");

            return new JsonResult() { Data = new { otherIncomesGroup }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public JsonResult GetOtherIncomePaymentList(int otherIncomeGroupId)
        {
            var context = new TicketsEntities();
            //var otherIncomesPayments = context.OtherIncomesGroups.Where(w => w.Id == otherIncomeGroupId && (w.OtherIncomeDetails.Any(a => a.OtherIncomeId == otherIncomeId) || otherIncomeId == 0)).AsEnumerable().Select(s => OtherIncomeDetailList(s)).ToList();
            var otherIncomesPayments = context.OtherIncomesGroups.Where(w => w.Id == otherIncomeGroupId || otherIncomeGroupId == 0).AsEnumerable().Select(s => new
            {
                s.Id,
                Accounts = string.Join(", ", s.OtherIncomeDetails.Select(ss => String.Concat(ss.OtherIncome.NoCatalogAccount, " - ", ss.OtherIncome.AccountName))),
                Total = s.OtherIncomeDetails.Sum(ss => ss.Total),
                s.Description,
                CreateDate = s.CreateDate.Value.ToShortDateString(),
                s.SequenceNumber
            }).OrderByDescending(o => o.SequenceNumber).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de pagos por otros ingresos");

            return new JsonResult() { Data = new { otherIncomesPayments }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object OtherIncomeGroupList(OtherIncomesGroup o)
        {
            return new
            {
                o.Id,
                o.SequenceNumber,
                o.Description,
                Text = String.Concat(o.Symbol, o.SequenceNumber.ToString().PadLeft(o.LengthZero.Value, '0')),
                Origin = o.Catalog.NameDetail
            };
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
        public ActionResult CreatePayment(OtherIncomesModel otherIncomesModel)
        {
            if (otherIncomesModel.Id <= 0)
            {
                using (var context = new TicketsEntities())
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var otherIncomeDetails = new OtherIncomeDetail()
                            {
                                Total = otherIncomesModel.TotalPayment,
                                Description = otherIncomesModel.OtherIncomeDetailDescription,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                PaymentDate = otherIncomesModel.PaymentDate,
                                OtherIncomeId = otherIncomesModel.OtherIncomeId,
                                BankAccountCatalogId = otherIncomesModel.BankAccountCatalogId,
                                OtherIncomeGroupId = otherIncomesModel.OtherIncomesGroupId
                            };

                            context.OtherIncomeDetails.Add(otherIncomeDetails);
                            context.SaveChanges();

                            otherIncomesModel.Id = otherIncomeDetails.Id;
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
                        var motherIncomeDetail = context.OtherIncomeDetails.FirstOrDefault(c => c.Id == otherIncomesModel.Id);

                        motherIncomeDetail.Total = otherIncomesModel.TotalPayment;
                        motherIncomeDetail.Description = otherIncomesModel.OtherIncomeDetailDescription;
                        motherIncomeDetail.PaymentDate = otherIncomesModel.PaymentDate;
                        motherIncomeDetail.OtherIncomeId = otherIncomesModel.OtherIncomeId;
                        motherIncomeDetail.BankAccountCatalogId = otherIncomesModel.BankAccountCatalogId;
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

            Utils.SaveLog(WebSecurity.CurrentUserName, otherIncomesModel.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Pago por otros ingresos", OtherIncomePaymentToObject(otherIncomesModel));
            return new JsonResult() { Data = new { result = true, paymentId = otherIncomesModel.Id, groupId = otherIncomesModel.OtherIncomesGroupId } };
        }

        // POST: OtherIncomes/Create
        [HttpPost]
        public ActionResult CreateGroup(OtherIncomesGroupModel otherIncomeGroup)
        {
            if (otherIncomeGroup.Id <= 0)
            {
                using (var context = new TicketsEntities())
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var otherIncomesGroup = new OtherIncomesGroup()
                            {
                                Description = otherIncomeGroup.Description,
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Symbol = null,
                                LengthZero = null,
                                Status = (int)OtherIncomesStatusEnum.Pending
                            };
                            context.OtherIncomesGroups.Add(otherIncomesGroup);
                            context.SaveChanges();
                            otherIncomeGroup.Id = otherIncomesGroup.Id;
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
                        var motherIncomeGroup = context.OtherIncomesGroups.FirstOrDefault(c => c.Id == otherIncomeGroup.Id);
                        motherIncomeGroup.Description = otherIncomeGroup.Description;
                        motherIncomeGroup.Status = otherIncomeGroup.Status;
                        context.Entry(motherIncomeGroup).State = System.Data.Entity.EntityState.Modified;
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

            Utils.SaveLog(WebSecurity.CurrentUserName, otherIncomeGroup.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Otros ingresos", OtherIncomeGroupToObject(otherIncomeGroup));
            return new JsonResult() { Data = new { Value = true, Results = otherIncomeGroup } };
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

        // GET: OtherIncomes/GetOtherGroupData
        [HttpGet]
        public JsonResult GetOtherGroupData(int otherincomeGroupId)
        {
            var context = new TicketsEntities();
            object otherIncomeGroup = null;
            if (otherincomeGroupId > 0)
            {
                otherIncomeGroup = context.OtherIncomesGroups.AsEnumerable().Where(a => a.Id == otherincomeGroupId).Select(e => new
                {
                    e.Id,
                    e.Description,
                    e.Status
                }).FirstOrDefault();
            }

            var status = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.GeneralStatus).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            return new JsonResult() { Data = new { otherIncomeGroup, status }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: OtherIncomes/GetOtherIncomeData
        [HttpGet]
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

        // GET: OtherIncomes/GetOtherIncomeData
        [HttpGet]
        public JsonResult GetOtherIncomeDetailData(int otherincomeGroupId)
        {
            var context = new TicketsEntities();
            object otherIncomeGroup = null;
            if (otherincomeGroupId > 0)
            {
                otherIncomeGroup = context.OtherIncomesGroups.AsEnumerable().Where(a => a.Id == otherincomeGroupId).Select(e => e.Id).FirstOrDefault();
            }
            else
            {
                otherIncomeGroup = otherincomeGroupId;
            }

            var otherIncome = context.OtherIncomes.AsEnumerable().Where(w => w.Status == (int)GeneralStatusEnum.Active).Select(s => new
            {
                s.Id,
                s.NoCatalogAccount,
                s.AccountName
            }).ToList();

            var bankAccount = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.BankAccountCatalog).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            return new JsonResult() { Data = new { otherIncomeGroup, otherIncome, bankAccount }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object OtherIncomeGroupToObject(OtherIncomesGroupModel c)
        {
            var context = new TicketsEntities();
            var otherIncomeGroup = context.OtherIncomesGroups.FirstOrDefault(f => f.Id == c.Id);
            return new
            {
                otherIncomeGroup.Id,
                otherIncomeGroup.Description,
                otherIncomeGroup.CreateDate,
                otherIncomeGroup.CreateUser,
                otherIncomeGroup.Status,
                StatusDesc = otherIncomeGroup.Catalog.NameDetail,
            };
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

        private object OtherIncomePaymentToObject(OtherIncomesModel c)
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
                OtherIncomeGroupId = OtherIncomeInfo.OtherIncomesGroup.Id,
                OtherIncomeGroupDescription = OtherIncomeInfo.OtherIncomesGroup.Description,
                OtherIncomeGroupStatusId = OtherIncomeInfo.OtherIncomesGroup.Status,
                OtherIncomeGroupStatusDesc = OtherIncomeInfo.OtherIncomesGroup.Catalog.NameGroup,
                BankAccountId = OtherIncomeInfo.BankAccountCatalogId,
                BankAccountDesc = OtherIncomeInfo.Catalog.NameGroup
            };
        }
    }
}
