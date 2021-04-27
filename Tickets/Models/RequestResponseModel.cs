using Newtonsoft.Json;

namespace Tickets.Models
{
    public class RequestResponseModel
    {
        [JsonProperty(PropertyName = "result")]
        public bool Result { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "object")]
        public object Object { get; set; }
    }
}