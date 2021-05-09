using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Raffles;
using Tickets.Models.Ticket;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class RaffleController : Controller
    {
        #region Open Returned
        //
        // GET: /Raffle/OpenReturned
        [Authorize]
        [HttpGet]
        public ActionResult OpenReturned()
        {
            return View();
        }

        // GET: /Raffle/GetOpenReturned
        [HttpGet]
        [Authorize]
        public JsonResult GetOpenReturned()
        {
            var context = new TicketsEntities();

            var config = context.SystemConfigs.FirstOrDefault();
            int xpiredMount = 6;
            if (config != null)
            {
                var xpired = context.Catalogs.FirstOrDefault(c => c.Id == config.RaffleXpiredTime);
                if (xpired != null)
                {
                    xpiredMount = int.Parse(xpired.Description);
                }
            }
            var date = DateTime.Now.AddMonths(-xpiredMount);
            var raffles = context.Raffles.AsEnumerable().Where(r =>
                ((r.Statu != (int)RaffleStatusEnum.Suspended
                && r.EndReturnDate < DateTime.Now)
                || r.Statu == (int)RaffleStatusEnum.Generated)
                && date <= r.DateSolteo
                ).Select(r => new
                {
                    r.Name,
                    r.Id,
                    DateSolteo = r.DateSolteo.ToUnixTime()
                }).OrderByDescending(r => r.Id).ToList();
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { raffles }
            };
        }

        //
        // GET: /Raffle/OpenReturned
        [Authorize]
        [HttpPost]
        public JsonResult OpenReturned(OpenReturnedModel open)
        {
            object obj = null;
            using (var context = new TicketsEntities())
            {
                using (var tm = context.Database.BeginTransaction())
                {
                    try
                    {
                        var openReturned = new ReturnedOpen()
                        {
                            CreateDate = DateTime.Now,
                            EndReturnedDate = open.EndReturnedDate,
                            CreateUser = WebSecurity.CurrentUserId,
                            Note = open.Note,
                            RaffleId = open.RaffleId
                        };
                        context.ReturnedOpens.Add(openReturned);
                        context.SaveChanges();

                        var raffle = context.Raffles.FirstOrDefault(r => r.Id == open.RaffleId);
                        var endDate = raffle.EndReturnDate;
                        raffle.EndReturnDate = open.EndReturnedDate;
                        context.SaveChanges();
                        tm.Commit();
                        obj = new
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            Note = open.Note,
                            RaffleId = open.RaffleId,
                            NewReturnedDate = open.EndReturnedDate,
                            ReturnedDate = endDate
                        };
                    }
                    catch (Exception e)
                    {
                        tm.Rollback();
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

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Abriendo devolución", obj);
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new
                {
                    result = true,
                    message = "Devolucion abierta correctamente"
                }
            };
        }
        #endregion

        #region Print Report Viewes
        //
        // GET: /RafflePrintGeneratedList
        [Authorize]
        [HttpGet]
        public ActionResult PrintGeneratedList()
        {
            return View();
        }

        //
        // GET: /PrintReportList
        [Authorize]
        [HttpGet]
        public ActionResult PrintReportList()
        {
            return View();
        }

        //
        // GET: /InspectorReports
        [Authorize]
        [HttpGet]
        public ActionResult InspectorReports()
        {
            return View();
        }

        //
        // GET: /CreditReportsByRaffle
        [Authorize]
        [HttpGet]
        public ActionResult CreditReportsByRaffle()
        {
            return View();
        }

        //
        // GET: /RafflePrintGeneratedTypes
        [Authorize]
        [HttpGet]
        public ActionResult PrintGeneratedTypes()
        {
            return View();
        }

        //
        // GET: /Raffle/DashboardRaffleReport
        [Authorize]
        [HttpGet]
        public ActionResult DashboardRaffleReport()
        {
            return View();
        }
        #endregion

        #region Raffle
        //
        // GET: /Raffle/List
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        //
        // GET: /Raffle/ActiveRaffle
        [Authorize]
        [HttpGet]
        public ActionResult ActiveRaffle()
        {
            return View();
        }

        //
        // GET: /Raffle/GeneratedList
        [Authorize]
        [HttpGet]
        public ActionResult GeneratedList()
        {
            return View();
        }

        //
        // GET: /Raffle/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //
        //GET: Raffle/RaffleAwardNumber
        [HttpGet]
        [Authorize]
        public ActionResult RaffleAwardNumber()
        {
            return View();
        }

        // GET: /Raffle/GetRaffleList
        [HttpGet]
        [Authorize]
        public JsonResult GetRaffleList(string status)
        {
            var response = new RaffleModel().GetRaffleList(status);
            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //GET: Raffle/RaffleAwardDetails
        [HttpGet]
        [Authorize]
        public JsonResult RaffleAwardDetails()
        {
            var response = new RaffleModel().RaffleAwardDetails(TypesAwardCreationEnum.Digited, RaffleAwardTypeEnum.Digite);
            return new JsonResult()
            {
                Data = response
            };
        }

        #endregion

        #region Virtual Raffle
        //
        //GET: Raffle/VirtualRaffle
        [HttpGet]
        [Authorize]
        public ActionResult VirtualRaffle()
        {
            return View();
        }

        //POST: Raffle/RaffleGenerate
        [HttpPost]
        [Authorize]
        public JsonResult RaffleGenerate(List<RaffleAwardModel> raffleAwards)
        {
            var response = new VirtualRaffleModel().RaffleGenerate(raffleAwards);
            if (response.Message.Contains("correctamente"))
            {
                using (var context = new TicketsEntities())
                {
                    var raffle = context.Raffles.OrderByDescending(r => r.Id).FirstOrDefault();
                    if (raffle != null)
                    {
                        EmailUtil.SendReportsEmail(ReportByEmailEnum.SEND_REPORTS_EMAIL, raffle.Id, Request);
                    }
                }
            }
            return new JsonResult()
            {
                Data = response
            };
        }

        //GET: Raffle/VirtualRaffle
        [HttpGet]
        [Authorize]
        public JsonResult VirtualRaffleDetails(int raffleId)
        {
            var response = new VirtualRaffleModel().VirtualRaffleDetails(raffleId);
            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult ProfitabilityRaffle()
        {
            return View();
        }
        #endregion
    }
}