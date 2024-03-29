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
    
    public partial class TaxReceipt
    {
        public TaxReceipt()
        {
            this.TaxReceiptNumbers = new HashSet<TaxReceiptNumber>();
        }
    
        public int Id { get; set; }
        public int Statu { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Type { get; set; }
        public string Notas { get; set; }
        public System.DateTime DueDate { get; set; }
        public int SequenceFrom { get; set; }
        public int SequenceTo { get; set; }
    
        public virtual User User { get; set; }
        public virtual ICollection<TaxReceiptNumber> TaxReceiptNumbers { get; set; }
        public virtual Catalog Catalog { get; set; }
    }
}
