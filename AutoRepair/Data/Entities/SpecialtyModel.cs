using System.ComponentModel.DataAnnotations;

namespace AutoRepair.Data.Entities
{
    public class SpecialtyModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The name of the specialty is not specified")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description of the specialty is not specified")]
        public string Description { get; set; }
    }
}