using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Tickets.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("User")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class RolUserStatuModel
    {
        public int UserId { get; set; }
        public int RolId { get; set; }
        public bool Statu { get; set; }
    }
    public class RolOfficeStatuModel
    {
        public int OfficeId { get; set; }
        public int RolId { get; set; }
        public bool Statu { get; set; }
    }

    public class LoginModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class ChangePasswordModel
    {
        public string Password { get; set; }
    }

    public class UserCreateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Statu { get; set; }
        public int? EmpleadoId { get; set; }
    }

    public enum UserStatusEnum
    {
        Active = 1,
        Suspend = 2
    }
}
