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
    
    public partial class webpages_Roles
    {
        public webpages_Roles()
        {
            this.Rol_Module = new HashSet<Rol_Module>();
            this.Agencies = new HashSet<Agency>();
            this.Users = new HashSet<User>();
        }
    
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<Rol_Module> Rol_Module { get; set; }
        public virtual ICollection<Agency> Agencies { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
