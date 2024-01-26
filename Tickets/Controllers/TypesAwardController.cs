using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TypesAwardController : Controller
    {
        // GET: TypesAward/List
        [HttpGet]
        [Authorize]
        public ActionResult List()
        {
            return View();
        }

        // GET: TypesAward/PrintList
        [HttpGet]
        [Authorize]
        public ActionResult PrintList()
        {
            return View();
        }

        // GET: /TypesAward/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var typesAwards = context.TypesAwards.AsEnumerable().Where(p => p.Status != (int)GeneralStatusEnum.Delete).Select(e => TypesAwardToObjec(e)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de tipo de premio");

            return new JsonResult() { Data = new { typesAwards }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        private object TypesAwardToObjec(TypesAward typesAward)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs;
            return new
            {
                typesAward.Id,
                typesAward.Name,
                typesAward.Description,
                typesAward.CreateUser,
                CreateDate = typesAward.CreateDate.ToString(),
                typesAward.Creation,
                CreationDesc = catalog.FirstOrDefault(c => c.Id == typesAward.Creation).NameDetail,
                typesAward.GroupId,
                GroupDesc = catalog.FirstOrDefault(c => c.Id == typesAward.GroupId).NameDetail,
                typesAward.Status
            };
        }
        // GET: TypesAward/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // GET: TypesAward/Create
        [HttpPost]
        [Authorize]
        public JsonResult Create(TypesAward typesAward)
        {
            var context = new TicketsEntities();
            if (typesAward.Id <= 0)
            {
                typesAward.CreateDate = DateTime.Now;
                typesAward.CreateUser = WebSecurity.CurrentUserId;
                typesAward.Status = (int)GeneralStatusEnum.Active;
                typesAward.Description = string.IsNullOrEmpty(typesAward.Description) ? "" : typesAward.Description;
                context.TypesAwards.Add(typesAward);
            }
            else
            {
                var mTypesAward = context.TypesAwards.FirstOrDefault(c => c.Id == typesAward.Id);

                mTypesAward.Name = typesAward.Name;
                mTypesAward.Description = typesAward.Description ?? "";
                mTypesAward.GroupId = typesAward.GroupId;
                mTypesAward.Creation = typesAward.Creation;
            }
            context.SaveChanges();
            Utils.SaveLog(WebSecurity.CurrentUserName, typesAward.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Tipo de premio", TypesAwardToObjec(typesAward));
            return new JsonResult() { Data = true };
        }

        // POST: TypesAward/Delete
        [HttpPost]
        [Authorize]
        public JsonResult Delete(int typesAwardId)
        {
            var context = new TicketsEntities();
            var typesAward = context.TypesAwards.FirstOrDefault(m => m.Id == typesAwardId);
            if (typesAward != null)
            {
                typesAward.Status = (int)GeneralStatusEnum.Delete;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Tipo de premio", TypesAwardToObjec(typesAward));
            }
            return new JsonResult() { Data = true };
        }


        // GET: TypesAward/AwardNumber
        [HttpGet]
        [Authorize]
        public ActionResult AwardNumber()
        {
            return View();
        }

        // GET: TypesAward/GetAwardNumberData
        [HttpGet]
        [Authorize]
        public JsonResult GetAwardNumberData()
        {
            var context = new TicketsEntities();
            var raffles1 = context.Raffles.Where(r => r.Statu != (int)RaffleStatusEnum.Suspended).Select(r => new
            {
                r.Id,
                r.SequenceNumber,
                r.Name,
                raffleNomenclature = r.Symbol + r.Separator + r.SequenceNumber,
                text = r.Symbol + r.Separator + r.SequenceNumber + " " + r.Name,
                r.DateSolteo
            }).OrderByDescending(r => r.Id).ToList();

            var raffles2 = raffles1.Select(s => new
            {
                s.Id,
                s.SequenceNumber,
                s.Name,
                s.raffleNomenclature,
                text = s.text + " " + s.DateSolteo.ToShortDateString()
            }).ToList();

            var raffles = raffles2.Select(s => new
            {
                s.Id,
                s.SequenceNumber,
                s.Name,
                s.raffleNomenclature,
                s.text
            }).OrderByDescending(o => o.Id);

            return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { raffles } };
        }

        // GET: TypesAward/AwardNumber
        [HttpPost]
        [Authorize]
        public JsonResult AwardNumber(AwardNumberModel awardNumber)
        {
            var context = new TicketsEntities();
            var awardNumbers = new List<object>();
            var noAwardNumbers = new List<object>();

            var number = context.TicketAllocationNumbers.Where(n => n.Number == awardNumber.Number && n.TicketAllocation.RaffleId == awardNumber.RaffleId).Select(n => new
            {
                n.Id,
                n.Number,
                n.ControlNumber,
                n.Statu,
                ClientDesc = n.TicketAllocation.Client.Name,
            }).FirstOrDefault();
            if (number == null)
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        result = false,
                        message = "El numero " + awardNumber.Number + " no fue asignado en el sorteo No." + awardNumber.RaffleId
                    }
                };
            }
            else
            {
                return new JsonResult()
                {
                    Data = new
                    {
                        result = true,
                        numberId = number.Id
                    }
                };
            }
        }

        // GET: TypesAward/ReturnedTraceability
        [HttpGet]
        [Authorize]
        public ActionResult ReturnedTraceability()
        {
            return View();
        }

        //
        //GET: TypesAward/IdentifyAwardsReports
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardsReports()
        {
            return View();
        }

        //
        //GET: TypesAward/IdentifyAwardsReportsResumen
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardsReportsResumen()
        {
            return View();
        }

        //
        //GET: TypesAward/AwardsCertifications
        [Authorize]
        [HttpGet]
        public ActionResult AwardsCertifications()
        {
            return View();
        }

        //
        //GET: TypesAward/AwardsCertifications
        [Authorize]
        [HttpGet]
        public ActionResult AwardsCertWorkflowList()
        {
            return View();
        }

        //
        //GET: TypesAward/AwardsCertifications
        [Authorize]
        [HttpGet]
        public ActionResult ApprovedAwardsCertProcess()
        {
            return View();
        }

        //
        //  GET: /TypesAward/GetAwardsList
        [Authorize]
        [HttpGet]
        public JsonResult GetAwardsList(int raffleId)
        {
            var response = new AwardTicketModel().GetAwardsList(raffleId);

            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

    }
}
