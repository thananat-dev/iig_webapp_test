using System;
using System.Collections.Generic;

namespace iig_webapp_test.Entities
{
    public partial class User
    {
        public long UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImage { get; set; }
    }
}
