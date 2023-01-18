using WatchDb.DataAccess.Models;

namespace WatchDb.DataAccess.Repositories
{
    public interface IUserRepository : IRepository<UserModel>
    {
        Task<UserModel> GetAsync(long chatId);
    }
}
