using System.ComponentModel.DataAnnotations;

namespace AutoRepair.Data.Entities
{
    public class InfoModel
    {
        [Key]
        public Guid ID { get; set; }

        [Required(ErrorMessage = "The user's last name is not specified")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The user's first name is not specified")]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; } = null;

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "The user's phone number is not specified")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "The user's address is not specified")]
        public string Address { get; set; }
    }
}
