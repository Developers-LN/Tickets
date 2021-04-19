using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tickets.Models.Prospects
{
    public class ProspectPriceModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "priceId")]
        public int PriceId { get; set; }

        [JsonProperty(PropertyName = "priceDesc")]
        public string PriceDesc { get; set; }

        [JsonProperty(PropertyName = "ticketPrice")]
        public decimal TicketPrice { get; set; }

        [JsonProperty(PropertyName = "factionPrice")]
        public decimal FactionPrice { get; set; }

        [JsonProperty(PropertyName = "seriePrice")]
        public decimal SeriePrice { get; set; }

        internal ProspectPriceModel ToObject(Prospect_Price prospectPrice)
        {
            var context = new TicketsEntities();
            var prospectPriceModel = new ProspectPriceModel()
            {
                Id = prospectPrice.Id,
                PriceId = prospectPrice.PriceId,
                PriceDesc = context.Catalogs.FirstOrDefault(c => c.Id == prospectPrice.PriceId).NameDetail,
                TicketPrice = prospectPrice.TicketPrice,
                SeriePrice = prospectPrice.SeriePrice,
                FactionPrice = prospectPrice.FactionPrice
            };

            return prospectPriceModel;
        }

        internal RequestResponseModel GetProspectPrice(int prospectId, int priceId)
        {
            var context = new TicketsEntities();
            var priceModel = new ProspectPriceModel();
            var price = context.Prospect_Price
                .Where(p => p.PriceId == priceId && p.ProspectId == prospectId).AsEnumerable()
                .Select(p => priceModel.ToObject(p)).FirstOrDefault();
            if (price == null)
            {
                return new RequestResponseModel()
                {
                    Result = false,
                    Message = "No se encontro un precio."
                };
            }
            return new RequestResponseModel()
            {
                Result = true,
                Object = price
            };
        }
    }
}
