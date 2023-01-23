using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchDb.API.Models;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController
    {
        private ShopDbContext context;
        public CategoryController(ShopDbContext context)
        {
            this.context = context;
        }

        [HttpGet("")]
        public async Task<Result<List<KeyValuePair<Category, int>>>> Get()
        {
            var watches = await context.Watches.GetAsync();
            return new Result<List<KeyValuePair<Category, int>>>
            {
                Value = (await context.Categories.GetAsync())
                .Select(x => new KeyValuePair<Category, int>(x, watches.Count(w => w.Category != null && w.Category.Id == x.Id))).ToList(),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpGet("{id:int}")]
        public async Task<Result<Category?>> Get(int id)
        {
            return new Result<Category?> {
                Value = await context.Categories.GetAsync(id),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpPost("")]
        public async Task<Result<Category>> Create([FromBody] Category category)
        {
            return new Result<Category>
            {
                Value = await context.Categories.CreateAsync(category),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpPut("")]
        public async Task<Result<Category?>> Update([FromBody] Category category)
        {
            return new Result<Category?>
            {
                Value = await context.Categories.UpdateAsync(category),
                Token = JWTHelper.GetToken()
            };
        }

        [HttpDelete("{id:int}")]
        public async Task<Result<bool>> Delete(int id)
        {
            return new Result<bool>
            {
                Value = await context.Categories.DeleteAsync(id),
                Token = JWTHelper.GetToken()
            };
        }
    }
}
