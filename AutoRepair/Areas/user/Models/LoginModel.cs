﻿namespace AutoRepair.Areas.user.Models
{
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool rememberMe { get; set; }
    }
}