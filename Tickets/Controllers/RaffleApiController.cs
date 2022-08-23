using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Raffles;
//using AttributeRouting;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/raffleApi")]
    public class RaffleApiController : ApiController
    {
        //
        //  GET: ticket/raffleApi/getRaffle
        [HttpGet]
        [Authorize]
        [ActionName("getRaffle")]
        public RequestResponseModel GetRaffle(int id)
        {
            var response = new RaffleModel().GetRaffle(id);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleListSP
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleListSP")]
        public RequestResponseModel GetRaffleListSP()
        {
            var response = new RaffleModel().GetRaffleListSP();
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleList
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleList")]
        public RequestResponseModel GetRaffleList(int statu)
        {
            var response = new RaffleModel().GetRaffleList(statu);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getPlannedRaffles
        [HttpGet]
        [Authorize]
        [ActionName("getPlannedRaffles")]
        public RequestResponseModel GetPlannedRaffles()
        {
            var response = new RaffleModel().GetRaffleList((int)RaffleStatusEnum.Planned);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getSuspendedRaffles
        [HttpGet]
        [Authorize]
        [ActionName("getSuspendedRaffles")]
        public RequestResponseModel GetSuspendedRaffles()
        {
            var response = new RaffleModel().GetRaffleList((int)RaffleStatusEnum.Suspended);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getGeneratedRaffles
        [HttpGet]
        [Authorize]
        [ActionName("getGeneratedRaffles")]
        public RequestResponseModel GetGeneratedRaffles()
        {
            var response = new RaffleModel().GetRaffleList((int)RaffleStatusEnum.Generated);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleSelect
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleSelect")]
        public RequestResponseModel GetRaffleSelect(int statu = 0)
        {
            var response = new RaffleModel().GetRaffleSelect(statu);
            return response;
        }

        [HttpGet]
        [Authorize]
        [ActionName("getRaffleConsignation")]
        public RequestResponseModel GetRaffleConsignation()
        {
            var response = new RaffleModel().GetRaffleConsignation();
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleForReturnedSelect
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleForReturnedSelect")]
        public RequestResponseModel GetRaffleForReturnedSelect()
        {
            var response = new RaffleModel().GetRaffleForReturnedSelect();
            return response;
        }

        [HttpGet]
        [Authorize]
        [ActionName("getRaffleForElectronicSales")]
        public RequestResponseModel GetRaffleForElectronicSales()
        {
            var response = new RaffleModel().GetRaffleForElectronicSales();
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleForAllocationSelect
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleForAllocationSelect")]
        public RequestResponseModel GetRaffleForAllocationSelect()
        {
            var response = new RaffleModel().GetRaffleForAllocationSelect();
            return response;
        }

        //
        //  GET: ticket/raffleApi/saveRaffle
        [HttpPost]
        [Authorize]
        [ActionName("saveRaffle")]
        public RequestResponseModel SaveRaffle(RaffleModel model)
        {
            var response = new RaffleModel().SaveRaffle(model);
            return response;
        }

        //
        //  GET: ticket/raffleApi/suspend
        [HttpPost]
        [Authorize]
        [ActionName("suspend")]
        public RequestResponseModel Suspend(RaffleModel model)
        {
            var response = new RaffleModel().Suspend(model);
            return response;
        }

        //
        //  GET: ticket/raffleApi/getActive
        [HttpGet]
        [Authorize]
        [ActionName("getActive")]
        public RequestResponseModel GetActive(RaffleModel model)
        {
            var response = new RaffleModel().GetActive();
            return response;
        }

        //
        //  GET: ticket/raffleApi/getRaffleDetails
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleDetails")]
        public RequestResponseModel GetRaffleDetails(int id)
        {
            var response = new VirtualRaffleModel().VirtualRaffleDetails(id);
            return response;
        }

    }
}