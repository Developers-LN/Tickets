//using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Raffles;
using Tickets.Models.Ticket;
using Tickets.Models.Workflows;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/ticketPrintApi")]
    public class TicketPrintApiController : ApiController
    {
        //
        //  POST: ticket/ticketPrintApi/ticketPrintDetails
        [HttpPost]
        [Authorize]
        [ActionName("ticketPrintDetails")]
        public RequestResponseModel TicketPrintDetails(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().TicketPrintDetails(model);
            return response;
        }

        //
        //  GET: ticket/ticketPrintApi/getPendintPrintedAllocations
        [HttpGet]
        [Authorize]
        [ActionName("getPendintPrintedAllocations")]
        public RequestResponseModel GetPendintPrintedAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetPendintPrintedAllocations(raffleId, clientId);
            return response;
        }

        //
        //  POST: ticket/ticketPrintApi/ticketAllocationReview
        [HttpPost]
        [Authorize]
        [ActionName("ticketAllocationReview")]
        public RequestResponseModel TicketAllocationReview(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().TicketAllocationReview(model);
            return response;
        }

        //
        //  POST: ticket/ticketPrintApi/save
        [Authorize]
        [HttpPost]
        [ActionName("save")]
        public RequestResponseModel Save(TicketReprintModel reprintModel)
        {
            var response = new TicketReprintModel().Save(reprintModel);
            return response;
        }

        //
        //  POST: ticket/ticketPrintApi/verify
        [HttpPost]
        [Authorize]
        [ActionName("verify")]
        public RequestResponseModel Verify(TicketReprintModel reprintModel)
        {
            var response = new TicketReprintModel().Verify(reprintModel);
            return response;
        }

        //
        //  POST: ticket/ticketPrintApi/delete
        [HttpPost]
        [Authorize]
        [ActionName("delete")]
        public RequestResponseModel Delete(TicketReprintModel model)
        {
            var response = new TicketReprintModel().Delete(model);
            return response;
        }

        //
        //  GET: ticket/ticketPrintApi/getReprint
        [HttpGet]
        [Authorize]
        [ActionName("getReprint")]
        public RequestResponseModel GetReprint(int id)
        {
            var response = new TicketReprintModel().GetReprint(id);
            return response;
        }

        //
        //  GET: ticket/ticketPrintApi/getReprintAppruved
        [HttpGet]
        [Authorize]
        [ActionName("getReprintAppruved")]
        public RequestResponseModel GetReprintAppruved(int raffleId)
        {
            var response = new TicketReprintModel().GetReprintList(raffleId, (int)TicketReprintStatuEnum.Approved);
            return response;
        }

        //
        //  GET: ticket/ticketPrintApi/getReprintList
        [HttpGet]
        [Authorize]
        [ActionName("getReprintList")]
        public RequestResponseModel GetReprintList(int raffleId = 0)
        {
            var response = new TicketReprintModel().GetReprintList(raffleId);
            return response;
        }
    }
}
