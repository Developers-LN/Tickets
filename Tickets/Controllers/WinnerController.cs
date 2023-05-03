using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Filters;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class WinnerController : Controller
    {
        // GET: Winner
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }
    }
}