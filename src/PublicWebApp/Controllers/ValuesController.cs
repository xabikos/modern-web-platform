using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublicWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;


        public ValuesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // create HTTP client
            var client = _httpClientFactory.CreateClient();

            // get current user access token and set it on HttpClient
            var token = await HttpContext.GetUserAccessTokenAsync();
            client.SetBearerToken(token.AccessToken);

            // call remote API
            var response = await client.GetAsync($"https://localhost:44343/WeatherForecast");
            return new JsonResult(await response.Content.ReadAsStringAsync());
            //return new string[] { "value1", "value2" };
        }
    }
}
