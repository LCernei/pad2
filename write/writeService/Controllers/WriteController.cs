using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace writeService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WriteController : ControllerBase
    {
        private readonly ILogger<WriteController> _logger;

        public WriteController(ILogger<WriteController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Movie parameter)
        {
            Console.WriteLine(parameter.name);
            Console.WriteLine(Request.Headers["Accept"] + "  " + Request.Headers["Content-Type"]);
            var cluster = Cluster.Builder()
                .AddContactPoints("172.27.0.2", "172.27.0.3")
                .Build();
            var session = cluster.Connect("my_keyspace");
            Guid guid = Guid.NewGuid();
            
            //Prepare a statement once
            var ps = session.Prepare("INSERT INTO Movies(id, name) values(?, ?)");
            //...bind different parameters every time you need to execute
            var statement = ps.Bind(guid.ToString(), parameter.name);
            //Execute the bound statement with the provided parameters
            session.Execute(statement);

            return Ok("wwwww");
        }
    }
}