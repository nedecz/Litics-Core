using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Litics.Model.Entites
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid AccountID { get; set; }
        public Account Account { get; set; }
    }
}
