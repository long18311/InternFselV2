using InternFselV2.Entities;
using InternFselV2.Model.QueryModel;
using InternFselV2.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace InternFselV2.Repositories.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(InternV2DbContext internV2DbContext) : base(internV2DbContext)
        {
        }

        public async Task<User?> GetUserbyLoginmodel(string userName, string password)
        {
            var user = await Queryable.FirstOrDefaultAsync(p => p.UserName == userName && p.Password == password);

            return user;
        }
        public async Task<User?> GetByUserName(string userName)
        {
            var user = await Queryable.FirstOrDefaultAsync(p => p.UserName == userName);

            return user;
        }

    }
}
