using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.Model.Responses
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public List<string> Roles { get; set; }
    }
}
