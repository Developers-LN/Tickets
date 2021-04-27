using System.IO;
using System.Web.Mvc;
using Tickets.Models;

namespace Tickets.Controllers
{
    public class MailController : Controller
    {
        //
        // GET: /Mail/WorkFlowProcess
        [Authorize]
        [HttpGet]
        public bool WorkFlowProcess(int workflowId)
        {
            var context = new TicketsEntities();

            return true;
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


    }
}