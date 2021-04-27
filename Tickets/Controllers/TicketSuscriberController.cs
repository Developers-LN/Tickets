using System.Web.Mvc;
using Tickets.Filters;

namespace Tickets.Controllers
{

    [Authorize]
    [InitializeSimpleMembership]
    public class TicketSuscriberController : Controller
    {
        //
        // GET: /TicketSuscriber/TicketSuscriberList
        [Authorize]
        [HttpGet]
        public ActionResult TicketSuscriberList()
        {
            return View();
        }

        //
        // GET: /TicketSuscriber/TicketSuscriberCreate
        [HttpGet]
        [Authorize]
        public ActionResult TicketSuscriberCreate()
        {
            return View();
        }

        //
        // GET: /TicketSuscriber/TicketSuscriberDetails
        [HttpGet]
        [Authorize]
        public ActionResult TicketSuscriberDetails()
        {
            return View();
        }

    }
}