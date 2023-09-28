using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Tickets.Models.Clients;
using Tickets.Models.Enums;
using Tickets.Models.Prospects;
using Tickets.Models.Raffles;
using WebMatrix.WebData;

namespace Tickets.Models.Workflows
{
    public class WorkflowModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createDateLong")]
        public long CreateDateLong { get; set; }

        [JsonProperty(PropertyName = "approvedCount")]
        public int ApprovedCount { get; set; }

        [JsonProperty(PropertyName = "totalAproved")]
        public int TotalAproved { get; set; }

        [JsonProperty(PropertyName = "proccess")]
        public List<ProccessModel> Proccess { get; set; }

        [JsonProperty(PropertyName = "prospect")]
        public ProspectModel Prospect { get; set; }

        [JsonProperty(PropertyName = "client")]
        public ClientModel Client { get; set; }

        [JsonProperty(PropertyName = "reprint")]
        public TicketReprintModel Reprint { get; set; }

        [JsonProperty(PropertyName = "award")]
        public IdentifyNumberModel Award { get; set; }


        public class IdentifyNumberModel
        {
            [JsonProperty(PropertyName = "numberId")]
            public int NumberId { get; set; }

            [JsonProperty(PropertyName = "raffleDesc")]
            public string RaffleDesc { get; set; }

            [JsonProperty(PropertyName = "fractionFrom")]
            public int FractionFrom { get; set; }

            [JsonProperty(PropertyName = "fractionTo")]
            public int FractionTo { get; set; }

            [JsonProperty(PropertyName = "identifyBachId")]
            public int IdentifyBachId { get; set; }

            [JsonProperty(PropertyName = "clientName")]
            public string ClientName { get; set; }

            [JsonProperty(PropertyName = "number")]
            public long Number { get; set; }

            [JsonProperty(PropertyName = "fractions")]
            public int Fractions { get; set; }

            [JsonProperty(PropertyName = "awards")]
            public List<AwardCertModel> Awards { get; set; }


            internal IdentifyNumberModel ToObject(IdentifyNumber number)
            {
                var context = new TicketsEntities();
                var awards = context.RaffleAwards.Where(w => w.RaffleId == number.IdentifyBach.RaffleId && w.ControlNumber == number.TicketAllocationNumber.Number)
                    .Select(s => new AwardCertModel
                    {
                        Award = new AwardModel
                        {
                            ByFraction = s.Award.ByFraction,
                            Name = s.Award.Name,
                            SourceAward = s.Award.SourceAward,
                            Value = s.Award.ByFraction == (int)ByFractionEnum.S ? s.Award.Value : ((s.Award.Value / (s.Raffle.Prospect.LeafFraction * s.Raffle.Prospect.LeafNumber)) * (number.FractionTo - number.FractionFrom + 1)),
                            TypesAwardDesc = s.Award.TypesAward.Name
                        },
                        AwardId = s.AwardId,
                        ControlNumber = s.ControlNumber,
                        CreateDate = s.CreateDate,
                        CreateUser = s.CreateUser,
                        Fraction = s.Fraction,
                        Id = s.Id,
                        RaffleId = s.RaffleId,
                        RaffleAwardType = s.RaffleAwardType

                    }).ToList();

                bool wfrac = true;
                var fFrom = number.FractionFrom;
                var fTo = number.FractionTo;
                foreach (var a in awards.Where(w => w.Award.ByFraction == (int)ByFractionEnum.S))
                {
                    for (int i = fFrom; i < fTo; i++)
                    {
                        if (i == a.Fraction)
                        {
                            wfrac = true;
                            break;
                        }
                        else
                        {
                            wfrac = false;
                        }
                    }
                }
                if (wfrac == false)
                {
                    awards.RemoveAll(w => w.Award.ByFraction == (int)ByFractionEnum.S);
                }
                var model = new IdentifyNumberModel()
                {
                    NumberId = number.NumberId,
                    FractionFrom = number.FractionFrom,
                    FractionTo = number.FractionTo,
                    Fractions = number.FractionTo - number.FractionFrom + 1,
                    IdentifyBachId = number.IdentifyBach.Id,
                    ClientName = number.IdentifyBach.Client.Name,
                    RaffleDesc = (number.IdentifyBach.RaffleId + " - " + number.IdentifyBach.Raffle.Name).ToString(),
                    Number = number.TicketAllocationNumber.Number,
                    Awards = awards
                };
                return model;
            }
        }


        internal WorkflowModel ToObject(Workflow model, int type)
        {
            var context = new TicketsEntities();
            var wordflow = new WorkflowModel()
            {
                Id = model.Id,
                UserName = model.User.Name,
                CreateDate = model.CreateDate,
                CreateDateLong = model.CreateDate.ToUnixTime(),
                ApprovedCount = model.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Approved).Count()
                - model.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Rejected).Count(),
                TotalAproved = model.WorkflowType.WorkflowType_User.Where(wu => wu.Statu != (int)GeneralStatusEnum.Delete).Count(),
                Proccess = model.WorkflowProccesses.Select(p => new ProccessModel().ToObject(p)).OrderByDescending(p => p.CreateDate).ToList()
            };

            int prospectType = int.Parse(ConfigurationManager.AppSettings["ProspectApprovedWorkflowTypeId"].ToString());
            int clientType = int.Parse(ConfigurationManager.AppSettings["ClientApprovedWorkflowTypeId"].ToString());
            int reprintType = int.Parse(ConfigurationManager.AppSettings["ReprintApprovedWorkflowTypeId"].ToString());
            int awardCertType = 6;

            if (type == prospectType)
            {
                var prospectObject = new ProspectModel();
                wordflow.Prospect = prospectObject.ToObject(context.Prospects.FirstOrDefault(p => p.Id == model.ProcessId));
            }
            if (type == clientType)
            {
                var clientModel = new ClientModel();
                wordflow.Client = clientModel.ToObject(context.Clients.FirstOrDefault(p => p.Id == model.ProcessId));
            }
            if (type == reprintType)
            {
                var ticketReprintModel = new TicketReprintModel();
                wordflow.Reprint = ticketReprintModel.ToObject(context.TicketRePrints.FirstOrDefault(p => p.Id == model.ProcessId), true);
            }

            if (type == awardCertType)
            {
                var identifyNumberModel = new IdentifyNumberModel();
                wordflow.Award = identifyNumberModel.ToObject(context.IdentifyNumbers.FirstOrDefault(p => p.Id + (p.FractionTo - p.FractionFrom + 1) == model.ProcessId));
            }

            return wordflow;
        }

        internal RequestResponseModel GetWorkflowList(int workFlowTypeId)
        {
            var context = new TicketsEntities();
            int userId = WebSecurity.CurrentUserId;
            var usersApproveWorkflow = context.WorkflowTypes.FirstOrDefault(w => w.Id == workFlowTypeId).WorkflowType_User.Where(wu => wu.Statu != (int)GeneralStatusEnum.Delete).ToList();

            var workflowList = context.Workflows.AsEnumerable().Where(w =>
                w.WorkflowTypeId == workFlowTypeId &&
                w.Statu == (int)WorkflowStatusEnum.Active &&
                usersApproveWorkflow.Any(u => u.UserId == userId 
                    //&& u.OrderApproval == ((w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Approved).Count() - w.WorkflowProccesses.Where(wf => wf.Statu == (int)WorkflowProccessStatuEnum.Rejected).Count()) + 1)
                    )
                ).Select(w => new WorkflowModel().ToObject(w, workFlowTypeId)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Proceso de Aprobación tipo " + workFlowTypeId);

            return new RequestResponseModel()
            {
                Result = true,
                Object = workflowList
            };
        }

        internal RequestResponseModel GetWorkflow(int id)
        {
            var context = new TicketsEntities();
            var workflow = context.Workflows.AsEnumerable().Where(w => w.Id == id)
                .Select(w => new WorkflowModel().ToObject(w, w.WorkflowTypeId)).FirstOrDefault();
            if (workflow == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Object = "No existe el proceso."
                };
            }
            return new RequestResponseModel()
            {
                Result = true,
                Object = workflow
            };
        }

        internal RequestResponseModel SaveWorkflowProccess(ProccessModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int prospectTypeId = int.Parse(ConfigurationManager.AppSettings["ProspectApprovedWorkflowTypeId"].ToString());
                        int clientType = int.Parse(ConfigurationManager.AppSettings["ClientApprovedWorkflowTypeId"].ToString());
                        int reprintType = int.Parse(ConfigurationManager.AppSettings["ReprintApprovedWorkflowTypeId"].ToString());
                        int awardCertType = 6;

                        var proccess = new WorkflowProccess()
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            Comment = model.Comment,
                            Statu = model.Statu,
                            WorkFlowId = model.WorkflowId
                        };
                        context.WorkflowProccesses.Add(proccess);
                        context.SaveChanges();

                        var workFlow = context.Workflows.FirstOrDefault(w => w.Id == proccess.WorkFlowId);
                        int proccessApprovedCount = workFlow.WorkflowProccesses.Count(p => p.Statu == (int)WorkflowProccessStatuEnum.Approved);
                        int proccessRejectedCount = workFlow.WorkflowProccesses.Count(p => p.Statu == (int)WorkflowProccessStatuEnum.Rejected);
                        int totalProccess = workFlow.WorkflowType.WorkflowType_User.AsEnumerable().Where(w => w.Statu != (int)GeneralStatusEnum.Delete).Count();
                        if ((proccessApprovedCount - proccessRejectedCount) >= totalProccess)
                        {
                            workFlow.Statu = (int)WorkflowStatusEnum.Approved;
                            context.SaveChanges();
                            if (workFlow.WorkflowTypeId == prospectTypeId)
                            {
                                var prospect = context.Prospects.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                                prospect.Statu = (int)ProspectStatuEnum.Approved;
                                context.SaveChanges();
                            }
                            if (workFlow.WorkflowTypeId == awardCertType)
                            {
                                var award = context.IdentifyNumbers.FirstOrDefault(p => p.Id + (p.FractionTo - p.FractionFrom + 1) == workFlow.ProcessId);
                                award.Status = (int)AwardCertificationStatuEnum.Approved;
                                context.SaveChanges();
                            }
                            else if (workFlow.WorkflowTypeId == clientType)
                            {
                            }
                            else
                            {
                                var reprint = context.TicketRePrints.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                                if (reprint != null)
                                {
                                    reprint.Statu = (int)TicketReprintStatuEnum.Approved;
                                    context.SaveChanges();
                                }
                            }
                        }
                        else if ((proccessApprovedCount - proccessRejectedCount) < 0)
                        {
                            workFlow.Statu = (int)WorkflowStatusEnum.Rejected;
                            context.SaveChanges();
                            if (workFlow.WorkflowTypeId == prospectTypeId)
                            {
                                var prospect = context.Prospects.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                                prospect.Statu = (int)ProspectStatuEnum.Rejected;
                                context.SaveChanges();
                            }

                            if (workFlow.WorkflowTypeId == awardCertType)
                            {
                                var award = context.IdentifyNumbers.FirstOrDefault(p => p.Id + (p.FractionTo - p.FractionFrom + 1) == workFlow.ProcessId);
                                award.Status = (int)AwardCertificationStatuEnum.Rejected;
                                context.SaveChanges();
                            }
                            else if (workFlow.WorkflowTypeId == clientType)
                            {
                            }
                            else
                            {
                                var reprint = context.TicketRePrints.FirstOrDefault(p => p.Id == workFlow.ProcessId);
                                reprint.Statu = (int)TicketReprintStatuEnum.Rejected;
                                context.SaveChanges();
                            }
                        }
                        dbContextTransaction.Commit();


                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Insert, "Aprobación de flujo", new ProccessModel().ToObject(proccess));

                        return new RequestResponseModel()
                        {
                            Result = true,
                            Message = (model.Statu == (int)WorkflowProccessStatuEnum.Approved ? "Proceso de aprobaci&#243;n aprobado correctamente!" : "Proceso de aprobaci&#243;n rechazado correctamente!")
                        };
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
                                ex.Message,
                                ex.InnerException,
                                ex.StackTrace
                            }
                        };
                    }
                }
            }
        }

        internal RequestResponseModel SendProspectToWorkflow(ProspectModel model)
        {
            var workflowId = 0;
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var currentProspect = context.Prospects.FirstOrDefault(p => p.Id == model.Id);

                        currentProspect.Statu = (int)ProspectStatuEnum.Approving;
                        context.SaveChanges();

                        int workFlowTypeId = int.Parse(ConfigurationManager.AppSettings["ProspectApprovedWorkflowTypeId"].ToString());

                        Workflow workflow = context.Workflows.Where(w => w.ProcessId == currentProspect.Id && w.WorkflowTypeId == workFlowTypeId).FirstOrDefault();
                        if (workflow == null)
                        {
                            workflow = new Workflow()
                            {
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                ProcessId = model.Id,
                                WorkflowTypeId = workFlowTypeId,
                                Statu = (int)WorkflowStatusEnum.Active
                            };
                            context.Workflows.Add(workflow);
                        }
                        else
                        {
                            workflow.Statu = (int)WorkflowStatusEnum.Active;
                            var proccess = new WorkflowProccess()
                            {
                                Comment = "Reenviado a Aprobación",
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Statu = (int)WorkflowProccessStatuEnum.Approved,
                                WorkFlowId = workflow.Id
                            };
                            context.WorkflowProccesses.Add(proccess);
                        }
                        context.SaveChanges();
                        workflowId = workflow.Id;
                        Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Submitted, "Prospecto a aprovacción", model);
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
                                ex.Source,
                                ex.StackTrace
                            }
                        };
                    }
                }
            }
            /*try
            {
                SendProspectMail(workflowId);
            }
            catch { }*/

            return new RequestResponseModel()
            {
                Result = true,
                Message = "El prospecto fue enviado al flujo de aprobación correctamente!"
            };
        }

        internal RequestResponseModel SendReprintToWorkflow(TicketReprintModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var reprint = context.TicketRePrints.FirstOrDefault(p => p.Id == model.Id);

                        int workFlowTypeId = int.Parse(ConfigurationManager.AppSettings["ReprintApprovedWorkflowTypeId"].ToString());
                        var workflow = new Workflow()
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            ProcessId = model.Id,
                            WorkflowTypeId = workFlowTypeId,
                            Statu = (int)WorkflowStatusEnum.Active
                        };
                        context.Workflows.Add(workflow);
                        context.SaveChanges();

                        if (reprint.Statu == (int)TicketReprintStatuEnum.Rejected)
                        {
                            var proccess = new WorkflowProccess()
                            {
                                WorkFlowId = workflow.Id,
                                Comment = "Reenviado a aprobación",
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Statu = (int)WorkflowProccessStatuEnum.Approved
                            };
                            context.WorkflowProccesses.Add(proccess);
                            context.SaveChanges();
                        }

                        //reprint.Statu = (int)TicketReprintStatuEnum.Improcess;
                        reprint.Statu = (int)TicketReprintStatuEnum.Approved;
                        context.SaveChanges();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Submitted, "Cliente a aprovacción", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "El cliente fue enviado al flujo de aprobación correctamente!"
            };
        }

        internal RequestResponseModel SendAwardCertificationToWorkflow(IdentifyNumber model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var identifyNumber = context.IdentifyNumbers.FirstOrDefault(p => p.Id == model.Id);

                        int workFlowTypeId = 6;
                        var workflow = new Workflow()
                        {
                            CreateDate = DateTime.Now,
                            CreateUser = WebSecurity.CurrentUserId,
                            ProcessId = model.Id + (model.FractionTo - model.FractionFrom + 1),
                            WorkflowTypeId = workFlowTypeId,
                            Statu = (int)WorkflowStatusEnum.Active
                        };
                        context.Workflows.Add(workflow);
                        context.SaveChanges();

                        if (identifyNumber.Status == (int)AwardCertificationStatuEnum.Rejected)
                        {
                            var proccess = new WorkflowProccess()
                            {
                                WorkFlowId = workflow.Id,
                                Comment = "Reenviado a aprobación",
                                CreateDate = DateTime.Now,
                                CreateUser = WebSecurity.CurrentUserId,
                                Statu = (int)WorkflowProccessStatuEnum.Approved
                            };
                            context.WorkflowProccesses.Add(proccess);
                            context.SaveChanges();
                        }

                        identifyNumber.Status = (int)AwardCertificationStatuEnum.Improcess;
                        context.SaveChanges();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = ex.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Submitted, "Premio a aprobación", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "El premio fue enviado al flujo de aprobación correctamente!"
            };
        }
    }


}