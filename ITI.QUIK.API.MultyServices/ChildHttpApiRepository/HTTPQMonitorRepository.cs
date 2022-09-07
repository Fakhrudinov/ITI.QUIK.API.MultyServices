using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ChildHttpApiRepository
{
    public class HTTPQMonitorRepository : IHTTPQMonitorRepository
    {
        private ILogger<HTTPQMonitorRepository> _logger;
        string _connection;

        public HTTPQMonitorRepository(ILogger<HTTPQMonitorRepository> logger, IOptions<HttpConfigurations> connections)
        {
            _logger = logger;
            _connection = connections.Value.QuikAPIConnectionString;
        }

        public async Task<ListStringResponseModel> ReloadFortsBrlSPBFUT()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HTTPQMonitorRepository ReloadFortsBrlSPBFUT Called");

            return await ReloadDealerLibByLink("/api/QuikQMonitor/ReloadDealerLib/Forts");
        }

        public async Task<ListStringResponseModel> ReloadSpotBrlMC013820000()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HTTPQMonitorRepository ReloadSpotBrlMC013820000 Called");

            return await ReloadDealerLibByLink("/api/QuikQMonitor/ReloadDealerLib/Spot");
        }

        private async Task<ListStringResponseModel> ReloadDealerLibByLink(string request)
        {
            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connection);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connection + request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HTTPQMonitorRepository ReloadDealerLibByLink '{request}' succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HTTPQMonitorRepository ReloadDealerLibByLink response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HTTPQMonitorRepository ReloadDealerLibByLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HTTPQMonitorRepository ReloadDealerLibByLink request url '{request}' NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HTTPQMonitorRepository ReloadDealerLibByLink request url NotFound; {ex.Message}");
                result.Messages.Add(_connection + request);
            }

            return result;
        }
    }
}
