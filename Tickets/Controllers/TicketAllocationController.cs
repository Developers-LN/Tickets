using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TicketAllocationController : Controller
    {
        #region Ticket Allocation 
        //
        // GET: /TicketAllocation/TicketAllocationList
        [Authorize]
        [HttpGet]
        public ActionResult TicketAllocationList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult TicketAllocationConsignmentList()
        {
            return View();
        }

        //
        // GET: /TicketAllocation/TicketAllocationCreate
        [HttpGet]
        [Authorize]
        public ActionResult TicketAllocationCreate()
        {
            return View();
        }

        //
        //  GET:    /TicketAllocation/NumberDetail
        [Authorize]
        [HttpGet]
        public ActionResult NumberDetail()
        {
            return View();
        }

        //
        // GET: /TicketAllocation/AllocationDetails
        [Authorize]
        [HttpGet]
        public ActionResult AllocationDetails()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult AllocationConsignedDetails()
        {
            return View();
        }

        //
        // GET: /TicketAllocation/AllocationSummaryReports
        [Authorize]
        [HttpGet]
        public ActionResult AllocationSummaryReports()
        {
            return View();
        }

        //
        //  GET: /TicketAllocation/TicketReassignt
        [Authorize]
        [HttpGet]
        public ActionResult TicketReassign()
        {
            return View();
        }

        //
        // GET: /TicketAllocation/CopyAllocation
        [Authorize]
        [HttpGet]
        public ActionResult CopyAllocation()
        {
            return View();
        }

        #endregion

        #region IdentifyAward
        //
        // GET: /Ticket/IdentifyAwardList
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardListToPay()
        {
            return View();
        }

        //
        // GET: /Ticket/IdentifyAwardDetail
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardDetail()
        {
            return View();
        }

        //
        // GET: /Ticket/IdentifyAwardToPayDetail
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAwardToPayDetail()
        {
            return View();
        }

        //
        //GET:  /Ticket/GetIdentifyList
        [Authorize]
        [HttpGet]
        public JsonResult GetIdentifyList(int raffleId = 0, int clientId = 0)
        {
            var respnse = new TicketIdentifyModel().GetIdentifyList(raffleId, clientId);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //GET:  /Ticket/GetIdentifyList
        [Authorize]
        [HttpGet]
        public JsonResult GetIdentifyListToPay(int raffleId = 0, int clientId = 0)
        {
            var respnse = new TicketIdentifyModel().GetIdentifyListToPay(raffleId, clientId);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetIdentifyListByBatch(int batchNumber)
        {
            using (var context = new TicketsEntities())
            {
                var identifyBachs = context.IdentifyBaches.Where(b => b.Id == batchNumber).Select(b => new
                {
                    b.Id,
                    b.RaffleId,
                    RaffleDesc = b.Raffle.Name,
                    ClientId = b.RaffleId,
                    ClientDesc = b.Client.Name,
                    hasPayment = (b.IdentifyBachPayments.Count > 0 || b.NoteCredits.Count > 0)
                }).ToList();
                //if (raffleId == 0 && clientId == 0)
                //{
                //    raffles = context.Raffles.Where(s => s.Statu == (int)RaffleStatusEnum.Generated).Select(r => new
                //    {
                //        r.Id,
                //        r.Name,
                //        Prices = r.Prospect.Prospect_Price.Select(p => new
                //        {
                //            p.PriceId,
                //            p.FactionPrice
                //        })
                //    }).ToList<object>();


                //    clients = context.Clients.Where(s => s.Statu == (int)ClientStatuEnum.Approbed).Select(r => new
                //    {
                //        r.Id,
                //        r.Name,
                //        r.PriceId
                //    }).ToList<object>();
                //}
                //else
                //{
                //    var awards = context.RaffleAwards.Where(r => r.RaffleId == raffleId).ToList();
                //    identifyBachs = context.IdentifyBaches.Where(a =>
                //        (a.RaffleId == raffleId || raffleId == 0)
                //        && (a.ClientId == clientId || clientId == 0))
                //        .AsEnumerable()
                //        .Select(t => this.IdentifyBachMainToObject(t, awards)).ToList();
                ////}
                //Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Lotes de Billetes");

                //return new { result = true, identifyBachs, raffles, clients }; ;
                return new JsonResult()
                {
                    Data = identifyBachs,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [Authorize]
        [HttpGet]
        public JsonResult GetBatchList()
        {
            using (var context = new TicketsEntities())
            {
                var data = context.IdentifyBaches.Select(b => b.Id).ToList();
                return new JsonResult
                {
                    Data = data,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };

            }
        }
        //
        //GET:  /Ticket/GetIdentifyDetilsData
        [Authorize]
        [HttpGet]
        public JsonResult GetIdentifyDetilsData(int identifyId)
        {
            var respnse = new TicketIdentifyModel().GetIdentifyDetilsData(identifyId);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //GET:  /Ticket/GetIdentifyData
        [Authorize]
        [HttpGet]
        public JsonResult GetIdentifyData(int identifyId)
        {
            var respnse = new TicketIdentifyModel().GetIdentifyData(identifyId);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //POST:  /Ticket/CertificationAwardData
        [Authorize]
        [HttpPost]
        public JsonResult CertificationAwardDataNew(int iNumberId, int number, int fractionFrom, int fractionTo, int raffleAwardId, int fractions)
        {
            var respnse = new TicketIdentifyModel().CertificationAwardData(iNumberId, number, fractionFrom, fractionTo, raffleAwardId, fractions);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [Authorize]
        [HttpPost]
        public JsonResult CertificationAwardData(int number, int raffleAwardId, int fractions)
        {
            var respnse = new TicketIdentifyModel().CertificationAwardData(number, raffleAwardId, fractions);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        //
        //GET:  /Ticket/GetClientNumbers
        [Authorize]
        [HttpGet]
        public JsonResult GetClientNumbers(int clientId, int raffleId)
        {
            var respnse = new TicketIdentifyModel().GetClientNumbers(clientId, raffleId);
            return new JsonResult()
            {
                Data = respnse,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        // GET: /Ticket/IdentifyAward
        [Authorize]
        [HttpGet]
        public ActionResult IdentifyAward()
        {
            return View();
        }

        //
        // POST: /Ticket/IdentifyAward
        [Authorize]
        [HttpPost]
        public JsonResult IdentifyAward(IdentifyBach identifyBach)
        {
            var respnse = new TicketIdentifyModel().IdentifyAward(identifyBach);
            return new JsonResult()
            {
                Data = respnse
            };
        }

        //
        //  POST:   /Ticket/ValidateNumberAward
        [Authorize]
        [HttpPost]
        public JsonResult ValidateNumberAward(AwardTicketModel awardTicket, int clientId)
        {
            var respnse = new TicketIdentifyModel().ValidateNumberAward(awardTicket, clientId);
            return new JsonResult()
            {
                Data = respnse
            };
        }


        // POST: Ticket/IdentifyNumberDelete
        [HttpPost]
        [Authorize]
        public JsonResult IdentifyNumberDelete(IdentifyNumber model)
        {
            var respnse = new TicketIdentifyModel().IdentifyNumberDelete(model);
            return new JsonResult()
            {
                Data = respnse
            };
        }

        // POST: Ticket/IdentifyBachDelete
        [HttpPost]
        [Authorize]
        public JsonResult IdentifyBachDelete(IdentifyBach model)
        {
            var respnse = new TicketIdentifyModel().IdentifyBachDelete(model);
            return new JsonResult()
            {
                Data = respnse
            };
        }
        #endregion
    }
}
