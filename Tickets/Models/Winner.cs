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
    
    public partial class Winner
    {
        public Winner()
        {
            this.IdentifyBaches = new HashSet<IdentifyBach>();
        }
    
        public int Id { get; set; }
        public Nullable<int> DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string Phone { get; set; }
        public string WinnerName { get; set; }
        public Nullable<int> GenderId { get; set; }
        public Nullable<int> CreateUser { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    
        public virtual Catalog Catalog { get; set; }
        public virtual ICollection<IdentifyBach> IdentifyBaches { get; set; }
        public virtual Catalog Catalog1 { get; set; }
    }
}
