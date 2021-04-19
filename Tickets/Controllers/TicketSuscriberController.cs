using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models.Ticket;

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