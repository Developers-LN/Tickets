using Newtonsoft.Json;
using System;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models.Ticket
{
    public class ReassignModel
    {
        [JsonProperty(PropertyName = "clientId")]
        public int ClientId { get; set; }

        [JsonProperty(PropertyName = "allocationId")]
        public int AllocationId { get; set; }

        internal RequestResponseModel TicketAllocationReassing(ReassignModel model)
        {
            var context = new TicketsEntities();

            var allowcation = context.TicketAllocations.FirstOrDefault(a => a.Id == model.AllocationId);
            var oldClientId = allowcation.ClientId;
            if (allowcation == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "El ID de Asignación no existe"
                };
            }

            allowcation.ClientId = model.ClientId;
            context.SaveChanges();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Reasignación de Billetes", new
            {
                oldClientId = oldClientId,
                newClientId = allowcation.ClientId,
                allocationId = allowcation.Id,
                date = DateTime.Now.ToString()
            });

            return new RequestResponseModel()
            {
                Result = true,
                Message = "Asignación trasnferida correctamente!"
            };
        }

    }
}