using System.Collections.Generic;

namespace Tickets.Models
{
    public class ReultRequestModel
    {
        public bool result { get; set; }
        public List<string> messages { get; set; }
    }
}