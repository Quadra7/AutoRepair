using System.ComponentModel.DataAnnotations;

namespace AutoRepair.Data.Entities
{
    public class StatusModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The status name is not specified")]
        public string Name { get; set; }
    }
}
