using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Models.Workflows
{
    public class ProccessModel
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "comment")]
        public string Comment { get; set; }

        [JsonProperty(PropertyName = "statu")]
        public int Statu { get; set; }

        [JsonProperty(PropertyName = "statuDesc")]
        public string StatuDesc { get; set; }

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate { get; set; }

        [JsonProperty(PropertyName = "createDateLong")]
        public long CreateDateLong { get; set; }

        [JsonProperty(PropertyName = "workflowId")]
        public int WorkflowId { get; set; }

        internal ProccessModel ToObject(WorkflowProccess model)
        {
            var context = new TicketsEntities();
            var process = new ProccessModel()
            {
                Id = model.Id,
                Statu = model.Statu,
                Comment = model.Comment,
                CreateDate = model.CreateDate,
                CreateDateLong = model.CreateDate.ToUnixTime(),
                StatuDesc = context.Catalogs.FirstOrDefault( f=> f.Id == model.Statu).NameDetail,
                UserName = context.Users.FirstOrDefault( u=> u.Id == model.CreateUser).Name,
                WorkflowId = model.WorkFlowId
            };
            return process;
        }
    }
}