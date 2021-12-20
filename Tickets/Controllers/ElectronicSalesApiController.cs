//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Ticket;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/electronicSalesApi")]
    public class ElectronicSalesApiController : ApiController
    {
        // GET: ElectronicSalesApi
        [HttpGet]
        [Authorize]
        [ActionName("getElectronicSalesList")]
        public RequestResponseModel GetElectronicSalesList(int raffleId, int clientId = 0)
        {
            var response = new ElectronicSalesModel().GetList(raffleId, clientId, (int)ElectronicSalesStatusEnum.Creada);
            return response;
        }

        [HttpGet]
        [Authorize]
        [ActionName("getElectronicSalesDetails")]
        public RequestResponseModel GetElectronicSalesDetails(int allocationId)
        {
            var response = new ElectronicSalesModel().GetElectronicSalesDetails(allocationId);
            return response;
        }
    }
}
