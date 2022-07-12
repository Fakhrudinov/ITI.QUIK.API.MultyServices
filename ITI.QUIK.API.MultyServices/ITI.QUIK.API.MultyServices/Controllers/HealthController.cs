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
        private HttpConfigurations _connections;

        public HealthController(ILogger<HealthController> logger, IOptions<HttpConfigurations> connections)
        {
            _logger = logger;
            _connections = connections.Value;
        }

        [HttpGet("MyHealth")]
        public IActionResult IsOk()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet MyHealth Call");

            return Ok("Ok");
        }

        [HttpGet("IsQuikApiHealthOk")]
        public async Task<IActionResult> IsQuikApiOk()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiHealthOk Call");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var responseMessage = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/HealthState/OK");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string responseBody = await responseMessage.Content.ReadAsStringAsync();
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiHealthOk Ok: " + responseBody);
                        return Ok(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiHealthOk request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString} /api/HealthState/OK");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpGet IsQuikApiHealthOk request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/HealthState/OK");
            }           

            return Ok(result);
        }

        [HttpGet("IsMatrixApiHealthOk")]
        public async Task<IActionResult> IsMatrixApiOk()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiHealthOk Call");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var responseMessage = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/HealthState/OK");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        string responseBody = await responseMessage.Content.ReadAsStringAsync();
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiHealthOk Ok: " + responseBody);
                        return Ok(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiHealthOk request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.MatrixAPIConnectionString} /api/HealthState/CheckConnections/MatrixDataBase");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpGet IsMatrixApiHealthOk request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.MatrixAPIConnectionString + "/api/HealthState/CheckConnections/MatrixDataBase");
            }

            return Ok(result);
        }

        [HttpGet("IsMatrixApiConnectionOk/ToMatrixDataBase")]
        public async Task<IActionResult> IsMatrixApiConnectionOkToMatrixDataBase()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiConnectionOk/ToMatrixDataBase Call");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/HealthState/CheckConnections/MatrixDataBase");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiConnectionOk/ToMatrixDataBase succes is {result.IsSuccess}");
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsMatrixApiConnectionOk/ToMatrixDataBase request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.MatrixAPIConnectionString} /api/HealthState/CheckConnections/MatrixDataBase");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpGet IsMatrixApiConnectionOk/ToMatrixDataBase request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.MatrixAPIConnectionString + "/api/HealthState/CheckConnections/MatrixDataBase");
            }

            return Ok(result);
        }

        [HttpGet("IsQuikApiConnectionOk/ToQuikDataBase")]
        public async Task<IActionResult> IsQuikApiConnectionOkToQuikDataBase()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiConnectionOk/ToQuikDataBase Call");

            return await GetResultOfQuikCheckConnection("/api/QuikDataBase/CheckConnections/QuikDataBase");
        }

        [HttpGet("IsQuikApiConnectionOk/ToQadmin/SpotBRL")]
        public async Task<IActionResult> IsQuikApiConnectionOkToQadminSpotBRL()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiConnectionOk/ToQadmin/SpotBRL Call");

            return await GetResultOfQuikCheckConnection("/api/QuikQAdminFortsApi/CheckConnections/FortsApi");
        }

        [HttpGet("IsQuikApiConnectionOk/ToQadmin/FortsBRL")]
        public async Task<IActionResult> IsQuikApiConnectionOkToQadminFortsBRL()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiConnectionOk/ToQadmin/FortsBRL Call");

            return await GetResultOfQuikCheckConnection("/api/QuikQAdminFortsApi/CheckConnections/FortsApi");
        }

        [HttpGet("IsQuikApiConnectionOk/ToQMonitor")]
        public async Task<IActionResult> IsQuikApiConnectionOkToQMonitor()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiConnectionOk/ToQMonitor Call");

            return await GetResultOfQuikCheckConnection("/api/QuikQMonitor/CheckConnections/QMonitorAPI");
        }

        [HttpGet("IsQuikApiConnectionOk/ToSFTP")]
        public async Task<IActionResult> IsQuikApiConnectionOkToSFTP()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet IsQuikApiConnectionOk/ToSFTP Call");

            return await GetResultOfQuikCheckConnection("/api/QuikSftpServer/CheckConnections/ServerSFTP");
        }

        private async Task<IActionResult> GetResultOfQuikCheckConnection(string apiRequest)
        {
            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + apiRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpGet {apiRequest} succes is {result.IsSuccess}");
                        return Ok(result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} GetResultOfQuikCheckConnection request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString + apiRequest}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpGet GetResultOfQuikCheckConnection request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + apiRequest);
            }

            return Ok(result);
        }
    }
}
