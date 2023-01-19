using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchDb.DataAccess.Models;
using WatchDb.DataAccess.Repositories;
using WatchUILibrary.Models;

namespace WatchUILibrary.Repositories
{
    public class UserRepository
    {
        private DbContext context;
        public UserRepository(DbContext context)
        {
            this.context = context;
        }
        public async Task<List<User>> GetAsync()
        {
            return (await context.Users.GetAsync()).Select(model => new User(model)).ToList();
        }

        public async Task<User?> GetAsync(int id)
        {
            var model = await context.Users.GetAsync(id);

            return model != null ? new User(model) : null;
        }

        public async Task<User?> GetAsync(long chatId)
        {
            var model = await context.Users.GetAsync(chatId);

            return model != null ? new User(model) : null;
        }

        public async Task<User?> UpdateAsync(User user)
        {
            var model = await context.Users.UpdateAsync(new UserModel()
            {
                Id = user.Id,
                ChatId = user.ChatId,
                Username = user.Username,
                FirstName = user.FirstName,
                Login = user.FirstName,
                Password = user.Password
            });

            if (model == null)
            {
                return null;
            }

            return new User(model);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            return await context.Users.DeleteAsync(id);
        }

        public async Task<User> CreateAsync(User user)
        {
            return new User(await context.Users.CreateAsync(new UserModel()
            {
                ChatId = user.ChatId,
                Username = user.Username,
                FirstName = user.FirstName,
                Login = user.Login,
                Password = user.Password
            }));
        }

        public async Task<User?> GetAsync(string? login, string? password)
        {
            var user = (await context.Users.GetAsync()).FirstOrDefault(x => x.Login != null && x.Login == login && x.Password != null && x.Password == password);
            return user != null ? new User(user) : null;
        }
    }
}
