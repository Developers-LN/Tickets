using System;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using Tickets.Models.Procedures;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ElectronicSalesController : Controller
    {
        // GET: ElectronicSales
        [Authorize]
        [HttpGet]
        public ActionResult ElectronicSalesList()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ElectronicSalesGroupDetails()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult ElectronicSalesDetails()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public JsonResult getElectronicSalesDetails(int allocationId)
        {
            var context = new TicketsEntities();
            var Allocation = context.TicketAllocations.FirstOrDefault(f => f.Id == allocationId);
            var ticketPrice = context.Prospect_Price.FirstOrDefault(w => w.ProspectId == Allocation.Raffle.Prospect.Id && w.PriceId == Allocation.Client.PriceId).TicketPrice;

            if (Allocation.Client.GroupId == (int)ClientGroupEnum.DistribuidorXML)
            {
                if (context.ElectronicTicketSales.Any(a => a.TicketAllocationId == allocationId))
                {
                    var ElectronicTicketsList = context.ElectronicTicketSales.Where(w => w.TicketAllocationId == allocationId).ToList();

                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            allocationId = allocationId,
                            allocationSequence = Allocation.AllocationSequence,
                            clientName = Allocation.Client.Name,
                            clientId = Allocation.Client.Id,
                            clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                            clientDiscount = Allocation.Client.Discount,
                            allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                            raffleId = Allocation.Raffle.Id,
                            raffleName = Allocation.Raffle.Name,
                            electronicSalesByDate = ElectronicTicketsList.GroupBy(g => g.SaleDate.ToShortDateString()).Select(s => new
                            {
                                date = s.Key,
                                totalSold = s.Count(),
                                amount = (s.Count() * ticketPrice) - ((s.Count() * ticketPrice) * (Allocation.Client.Discount / 100))
                            }).ToList()
                        }
                    };
                }
                else
                {
                    return new JsonResult()
                    {
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        Data = new
                        {
                            allocationId = allocationId,
                            allocationSequence = Allocation.AllocationSequence,
                            clientName = Allocation.Client.Name,
                            clientId = Allocation.Client.Id,
                            clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                            clientDiscount = Allocation.Client.Discount,
                            allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                            raffleId = Allocation.Raffle.Id,
                            raffleName = Allocation.Raffle.Name,
                            electronicSalesByDate = 0
                        }
                    };
                }
            }
            else
            {
                var ElectronicTicketsList = context.TicketAllocationNumbers.Where(w => w.TicketAllocationId == allocationId).ToList();

                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        allocationId = allocationId,
                        allocationSequence = Allocation.AllocationSequence,
                        clientName = Allocation.Client.Name,
                        clientId = Allocation.Client.Id,
                        clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                        clientDiscount = Allocation.Client.Discount,
                        allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                        raffleId = Allocation.Raffle.Id,
                        raffleName = Allocation.Raffle.Name,
                        electronicSalesByDate = ElectronicTicketsList.GroupBy(g => g.PrintedDate.Value.ToShortDateString()).Select(s => new
                        {
                            date = s.Key,
                            totalSold = s.Count()
                        }).ToList()
                    }
                };
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult ElectronicSalesValidate(int AllocationId)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var CurrentUser = WebSecurity.CurrentUserId;
                        Procedure_ValidateElectronicSales validateElectronicSalesProcedure = new Procedure_ValidateElectronicSales();
                        var Resultado = validateElectronicSalesProcedure.ValidateElectronicSales(AllocationId, CurrentUser);

                        if (Resultado.FirstOrDefault().Statu == 1)
                        {
                            context.SaveChanges();
                            tx.Commit();
                        }
                        else
                        {
                            tx.Rollback();
                            return new JsonResult() { Data = new { result = false, message = "Error al intentar validar las ventas electrónicas" } };
                        }
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        return new JsonResult() { Data = new { result = false, message = e.Message } };
                    }
                }
            }
            return new JsonResult() { Data = new { result = true, message = "Ventas electrónicas guardadas correctamente!" } };
        }
    }
}
