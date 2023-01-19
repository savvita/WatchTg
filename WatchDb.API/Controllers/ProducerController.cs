using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchUILibrary;
using WatchUILibrary.Models;

namespace WatchDb.API.Controllers
{
    [ApiController]
    [Route("api/producers")]
    [Authorize]
    public class ProducerController
    {
        private ShopDbContext context;
        public ProducerController(ShopDbContext context)
        {
            this.context = context;
        }

        [HttpGet("")]
        public async Task<List<Producer>> Get()
        {
            return await context.Producers.GetAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<Producer?> Get(int id)
        {
            return await context.Producers.GetAsync(id);
        }

        [HttpPost("")]
        public async Task<Producer> Create([FromBody] Producer producer)
        {
            return await context.Producers.CreateAsync(producer);
        }

        [HttpPut("")]
        public async Task<Producer?> Update([FromBody] Producer producer)
        {
            return await context.Producers.UpdateAsync(producer);
        }

        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            return await context.Producers.DeleteAsync(id);
        }
    }
}
