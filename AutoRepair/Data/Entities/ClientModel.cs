using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class ClientModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The client's info is not specified")]
        public Guid InfoID { get; set; }
        [ForeignKey("InfoID")]
        public InfoModel? Info { get; set; }

        public bool isInLoyalProgram { get; set; } = false;

        public bool isActive { get; set; } = true;

        [Required(ErrorMessage = "The client's account is not specified")]
        public Guid UserID { get; set; }
        [ForeignKey("UserID")]
        public UserModel? User { get; set; }
    }
}