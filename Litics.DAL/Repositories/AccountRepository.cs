using Litics.DAL.Interfaces;
using Litics.Model.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Litics.Model.Responses;

namespace Litics.DAL.Repositories
{
    public class AccountRepository : EntityBaseRepository<Account>, IAccountRepository
    {
        private LiticsContext _context;
        public AccountRepository(LiticsContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<User> Users(Guid accountId)
        {
            var usersWithRoles = (from user in _context.Users
                                  where user.AccountID == accountId
                                  select new User
                                  {
                                      AccountName = user.Account.Name,
                                      UserId = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in _context.UserRoles
                                                   join role in _context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   where user.Id == userRole.UserId
                                                   select role.Name).ToList()
                                  }).ToList();
            return usersWithRoles;
        }
    }
}

