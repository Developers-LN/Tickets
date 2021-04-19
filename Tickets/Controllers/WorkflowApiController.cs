//using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Prospects;
using Tickets.Models.Raffles;
using Tickets.Models.Workflows;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/workflowApi")]
    public class WorkflowApiController : ApiController
    {
        //
        //  GET: ticket/workflowApi/sendProspectToWorkflow
        [HttpPost]
        [ActionName("sendProspectToWorkflow")]
        [Authorize]
        public RequestResponseModel SendProspectToWorkflow(ProspectModel model)
        {
            var response = new WorkflowModel().SendProspectToWorkflow(model);
            return response;
        }

        //
        //  POST: ticket/workflowApi/sendReprintToWorkflow
        [HttpPost]
        [Authorize]
        [ActionName("sendReprintToWorkflow")]
        public RequestResponseModel SendReprintToWorkflow(TicketReprintModel model)
        {
            var response = new WorkflowModel().SendReprintToWorkflow(model);
            return response;
        }

        //
        //  POST: ticket/workflowApi/sendAwardCertificationToWorkflow
        [HttpPost]
        [Authorize]
        [ActionName("sendAwardCertificationToWorkflow")]
        public RequestResponseModel SendAwardCertificationToWorkflow(IdentifyNumber model)
        {
            var response = new WorkflowModel().SendAwardCertificationToWorkflow(model);
            return response;
        }


        //
        //  GET: ticket/workflowApi/getWorkflowList
        [HttpGet]
        [ActionName("getWorkflowList")]
        [Authorize]
        public RequestResponseModel GetWorkflowList(int type)
        {
            var response = new WorkflowModel().GetWorkflowList(type);
            return response;
        }

        //
        //  GET: ticket/workflowApi/getWorkflow
        [HttpGet]
        [ActionName("getWorkflow")]
        [Authorize]
        public RequestResponseModel GetWorkflow(int id)
        {
            var response = new WorkflowModel().GetWorkflow(id);
            return response;
        }

        //
        //  GET: ticket/workflowApi/saveWorkflowProccess
        [HttpPost]
        [ActionName("saveWorkflowProccess")]
        [Authorize]
        public RequestResponseModel SaveWorkflowProccess(ProccessModel model)
        {
            var response = new WorkflowModel().SaveWorkflowProccess(model);
            return response;
        }
    }
}