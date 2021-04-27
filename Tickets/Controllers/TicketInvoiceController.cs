using System.Web.Mvc;
using Tickets.Filters;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class TicketInvoiceController : Controller
    {
        //
        //  GET: /TicketInvoice/InvoiceListForSuspend
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceListForSuspend()
        {
            return View();
        }

        //
        //  GET: /TicketInvoice/InvoiceList
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceList()
        {
            return View();
        }

        //
        // GET: TicketInvoice/Invoice
        [HttpGet]
        [Authorize]
        public ActionResult Invoice()
        {
            return View();
        }

        //
        //  GET: /TicketInvoice/InvoiceListPrint
        [Authorize]
        [HttpGet]
        public ActionResult InvoiceListPrint()
        {
            return View();
        }
    }
}