using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class TicketSuscriberNumberModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int Number { get; set; }

        [JsonProperty(PropertyName = "ticketSuscriberId")]
        public int TicketSuscriberId { get; set; }

        internal TicketSuscriberNumberModel ToObject(TicketSuscriberNumber number)
        {
            var context = new TicketsEntities();
            var numbers = new TicketSuscriberNumberModel()
            {
                   Id = number.Id,
                   Number = number.Number,
                   TicketSuscriberId = number.TicketSuscriberId

            };
            return numbers; 
        }

        internal RequestResponseModel SuscriberNumberDelete(TicketSuscriberNumberModel model)
        {
            var context = new TicketsEntities();
            var suscriberNumber = context.TicketSuscriberNumbers.FirstOrDefault(s => s.Id == model.Id);
            if (suscriberNumber == null) 
            {
                return new RequestResponseModel() { 
                    Result = false, 
                    Message = "Error borrando número abonado!" 
                };
            }
            context.TicketSuscriberNumbers.Remove(suscriberNumber);
            context.SaveChanges();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Borrando Numero abonado.", model);
            return new RequestResponseModel() { 
                Result = true, 
                Message = "Número abonado borrado correctamente!" 
            };
        }
    }
}
