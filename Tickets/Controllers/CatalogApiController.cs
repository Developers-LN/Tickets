//using AttributeRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tickets.Models;
using Tickets.Models.Enums;

namespace Tickets.Controllers
{
    [Authorize]
    [RoutePrefix("ticket/catalogApi")]
    public class CatalogApiController : ApiController
    {
        //
        //  GET: ticket/catalogApi/getProspectGroupSelect
        [HttpGet]
        [Authorize]
        [ActionName("getProspectGroupSelect")]
        public RequestResponseModel GetProspectGroupSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.ProspectGroup);
            return response;
        }
        //
        //  GET: ticket/catalogApi/getProspectStatuSelect
        [HttpGet]
        [Authorize]
        [ActionName("getProspectStatuSelect")]
        public RequestResponseModel GetProspectStatuSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.ProspectStatu);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getProspectPrintSelect
        [HttpGet]
        [Authorize]
        [ActionName("getProspectPrintSelect")]
        public RequestResponseModel GetProspectPrintSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.PrintTechnique);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getPriceTypeSelect
        [HttpGet]
        [Authorize]
        [ActionName("getPriceTypeSelect")]
        public RequestResponseModel GetPriceTypeSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.ProspectPrice);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getByFractionSelect
        [HttpGet]
        [Authorize]
        [ActionName("getByFractionSelect")]
        public RequestResponseModel GetByFractionSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.ByFraction);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getAwardTypeSelect
        [HttpGet]
        [Authorize]
        [ActionName("getAwardTypeSelect")]
        public RequestResponseModel GetAwardTypeSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.RaffleAwardType);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getRaffleStatuSelect
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleStatuSelect")]
        public RequestResponseModel GetRaffleStatuSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.RaffleStatus);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getRaffleCommoditySelect
        [HttpGet]
        [Authorize]
        [ActionName("getRaffleCommoditySelect")]
        public RequestResponseModel GetRaffleCommoditySelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.SolteoCommodity);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getAllocationTypeSelect
        [HttpGet]
        [Authorize]
        [ActionName("getAllocationTypeSelect")]
        public RequestResponseModel GetAllocationTypeSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.AllocationType);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getInvoiceConditionSelect
        [HttpGet]
        [Authorize]
        [ActionName("getInvoiceConditionSelect")]
        public RequestResponseModel GetInvoiceConditionSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.InvoiceCondition);
            return response;
        }
        //
        //  GET: ticket/catalogApi/getXpriedInvoiceTimeSelect
        [HttpGet]
        [Authorize]
        [ActionName("getXpriedInvoiceTimeSelect")]
        public RequestResponseModel GetXpriedInvoiceTimeSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.XpriedInvoiceTime);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getPaymentTypeSelect
        [HttpGet]
        [Authorize]
        [ActionName("getPaymentTypeSelect")]
        public RequestResponseModel GetPaymentTypeSelect()
        {
            var response = new CatalogModel().GetByGroupIdSelect((int)CatalogGroupEnum.PaymentType);
            return response;
        }

        //
        //  GET: ticket/catalogApi/getReprintDesing
        [HttpGet]
        [Authorize]
        [ActionName("getReprintDesing")]
        public RequestResponseModel GetReprintDesing()
        {
            var response = new CatalogModel().GetReprintDesing();
            return response;
        }

        //
        //  GET: ticket/catalogApi/getPrintDesing
        [HttpGet]
        [Authorize]
        [ActionName("getPrintDesing")]
        public RequestResponseModel GetPrintDesing()
        {
            var response = new CatalogModel().GetPrintDesing();
            return response;
        }
    }
}