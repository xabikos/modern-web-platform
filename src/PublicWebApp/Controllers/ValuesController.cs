﻿using Common;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PublicWebApp.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
		private readonly ServicesConfiguration _config;

		public ValuesController(IHttpClientFactory httpClientFactory, ServicesConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
			_config = config;
		}
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            // create HTTP client
            var client = _httpClientFactory.CreateClient();

            // get current user access token and set it on HttpClient
            var token = await HttpContext.GetUserAccessTokenAsync();
            client.SetBearerToken(token.AccessToken);

			// call remote API
			try
			{
				var response = await client.GetAsync($"{_config.CoreDomainAPI.Url}WeatherForecast");
				if (!response.IsSuccessStatusCode)
				{
					var resp = await response.Content.ReadAsStringAsync();
					var headers = response.RequestMessage.Headers.Select(h => $"header key: {h.Key} value: {h.Value.First()}");
					var result = new List<string>() {
						$"requested URL: {_config.CoreDomainAPI.Url}WeatherForecast",
						$"token used: {token.AccessToken}",
						$"result code: {response.StatusCode}",
						$"response: {resp}"
					};
					result.AddRange(headers);
					return result;
				}
				return await response.Content.ReadFromJsonAsync<string[]>();

			}
			catch (Exception)
			{
				throw;
			}
        }
    }
}
