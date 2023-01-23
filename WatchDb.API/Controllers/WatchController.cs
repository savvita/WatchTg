using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchDb.API.Cache;
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
        private readonly ICacheService cacheService;
        public WatchController(ShopDbContext context, ICacheService cacheService)
        {
            this.context = context;
            this.cacheService = cacheService;
        }

        [HttpGet("")]
        public async Task<Result<List<Watch>>> Get()
        {
            var cached = await cacheService.GetData<List<Watch>>("watches");
            if(cached == null)
            {
                var watches = await context.Watches.GetAsync();
                if(watches.Count > 0)
                {
                    await cacheService.SetData("watches", watches, DateTimeOffset.Now.AddDays(1));
                    cached = watches;
                }
            }
            return new Result<List<Watch>>
            {
                Value = cached,
                Token = JWTHelper.GetToken()
            };
        }

        [HttpGet("category/{id:int}")]
        public async Task<Result<List<Watch>>> GetByCategoryId(int id)
        {
            var cached = await cacheService.GetData<List<Watch>>("watches");
            if (cached == null)
            {
                var watches = await context.Watches.GetAsync();
                if (watches.Count > 0)
                {
                    await cacheService.SetData("watches", watches, DateTimeOffset.Now.AddDays(1));
                    cached = watches;
                }
            }

            return new Result<List<Watch>>
            {
                Value = cached!.Where(x => x.Category != null && x.Category.Id == id).ToList(),
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
            await cacheService.RemoveData("watches");
            return new Result<Watch> {
                Value = await context.Watches.CreateAsync(watch),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpPut("")]
        public async Task<Result<Watch?>> Update([FromBody] Watch watch)
        {
            await cacheService.RemoveData("watches");
            return new Result<Watch?>
            {
                Value = await context.Watches.UpdateAsync(watch),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<Result<bool>> Delete(int id)
        {
            await cacheService.RemoveData("watches");
            return new Result<bool>
            {
                Value = await context.Watches.DeleteAsync(id),
                Token = JWTHelper.GetToken()
            };
        }
    }
}
