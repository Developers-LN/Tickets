using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ClientController : Controller
    {
        //
        // GET: /Client/List
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        //
        // GET: /Client/CreditReports
        [Authorize]
        [HttpGet]
        public ActionResult CreditReports()
        {
            return View();
        }
        //
        // GET: /Client/ClientGeneralReport
        [Authorize]
        [HttpGet]
        public ActionResult ClientGeneralReport()
        {
            return View();
        }


        //
        // GET: /Client/ListPrint
        [Authorize]
        [HttpGet]
        public ActionResult ListPrint()
        {
            return View();
        }

        // GET: /Client/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var clients = context.Clients.AsEnumerable().Where( c=>c.Statu != (int)GeneralStatusEnum.Suspended).Select(c => ClientToObject(c)).ToList();

            var raffles = context.Raffles.Select(r => new
            {
                r.Id,
                r.Name
            }).ToList();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Clientes");

            return new JsonResult() { Data = new { clients, raffles }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /Client/GetClientData
        [HttpGet]
        [Authorize]
        public JsonResult GetClientData(int clientId)
        {
            var context = new TicketsEntities();
            object client = null;
            if (clientId > 0)
            {
                client = context.Clients.AsEnumerable().Where(a => a.Id == clientId).Select(e => ClientToObject(e)).FirstOrDefault();
            }
            var provinces = context.Provinces.Select(p => new
            {
                p.Id,
                p.Name,
                Towns = p.Towns.Select(t => new
                {
                    t.Id,
                    t.Name,
                    DistTowns = t.DistTowns.Select(dt => new
                    {
                        dt.Id,
                        dt.Name
                    })
                })
            });
            var matritalStates = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.MaritalStatus).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            var prices = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.ProspectPrice).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            var genders = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.Gender).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            var clientTypes = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.ClientType).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            var invoiceStatus = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.CheckIn).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            var clientGroups = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.ClientGroup).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            return new JsonResult() { Data = new { clientGroups, invoiceStatus, client, provinces, matritalStates, prices, genders, clientTypes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        // GET: /Client/GetClientList
        [HttpGet]
        [Authorize]
        public JsonResult GetClientList()
        {
            var context = new TicketsEntities();

            var clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
            {
                r.Id,
                r.Name,
                r.PriceId
            }).ToList();

            return new JsonResult() { Data = new { clients }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object ClientToObject(Client c)
        {
            var context = new TicketsEntities();
            var catalogs = context.Catalogs.ToList();
            return new
            {
                c.Id,
                c.Addres,
                Birthday = c.Birthday.ToUnixTime(),
                c.AmountDeposit,
                c.AdminDocument,
                c.DepositDocument,
                c.PreviousDebt,
                c.ClientType,
                ClientTypeDesc = catalogs.FirstOrDefault( ct=>ct.Id == c.ClientType).NameDetail,
                c.Comment,
                c.CreateDate,
                c.CreateUser,
                c.CreditLimit,
                c.PriceId,
                PriceDesc = catalogs.FirstOrDefault(ct => ct.Id == c.PriceId).NameDetail,
                c.DocumentNumber,
                c.Email,
                c.Discount,
                c.Fax,
                c.Gender,
                GenderDesc = catalogs.FirstOrDefault(ct => ct.Id == c.Gender).NameDetail,
                c.GroupId,
                GroupDesc = catalogs.FirstOrDefault(ct => ct.Id == c.GroupId).NameDetail,
                c.MaritalStatus,
                MaritalStatusDesc = catalogs.FirstOrDefault(ct => ct.Id == c.MaritalStatus).NameDetail,
                c.Name,
                c.Phone,
                c.Province,
                ProvinceDesc = context.Provinces.FirstOrDefault(p => p.Id == c.Province).Name,
                c.RNC,
                c.Section,
                SectionDesc = context.DistTowns.FirstOrDefault( dt=>dt.Id == c.Section).Name,
                c.Statu,
                StatuDesc = catalogs.FirstOrDefault( ct=>ct.Id == c.Statu).NameDetail,
                c.Town,
                TownDesc = context.Towns.FirstOrDefault(t=>t.Id == c.Town).Name,
                c.Tradename
            };
        }

        //
        // GET: /Client/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [Authorize]
        public JsonResult Create(Client client)
        {
            
            
            if (client.Id <= 0)
            {
                try
                {
                    using (var context = new TicketsEntities())
                    {
                        client.Statu = (int)ClientStatuEnum.Created;
                        client.Fax = string.IsNullOrEmpty(client.Fax) ? "" : client.Fax;
                        client.RNC = string.IsNullOrEmpty(client.RNC) ? "" : client.RNC;
                        client.Tradename = string.IsNullOrEmpty(client.Tradename) ? "" : client.Tradename;
                        client.Comment = string.IsNullOrEmpty(client.Comment) ? "NA" : client.Comment;
                        client.CreateDate = DateTime.Now;
                        client.CreateUser = WebSecurity.CurrentUserId;
                        context.Clients.Add(client);
                        EmailUtil.SendClientEmail(ReportByEmailEnum.NEW_CLIENT, client);
                    }
                }
                catch (Exception)
                {

                    Utils.SaveLog(WebSecurity.CurrentUserName, client.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Cliente", ClientToObject(client));
                }
            
            }
            else
            {
                try
                {
                    using (var context = new TicketsEntities())
                    {
                        var mClient = context.Clients.FirstOrDefault(c => c.Id == client.Id);
                        mClient.Statu = (int)ClientStatuEnum.Created;
                        mClient.Addres = client.Addres;
                        mClient.Birthday = client.Birthday;
                        mClient.AmountDeposit = client.AmountDeposit;
                        mClient.AdminDocument = string.IsNullOrEmpty(client.AdminDocument) ? "" : client.AdminDocument;
                        mClient.DepositDocument = string.IsNullOrEmpty(client.DepositDocument) ? "" : client.DepositDocument;
                        mClient.ClientType = client.ClientType;
                        mClient.Comment = string.IsNullOrEmpty(client.Comment) ? "NA" : client.Comment;
                        mClient.CreditLimit = client.CreditLimit;
                        mClient.DocumentNumber = client.DocumentNumber;
                        mClient.Email = client.Email;
                        mClient.PriceId = client.PriceId;
                        mClient.Fax = string.IsNullOrEmpty(client.Fax) ? "" : client.Fax;
                        mClient.Gender = client.Gender;
                        mClient.GroupId = client.GroupId;
                        mClient.MaritalStatus = client.MaritalStatus;
                        mClient.Name = client.Name;
                        mClient.Phone = client.Phone;
                        mClient.Province = client.Province;
                        mClient.RNC = string.IsNullOrEmpty(client.RNC) ? "" : client.RNC;
                        mClient.Section = client.Section;
                       // mClient.Statu = client.Statu;
                        mClient.Town = client.Town;
                        mClient.Discount = client.Discount;
                        mClient.Tradename = string.IsNullOrEmpty(client.Tradename) ? "" : client.Tradename;
                        mClient.PreviousDebt = client.PreviousDebt;
                        context.Entry(mClient).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        EmailUtil.SendClientEmail(ReportByEmailEnum.MODIFIED_CLIENT, client);
                    }
                    
                }
                catch (Exception ex)
                { }
                Utils.SaveLog(WebSecurity.CurrentUserName, client.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Cliente", ClientToObject(client));
                
            }
            return new JsonResult() { Data = true };
        }

        // POST: Client/Delete
        [HttpPost]
        [Authorize]
        public JsonResult Delete(int clientId)
        {
            var context = new TicketsEntities();
            var client = context.Clients.FirstOrDefault(m => m.Id == clientId);
            if (client != null)
            {
                client.Statu = (int)GeneralStatusEnum.Delete;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Cliente", ClientToObject(client));
            }
            return new JsonResult() { Data = true };
        }

        #region Client Workflow

        // GET: Cliente/SendClientProccess
        [HttpPost]
        [Authorize]
        public JsonResult SendClientProccess(int clientId)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var currentClient = context.Clients.FirstOrDefault(p => p.Id == clientId);
                        currentClient.Statu = (int)ClientStatuEnum.InProcess;
                        context.SaveChanges();

                        int workFlowTypeId = int.Parse(ConfigurationManager.AppSettings["ClientApprovedWorkflowTypeId"].ToString());
                        var workflow = new Workflow()
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            ProcessId = clientId,
                            WorkflowTypeId = workFlowTypeId,
                            Statu = (int)WorkflowStatusEnum.Active
                        };
                        context.Workflows.Add(workflow);
                        context.SaveChanges();
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Submitted, "Cliente a aprovacción", ClientToObject(currentClient));
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return new JsonResult() { Data = new { result = false, message = ex.Message } };
                    }
                }
            }
            return new JsonResult() { Data = new { result = true, message = "" } };
        }

        // GET: Client/WorkflowList
        [HttpGet]
        [Authorize]
        public ActionResult WorkflowList()
        {
            return View();
        }
        
        // GET: Client/GetWorkflowList
        [HttpGet]
        [Authorize]
        public JsonResult GetWorkflowList()
        {
            var context = new TicketsEntities();
            int userId = WebSecurity.CurrentUserId;
            int workFlowTypeId = int.Parse(ConfigurationManager.AppSettings["ClientApprovedWorkflowTypeId"].ToString());
            var usersApproveWorkflow = context.WorkflowTypes.FirstOrDefault(w => w.Id == workFlowTypeId).WorkflowType_User.Where( wu=>wu.Statu != (int)GeneralStatusEnum.Delete);

            var workflowList = context.Workflows.AsEnumerable().Where(w =>
                w.WorkflowTypeId == workFlowTypeId &&
                w.Statu == (int)WorkflowStatusEnum.Active &&
                usersApproveWorkflow.Any(u => u.UserId == userId
                    && u.OrderApproval ==
                    ((w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Approved).Count()
                    - w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Rejected).Count()) + 1))
                ).Select(w => new
                {
                    w.Id,
                    Client = ClientToObject(context.Clients.FirstOrDefault(p => p.Id == w.ProcessId)),
                    UserName = w.User.Name,
                    CreateDate = w.CreateDate.ToString(),
                    proccess= w.WorkflowProccesses.Select( p=> new{
                        UserName = p.User.Name,
                        p.Comment,
                        p.Statu,
                        CreateDate = p.CreateDate.ToUnixTime()
                    }),
                    ApprovedCount = w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Approved).Count()
                    - w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Rejected).Count(),
                    TotalAproved = usersApproveWorkflow.Count()
                }).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Proceso de Aprobación de Cliente");
            return new JsonResult() { Data = workflowList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Client/ApprovedClientProcess
        [HttpGet]
        public ActionResult ApprovedClientProcess()
        {
            return View();
        }

        // GET: Client/ApprovedClientProcess
        [HttpPost]
        [Authorize]
        public JsonResult ApprovedClientProcess(WorkflowProccess proccess)
        {
            object proccessObject;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        proccess.CreateDate = DateTime.Now;
                        proccess.CreateUser = WebSecurity.CurrentUserId;
                        context.WorkflowProccesses.Add(proccess);
                        context.SaveChanges();

                        var workFlow = context.Workflows.FirstOrDefault(w => w.Id == proccess.WorkFlowId);
                        int proccessApprovedCount = workFlow.WorkflowProccesses.Count(p => p.Statu == (int)WorkflowProccessStatuEnum.Approved);
                        int proccessRejectedCount = workFlow.WorkflowProccesses.Count(p => p.Statu == (int)WorkflowProccessStatuEnum.Rejected);
                        int totalProccess = workFlow.WorkflowType.WorkflowType_User.AsEnumerable().Where(w => w.Statu != (int)GeneralStatusEnum.Delete).Count();
                        if ((proccessApprovedCount - proccessRejectedCount) >= totalProccess)
                        {
                            workFlow.Statu = (int)WorkflowStatusEnum.Approved;
                            context.SaveChanges();
                            var client = context.Clients.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                            client.Statu = (int)ClientStatuEnum.Approbed;
                            context.SaveChanges();
                        }
                        else if ((proccessApprovedCount - proccessRejectedCount) < 0)
                        {
                            workFlow.Statu = (int)WorkflowStatusEnum.Rejected;
                            context.SaveChanges();
                            var client = context.Clients.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                            client.Statu = (int)ClientStatuEnum.Created;
                            context.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                        proccessObject = GetProccessObject(proccess);
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return new JsonResult() { Data = new { result = false, message = ex.Message } };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Aprobación de flujo de cliente", proccessObject);
            return new JsonResult() { Data = new { result = true, message = "" } };
        }

        private object GetProccessObject(WorkflowProccess proccess)
        {
            return new
            {
                proccess.WorkFlowId,
                proccess.Statu,
                proccess.Id,
                proccess.Comment,
                proccess.CreateUser,
                proccess.CreateDate
            };
        }
        #endregion


        // POST: Client/PreviousDebtPaymentSave
        [HttpPost]
        [Authorize]
        public JsonResult PreviousDebtPaymentSave(PreviousDebtPayment model)
        {
            object payment;
            using(var context = new TicketsEntities()){
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        model.CreateUser = WebSecurity.CurrentUserId;
                        model.CreateDate = DateTime.Now;
                        context.PreviousDebtPayments.Add(model);
                        context.SaveChanges();
                        payment = new
                        {
                            model.Id,
                            model.ClientId,
                            model.CreateDate,
                            model.CreateUser,
                            model.Note,
                            model.PaimentValue
                        };
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { 
                            result = false,
                            message = e.Message
                        } };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Pago Deuda Antigua", payment);
            return new JsonResult()
            {
                Data = new
                {
                    result = true,
                    message = "Pago realizado correctamente.",
                    payment
                }
            };
        }
	}
}