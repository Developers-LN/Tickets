﻿//using AttributeRouting;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Clients;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/clientApi")]
    public class ClientApiController : ApiController
    {
        //
        //  GET: ticket/clientApi/getClientSelect
        [HttpGet]
        [Authorize]
        [ActionName("getClientSelect")]
        public RequestResponseModel GetClientSelect(int statu = 0)
        {
            var response = new ClientModel().GetClientSelect(statu);
            return response;
        }
    }
}