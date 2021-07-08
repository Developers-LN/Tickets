//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Prospects;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/prospectApi")]
    public class ProspectApiController : ApiController
    {
        //
        //  GET: ticket/prospectApi/getProspect
        [HttpGet]
        [ActionName("getProspect")]
        [Authorize]
        public RequestResponseModel GetProspect(int id)
        {
            var response = new ProspectModel().GetProspect(id);
            return response;
        }

        //
        //  GET: ticket/prospectApi/getProspectPrice
        [HttpGet]
        [ActionName("getProspectPrice")]
        [Authorize]
        public RequestResponseModel GetProspectPrice(int prospectId, int priceId)
        {
            var response = new ProspectPriceModel().GetProspectPrice(prospectId, priceId);
            return response;
        }

        //
        //  GET: ticket/prospectApi/saveProspect
        [HttpPost]
        [ActionName("saveProspect")]
        [Authorize]
        public RequestResponseModel SaveProspect(ProspectModel model)
        {
            var response = new ProspectModel().SaveProspect(model);
            return response;
        }

        //
        //  GET: ticket/prospectApi/getProspects
        [HttpGet]
        [ActionName("getProspects")]
        [Authorize]
        public RequestResponseModel GetProspects(int statu = 0)
        {
            var response = new ProspectModel().GetProspects(statu);
            return response;
        }

        //
        //  GET: ticket/prospectApi/getProspectSelect
        [HttpGet]
        [ActionName("getProspectSelect")]
        [Authorize]
        public RequestResponseModel GetProspectSelect()
        {
            var response = new ProspectModel().GetProspectSelect();
            return response;
        }

        //
        //  GET: ticket/prospectApi/getPoolsProspectSelect
        [HttpGet]
        [ActionName("getPoolsProspectSelect")]
        [Authorize]
        public RequestResponseModel GetPoolsProspectSelect(int statu = 0)
        {
            var response = new ProspectModel().GetProspectSelect(statu);
            return response;
        }

        //
        //  GET: ticket/prospectApi/getTicketProspectSelect
        [HttpGet]
        [ActionName("getTicketProspectSelect")]
        [Authorize]
        public RequestResponseModel GetTicketProspectSelect(int statu = 0)
        {
            var response = new ProspectModel().GetProspectSelect(statu);
            return response;
        }

        //
        //  GET: ticket/prospectApi/getTypeAwardtSelect
        [HttpGet]
        [ActionName("getTypeAwardtSelect")]
        [Authorize]
        public RequestResponseModel GetTypeAwardSelect()
        {
            var response = new ProspectModel().GetTypeAwardSelect();
            return response;
        }

    }
}
