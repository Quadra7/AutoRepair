using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class AccountantModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The accountant's info is not specified")]
        public Guid InfoID { get; set; }
        [ForeignKey("InfoID")]
        public InfoModel? Info { get; set; }

        public bool isActive { get; set; } = true;

        [Required(ErrorMessage = "The accountant's account is not specified")]
        public Guid UserID { get; set; }
        [ForeignKey("UserID")]
        public UserModel? User { get; set; }
    }
}