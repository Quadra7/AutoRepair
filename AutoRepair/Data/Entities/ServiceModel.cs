using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class ServiceModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The name of the service is not specified")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description of the service is not specified")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The price of the service is not specified")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "The specialty of the service is not specified")]
        public Guid SpecialtyID { get; set; }
        [ForeignKey("SpecialtyID")]
        public SpecialtyModel? Specialty { get; set; }
    }
}