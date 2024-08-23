using System.Web.Mvc;
using Tickets.Filters;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ProspectController : Controller
    {
        // GET: Prospect/ProspectCreateWizard
        [HttpGet]
        public ActionResult ProspectCreateWizard()
        {
            return View();
        }

        #region Creation Prospect Wizard
        // GET: Prospect/_CreationType
        [HttpGet]
        [Authorize]
        public ActionResult _CreationTypePartial()
        {
            return View();
        }

        // GET: Prospect/_ProspectType
        [HttpGet]
        [Authorize]
        public ActionResult _ProspectTypePartial()
        {
            return View();
        }

        // GET: Prospect/_ProspectDesing
        [HttpGet]
        [Authorize]
        public ActionResult _ProspectDesingPartial()
        {
            return View();
        }

        // GET: Prospect/_ProspectPrice
        [HttpGet]
        [Authorize]
        public ActionResult _ProspectPricePartial()
        {
            return View();
        }

        // GET: Prospect/_ProspectAward
        [HttpGet]
        [Authorize]
        public ActionResult _ProspectAwardPartial()
        {
            return View();
        }

        // GET: Prospect/_ProspectVerify
        [HttpGet]
        [Authorize]
        public ActionResult _ProspectVerifyPartial()
        {
            return View();
        }
        #endregion

        // GET: Prospect/List
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        // GET: Prospect/SuspendedList
        [HttpGet]
        public ActionResult SuspendedList()
        {
            return View();
        }

        //
        //  GET: Prospect/AllProspects
        [HttpGet]
        public ActionResult AllProspects()
        {
            return View();
        }

        // GET: Prospect/WorkflowList
        [HttpGet]
        public ActionResult WorkflowList()
        {
            return View();
        }

        // GET: Prospect/ApprovedProspectProcess
        [HttpGet]
        public ActionResult ApprovedProcess()
        {
            return View();
        }
    }
}