using InternFselV2.Entities;
using InternFselV2.Model.QueryModel;

namespace InternFselV2.Repositories.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> GetUserbyLoginmodel(string userName,string password);
        public Task<User?> GetByUserName(string userName);
    }
}
