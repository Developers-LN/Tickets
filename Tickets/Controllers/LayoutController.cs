using System.Web.Mvc;
using Tickets.Filters;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [InitializeSimpleMembership]
    public class LayoutController : Controller
    {
        // GET: Layout
        public ActionResult Index()
        {
            if (!WebSecurity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Security");
            }
            return View();
        }

        // GET: Layout
        public ActionResult Reports()
        {
            return View();
        }
    }
}