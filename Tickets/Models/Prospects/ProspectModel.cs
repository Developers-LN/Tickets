using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.AuxModels;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Prospects
{
    public class ProspectModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "groupId")]
        public int GroupId { get; set; }

        [JsonProperty(PropertyName = "groupDesc")]
        public string GroupDesc { get; set; }

        [JsonProperty(PropertyName = "production")]
        public int Production { get; set; }

        [JsonProperty(PropertyName = "leafNumber")]
        public int LeafNumber { get; set; }

        [JsonProperty(PropertyName = "leafFraction")]
        public int LeafFraction { get; set; }

        [JsonProperty(PropertyName = "expirateDateLong")]
        public long ExpirateDateLong { get; set; }

        [JsonProperty(PropertyName = "expirateDate")]
        public DateTime ExpirateDate { get; set; }

        [JsonProperty(PropertyName = "maxReturnTickets")]
        public decimal MaxReturnTickets { get; set; }

        [JsonProperty(PropertyName = "impresionType")]
        public int ImpresionType { get; set; }

        [JsonProperty(PropertyName = "impresionTypeDesc")]
        public string ImpresionTypeDescription { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        [JsonProperty(PropertyName = "prospectType")]
        public int ProspectType { get; set; }

        [JsonProperty(PropertyName = "prospectTypeDesc")]
        public string ProspectTypeDesc { get; set; }

        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }

        [JsonProperty(PropertyName = "awards")]
        public List<AwardModel> Awards { get; set; }

        [JsonProperty(PropertyName = "prospectPrices")]
        public List<ProspectPriceModel> ProspectPrices { get; set; }

        internal ProspectModel ListadoProspecto(Prospect prospect)
        {
            var context = new TicketsEntities();
            var prospectModel = new ProspectModel()
            {
                Id = prospect.Id,
                Name = prospect.Name,
                Description = prospect.Description,
                GroupId = prospect.GroupId,
                GroupDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospect.GroupId).NameDetail,
                Production = prospect.Production,
                LeafNumber = prospect.LeafNumber,
                LeafFraction = prospect.LeafFraction,
                ExpirateDateLong = prospect.ExpirateDate.ToUnixTime(),
                ExpirateDate = prospect.ExpirateDate,
                MaxReturnTickets = prospect.MaxReturnTickets,
                ImpresionType = prospect.ImpresionType,
                ImpresionTypeDescription = context.Catalogs.FirstOrDefault(c => c.Id == prospect.ImpresionType).NameDetail,
                Statu = prospect.Statu,
                Price = prospect.Price,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospect.Statu).NameDetail
            };

            return prospectModel;
        }

        internal ProspectModel ToObject(Prospect prospect, bool hasAwards = true, bool hasPrices = true)
        {
            var context = new TicketsEntities();
            var awardModel = new AwardModel();
            var awardList = context.Awards.OrderBy(o => o.OrderAward).Where(w => w.ProspectId == prospect.Id).ToList();
            var prospectModel = new ProspectModel()
            {
                Id = prospect.Id,
                Name = prospect.Name,
                Description = prospect.Description,
                GroupId = prospect.GroupId,
                GroupDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospect.GroupId).NameDetail,
                Production = prospect.Production,
                LeafNumber = prospect.LeafNumber,
                LeafFraction = prospect.LeafFraction,
                ExpirateDateLong = prospect.ExpirateDate.ToUnixTime(),
                ExpirateDate = prospect.ExpirateDate,
                MaxReturnTickets = prospect.MaxReturnTickets,
                ImpresionType = prospect.ImpresionType,
                ImpresionTypeDescription = context.Catalogs.FirstOrDefault(c => c.Id == prospect.ImpresionType).NameDetail,
                Statu = prospect.Statu,
                //ProspectType = prospect.Type,
                //ProspectTypeDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospect.Type).NameDetail,
                Price = prospect.Price,
                StatuDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospect.Statu).NameDetail
            };
            if (hasAwards)
            {
                prospectModel.Awards = prospect.Awards.Select(a => new AwardModel().AwardToObject(a, awardList)).ToList();
            }
            if (hasPrices)
            {
                prospectModel.ProspectPrices = prospect.Prospect_Price.Select(pc => new ProspectPriceModel().ToObject(pc)).ToList();
            }
            return prospectModel;
        }

        internal RequestResponseModel GetProspect(int id)
        {
            var context = new TicketsEntities();
            var prospect = context.Prospects
                .Where(p => p.Id == id).AsEnumerable()
                .Select(p => this.ToObject(p)).FirstOrDefault();
            if (prospect == null)
            {
                prospect = new ProspectModel()
                {
                    Awards = new List<AwardModel>(),
                    ProspectPrices = new List<ProspectPriceModel>()
                };
            }
            return new RequestResponseModel()
            {
                Result = true,
                Object = prospect,
                Message = ""
            };
        }

        internal RequestResponseModel GetProspects(int statu = 0)
        {
            using (var context = new TicketsEntities())
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        var expiredProspects = context.Prospects.AsEnumerable()
                            .Where(p => p.Statu != (int)ProspectStatuEnum.Suspended
                                && p.ExpirateDate.Date < DateTime.Now.Date).ToList();
                        foreach (var p in expiredProspects)
                        {
                            p.Statu = (int)ProspectStatuEnum.Suspended;
                            context.SaveChanges();
                        }
                        tx.Commit();

                        if (statu == 0)
                        {
                            var prospects = context.Prospects.AsEnumerable()//.Where(p => p.Statu != 2074)
                                .Select(p => this.ListadoProspecto(p)).ToList();

                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Prospectos");
                            return new RequestResponseModel()
                            {
                                Result = true,
                                Object = prospects,
                                Message = ""
                            };
                        }
                        else
                        {
                            var prospects = context.Prospects.AsEnumerable().Where(p => p.Statu == statu)
                                .Select(p => this.ListadoProspecto(p)).ToList();

                            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Prospectos");
                            return new RequestResponseModel()
                            {
                                Result = true,
                                Object = prospects,
                                Message = ""
                            };
                        }
                    }
                    catch (Exception e)
                    {
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Object = new
                            {
                                e.InnerException,
                                e.StackTrace
                            },
                            Message = e.Message
                        };
                    }
                }
            }
        }

        internal RequestResponseModel SaveProspect(ProspectModel model)
        {
            Prospect prospect;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        #region Saving prospect data
                        if (model.Id <= 0)
                        {
                            prospect = new Prospect();
                            prospect.CreateDate = DateTime.Now;
                            prospect.CreateUser = WebSecurity.CurrentUserId;
                        }
                        else
                        {
                            prospect = context.Prospects.FirstOrDefault(u => u.Id == model.Id);

                            var awardForDelete = prospect.Awards.Where(a => model.Awards.Any(ap => ap.Id == a.Id) == false).ToList();
                            var prospectPricesDelete = prospect.Prospect_Price.Where(a => model.ProspectPrices.Any(ap => ap.Id == a.Id) == false).ToList();

                            foreach (var deleteAward in awardForDelete)
                            {
                                context.Awards.Remove(deleteAward);
                            }

                            foreach (var deletePrice in prospectPricesDelete)
                            {
                                context.Prospect_Price.Remove(deletePrice);
                            }
                        }

                        prospect.Name = model.Name;
                        prospect.Description = model.Description == null ? "" : model.Description;
                        prospect.GroupId = 25;
                        prospect.Production = model.Production;
                        prospect.LeafNumber = model.LeafNumber;
                        prospect.LeafFraction = model.LeafFraction;
                        prospect.ExpirateDate = model.ExpirateDate;
                        prospect.MaxReturnTickets = model.MaxReturnTickets;
                        prospect.ImpresionType = model.ImpresionType;
                        prospect.Price = model.Price;
                        prospect.Statu = model.Statu;
                        //prospect.Type = model.ProspectType;

                        if (model.Id <= 0)
                        {
                            context.Prospects.Add(prospect);
                        }
                        context.SaveChanges();
                        #endregion

                        #region Saving prices
                        model.ProspectPrices = model.ProspectPrices == null ? new List<ProspectPriceModel>() : model.ProspectPrices;
                        var prospectPrices = new List<Prospect_Price>();
                        foreach (var price in model.ProspectPrices)
                        {
                            var pedit = context.Prospect_Price.FirstOrDefault(p => p.Id == price.Id);
                            if (pedit == null)
                            {
                                var prospectPrice = new Prospect_Price()
                                {
                                    ProspectId = prospect.Id,
                                    PriceId = price.PriceId,
                                    FactionPrice = price.FactionPrice,
                                    TicketPrice = price.TicketPrice,
                                    SeriePrice = price.SeriePrice
                                };
                                prospectPrices.Add(prospectPrice);
                            }
                            else
                            {
                                pedit.PriceId = price.PriceId;
                                pedit.FactionPrice = price.FactionPrice;
                                pedit.TicketPrice = price.TicketPrice;
                                pedit.SeriePrice = price.SeriePrice;
                                context.SaveChanges();
                            }
                        }
                        context.Prospect_Price.AddRange(prospectPrices);
                        context.SaveChanges();
                        #endregion

                        #region Saveing awards
                        var createdAwards = new List<AuxAward>();
                        model.Awards = model.Awards == null ? new List<AwardModel>() : model.Awards;
                        foreach (var awardModel in model.Awards.OrderBy(a => a.OrderAward))
                        {
                            Award award;

                            if (awardModel.Id == 0)
                            {
                                award = new Award();
                                award.CreateDate = DateTime.Now;
                                award.CreateUser = WebSecurity.CurrentUserId;
                                award.ProspectId = prospect.Id;
                            }
                            else
                            {
                                award = context.Awards.FirstOrDefault(a => a.Id == awardModel.Id);
                            }

                            if (awardModel.SourceAwardDescription != null)
                            {
                                int? sourceAward = null;
                                awardModel.SourceAward = createdAwards.Any(a => a.ProspectId == prospect.Id && (a.Name == awardModel.SourceAwardDescription.ToUpper() || a.Id == awardModel.SourceAward)) ? createdAwards.FirstOrDefault(a => a.ProspectId == prospect.Id && (a.Name == awardModel.SourceAwardDescription.ToUpper() || a.Id == awardModel.SourceAward)).Id : sourceAward;
                            }

                            award.Name = awardModel.Name.ToUpper();
                            award.Description = awardModel.Description == null ? "" : awardModel.Description;
                            award.SourceAward = awardModel.SourceAward;
                            award.OrderAward = awardModel.OrderAward;
                            award.Quantity = awardModel.Quantity;
                            award.Terminal = awardModel.Terminal;
                            award.ByFraction = awardModel.ByFraction;
                            award.Value = awardModel.Value;
                            award.TotalValue = awardModel.Quantity * awardModel.Value;
                            award.TypesAwardId = awardModel.TypesAwardId;

                            if (awardModel.Id == 0)
                            {
                                context.Awards.Add(award);
                            }
                            context.SaveChanges();

                            var auxAward = new AuxAward()
                            {
                                Id = award.Id,
                                ProspectId = award.ProspectId,
                                OrderAward = award.OrderAward,
                                Name = award.Name,
                                Description = award.Description,
                                Quantity = award.Quantity,
                                SourceAward = award.SourceAward,
                                Terminal = award.Terminal,
                                Value = award.Value,
                                TotalValue = award.TotalValue,
                                ByFraction = award.ByFraction,
                                CreateDate = award.CreateDate,
                                CreateUser = award.CreateUser,
                                TypesAwardId = award.TypesAwardId
                            };

                            createdAwards.Add(auxAward);
                        }
                        #endregion

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = ex.Message,
                            Object = new
                            {
                                ex.InnerException,
                                ex.Message,
                                ex.StackTrace
                            }
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, model.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Prospecto", this.ToObject(prospect));

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Prospecto guardado correctamente."
            };
        }

        internal RequestResponseModel GetProspectSelect(int statu = 0)
        {
            var context = new TicketsEntities();
            var prospects = context.Prospects
                .AsEnumerable()
                .Where(p => (p.Statu == statu || (statu == 0 && p.Statu != (int)ProspectStatuEnum.Suspended)))
                .Select(p => new
                {
                    text = p.Name,
                    value = p.Id,
                    expirateDate = p.ExpirateDate.ToUnixTime()
                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = prospects,
                Message = ""
            };
        }

        internal RequestResponseModel GetTypeAwardSelect()
        {
            var context = new TicketsEntities();
            var awardTypes = context.TypesAwards
                .AsEnumerable()
                .Where(p => p.Status != (int)GeneralStatusEnum.Delete)
                .Select(p => new
                {
                    text = p.Name,
                    value = p.Id
                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = awardTypes
            };
        }

        private void SendProspectMail(int workflowId)
        {
            var context = new TicketsEntities();
            var workflow = context.Workflows.Where(wde => wde.Id == workflowId).FirstOrDefault();
            var usersApproveWorkflow = context.WorkflowTypes.FirstOrDefault(w => w.Id == workflow.WorkflowTypeId).WorkflowType_User.Where(wu => wu.Statu != (int)GeneralStatusEnum.Delete);
            int nextUsers = ((workflow.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Approved).Count()
                    - workflow.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Rejected).Count()) + 1);
            var mailList = usersApproveWorkflow.Where(u => u.OrderApproval == nextUsers)
                .Select(u => u.User.Employee.Email).ToArray();

            string htmlMail = "";// RenderRazorViewToString("WorkFlowProccessMail", workflow);
            Utils.SendMail("Notificación", htmlMail, mailList);

        }
    }
}
