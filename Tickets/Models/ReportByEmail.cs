//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tickets.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportByEmail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ModuleName { get; set; }
        public string Subject { get; set; }
        public string Recipients { get; set; }
        public string Message { get; set; }
    }
}
