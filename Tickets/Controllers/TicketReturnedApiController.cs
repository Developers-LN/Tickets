//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/ticketReturnedApi")]
    public class TicketReturnedApiController : ApiController
    {
        //
        //  GET: ticket/ticketReturnedApi/getCreatedList
        [HttpGet]
        [Authorize]
        [ActionName("getCreatedList")]
        public RequestResponseModel GetCreatedList(int raffleId, int clientId = 0)
        {
            var response = new TicketReturnedModel().GetList(raffleId, clientId, (int)TicketReturnedStatuEnum.Created);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/geListtByClient
        [HttpGet]
        [Authorize]
        [ActionName("geListtByClient")]
        public RequestResponseModel GeListtByClient(int raffleId, int clientId = 0)
        {
            var response = new TicketReturnedModel().GeListtByClient(raffleId, clientId, false);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/geListtByClient
        [HttpGet]
        [Authorize]
        [ActionName("getListtByValidation")]
        public RequestResponseModel GetListtByValidation(int raffleId, int clientId = 0)
        {
            var response = new TicketReturnedModel().GetListtByValidation(raffleId, clientId, false);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/getGroupListSelect
        [HttpGet]
        [Authorize]
        [ActionName("getGroupListSelect")]
        public RequestResponseModel GetGroupListSelect(int raffleId)
        {
            var response = new TicketReturnedModel().GetGroupListSelect(raffleId, TicketReturnedStatuEnum.Created);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/details
        [HttpGet]
        [Authorize]
        [ActionName("details")]
        public RequestResponseModel Details(string group, int raffleId)
        {
            var response = new TicketReturnedModel().GetList(raffleId, group);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/verify
        [HttpPost]
        [Authorize]
        [ActionName("verify")]
        public RequestResponseModel Verify(TicketReturnedModel model)
        {
            return new TicketReturnedModel().ValidateTicketReturned(model);
        }

        //
        //  GET: ticket/ticketReturnedApi/save
        [HttpPost]
        [Authorize]
        [ActionName("save")]
        public RequestResponseModel Save(TicketReturnedModel model)
        {
            var response = new TicketReturnedModel().Save(model);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/deleteNumber
        [HttpPost]
        [Authorize]
        [ActionName("deleteNumber")]
        public RequestResponseModel Save(TicketReturnedNumberModel model)
        {
            var response = new TicketReturnedNumberModel().Delete(model);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/deleteGroup
        [HttpPost]
        [Authorize]
        [ActionName("deleteGroup")]
        public RequestResponseModel DeleteGroup(TicketReturnedModel model)
        {
            var response = new TicketReturnedModel().Delete(model);
            return response;
        }

        //
        //  GET: ticket/ticketReturnedApi/moveReturnedGroup
        [HttpPost]
        [Authorize]
        [ActionName("moveReturnedGroup")]
        public RequestResponseModel MoveReturnedGroup(MoveReturnedGroupModel model)
        {
            var response = new MoveReturnedGroupModel().MoveReturnedGroup(model);
            return response;
        }
    }
}
