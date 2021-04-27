using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketSuscriberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "clientDesc")]
        public string ClientDesc { get; set; }

        [JsonProperty(PropertyName = "fractionFrom")]
        public int FractionFrom { get; set; }

        [JsonProperty(PropertyName = "fractionTo")]
        public int FractionTo { get; set; }

        [JsonProperty(PropertyName = "ticketSuscriberNumbers")]
        public List<TicketSuscriberNumberModel> TicketSuscriberNumbers { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createDateLong")]
        public long CreateDateLong { get; set; }

        [JsonProperty(PropertyName = "createUser")]
        public int CreateUser { get; set; }

        [JsonProperty(PropertyName = "createUserDesc")]
        public string CreateUserDesc { get; set; }

        [JsonProperty(PropertyName = "numberCount")]
        public int NumberCount { get; set; }

        internal RequestResponseModel GetSuscriberList(int clientId = 0)
        {
            var context = new TicketsEntities();
            var tickets = context.TicketSuscribers.Where(t => t.Statu == (int)GeneralStatusEnum.Active
                && (t.ClientId == clientId || clientId == 0)).AsEnumerable().Select(t => this.ToObject(t)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Asignación de Billetes");

            return new RequestResponseModel()
            {
                Result = true,
                Object = tickets
            };
        }

        internal RequestResponseModel Verify(TicketSuscriberModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {

                    var duplicateNumbers = context.TicketSuscriberNumbers.AsEnumerable().Where(t =>
                        model.TicketSuscriberNumbers.Where(n => n.Number == t.Number).Any()).ToList();
                    string number = "";
                    foreach (var duplicateNumber in duplicateNumbers)
                    {
                        number += duplicateNumber.Number + ", ";
                    }

                    if (number != "")
                    {
                        number = number.Substring(0, number.Length - 2);

                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = "Los numeros ( " + number + " ) ya fueron abonado."
                        };
                    }
                    else
                    {
                        return new RequestResponseModel()
                        {
                            Result = true,
                            Message = "Nummero agregado correctamente"
                        };
                    }
                }
            }
        }

        internal RequestResponseModel Save(TicketSuscriberModel model)
        {
            using (var context = new TicketsEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {

                        var ticketNumberList = new List<TicketSuscriberNumber>();
                        if (model.Id == 0)
                        {
                            var ticketSuscriber = new TicketSuscriber()
                            {
                                ClientId = model.ClientId,
                                FractionFrom = 0,
                                FractionTo = 0,
                                Statu = (int)GeneralStatusEnum.Active,
                                CreateDate = DateTime.Now,
                                Createuser = WebSecurity.CurrentUserId
                            };
                            context.TicketSuscribers.Add(ticketSuscriber);
                            context.SaveChanges();

                            foreach (var number in model.TicketSuscriberNumbers)
                            {
                                var suscriberNumber = new TicketSuscriberNumber()
                                {
                                    Number = number.Number,
                                    TicketSuscriberId = ticketSuscriber.Id
                                };
                                ticketNumberList.Add(suscriberNumber);
                            }

                            context.TicketSuscriberNumbers.AddRange(ticketNumberList);
                            context.SaveChanges();
                        }
                        else
                        {
                            foreach (var number in model.TicketSuscriberNumbers)
                            {
                                var suscriberNumber = new TicketSuscriberNumber()
                                {
                                    Number = number.Number,
                                    TicketSuscriberId = model.Id
                                };
                                ticketNumberList.Add(suscriberNumber);
                            }

                            context.TicketSuscriberNumbers.AddRange(ticketNumberList);
                            context.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();

                        return new RequestResponseModel()
                        {
                            Result = false,
                            Message = e.Message
                        };
                    }
                }
            }
            Utils.SaveLog(WebSecurity.CurrentUserName, model.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Billete Abonado", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Abonado Guardado exitosamente."
            };
        }

        internal RequestResponseModel TicketSuscriberDelete(TicketSuscriberModel model)
        {
            var context = new TicketsEntities();
            var ticket = context.TicketSuscribers.FirstOrDefault(m => m.Id == model.Id);
            if (ticket == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "error al borrar abonado!"
                };
            }

            var suscriberNumbers = new List<TicketSuscriberNumber>();
            ticket.TicketSuscriberNumbers.ToList().ForEach(s => suscriberNumbers.Add(s));

            context.TicketSuscriberNumbers.RemoveRange(suscriberNumbers);
            context.SaveChanges();

            context.TicketSuscribers.Remove(ticket);
            context.SaveChanges();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Billete Abonado", model);

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Abonado borrado correctamente!"
            };
        }

        internal RequestResponseModel GetSuscriber(int id)
        {
            var context = new TicketsEntities();

            var ticket = context.TicketSuscribers.Where(t => t.Id == id).AsEnumerable()
                .Select(t => this.ToObject(t, true)).FirstOrDefault();
            if (ticket == null)
            {
                ticket = new TicketSuscriberModel()
                {
                    Id = 0,
                    TicketSuscriberNumbers = new List<TicketSuscriberNumberModel>()
                };
            }

            return new RequestResponseModel()
            {
                Result = true,
                Object = ticket
            };
        }

        internal TicketSuscriberModel ToObject(TicketSuscriber ticket, bool hasNumbers = false)
        {
            var context = new TicketsEntities();
            var model = new TicketSuscriberModel()
            {


                Id = ticket.Id,
                ClientId = ticket.ClientId,
                ClientDesc = context.Clients.FirstOrDefault(c => c.Id == ticket.ClientId).Name,
                FractionFrom = ticket.FractionFrom,
                FractionTo = ticket.FractionTo,
                TicketSuscriberNumbers = new List<TicketSuscriberNumberModel>(),
                CreateDate = ticket.CreateDate,
                CreateDateLong = ticket.CreateDate.ToUnixTime(),
                CreateUser = ticket.Createuser,
                CreateUserDesc = context.Users.FirstOrDefault(u => u.Id == ticket.Createuser).Name,
                NumberCount = ticket.TicketSuscriberNumbers.Count
            };
            if (hasNumbers == true)
            {
                var ticketSuscriberNumber = new TicketSuscriberNumberModel();
                model.TicketSuscriberNumbers = context.TicketSuscriberNumbers
                    .Where(ts => ts.TicketSuscriberId == ticket.Id).AsEnumerable().Select(t => ticketSuscriberNumber.ToObject(t)).ToList();
            }
            return model;
        }
    }
}