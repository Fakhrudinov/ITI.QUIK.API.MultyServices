using DataAbstraction.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITI.QUIK.API.MultyServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private ILogger<HealthController> _logger;
        private HttpClientConfig _connection;

        public HealthController(ILogger<HealthController> logger, IOptions<HttpClientConfig> connection)
        {
            _logger = logger;
            _connection = connection.Value;
        }

        [HttpGet("MyHealth")]
        public IActionResult IsOk()
        {
            _logger.LogInformation("HttpGet MyHealth Call");

            return Ok("Ok");
        }

        [HttpGet("IsChildHealthOk")]
        public async Task<IActionResult> IsChildOk()
        {
            _logger.LogInformation("HttpGet IsChildHealthOk Call");

            using (var client = new HttpClient())
            {
                var responseMessage = await client.GetAsync(_connection.ConnectionString + "/api/HealthState/OK");

                if (responseMessage.IsSuccessStatusCode)
                {
                    string responseBody = await responseMessage.Content.ReadAsStringAsync();
                    _logger.LogInformation("HttpGet IsChildHealthOk Ok: " + responseBody);
                    return Ok(responseBody);
                }
            }

            _logger.LogWarning("HttpGet IsChildHealthOk result NotFound");
            return NotFound();
        }
    }
}
