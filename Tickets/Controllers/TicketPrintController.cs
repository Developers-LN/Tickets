using System.Web.Mvc;

namespace Tickets.Controllers
{
    public class TicketPrintController : Controller
    {
        //
        // GET: /TicketPrint/TicketPrint
        [Authorize]
        [HttpGet]
        public ActionResult TicketPrint()
        {
            return View();
        }

        //
        // GET: /TicketPrint/AllocationPrintList
        [Authorize]
        [HttpGet]
        public ActionResult AllocationPrintList()
        {
            return View();
        }

        //
        // GET: /TicketPrint/AllocationListPrint
        [Authorize]
        [HttpGet]
        public ActionResult AllocationListPrint()
        {
            return View();
        }

        //
        // GET: /TicketPrint/TicketReviewList
        [Authorize]
        [HttpGet]
        public ActionResult TicketReviewList()
        {
            return View();
        }


        #region Ticket Reprint
        //
        // GET: /TicketPrint/TicketReprintList
        [Authorize]
        [HttpGet]
        public ActionResult TicketReprintList()
        {
            return View();
        }

        //
        // GET: /TicketPrint/TicketReprintCreate
        [Authorize]
        [HttpGet]
        public ActionResult TicketReprintCreate()
        {
            return View();
        }

        //
        // GET: /TicketPrint/TicketReprint
        [Authorize]
        [HttpGet]
        public ActionResult TicketReprint()
        {
            return View();
        }

        #endregion

        #region Reprint Workflow
        //
        // GET: TicketPrint/ReprintWorkflowList
        [HttpGet]
        [Authorize]
        public ActionResult ReprintWorkflowList()
        {
            return View();
        }

        //
        // GET: TicketPrint/ApprovedReprintProcess
        [HttpGet]
        public ActionResult ApprovedReprintProcess()
        {
            return View();
        }

        #endregion

    }
}