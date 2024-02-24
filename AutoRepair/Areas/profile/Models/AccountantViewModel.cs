using AutoRepair.Data.Entities;

namespace AutoRepair.Areas.profile.Models
{
    public class AccountantViewModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public IEnumerable<OrderModel> NotPayedOrders { get; set; }
    }
}
