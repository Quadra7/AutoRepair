﻿namespace AutoRepair.Areas.admin.Models
{
    public class AccountantEditModel
    {
        public Guid ID { get; set; }
        public Guid InfoID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; } = null;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Guid UserID { get; set; }
    }
}
