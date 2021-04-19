using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tickets.Models.Enums;

namespace Tickets.Models.Clients
{
    public class ClientModel
    {
        internal ClientModel ToObject(Client client)
        {
            var model = new ClientModel()
            {
            };
            return model;
        }

        internal RequestResponseModel GetClientSelect(int statu)
        {
            var context = new TicketsEntities();
            var clients = context.Clients.AsEnumerable()
                .Where(r => r.Statu == statu ||( statu == 0 && r.Statu != (int)ClientStatuEnum.Suspended))
                .OrderByDescending(r => r.CreateDate)
                .Select(c => new
                {
                    value = c.Id,
                    text = c.Name,
                    priceId = c.PriceId,
                    discount = c.Discount
                }).ToList();

            return new RequestResponseModel()
            {
                Result = true,
                Object = clients
            };
        }
    }
}