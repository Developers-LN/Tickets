using System;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ConfigController : Controller
    {

        #region Catalog
        //
        // GET: /Config/CatalogList
        [Authorize]
        [HttpGet]
        public ActionResult CatalogList()
        {
            return View();
        }

        public ActionResult ConfigEmail()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetReportMessage()
        {
            using (var context = new TicketsEntities())
            {
                var EmployeesEmails = context.Employees.Select(e => new
                {
                    id = e.Id,
                    text = e.Name + " " + e.LastName + " - " + e.Email,
                    email = e.Email

                }).ToList();
                var reportByEmail = context.ReportByEmails.Select(m => m).ToList();
                return new JsonResult()
                {
                    Data = new
                    {
                        EmployeesEmails = EmployeesEmails,
                        ReportByEmail = reportByEmail
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

        }


        [HttpPost]
        [Authorize]
        public JsonResult SaveReportMessage(ReportByEmail report)
        {
            using (var context = new TicketsEntities())
            {
                context.Entry(report).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return new JsonResult();
            }

        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveProductionCost(ProductionCost[] productionCost)
        {
            using (var context = new TicketsEntities())
            {
                foreach (var item in productionCost)
                {
                    if (item.Id == 0)
                    {
                        item.Created = DateTime.Now;
                        item.Status = true;
                        context.ProductionCosts.Add(item);

                    }
                    else
                    {
                        var modified = context.ProductionCosts.Where(p => p.Id == item.Id).FirstOrDefault();
                        if (modified != null && item.Status == false)
                        {
                            modified.Status = false;
                            context.Entry(modified).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                }
                context.SaveChanges();
                return new JsonResult
                {
                    Data = new
                    {
                        Message = "Datos guardados correctamente!"
                    }
                };
            }
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetProductionCost(int raffleId)
        {
            var context = new TicketsEntities();

            var result = context.ProductionCosts.Where(p => p.RaffleId == raffleId && p.Status == true).Select(p => new
            {
                Id = p.Id,
                RaffleId = p.RaffleId,
                Detalle = p.Detalle,
                Cantidad = p.Cantidad,
                Monto = p.Monto,
                Status = p.Status,
                Created = p.Created

            }).ToList();
            return new JsonResult()
            {
                Data = result,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: /Config/GetCatalogList
        [HttpGet]
        [Authorize]
        public JsonResult GetCatalogList()
        {
            var response = new CatalogModel().GetCatalogList();
            return new JsonResult() { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Config/CatalogCreate
        [HttpGet]
        [Authorize]
        public ActionResult CatalogCreate()
        {
            return View();
        }

        // POST: Config/CatalogCreate
        [HttpPost]
        [Authorize]
        public JsonResult CatalogCreate(Catalog catalog)
        {
            var response = new CatalogModel().CatalogCreate(catalog);
            return new JsonResult() { Data = true };
        }

        public ActionResult CostosProduccion()
        {
            return View();
        }

        // POST: Config/CatalogDelete
        [HttpPost]
        [Authorize]
        public JsonResult CatalogDelete(int catalogId)
        {

            var response = new CatalogModel().CatalogDelete(catalogId);
            return new JsonResult() { Data = true };
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetSorteoList()
        {
            using (var context = new TicketsEntities())
            {
                //var RaffleList = context.Raffles.Select(r => new
                //{
                //    Id = r.Id,
                //    RaffleName = r.Name
                //}).ToList();
                var RaffleList1 = context.Raffles.Select(r => new
                {
                    id = r.Id,
                    sequenceNumber = r.SequenceNumber,
                    raffleNomenclature = r.Symbol + r.Separator + r.SequenceNumber,
                    text = r.Symbol + r.Separator + r.SequenceNumber + " " + r.Name,
                    r.DateSolteo
                    //text = r.SequenceNumber + " - " + r.Name
                }).ToList();

                var RaffleList = RaffleList1.Select(s => new
                {
                    s.id,
                    s.sequenceNumber,
                    s.raffleNomenclature,
                    text = s.text + " " + s.DateSolteo.ToShortDateString()
                }).ToList();

                return new JsonResult
                {
                    Data = RaffleList,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        private object GetCatalogObject(Catalog catalog)
        {
            var response = new CatalogModel().GetCatalogObject(catalog);
            return response;
        }
        #endregion

        #region WorkflowType
        //
        // GET: /Config/WorkflowTypeList
        [Authorize]
        [HttpGet]
        public ActionResult WorkflowTypeList()
        {
            return View();
        }

        // GET: /Config/GetWorkflowTypeList
        [HttpGet]
        [Authorize]
        public JsonResult GetWorkflowTypeList()
        {
            var response = new WorkflowTypeModel().GetWorkflowTypeList();
            return new JsonResult() { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Config/WorkflowTypeCreate
        [HttpGet]
        [Authorize]
        public ActionResult WorkflowTypeCreate()
        {
            return View();
        }

        // POST: Config/WorkflowTypeCreate
        [HttpPost]
        [Authorize]
        public JsonResult WorkflowTypeCreate(WorkflowType workflowType)
        {
            var response = new WorkflowTypeModel().WorkflowTypeCreate(workflowType);
            return new JsonResult() { Data = true };
        }

        // POST: Config/WorkflowTypeDelete
        [HttpPost]
        [Authorize]
        public JsonResult WorkflowTypeDelete(int workflowTypeId)
        {
            var response = new WorkflowTypeModel().WorkflowTypeDelete(workflowTypeId);
            return new JsonResult() { Data = true };
        }

        private object GetWorkflowTypeObject(WorkflowType workfowType)
        {
            var response = new WorkflowTypeModel().GetWorkflowTypeObject(workfowType);
            return response;
        }
        #endregion

        #region WorkflowTypeUser
        //
        // GET: /Config/WorkflowTypeUserList
        [Authorize]
        [HttpGet]
        public ActionResult WorkflowTypeUserList()
        {
            return View();
        }

        // GET: /Config/GetWorkflowTypeUserList
        [HttpGet]
        [Authorize]
        public JsonResult GetWorkflowTypeUserList(int workflowTypeId)
        {
            var response = new WorkflowTypeUserModel().GetWorkflowTypeUserList(workflowTypeId);
            return new JsonResult() { Data = response, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Config/WorkflowTypeUserCreate
        [HttpGet]
        [Authorize]
        public ActionResult WorkflowTypeUserCreate()
        {
            return View();
        }

        // POST: Config/WorkflowTypeUserCreate
        [HttpPost]
        [Authorize]
        public JsonResult WorkflowTypeUserCreate(WorkflowType_User workflowTypeUser)
        {
            var response = new WorkflowTypeUserModel().WorkflowTypeUserCreate(workflowTypeUser);
            return new JsonResult() { Data = true };
        }

        // POST: Config/WorkflowTypeUserDelete
        [HttpPost]
        [Authorize]
        public JsonResult WorkflowTypeUserDelete(int workflowTypeUserId)
        {
            var response = new WorkflowTypeUserModel().WorkflowTypeUserDelete(workflowTypeUserId);
            return new JsonResult() { Data = true };
        }

        private object GetWorkflowTypeUserObject(WorkflowType_User workfowTypeUser)
        {
            var response = new WorkflowTypeUserModel().GetWorkflowTypeUserObject(workfowTypeUser);
            return response;
        }
        #endregion


        #region System Config
        //
        //  GET: Config/SystemConfig
        [Authorize]
        [HttpGet]
        public ActionResult SystemConfig()
        {
            return View();
        }
        //
        // GET: Config/GetSystemConfigData
        [HttpGet]
        [Authorize]
        public JsonResult GetSystemConfigData()
        {
            var response = new SystemConfigModel().GetSystemConfigData();
            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //
        //  GET: Config/SystemConfig
        [HttpPost]
        [Authorize]
        public JsonResult SystemConfig(SystemConfig systemConfig)
        {
            var response = new SystemConfigModel().SystemConfig(systemConfig);
            return new JsonResult() { Data = response };
        }
        #endregion
    }
}