using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Models
{
    public class ReultRequestModel
    {
        public bool result{get;set;}
        public List<string> messages { get; set; }
    }
}