using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Cache.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly ILogger<CacheController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private static CacheItem _cache = new CacheItem();
        
        public CacheController(ILogger<CacheController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
           _clientFactory = clientFactory;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Get()
        {
            Console.WriteLine(_cache.Timestamp.Ticks);
            Console.WriteLine(_cache.Response == null);
            
            if (_cache.Timestamp.AddMinutes(1) <= DateTime.Now)
            {
                Console.WriteLine("if");
                return Ok(_cache.Response);
            }

            _cache = new CacheItem();
            return Ok();
        }
        
        [HttpPost]
        [Route("[action]")]
        public IActionResult Post([FromBody] List<Movie> parameter)
        {
            _cache.Response = parameter;
            _cache.Timestamp = DateTime.Now;
            return Ok();
        }
    }
}
