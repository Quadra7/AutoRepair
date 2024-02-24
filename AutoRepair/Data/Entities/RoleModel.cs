using System.ComponentModel.DataAnnotations;

namespace AutoRepair.Data.Entities
{
    public class RoleModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The name of the role is not specified")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description of the service is not specified")]
        public string Description { get; set; }
    }
}