using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoRepair.Data.Entities
{
    public class CarModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The name of the car is not specified")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The color of the car is not specified")]
        public string Color { get; set; }

        [Required(ErrorMessage = "The license plate of the car is not specified")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "The status of the car is not specified")]
        public bool isActive { get; set; } = true;

        [Required(ErrorMessage = "The owner of the car is not specified")]
        public Guid OwnerID { get; set; }
        [ForeignKey("OwnerID")]
        public ClientModel? Owner { get; set; }
    }
}