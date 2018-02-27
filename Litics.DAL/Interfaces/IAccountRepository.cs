using Litics.Model.Entites;
using Litics.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litics.DAL.Interfaces
{
    public interface IAccountRepository: IEntityBaseRepository<Account>
    {
        IEnumerable<User> Users(Guid accountId);
    }
}
