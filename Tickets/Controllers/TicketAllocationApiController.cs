//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/ticketAllocationApi")]
    public class TicketAllocationApiController : ApiController
    {
        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocationList
        [HttpGet]
        [Authorize]
        [ActionName("getTicketAllocationList")]
        public RequestResponseModel GetTicketAllocationList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getReviewAllocations
        [HttpGet]
        [Authorize]
        [ActionName("getReviewAllocations")]
        public RequestResponseModel GetReviewAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, (int)AllocationStatuEnum.Review);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getPrintedAllocations
        [HttpGet]
        [Authorize]
        [ActionName("getPrintedAllocations")]
        public RequestResponseModel GetPrintedAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, (int)AllocationStatuEnum.Printed);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getPendintPrintAllocations
        [HttpGet]
        [Authorize]
        [ActionName("getPendintPrintAllocations")]
        public RequestResponseModel GetPendintPrintAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, (int)AllocationStatuEnum.PendientPrint);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getAllocationMainList
        [HttpGet]
        [Authorize]
        [ActionName("getAllocationMainList")]
        public RequestResponseModel GetAllocationMainList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, 0, true);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/TicketAllocationListForPrint
        [HttpGet]
        [Authorize]
        [ActionName("ticketAllocationListForPrint")]
        public RequestResponseModel TicketAllocationListForPrint(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().TicketAllocationListForPrint(raffleId, clientId, 0, true);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocation
        [HttpGet]
        [Authorize]
        [ActionName("getTicketAllocation")]
        public RequestResponseModel GetTicketAllocation(int id)
        {
            var response = new TicketAllocationModel().GetTicketAllocation(id);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/copyAllocations
        [HttpPost]
        [Authorize]
        [ActionName("copyAllocations")]
        public RequestResponseModel CopyAllocations(CopyAllocationModel model)
        {
            var response = new TicketAllocationModel().CopyAllocations(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/validateAllocation
        [HttpPost]
        [Authorize]
        [ActionName("validateAllocation")]
        public RequestResponseModel ValidateAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().ValidateAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/saveAllocation
        [HttpPost]
        [Authorize]
        [ActionName("saveAllocation")]
        public RequestResponseModel SaveAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().SaveAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/deleteAllocation
        [HttpPost]
        [Authorize]
        [ActionName("deleteAllocation")]
        public RequestResponseModel DeleteAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().DeleteAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/deleteAllocationNumber
        [HttpPost]
        [Authorize]
        [ActionName("deleteAllocationNumber")]
        public RequestResponseModel DeleteAllocationNumber(TicketAllocationNumberModel model)
        {
            var response = new TicketAllocationNumberModel().DeleteAllocationNumber(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/startAllocationPrint
        [HttpPost]
        [Authorize]
        [ActionName("startAllocationPrint")]
        public RequestResponseModel StartAllocationPrint(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().ChangeAllocationStatu(model, (int)AllocationStatuEnum.PendientPrint);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/ticketAllocationReassing
        [HttpPost]
        [Authorize]
        [ActionName("ticketAllocationReassing")]
        public RequestResponseModel TicketAllocationReassing(ReassignModel model)
        {
            var response = new ReassignModel().TicketAllocationReassing(model);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketNumberDetails
        [HttpGet]
        [Authorize]
        [ActionName("getTicketNumberDetails")]
        public RequestResponseModel GetTicketNumberDetails(int numberId)
        {
            var response = new TicketAllocationNumberModel().GetTicketNumberDetails(numberId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketNumberDetails
        [HttpGet]
        [Authorize]
        [ActionName("awardNumberDetails")]
        public RequestResponseModel AwardNumberDetails(int number, int raffleId, int fractionFrom, int fractionTo)
        {
            var response = new TicketAllocationNumberModel().awardNumberDetails(number, raffleId, fractionFrom, fractionTo);
            return response;
        }
    }
}