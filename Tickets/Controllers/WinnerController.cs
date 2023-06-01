﻿using System;
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
    public class WinnerController : Controller
    {
        // GET: Winner
        [Authorize]
        [HttpGet]
        public ActionResult List()
        {
            return View();
        }

        // GET: /Winner/GetList
        [HttpGet]
        [Authorize]
        public JsonResult GetList()
        {
            var context = new TicketsEntities();
            var WinnerData = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.DocumentType || w.IdGroup == (int)CatalogGroupEnum.Gender).ToList();
            var winners = context.Winners.AsEnumerable().Select(c => new
            {
                c.Id,
                Name = c.WinnerName,
                c.DocumentNumber,
                c.Phone,
                Gender = WinnerData.FirstOrDefault(f => f.Id == c.GenderId).NameDetail,
                DocumentType = WinnerData.FirstOrDefault(f => f.Id == c.DocumentType).NameDetail
            }).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de ganadores");

            return new JsonResult() { Data = new { winners }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //
        // GET: /Winner/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        [HttpPost]
        [Authorize]
        public JsonResult Create(Winner winner)
        {
            if (winner.Id <= 0)
            {
                using (var context = new TicketsEntities())
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        try
                        {
                            winner.DocumentType = winner.DocumentType;
                            winner.DocumentNumber = winner.DocumentNumber;
                            winner.WinnerName = winner.WinnerName.ToUpper();
                            winner.Phone = winner.Phone;
                            winner.GenderId = winner.GenderId;
                            winner.CreateDate = DateTime.Now;
                            winner.CreateUser = WebSecurity.CurrentUserId;
                            context.Winners.Add(winner);
                            context.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            tx.Rollback();
                            return new JsonResult()
                            {
                                Data = new
                                {
                                    result = false,
                                    message = e.Message
                                }
                            };

                        }
                        Utils.SaveLog(WebSecurity.CurrentUserName, winner.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Ganador", WinnerToObject(winner));
                        tx.Commit();
                    }
                }
            }
            else
            {
                try
                {
                    using (var context = new TicketsEntities())
                    {
                        var dWinner = context.Winners.FirstOrDefault(c => c.Id == winner.Id);
                        dWinner.DocumentType = winner.DocumentType;
                        dWinner.DocumentNumber = winner.DocumentNumber;
                        dWinner.WinnerName = winner.WinnerName.ToUpper();
                        dWinner.Phone = winner.Phone;
                        dWinner.GenderId = winner.GenderId;
                        context.Entry(dWinner).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch (Exception)
                { }
                Utils.SaveLog(WebSecurity.CurrentUserName, winner.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Cliente", WinnerToObject(winner));
            }
            return new JsonResult() { Data = true };
        }

        private object WinnerToObject(Winner c)
        {
            var context = new TicketsEntities();
            var catalogs = context.Catalogs.Where(w => w.IdGroup == (int)CatalogGroupEnum.DocumentType || w.IdGroup == (int)CatalogGroupEnum.Gender).ToList();
            return new
            {
                c.Id,
                DocumentType = catalogs.FirstOrDefault(f => f.Id == c.DocumentType).NameDetail,
                c.DocumentNumber,
                Name = c.WinnerName,
                c.Phone,
                GenderId = catalogs.FirstOrDefault(f => f.Id == c.GenderId).NameDetail,
                c.CreateUser,
                c.CreateDate
            };
        }

        // GET: /Winner/GetWinnerData
        [HttpGet]
        [Authorize]
        public JsonResult GetWinnerData(int winnerId)
        {
            var context = new TicketsEntities();
            object winner = null;
            if (winnerId > 0)
            {
                winner = context.Winners.AsEnumerable().Where(a => a.Id == winnerId).Select(e => WinnerToObject(e)).FirstOrDefault();
            }
            var genders = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.Gender).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });

            var documentType = context.Catalogs.Where(c => c.IdGroup == (int)CatalogGroupEnum.DocumentType).Select(g => new
            {
                g.Id,
                Name = g.NameDetail
            });
            return new JsonResult() { Data = new { winner, genders, documentType }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}
