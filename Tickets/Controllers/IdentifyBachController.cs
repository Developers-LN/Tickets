using System.Web.Mvc;
using Tickets.Filters;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class IdentifyBachController : Controller
    {
        // GET: IdentifyBach/WorkflowList
        [HttpGet]
        public ActionResult WorkflowList()
        {
            return View();
        }
    }
}
