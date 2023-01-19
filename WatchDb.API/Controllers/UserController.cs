using Microsoft.AspNetCore.Mvc;
using WatchDb.API.Models;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private ShopDbContext context;

        public UserController(ShopDbContext context)
        {
            this.context = context;
        }

        [HttpPost("signin")]
        public async Task<Result<User>> SignIn([FromBody]User user)
        {
            var checkedUser = await context.Users.GetAsync(user.Login, user.Password);
            if(checkedUser != null)
            {
                checkedUser.Password = null;
                return new Result<User>
                {
                    Token = JWTHelper.GetToken(),
                    Value = checkedUser
                };
            }

            return new Result<User>
            {
                Token = null,
                Value = null
            };
        }
        [HttpPost("")]
        public async Task<Result<User>> SignUp([FromBody] User user)
        {
            if(user.Login == null || user.Password == null)
            {
                return new Result<User>
                {
                    Token = null,
                    Value = null
                };
            }

            if((await context.Users.GetAsync()).FirstOrDefault(x => x.Login == user.Login) != null)
            {
                return new Result<User>
                {
                    Token = null,
                    Value = null
                };
            }
            var u = await context.Users.CreateAsync(user);
            u.Password = null;

            return new Result<User>
            {
                Token = JWTHelper.GetToken(),
                Value = u
            };
        }
    }
}
