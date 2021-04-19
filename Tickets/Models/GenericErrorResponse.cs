using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tickets.Models
{
    public class GenericErrorResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}