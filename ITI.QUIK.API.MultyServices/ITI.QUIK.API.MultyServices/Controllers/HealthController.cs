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
        private HttpConfigurations _connection;

        public HealthController(ILogger<HealthController> logger, IOptions<HttpConfigurations> connection)
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
                var responseMessage = await client.GetAsync(_connection.QuikAPIConnectionString + "/api/HealthState/OK");

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

            return await GetResultOfCheckConnection("/api/QuikDataBase/CheckConnections/QuikDataBase");
        }

        [HttpGet("IsChildConnectionOk/ToQadmin/SpotBRL")]
        public async Task<IActionResult> IsChildConnectionOkToQadminSpotBRL()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToQadmin/SpotBRL Call");

            return await GetResultOfCheckConnection("/api/QuikQAdminFortsApi/CheckConnections/FortsApi");
        }

        [HttpGet("IsChildConnectionOk/ToQadmin/FortsBRL")]
        public async Task<IActionResult> IsChildConnectionOkToQadminFortsBRL()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToQadmin/FortsBRL Call");

            return await GetResultOfCheckConnection("/api/QuikQAdminFortsApi/CheckConnections/FortsApi");
        }

        [HttpGet("IsChildConnectionOk/ToQMonitor")]
        public async Task<IActionResult> IsChildConnectionOkToQMonitor()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToQMonitor Call");

            return await GetResultOfCheckConnection("/api/QuikQMonitor/CheckConnections/QMonitorAPI");
        }

        [HttpGet("IsChildConnectionOk/ToSFTP")]
        public async Task<IActionResult> IsChildConnectionOkToSFTP()
        {
            _logger.LogInformation("HttpGet IsChildConnectionOk/ToSFTP Call");

            return await GetResultOfCheckConnection("/api/QuikSftpServer/CheckConnections/ServerSFTP");
        }

        private async Task<IActionResult> GetResultOfCheckConnection(string apiRequest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connection.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connection.QuikAPIConnectionString + apiRequest);

                if (response.IsSuccessStatusCode)
                {
                    ListStringResponseModel result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation($"HttpGet {apiRequest} succes is {result.IsSuccess}");
                    return Ok(result);
                }
            }

            _logger.LogWarning($"HttpGet {apiRequest} NotFound");
            return NotFound();
        }
    }
}
