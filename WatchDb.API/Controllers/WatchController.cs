using Microsoft.AspNetCore.Mvc;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.API.Controllers
{
    [ApiController]
    [Route("api/watches")]
    public class WatchController
    {
        private ShopDbContext context;
        public WatchController(ShopDbContext context)
        {
            this.context = context;
        }

        [HttpGet("")]
        public async Task<List<Watch>> Get()
        {
            return await context.Watches.GetAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<Watch?> Get(int id)
        {
            return await context.Watches.GetAsync(id);
        }

        [HttpGet("model")]
        public async Task<List<Watch>> Get(string model)
        {
            return await context.Watches.GetAsync(model);
        }

        [HttpPost("")]
        public async Task<Watch> Create([FromBody]Watch watch)
        {
            return await context.Watches.CreateAsync(watch);
        }

        [HttpPut("")]
        public async Task<Watch?> Update([FromBody] Watch watch)
        {
            return await context.Watches.UpdateAsync(watch);
        }

        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            return await context.Watches.DeleteAsync(id);
        }
    }
}
