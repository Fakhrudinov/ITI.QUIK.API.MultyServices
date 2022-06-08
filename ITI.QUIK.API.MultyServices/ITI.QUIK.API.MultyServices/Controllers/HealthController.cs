using DataAbstraction.Connections;
using DataAbstraction.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

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

        [HttpGet("IsChildConnectionOk/ToQuikDataBase")]
        public async Task<IActionResult> IsChildConnectionOkToQuikDataBase()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToQuikDataBase Call");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connection.ConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connection.ConnectionString + "/api/QuikDataBase/CheckConnections/QuikDataBase");

                if (response.IsSuccessStatusCode)
                {
                    ListStringResponseModel result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation("HttpGet IsChildConnectionOk/ToQuikDataBase succes is " + result.IsSuccess);
                    return Ok(result);
                }
            }

            _logger.LogWarning("HttpGet IsChildConnectionOk/ToQuikDataBase NotFound");
            return NotFound();
        }


        [HttpGet("IsChildConnectionOk/ToQadmin/SpotBRL")]
        public async Task<IActionResult> IsChildConnectionOkToQadminSpotBRL()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToQadmin/SpotBRL Call");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connection.ConnectionString);
                client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connection.ConnectionString + "/api/QuikQAdminFortsApi/CheckConnections/FortsApi");

                if (response.IsSuccessStatusCode)
                {
                    ListStringResponseModel result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation("HttpGet IsChildConnectionOk/ToQadmin/SpotBRL succes is " + result.IsSuccess);
                    return Ok(result);
                }
            }

            _logger.LogWarning("HttpGet IsChildConnectionOk/ToQadmin/SpotBRL NotFound");
            return NotFound();
        }

    }
}
