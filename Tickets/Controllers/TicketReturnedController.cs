using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TicketReturnedController : Controller
    {
        //
        //  GET: /TicketReturned/ReturnedList
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedList()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedAwardList
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedAwardList()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/PrintReturnedByGroup
        [Authorize]
        [HttpGet]
        public ActionResult PrintReturnedByGroup()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedValidation
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedValidation()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedValidationGroupDetails
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedValidationGroupDetails()
        {
            return View();
        }

        //
        // GET: /TicketReturned/ReturnedAwardReports
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedAwardReports()
        {
            return View();
        }

        //
        // GET: /TicketReturned/PrintReturnedList
        [Authorize]
        [HttpGet]
        public ActionResult PrintReturnedList()
        {
            return View();
        }

        //
        // GET: TicketReturned/Returned
        [HttpGet]
        [Authorize]
        public ActionResult Returned()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedListQuery
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedListQuery()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedGroupDetails
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedGroupDetails()
        {
            return View();
        }

        //
        //  GET: /TicketReturned/ReturnedGroupDetails
        [Authorize]
        [HttpGet]
        public ActionResult ReturnedGroupClientDetails()
        {
            return View();
        }

        //
        //  GET:    /TicketReturned/GetReturnedDetail
        [Authorize]
        [HttpGet]
        public JsonResult GetReturnedDetail(int numberId)
        {
            var response = new TicketReturnedNumberModel().GetReturnedDetail(numberId);

            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //  GET: /TicketReturned/GetReturnedGroupList
        [Authorize]
        [HttpGet]
        public JsonResult GetReturnedGroupList(int raffleId)
        {
            var response = new TicketReturnedNumberModel().GetReturnedGroupList(raffleId);

            return new JsonResult()
            {
                Data = response
            };
        }

        //
        //  GET: /TicketReturned/GetReturnedAwardList
        [Authorize]
        [HttpGet]
        public JsonResult GetReturnedAwardList(int raffleId)
        {
            var response = new TicketReturnedNumberModel().GetReturnedAwardList(raffleId);

            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //  GET: /TicketReturned/GetReturnedList
        [Authorize]
        [HttpGet]
        public JsonResult GetReturnedList(int raffleId, int clientId = 0)
        {
            var response = new TicketReturnedNumberModel().GetReturnedList(raffleId, clientId);

            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        //
        //  GET: /TicketReturned/ReturnedAwardReportData
        [Authorize]
        [HttpGet]
        public JsonResult ReturnedAwardReportData(int raffleId = 0)
        {
            var response = new TicketReturnedNumberModel().ReturnedAwardReportData(raffleId);

            return new JsonResult()
            {
                Data = response,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}