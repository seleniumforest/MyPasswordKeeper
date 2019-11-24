using System;
using System.Collections.Generic;
using System.Text;

namespace MyPasswordKeeper.Models
{
    public class Identity
    {
        public string ServiceName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public bool IsValid() => !string.IsNullOrEmpty(ServiceName) && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
    }
}
