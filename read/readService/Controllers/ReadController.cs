using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace readService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReadController : ControllerBase
    {
        private readonly ILogger<ReadController> _logger;

        public ReadController(ILogger<ReadController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var cluster = Cluster.Builder()
                .AddContactPoints("172.17.0.4")
                .Build();
            var session = cluster.Connect("my_keyspace");
            var rs = session.Execute("SELECT * FROM Movies");

            var movieList = new List<Movie>();
            foreach (var row in rs)
            {
                var name = row.GetValue<string>("name");
                Console.WriteLine(name);
                movieList.Add(new Movie{name = name});
            }
            
//            var accept = (string)Request.Headers["Accept"];
//            Console.WriteLine(accept);

//            var resultString = string.Empty;
//            if (accept.Contains("json"))
//            {
//                
//                resultString = JsonConvert.SerializeObject(movieList);
//                Console.WriteLine($"in if {resultString}");
//            }

            return Ok(movieList);
        }
    }
}
