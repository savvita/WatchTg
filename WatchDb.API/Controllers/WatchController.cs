using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchDb.API.Models;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.API.Controllers
{
    [ApiController]
    [Route("api/watches")]
    [Authorize]
    public class WatchController
    {
        private ShopDbContext context;
        public WatchController(ShopDbContext context)
        {
            this.context = context;
        }

        [HttpGet("")]
        public async Task<Result<List<Watch>>> Get()
        {
            return new Result<List<Watch>>
            {
                Value = await context.Watches.GetAsync(),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpGet("{id:int}")]
        public async Task<Result<Watch?>> Get(int id)
        {
            return new Result<Watch?>
            {
                Value = await context.Watches.GetAsync(id),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpGet("model")]
        public async Task<Result<List<Watch>>> Get(string model)
        {
            return new Result<List<Watch>>
            {
                Value = await context.Watches.GetAsync(model),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpPost("")]
        public async Task<Result<Watch>> Create([FromBody] Watch watch)
        {
            return new Result<Watch> {
                Value = await context.Watches.CreateAsync(watch),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpPut("")]
        public async Task<Result<Watch?>> Update([FromBody] Watch watch)
        {
            return new Result<Watch?>
            {
                Value = await context.Watches.UpdateAsync(watch),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<Result<bool>> Delete(int id)
        {
            return new Result<bool>
            {
                Value = await context.Watches.DeleteAsync(id),
                Token = JWTHelper.GetToken()
            };
        }
    }
}
