//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/ticketInvoiceApi")]
    public class TicketInvoiceApiController : ApiController
    {
        //
        //  GET: ticket/ticketInvoiceApi/getInvoiceList
        [HttpGet]
        [Authorize]
        [ActionName("getInvoiceList")]
        public RequestResponseModel GetInvoiceList(int raffleId, int clientId = 0)
        {
            var response = new TicketInvoiceModel().GetInvoiceList(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketInvoiceApi/getInvoicePendientList
        [HttpGet]
        [Authorize]
        [ActionName("getInvoicePendientList")]
        public RequestResponseModel GetInvoicePendientList(int raffleId, int clientId = 0)
        {
            var response = new TicketInvoiceModel().GetInvoiceList(raffleId, clientId, (int)InvoicePaymentStatuEnum.Pendient);
            return response;
        }

        //
        //  GET: ticket/ticketInvoiceApi/getInvoice
        [HttpGet]
        [Authorize]
        [ActionName("getInvoice")]
        public RequestResponseModel GetInvoice(int id)
        {
            var response = new TicketInvoiceModel().GetInvoice(id);
            return response;
        }

        //
        //  GET: ticket/ticketInvoiceApi/save
        [HttpPost]
        [Authorize]
        [ActionName("save")]
        public RequestResponseModel Save(TicketInvoiceModel invoiceModel)
        {
            var response = new TicketInvoiceModel().Save(invoiceModel);
            return response;
        }

        //
        //  GET: ticket/ticketInvoiceApi/suspend
        [HttpPost]
        [Authorize]
        [ActionName("suspend")]
        public RequestResponseModel Suspend(TicketInvoiceModel invoiceModel)
        {
            var response = new TicketInvoiceModel().Suspend(invoiceModel);
            return response;
        }
    }
}
