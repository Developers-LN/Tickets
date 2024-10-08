﻿using System.Web.Http;
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
        //  POST: ticket/identifyBachApi/approveBachPay
        [HttpPost]
        [ActionName("approveBachPay")]
        public RequestResponseModel ApproveBachPay(int id)
        {
            var response = new TicketIdentifyModel().ApproveBachPay(id);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocationList
        [HttpGet]
        [ActionName("getTicketAllocationList")]
        public RequestResponseModel GetTicketAllocationList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getReturnedByGroup
        [HttpGet]
        [ActionName("getReturnedByGroup")]
        public RequestResponseModel getReturnedByGroup(int raffleId)
        {
            var response = new TicketAllocationModel().GetReturnedByGroup(raffleId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocationToDeliverList
        [HttpGet]
        [ActionName("getTicketAllocationToDeliverList")]
        public RequestResponseModel GetTicketAllocationToDeliverList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationToDeliverList(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocationConsignateList
        [HttpGet]
        [ActionName("getTicketAllocationConsignateList")]
        public RequestResponseModel GetTicketAllocationConsignateList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationConsignateList(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getReviewAllocations
        [HttpGet]
        [ActionName("getReviewAllocations")]
        public RequestResponseModel GetReviewAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationListForInvoice(raffleId, clientId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getPrintedAllocations
        [HttpGet]
        [ActionName("getPrintedAllocations")]
        public RequestResponseModel GetPrintedAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, (int)AllocationStatuEnum.Printed);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getPendintPrintAllocations
        [HttpGet]
        [ActionName("getPendintPrintAllocations")]
        public RequestResponseModel GetPendintPrintAllocations(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, (int)AllocationStatuEnum.PendientPrint);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getAllocationMainList
        [HttpGet]
        [ActionName("getAllocationMainList")]
        public RequestResponseModel GetAllocationMainList(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().GetTicketAllocationList(raffleId, clientId, 0, true);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/TicketAllocationListForPrint
        [HttpGet]
        [ActionName("ticketAllocationListForPrint")]
        public RequestResponseModel TicketAllocationListForPrint(int raffleId, int clientId = 0)
        {
            var response = new TicketAllocationModel().TicketAllocationListForPrint(raffleId, clientId, 0, true);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketAllocation
        [HttpGet]
        [ActionName("getTicketAllocation")]
        public RequestResponseModel GetTicketAllocation(int id)
        {
            var response = new TicketAllocationModel().GetTicketAllocation(id);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/copyAllocations
        [HttpPost]
        [ActionName("copyAllocations")]
        public RequestResponseModel CopyAllocations(CopyAllocationModel model)
        {
            var response = new TicketAllocationModel().CopyAllocations(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/validateAllocation
        [HttpPost]
        [ActionName("validateAllocation")]
        public RequestResponseModel ValidateAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().ValidateAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/saveAllocation
        [HttpPost]
        [ActionName("saveAllocation")]
        public RequestResponseModel SaveAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().SaveAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/deleteAllocation
        [HttpPost]
        [ActionName("deleteAllocation")]
        public RequestResponseModel DeleteAllocation(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().DeleteAllocation(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/deleteAllocationNumber
        [HttpPost]
        [ActionName("deleteAllocationNumber")]
        public RequestResponseModel DeleteAllocationNumber(TicketAllocationNumberModel model)
        {
            var response = new TicketAllocationNumberModel().DeleteAllocationNumber(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/unConsignateNumber
        [HttpPost]
        [ActionName("unConsignateNumber")]
        public RequestResponseModel UnConsignateNumber(TicketAllocationNumberModel model)
        {
            var response = new TicketAllocationNumberModel().UnConsignateNumber(model);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/removeAllocation
        [HttpPost]
        [ActionName("removeAllocation")]
        public RequestResponseModel RemoveAllocation(int id)
        {
            var response = new TicketAllocationNumberModel().RemoveAllocation(id);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/consingAllocation
        [HttpPost]
        [ActionName("consingAllocation")]
        public RequestResponseModel ConsingAllocation(int id)
        {
            var response = new TicketAllocationNumberModel().ConsignateAllocation(id);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/desconsingAllocation
        [HttpPost]
        [ActionName("unConsingAllocation")]
        public RequestResponseModel DesonsingAllocation(int id)
        {
            var response = new TicketAllocationNumberModel().DesconsignateAllocation(id);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/deliverAllocation
        [HttpPost]
        [ActionName("deliverAllocation")]
        public RequestResponseModel DeliverAllocation(int id)
        {
            var response = new TicketAllocationNumberModel().DeliverAllocation(id);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/startAllocationPrint
        [HttpPost]
        [ActionName("startAllocationPrint")]
        public RequestResponseModel StartAllocationPrint(TicketAllocationModel model)
        {
            var response = new TicketAllocationModel().ChangeAllocationStatu(model, (int)AllocationStatuEnum.PendientPrint);
            return response;
        }

        //
        //  POST: ticket/ticketAllocationApi/ticketAllocationReassing
        [HttpPost]
        [ActionName("ticketAllocationReassing")]
        public RequestResponseModel TicketAllocationReassing(ReassignModel model)
        {
            var response = new ReassignModel().TicketAllocationReassing(model);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketNumberDetails
        [HttpGet]
        [ActionName("getTicketNumberDetails")]
        public RequestResponseModel GetTicketNumberDetails(int numberId)
        {
            var response = new TicketAllocationNumberModel().GetTicketNumberDetails(numberId);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketNumberDetails
        [HttpGet]
        [ActionName("awardNumberDetails")]
        public RequestResponseModel AwardNumberDetails(int number, int raffleId, int fractionFrom, int fractionTo)
        {
            var response = new TicketAllocationNumberModel().AwardNumberDetails(number, raffleId, fractionFrom, fractionTo);
            return response;
        }

        //
        //  GET: ticket/ticketAllocationApi/getTicketNumberDetails
        [HttpGet]
        [ActionName("awardSellerNumberDetails")]
        public RequestResponseModel AwardSellerNumberDetails(int number, int raffleId, int fractionFrom, int fractionTo)
        {
            var response = new TicketAllocationNumberModel().AwardSellerNumberDetails(number, raffleId, fractionFrom, fractionTo);
            return response;
        }
    }
}
