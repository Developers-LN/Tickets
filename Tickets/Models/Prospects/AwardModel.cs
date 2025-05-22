using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Tickets.Models.Prospects
{
    public class AwardModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "nameAux")]
        public string NameAux { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "orderAward")]
        public int OrderAward { get; set; }

        [JsonProperty(PropertyName = "byFraction")]
        public int ByFraction { get; set; }

        [JsonProperty(PropertyName = "byFractionDesc")]
        public string ByFractionDesc { get; set; }

        [JsonProperty(PropertyName = "prospectId")]
        public int ProspectId { get; set; }

        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "sourceAward")]
        public int? SourceAward { get; set; }

        [JsonProperty(PropertyName = "sourceAwardDescription")]
        public string SourceAwardDescription { get; set; }

        [JsonProperty(PropertyName = "terminal")]
        public int? Terminal { get; set; }

        [JsonProperty(PropertyName = "totalValue")]
        public decimal TotalValue { get; set; }

        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "typesAwardId")]
        public int TypesAwardId { get; set; }

        [JsonProperty(PropertyName = "typesAwardDesc")]
        public string TypesAwardDesc { get; set; }

        internal AwardModel AwardToObject(Award award, List<Award> awardList)
        {
            var context = new TicketsEntities();
            var awardModel = new AwardModel()
            {
                Id = award.Id,
                Name = award.Name,
                NameAux = award.Name,
                Description = award.Description,
                OrderAward = award.OrderAward,
                ByFraction = award.ByFraction,
                ByFractionDesc = context.Catalogs.FirstOrDefault(c => c.Id == award.ByFraction).NameDetail,
                ProspectId = award.ProspectId,
                Quantity = award.Quantity,
                SourceAward = award.SourceAward,
                SourceAwardDescription = award.SourceAward.HasValue ? awardList.FirstOrDefault(sa => sa.Id == award.SourceAward.Value).Name : "",
                Terminal = award.Terminal,
                TotalValue = award.TotalValue,
                Value = award.Value,
                TypesAwardId = award.TypesAwardId > 0 ? award.TypesAwardId : 1,
                TypesAwardDesc = award.TypesAwardId > 0 ? context.TypesAwards.FirstOrDefault(sa => sa.Id == award.TypesAwardId).Name : ""
            };

            return awardModel;
        }
    }
}
