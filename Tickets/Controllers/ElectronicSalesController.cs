using System;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
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
                        ValidateElectronicSalesProcedure validateElectronicSalesProcedure = new ValidateElectronicSalesProcedure();
                        var Resultado = validateElectronicSalesProcedure.ValidateElectronicSales(AllocationId, CurrentUser);

                        if(Resultado.FirstOrDefault().Statu == 1)
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
