using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace redis_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private string key = "Category";

        public CategoryController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<ResponseModel>> Get()
        {
            var categorys = new List<string>();

            var cacheList = await _cache.GetAsync(key);

            if (cacheList is not null)
            {
                categorys = JsonSerializer.Deserialize<List<string>>(cacheList);
            }

            return new List<ResponseModel>
            {
                new ResponseModel
                {
                    status = 200,
                    errorMessage = string.Empty,
                    result = categorys!
                }
            };
        }

        [HttpPost]
        public async Task Post([FromBody] string value)
        { 
            var categorys = new List<string>();

            var cacheList = await _cache.GetAsync(key);

            if(cacheList is not null)
            {
                categorys = JsonSerializer.Deserialize<List<string>>(cacheList);
            }

            categorys?.Add(value);

            var serialized = JsonSerializer.Serialize(categorys);

            var redisList = Encoding.UTF8.GetBytes(serialized);

            var options = new DistributedCacheEntryOptions()
                  .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                  .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            await _cache.SetAsync(key, redisList, options);
        }
    }
}
