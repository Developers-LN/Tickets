using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Filters;
using Tickets.Models;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AgencyController : Controller
    {
        //
        // GET: /Agency/List
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        // GET: /Agency/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var agencys = context.Agencies.AsEnumerable().Where(p => p.Statu != (int)GeneralStatusEnum.Delete).Select(e => AgencyToObject(e)).ToList();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Agencias");

            return new JsonResult() { Data = new { agencys = agencys}, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: /Agency/GetAgencyData
        [HttpGet]
        [Authorize]
        public JsonResult GetAgencyData(int agencyId)
        {
            var context = new TicketsEntities();
            object agency = null;
            if (agencyId > 0)
            {
                agency = context.Agencies.AsEnumerable().Where(a => a.Id == agencyId).Select(e => AgencyToObject(e)).FirstOrDefault();
            }
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
            var employees = context.Employees.Where(e => e.Statu == (int)GeneralStatusEnum.Active).Select(e => new
            {
                e.Id,
                Name = e.Name + " " + e.LastName
            });
            var groups = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.ClientGroup).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            var generalStatus = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.GeneralStatus).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            return new JsonResult() { Data = new { agency, provinces, employees, groups, generalStatus }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Agency/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agency/Create
        [HttpPost]
        [Authorize]
        public JsonResult Create(Agency agency)
        {
            var context = new TicketsEntities();
            if (agency.Id <= 0)
            {
                agency.CreateDate = DateTime.Now;
                agency.CreateUser = WebSecurity.CurrentUserId;
                context.Agencies.Add(agency);
            }
            else
            {
                var mAgency = context.Agencies.FirstOrDefault(c => c.Id == agency.Id);

                mAgency.Name = agency.Name;
                mAgency.Description = agency.Description;
                mAgency.EmployeeId = agency.EmployeeId;
                mAgency.GroupId = agency.GroupId;
                mAgency.Province = agency.Province;
                mAgency.Section = agency.Section;
                mAgency.Town = agency.Town;
                mAgency.Addres = agency.Addres;
                mAgency.Email = agency.Email;
                mAgency.Fax = agency.Fax;
                mAgency.Phone = agency.Phone;
                mAgency.IntDate = agency.IntDate;
                mAgency.Statu = agency.Statu;
            }
            context.SaveChanges(); 
            Utils.SaveLog(WebSecurity.CurrentUserName, agency.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Agencia", AgencyToObject(agency));
            return new JsonResult() { Data = true };
        }

        // POST: Agency/Delete
        [HttpPost]
        [Authorize]
        public JsonResult Delete(int agencyId)
        {
            var context = new TicketsEntities();
            var agency = context.Agencies.FirstOrDefault(m => m.Id == agencyId);
            if (agency != null)
            {
                agency.Statu = (int)GeneralStatusEnum.Delete;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Agencia", AgencyToObject(agency));
            }
            return new JsonResult() { Data = true };
        }

        public object AgencyToObject(Agency agency)
        {
            var context = new TicketsEntities();
            var catalogs = context.Catalogs.ToList();
            return new
            {
                agency.Id,
                agency.Name,
                agency.Description,
                agency.EmployeeId,
                EmployeeDesc = context.Employees.FirstOrDefault(ct => ct.Id == agency.EmployeeId).Name + " " + context.Employees.FirstOrDefault(ct => ct.Id == agency.EmployeeId).LastName,
                agency.GroupId,
                GroupDesc = catalogs.FirstOrDefault(ct => ct.Id == agency.GroupId).NameDetail,
                agency.Province,
                ProvinceDesc = context.Provinces.FirstOrDefault(p => p.Id == agency.Province).Name,
                agency.Section,
                SectionDesc = context.DistTowns.FirstOrDefault(dt => dt.Id == agency.Section).Name,
                agency.Town,
                TownDesc = context.Towns.FirstOrDefault(t => t.Id == agency.Town).Name,
                agency.Addres,
                agency.Phone,
                agency.Email,
                agency.Fax,
                IntDate = agency.IntDate.ToUnixTime(),
                agency.Statu,
                StatuDesc = catalogs.FirstOrDefault(ct => ct.Id == agency.Statu).NameDetail,
                CreateDate = agency.CreateDate.ToString(),
                agency.CreateUser
            };
        }

	}
}