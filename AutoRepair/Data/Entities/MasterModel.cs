using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class MasterModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The master's info is not specified")]
        public Guid InfoID { get; set; }
        [ForeignKey("InfoID")]
        public InfoModel? Info { get; set; }

        [Required(ErrorMessage = "The specialty of master is not specified")]
        public Guid SpecialtyID { get; set; }
        [ForeignKey("SpecialtyID")]
        public SpecialtyModel? Specialty { get; set; }

        public decimal Balance { get; set; } = 0;

        public bool isActive { get; set; } = true;

        [Required(ErrorMessage = "The account of the master is not specified")]
        public Guid UserID { get; set; }
        [ForeignKey("UserID")]
        public UserModel? User { get; set; }
    }
}