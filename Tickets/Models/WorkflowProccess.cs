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
    
    public partial class WorkflowProccess
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Statu { get; set; }
        public int WorkFlowId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
    
        public virtual Catalog Catalog { get; set; }
        public virtual User User { get; set; }
        public virtual Workflow Workflow { get; set; }
    }
}
