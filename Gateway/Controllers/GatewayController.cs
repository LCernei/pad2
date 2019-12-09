using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(ILogger<GatewayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Get()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(" http://localhost:3000/");
                var accept = Request.Headers["Accept"];
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                var responseTask = client.GetAsync("read");
                responseTask.Wait();
                
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();
                    
                    var str = readTask.Result;
                    object obj = new List<Movie>();
                    
                    if (((string) accept).Contains("xml"))
                    {
                        var serializer = new XmlSerializer(typeof(List<Movie>));
                        using (TextReader reader = new StringReader(str))
                        {
                            obj = serializer.Deserialize(reader);
                        }
                    }
                    else
                    {
                        obj = JsonSerializer.Deserialize<List<Movie>>(str);
                        
                    }
                    
                    return Ok(obj);
                }
            }

            return Ok("aaa");
        }
        
        [HttpPost]
        [Route("[action]")]
        public IActionResult Post([FromBody] Movie parameters)
        {
            Console.WriteLine(parameters.name);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(" http://localhost:4000/");
                var accept = Request.Headers["Accept"];
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "write");
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(parameters),
                    Encoding.UTF8, 
                    "application/json");//CONTENT-TYPE header

                var responseTask = client.SendAsync(requestMessage);

                responseTask.Wait();
                
                var result = responseTask.Result;

                var readTask = result.Content.ReadAsStringAsync();
                readTask.Wait();
                    
                var str = readTask.Result;
//                return Ok($"{str}@\n{result.RequestMessage}@\n{result.StatusCode}@\n{requestMessage.Content.Headers.ContentType}");
                return Ok(str);
            }
        }
    }
}
