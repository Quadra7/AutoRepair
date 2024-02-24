using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class UserModel
    {
        [Key]
        public Guid ID { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The user's email is not specified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The user's password is not specified")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The user's role is not specified")]
        public Guid RoleID { get; set; }
        [ForeignKey("RoleID")]
        public RoleModel Role { get; set; }
    }
}