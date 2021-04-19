//using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{

    [Authorize]
    [RoutePrefix("ticket/ticketSuscriberApi")]
    public class TicketSuscriberApiController : ApiController
    {
        //
        //  GET: ticket/ticketSuscriberApi/getList
        [HttpGet]
        [Authorize]
        [ActionName("getList")]
        public RequestResponseModel GetList(int clientId = 0)
        {
            var response = new TicketSuscriberModel().GetSuscriberList(clientId);
            return response;
        }


        //
        //  GET: ticket/ticketSuscriberApi/get
        [HttpGet]
        [Authorize]
        [ActionName("get")]
        public RequestResponseModel Get(int id)
        {
            var response = new TicketSuscriberModel().GetSuscriber(id);
            return response;
        }

        //
        //  POST: ticket/ticketSuscriberApi/save
        [HttpPost]
        [Authorize]
        [ActionName("save")]
        public RequestResponseModel Save(TicketSuscriberModel model)
        {
            var response = new TicketSuscriberModel().Save(model);
            return response;
        }

        //
        //  POST: ticket/ticketSuscriberApi/verify
        [HttpPost]
        [Authorize]
        [ActionName("verify")]
        public RequestResponseModel Verify(TicketSuscriberModel model)
        {
            var response = new TicketSuscriberModel().Verify(model);
            return response;
            
        } 

        //
        //  POST: ticket/ticketSuscriberApi/delete
        [HttpPost]
        [Authorize]
        [ActionName("delete")]
        public RequestResponseModel Delete(TicketSuscriberModel model)
        {
            var response = new TicketSuscriberModel().TicketSuscriberDelete(model);
            return response;
        }
        //
        //  POST: ticket/ticketSuscriberApi/deleteNumber
        [HttpPost]
        [Authorize]
        [ActionName("deleteNumber")]
        public RequestResponseModel DeleteNumber(TicketSuscriberNumberModel model)
        {
            var response = new TicketSuscriberNumberModel().SuscriberNumberDelete(model);
            return response;
        }
    }
}
