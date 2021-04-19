using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Data.DataModels
{
    public class User
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string Password { get; set; }

        public DateTime JoinUtcDate { get; set; }
    }
}
