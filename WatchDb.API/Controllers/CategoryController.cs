using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<List<Category>> Get()
        {
            return await context.Categories.GetAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<Category?> Get(int id)
        {
            return await context.Categories.GetAsync(id);
        }

        [HttpPost("")]
        public async Task<Category> Create([FromBody] Category category)
        {
            return await context.Categories.CreateAsync(category);
        }

        [HttpPut("")]
        public async Task<Category?> Update([FromBody] Category category)
        {
            return await context.Categories.UpdateAsync(category);
        }

        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            return await context.Categories.DeleteAsync(id);
        }
    }
}
