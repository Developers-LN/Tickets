using System.Web.Mvc;

namespace Tickets.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            string query = Request.Url.ToString();

            if (!string.IsNullOrWhiteSpace(query))
            {
                return Redirect(query.Replace("Account", "Security"));
            }
            return Redirect("/Security/Login");

        }
    }
}