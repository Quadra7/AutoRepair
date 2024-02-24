using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.profile.Models
{
    public class ClientViewModel
    {
        public Guid ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool isInLoyalProgram { get; set; }

        public IEnumerable<CarModel>? Cars { get; set; }
        public IEnumerable<OrderModel>? Orders { get; set; }
    }
}
