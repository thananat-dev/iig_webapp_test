using System;
using System.Collections.Generic;

namespace iig_webapp_test.Entities
{
    public partial class ChangeUserPassword
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? UserOldPassword { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
