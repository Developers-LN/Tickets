using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        //
        //  GET: /Dashboard/AboutUs
        [Authorize]
        [HttpGet]
        public ActionResult AboutUs()
        {
            return View();
        }
        
        //
        //  GET: /Dashboard/infoAboutUs
        [Authorize]
        [HttpGet]
        public JsonResult infoAboutUs()
        {
            var json = new { 
                appName = "Comercialización de Billetes",
                version = "1.3",
                companyName = "Concentra SRL",
                publicDate = "15/02/2015",
                infoCompany = "Somos una empresa Consultora conformada por expertos de alto prestigio, calificación y capacidad para interactuar de manera interdisciplinaria en las distintas misiones que ejecuta. <br/>Concentra, cuenta con un equipo de especialistas con sólidos conocimientos y amplia experiencia en las más modernas tecnologías y técnicas de gestión, lo que nos permite ofrecer una amplia gama de servicios de consultoría en Tecnología de información.",
                team = new string[]{ "Wilson A. Hamilton", "Billy J. Taylor", "Juan O. Acosta", "Saul Coronado", "Luis Cedeño", "Jose Fermín"}
            };

            return new JsonResult()
            {
                Data = json,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        // GET: /Dashboard/GetClientListSelect
        [HttpGet]
        [Authorize]
        public JsonResult GetClientListSelect()
        {
            var context = new TicketsEntities();
            var clients = context.Clients.AsEnumerable()
                .Where(c => c.Statu != (int)GeneralStatusEnum.Suspended)
                .Select(c => new
                {
                    value = c.Id,
                    text = c.Name
                }).ToList<object>();

            return new JsonResult() { Data = new { clients }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /Dashboard/GetRaffleProduction
        [HttpGet]
        [Authorize]
        public JsonResult GetRaffleProduction(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.AsEnumerable().FirstOrDefault(r => r.Id == raffleId);
            var productions = raffle.TicketAllocations.AsEnumerable()
                .GroupBy(t=> t.Statu).Select(t => new
            {
                value = GetTiketAllocationCount(t.ToList()),
                color= GetColor(t.FirstOrDefault().Statu),
                highlight = "#48AB6C",
                label= context.Catalogs.AsEnumerable().FirstOrDefault( c => c.Id == t.FirstOrDefault().Statu).NameDetail
            }).ToList<object>();
            if (raffle.TicketReturns.Count > 0)
            {
                productions.Add(new
                {
                    value = raffle.TicketReturns.GroupBy(r => r.TicketAllocationNumber.Number).Count(),
                    color = "#4D5360",
                    highlight = "#48AB6C",
                    label = "Billetes Devueltos"
                });
            }
            /*int totalAllocate = GetTiketAllocationCount(raffle.TicketAllocations.ToList());
            productions.Add(new
            {
                value = raffle.Prospect.Production - totalAllocate,
                color = "#EFF3F9",
                highlight = "#48AB6C",
                label = "Billetes no Asignados"
            });*/
            return new JsonResult() { Data = new { productions }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        private int GetTiketAllocationCount(List<TicketAllocation> allocatinos)
        {
            int numberCount = 0;
            allocatinos.ForEach(a => numberCount += a.TicketAllocationNumbers.Count());
            return numberCount;
        }
        private string GetColor(int statu)
        {
            string color = "#FF5A5E";
            switch (statu)
            {
                case (int)AllocationStatuEnum.Created:
                    color = "#FF5A5E";
                    break;
                case (int)AllocationStatuEnum.Invoiced:
                    color = "#46BFBD";
                    break;
                case (int)AllocationStatuEnum.PendientPrint:
                    color = "#FDB45C";
                    break;
                case (int)AllocationStatuEnum.Printed:
                    color = "#949FB1";
                    break;
            }
            return color;
        }



        // GET: /Dashboard/GetRaffleAllocation
        [HttpGet]
        [Authorize]
        public JsonResult GetRaffleAllocation(int raffleId)
        {
            var context = new TicketsEntities();
            var raffle = context.Raffles.AsEnumerable().FirstOrDefault(r => r.Id == raffleId);
            var allocations = raffle.TicketAllocations.AsEnumerable()
                .GroupBy(t => t.ClientId).Select(t => new
                {
                    value = GetTiketAllocationCount(t.ToList()),
                    color = "",
                    highlight = "#48AB6C",
                    label = t.FirstOrDefault().Client.Name
                }).ToList<object>();
            Random randomGen = new Random();
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            List<string> colors = names.Select(n => n.ToString()).ToList();
            return new JsonResult() { Data = new { allocations, colors }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        /*GET: /Dashboard/SendMail
        [HttpGet]
        [AllowAnonymous]
        public string SendMail() {
            var path = HttpContext.Server.MapPath("~") + "email\\test.html";
            string html = System.IO.File.ReadAllText(path);
            var emails =  new List<string>() { "wilsonHamiltond@gmail.com" }.ToArray();

            var context = new TicketsEntities();
            var raffleAwards = context.RaffleAwards.Where(a => a.Award.TypesAwardId == 4 && a.RaffleId == 3744)
                .OrderBy( r=>r.Award.OrderAward)
                .OrderBy(r=>r.ControlNumber)
                .GroupBy(r=>r.AwardId)
                .Select( r=> new{ 
                    awardName = r.FirstOrDefault().Award.Description, 
                    numbers = r, 
                    value = r.FirstOrDefault().Award.Value
                }).OrderByDescending(r=>r.value);
            var s = "";
            foreach (var ra in raffleAwards){
                var d = "";

                foreach (var number in ra.numbers)
                {
                d += "<li>" + Tickets.Models.Enum.Utils.AddZeroToNumber(5, (int)number.ControlNumber) +"</li>";
                };



               s += "<div class='col-lg-12'>" +
               "<div class='col-lg-4 no-padder cash m-t-2-px'  style='width: 550px !important;'>" +
                   "<div class='col-lg-12 no-padder title color' style='margin-bottom:20px; margin-top:20px;font-size: 12pt;'>" +
                       "CON VALOR DE "+ ra.awardName+" CADA UNO </div>" +
                   "<div class='col-lg-12 no-padder'>" +
                       "<ul>" + d+ "</ul>" +
                   "</div>" +
               "</div>" +
           "</div>";
            }

            html += html.Replace("{{context}}", s);

            var result = Utils.SendMail("Prueba de correo", html, emails);
            return result;
        }*/
    }
}