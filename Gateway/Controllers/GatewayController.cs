using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private readonly IHttpClientFactory _clientFactory;
        public GatewayController(ILogger<GatewayController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var accept = Request.Headers["Accept"].ToString();
            var request = new HttpRequestMessage(HttpMethod.Get,
                "http://localhost:3000/read");
            request.Headers.Add("Accept", accept);

            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
                return StatusCode((int) response.StatusCode); // return fail
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();

            List<Movie> movies;
            if (accept.Contains("xml"))
            {
                var serializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>)serializer.Deserialize(responseStream);
            }
            else
            {
                movies = await JsonSerializer.DeserializeAsync
                    <List<Movie>>(responseStream);
            }
            return Ok(movies);
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Post([FromBody] Movie parameters)
        {
            var accept = Request.Headers["Accept"].ToString();
            var contentType = Request.Headers["Content-Type"].ToString();
            
            var request = new HttpRequestMessage(HttpMethod.Post,
                "http://localhost:4000/write");
            request.Content = new StringContent(JsonSerializer.Serialize(parameters),
                Encoding.UTF8, 
                "application/json");
            request.Headers.Add("Accept", accept);

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int) response.StatusCode); // return fail
            
            await using var responseStream = await response.Content.ReadAsStreamAsync();

            string content;
            if (accept.Contains("xml"))
            {
                var serializer = new XmlSerializer(typeof(string));
                content = (string)serializer.Deserialize(responseStream);
            }
            else
            {
                content = await JsonSerializer.DeserializeAsync
                    <string>(responseStream);
            }
            return Ok(content);
        }
    }
}
