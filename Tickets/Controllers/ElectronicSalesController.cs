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
                            sequenceNumberTicketAllocation = Allocation.SequenceNumber,
                            clientName = Allocation.Client.Name,
                            clientId = Allocation.Client.Id,
                            clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                            clientDiscount = Allocation.Client.Discount,
                            allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                            raffleId = Allocation.Raffle.Id,
                            //raffleName = Allocation.Raffle.Name,
                            sequenceNumberRaffle = Allocation.Raffle.SequenceNumber,
                            raffleName = Allocation.Raffle.Symbol + Allocation.Raffle.Separator + Allocation.Raffle.SequenceNumber + " " + Allocation.Raffle.Name + " " + Allocation.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
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
                            sequenceNumberTicketAllocation = Allocation.SequenceNumber,
                            clientName = Allocation.Client.Name,
                            clientId = Allocation.Client.Id,
                            clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                            clientDiscount = Allocation.Client.Discount,
                            allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                            raffleId = Allocation.Raffle.Id,
                            sequenceNumberRaffle = Allocation.Raffle.SequenceNumber,
                            raffleNomenclature = Allocation.Raffle.Symbol + Allocation.Raffle.Separator + Allocation.Raffle.SequenceNumber,
                            //raffleName = Allocation.Raffle.Name,
                            raffleName = Allocation.Raffle.Symbol + Allocation.Raffle.Separator + Allocation.Raffle.SequenceNumber + " " + Allocation.Raffle.Name + " " + Allocation.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                            electronicSalesByDate = 0
                        }
                    };
                }
            }
            else
            {
                var ElectronicTicketsListFirst = context.TicketAllocationNumbers.Select(s => new
                {
                    s.TicketAllocationId,
                    s.PrintedDate,
                    s.FractionFrom,
                    s.FractionTo,
                    s.Statu
                }).Where(w => w.TicketAllocationId == allocationId).ToList();

                var ElectronicTicketsList = ElectronicTicketsListFirst.Select(s => new
                {
                    s.TicketAllocationId,
                    PrintedDate = s.PrintedDate.Value.ToShortDateString(),
                    s.FractionFrom,
                    s.FractionTo,
                    s.Statu
                }).Where(w => w.TicketAllocationId == allocationId).ToList();

                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        allocationId = allocationId,
                        sequenceNumberTicketAllocation = Allocation.SequenceNumber,
                        clientName = Allocation.Client.Name,
                        clientId = Allocation.Client.Id,
                        clientType = context.Catalogs.FirstOrDefault(f => f.Id == Allocation.Client.GroupId).NameDetail,
                        clientDiscount = Allocation.Client.Discount,
                        allocateDate = Allocation.CreateDate.ToString("dd/MM/yyyy"),
                        raffleId = Allocation.Raffle.Id,
                        sequenceNumberRaffle = Allocation.Raffle.SequenceNumber,
                        //raffleName = Allocation.Raffle.Name,
                        raffleNomenclature = Allocation.Raffle.Symbol + Allocation.Raffle.Separator + Allocation.Raffle.SequenceNumber,
                        raffleName = Allocation.Raffle.Symbol + Allocation.Raffle.Separator + Allocation.Raffle.SequenceNumber + " " + Allocation.Raffle.Name + " " + Allocation.Raffle.DateSolteo.ToString("dd/MM/yyyy"),
                        electronicSalesByDate = ElectronicTicketsList.GroupBy(g => g.PrintedDate).Select(s => new
                        {
                            date = s.Key,
                            ticketsQuantity = ElectronicTicketsList.Where(w => w.PrintedDate == s.Key).Sum(ss => ss.FractionTo - ss.FractionFrom + 1) / (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber),
                            fractionQuantity = ElectronicTicketsList.Where(w => w.PrintedDate == s.Key).Sum(ss => ss.FractionTo - ss.FractionFrom + 1) % (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber),
                            allocationFractionQuantity = ElectronicTicketsList.Any(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu != (int)TicketStatusEnum.Anulated) ? ElectronicTicketsList.Where(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu != (int)TicketStatusEnum.Anulated).Select(ss => ss.FractionTo - ss.FractionFrom + 1).Sum() % (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber) : 0,
                            allocationTicketsQuantity = ElectronicTicketsList.Any(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu != (int)TicketStatusEnum.Anulated) ? ElectronicTicketsList.Where(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu != (int)TicketStatusEnum.Anulated).Select(ss => ss.FractionTo - ss.FractionFrom + 1).Sum() / (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber) : 0,
                            totalRest = ElectronicTicketsList.Any(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu == (int)TicketStatusEnum.Anulated) ? ElectronicTicketsList.Where(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu == (int)TicketStatusEnum.Anulated).Select(ss => ss.FractionTo - ss.FractionFrom + 1).Sum() % (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber) : 0,
                            totalRestTickets = ElectronicTicketsList.Any(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu == (int)TicketStatusEnum.Anulated) ? ElectronicTicketsList.Where(w => w.PrintedDate == s.Key && w.TicketAllocationId == allocationId && w.Statu == (int)TicketStatusEnum.Anulated).Select(ss => ss.FractionTo - ss.FractionFrom + 1).Sum() / (Allocation.Raffle.Prospect.LeafFraction * Allocation.Raffle.Prospect.LeafNumber) : 0
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
