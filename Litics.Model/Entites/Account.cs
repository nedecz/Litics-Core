using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.Model.Entites
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AccountID { get; set; }
        public string Name { get; set; }
        public string Index { get; set; }
        public string ApplicationID { get; set; }
        public string ApiKey { get; set; }
    }
}
