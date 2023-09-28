using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class MoveReturnedGroupModel
    {
        [JsonProperty(PropertyName = "returnedId")]
        public int ReturnedId { get; set; }

        [JsonProperty(PropertyName = "currentGroup")]
        public string CurrentGroup { get; set; }

        [JsonProperty(PropertyName = "group")]
        public string Group { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "raffleId")]
        public int RaffleId { get; set; }

        internal RequestResponseModel MoveReturnedGroup(MoveReturnedGroupModel model)
        {
            List<object> returnedsObject;
            using (var context = new TicketsEntities())
            {

                using (var tm = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (model.ReturnedId > 0)
                        {
                            var returned = context.TicketReturns.FirstOrDefault(r => r.Id == model.ReturnedId);


                            returnedsObject = new List<object>(){new {
                                returned.ReturnedDate,
                                returned.Id,
                                returned.ClientId,
                                returned.CreateDate,
                                returned.CreateUser,
                                returned.FractionFrom,
                                returned.FractionTo,
                                returned.RaffleId,
                                returned.Raffle.RaffleSequence,
                                returned.ReturnedGroup,
                                returned.Statu,
                                returned.TicketAllocationNimberId
                            }};
                            returned.ReturnedGroup = model.Group;
                            returned.ClientId = model.ClientId;
                        }
                        else
                        {
                            var returneds = context.TicketReturns.Where(r => r.ReturnedGroup == model.CurrentGroup && r.RaffleId == model.RaffleId);
                            returnedsObject = returneds.Select(g => new
                            {
                                g.ReturnedDate,
                                g.Id,
                                g.ClientId,
                                g.CreateDate,
                                g.CreateUser,
                                g.FractionFrom,
                                g.FractionTo,
                                g.RaffleId,
                                g.Raffle.RaffleSequence,
                                g.ReturnedGroup,
                                g.Statu,
                                g.TicketAllocationNimberId
                            }).ToList<object>();
                            foreach (var returned in returneds)
                            {
                                returned.ReturnedGroup = model.Group;
                                returned.ClientId = model.ClientId;
                            }
                        }

                        context.SaveChanges();
                        tm.Commit();
                    }
                    catch (Exception e)
                    {
                        tm.Rollback();

                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message
                        };
                    }
                }
            }

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Movimiento de Grupo", returnedsObject);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Grupo movido correctamente"
            };
        }

    }
}