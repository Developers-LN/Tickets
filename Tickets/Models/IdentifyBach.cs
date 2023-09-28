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
    
    public partial class IdentifyBach
    {
        public IdentifyBach()
        {
            this.IdentifyNumbers = new HashSet<IdentifyNumber>();
            this.IdentifyBachPayments = new HashSet<IdentifyBachPayment>();
            this.NoteCredits = new HashSet<NoteCredit>();
        }
    
        public int Id { get; set; }
        public int RaffleId { get; set; }
        public int ClientId { get; set; }
        public int Statu { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int Type { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Notas { get; set; }
        public Nullable<int> IdentifyType { get; set; }
        public string Cedula2 { get; set; }
        public string Telefono2 { get; set; }
        public string Nombre2 { get; set; }
        public Nullable<int> Trabajado { get; set; }
        public Nullable<int> WinnerId { get; set; }
        public Nullable<int> IdentifyBachSequence { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Raffle Raffle { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<IdentifyNumber> IdentifyNumbers { get; set; }
        public virtual ICollection<IdentifyBachPayment> IdentifyBachPayments { get; set; }
        public virtual ICollection<NoteCredit> NoteCredits { get; set; }
        public virtual Winner Winner { get; set; }
    }
}
