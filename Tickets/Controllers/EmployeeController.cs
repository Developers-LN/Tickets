using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class EmployeeController : Controller
    {

        //
        // GET: /Employee/List
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        // GET: /Employee/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            try
            {
                var context = new TicketsEntities();
                var employees = context.Employees.AsEnumerable().Where(p => p.Statu != (int)GeneralStatusEnum.Delete).Select(e => EmployeeToObject(e)).ToList();
                var provinces = context.Provinces.Select(p => new
                {
                    p.Id,
                    p.Name,
                    Towns = p.Towns.Select(t => new
                    {
                        t.Id,
                        t.Name,
                        DistTowns = t.DistTowns.Select(dt => new
                        {
                            dt.Id,
                            dt.Name
                        })
                    })
                });
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Empleados");

                return new JsonResult() { Data = new { result = true, employees = employees, provinces = provinces }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception e)
            {
                return new JsonResult() { Data = new { result = false, message = e.Message }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

        //
        // GET: /Employee/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [Authorize]
        public JsonResult Create(Employee employee)
        {
            using (TicketsEntities context = new TicketsEntities())
            {
                try
                {


                    if (employee.Id <= 0)
                    {
                        employee.CreateDate = DateTime.Now;
                        employee.CreateUser = WebSecurity.CurrentUserId;
                        context.Employees.Add(employee);
                    }
                    else
                    {
                        var mEmployee = context.Employees.FirstOrDefault(c => c.Id == employee.Id);

                        mEmployee.Name = employee.Name;
                        mEmployee.LastName = employee.LastName;
                        mEmployee.DocumentNumber = employee.DocumentNumber;
                        mEmployee.MaritalStatus = employee.MaritalStatus;
                        mEmployee.Gender = employee.Gender;
                        mEmployee.Birthday = employee.Birthday;
                        mEmployee.Province = employee.Province;
                        mEmployee.Section = employee.Section;
                        mEmployee.Town = employee.Town;
                        mEmployee.Addres = employee.Addres;
                        mEmployee.Phone = employee.Phone;
                        mEmployee.Email = employee.Email;
                        mEmployee.Department = employee.Department;
                        mEmployee.Office = employee.Office;
                        mEmployee.GroupId = employee.GroupId;
                        mEmployee.Statu = employee.Statu;
                        mEmployee.Comment = employee.Comment;
                        mEmployee.AgencyId = employee.AgencyId;
                    }
                    context.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var error in ex.EntityValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("entity of type {0} in state {1} has the following validations errors:",
                            error.Entry.Entity.GetType(), error.Entry.State));
                        foreach (var e in error.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                e.PropertyName, e.ErrorMessage);
                        }
                    }

                }

            }
            Utils.SaveLog(WebSecurity.CurrentUserName, employee.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Empleado", EmployeeToObject(employee));
            return new JsonResult() { Data = true };
        }

        // POST: Employee/Delete
        [HttpPost]
        [Authorize]
        public JsonResult Delete(int employeeId)
        {
            var context = new TicketsEntities();
            var employee = context.Employees.FirstOrDefault(m => m.Id == employeeId);
            if (employee != null)
            {
                employee.Statu = (int)GeneralStatusEnum.Delete;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Empleado", EmployeeToObject(employee));
            }
            return new JsonResult() { Data = true };
        }

        private object EmployeeToObject(Employee employee)
        {
            var context = new TicketsEntities();
            var catalogs = context.Catalogs.ToList();
            var departament = catalogs.FirstOrDefault(ct => ct.Id == employee.Department);
            var section = context.DistTowns.FirstOrDefault(dt => dt.Id == employee.Section);
            var agency = context.Agencies.FirstOrDefault(ag => ag.Id == employee.AgencyId);
            return new
            {
                employee.Id,
                employee.Name,
                employee.LastName,
                employee.DocumentNumber,
                employee.MaritalStatus,
                MaritalStatusDesc = catalogs.FirstOrDefault(ct => ct.Id == employee.MaritalStatus).NameDetail,
                employee.Gender,
                employee.AgencyId,
                AgencyDesc = agency == null ? "" : agency.Name,
                GenderDesc = catalogs.FirstOrDefault(ct => ct.Id == employee.Gender).NameDetail,
                Birthday = employee.Birthday.ToShortDateString(),
                employee.Province,
                ProvinceDesc = context.Provinces.FirstOrDefault(p => p.Id == employee.Province).Name,
                employee.Section,
                SectionDesc = section == null ? "" : section.Name,
                employee.Town,
                TownDesc = context.Towns.FirstOrDefault(t => t.Id == employee.Town).Name,
                employee.Addres,
                employee.Phone,
                employee.Email,
                employee.Department,
                DepartmentDesc = departament == null ? "" : departament.NameDetail,
                employee.Office,
                OfficeDesc = catalogs.FirstOrDefault(ct => ct.Id == employee.Office).NameDetail,
                employee.GroupId,
                GroupDesc = catalogs.FirstOrDefault(ct => ct.Id == employee.GroupId).NameDetail,
                employee.Comment,
                employee.Statu,
                StatuDesc = catalogs.FirstOrDefault(ct => ct.Id == employee.Statu).NameDetail,
                CreateDate = employee.CreateDate.ToString(),
                employee.CreateUser
            };
        }

        #region Department
        //
        // GET: /Employee/DepartmentList
        [Authorize]
        [HttpGet]
        public ActionResult DepartmentList()
        {
            return View();
        }

        // GET: /Employee/GetDepartmentList
        [HttpGet]
        [Authorize]
        public JsonResult GetDepartmentList()
        {
            var context = new TicketsEntities();
            var departmentGroupName = "TicketsDepartmentCatalog";
            var catalogs = context.Catalogs.AsEnumerable().Where(c => c.Statu == true && c.NameGroup == departmentGroupName).Select(u => CatalogToObject(u)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Departamentos");

            return new JsonResult() { Data = catalogs, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Employee/DepartmentCreate
        [HttpGet]
        [Authorize]
        public ActionResult DepartmentCreate()
        {
            return View();
        }

        // POST: Employee/DepartmentCreate
        [HttpPost]
        [Authorize]
        public JsonResult DepartmentCreate(Catalog departament)
        {
            departament.NameGroup = "TicketsDepartmentCatalog";
            var context = new TicketsEntities();
            if (departament.Id <= 0)
            {
                var item = context.Catalogs.Where(c => c.NameGroup == departament.NameGroup);
                if (item.Any())
                {
                    departament.IdGroup = item.FirstOrDefault().IdGroup;
                    departament.IdDetail = item.OrderByDescending(i => i.IdDetail).FirstOrDefault().IdDetail + 1;
                }
                else
                {
                    departament.IdDetail = 1;
                    var groupList = context.Catalogs.Select(c => c.IdGroup);
                    if (groupList.Any())
                    {
                        departament.IdGroup = groupList.OrderByDescending(g => g).FirstOrDefault() + 1;
                    }
                    else
                    {
                        departament.IdGroup = 1;
                    }
                }
                context.Catalogs.Add(departament);
            }
            else
            {
                var modifyCatalog = context.Catalogs.FirstOrDefault(u => u.Id == departament.Id);
                modifyCatalog.NameDetail = departament.NameDetail;
                modifyCatalog.Statu = departament.Statu;
                modifyCatalog.Description = departament.Description;
            }
            context.SaveChanges();


            Utils.SaveLog(WebSecurity.CurrentUserName, departament.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Departamento", CatalogToObject(departament));
            return new JsonResult() { Data = true };
        }

        // POST: Employee/DepartmentDelete
        [HttpPost]
        [Authorize]
        public JsonResult DepartmentDelete(int departmentId)
        {
            var context = new TicketsEntities();
            var catalog = context.Catalogs.FirstOrDefault(m => m.Id == departmentId);
            if (catalog != null)
            {
                catalog.Statu = false;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Departamento", CatalogToObject(catalog));
            }
            return new JsonResult() { Data = true };
        }

        private object CatalogToObject(Catalog departament)
        {
            var obj = new
            {
                departament.Id,
                departament.IdGroup,
                departament.NameGroup,
                departament.IdDetail,
                departament.NameDetail,
                departament.Description,
                departament.Statu,
                StatuDesc = departament.Statu ? "Activo" : "In activo"
            };
            return obj;
        }
        #endregion
    }
}