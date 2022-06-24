using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ChildHttpApiRepository
{
    public class HttpApiRepository : IHttpApiRepository
    {
        private ILogger<HttpApiRepository> _logger;
        private HttpConfigurations _connections;

        public HttpApiRepository(ILogger<HttpApiRepository> logger, IOptions<HttpConfigurations> connections)
        {
            _logger = logger;
            _connections = connections.Value;
        }

        public async Task<ClientInformationResponse> GetClientInformation(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientInformation '{clientCode}' Called");

            ClientInformationResponse result = new ClientInformationResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ClientInformationResponse>();

                    _logger.LogInformation($"HttpApiRepository GetClientInformation '{clientCode}' succes is {result.Response.IsSuccess}");

                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository GetClientInformation '{clientCode}' NotFound");
            
            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientInformation '{clientCode}' NotFound");
            
            return result;
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientNonEdpFortsCodes(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientNonEdpFortsCodes '{clientCode}' Called");

            return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/NoEDP/" + clientCode);
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientAllFortsCodes(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientAllFortsCodes '{clientCode}' Called");

            return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/" + clientCode);
        }

        private async Task<MatrixToFortsCodesMappingResponse> GetClientsFortsCodes(string request)
        {
            MatrixToFortsCodesMappingResponse result = new MatrixToFortsCodesMappingResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<MatrixToFortsCodesMappingResponse>();

                    _logger.LogInformation($"HttpApiRepository GetClientsFortsCodes '{request}' succes is {result.Response.IsSuccess}");

                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository GetClientsFortsCodes '{request}' NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientsFortsCodes '{request}' NotFound");

            return result;
        }

        public async Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientBOInformation '{clientCode}' Called");

            ClientBOInformationResponse result = new ClientBOInformationResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ClientBOInformationResponse>();

                    _logger.LogInformation($"HttpApiRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} succes is {result.Response.IsSuccess}");

                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientAllSpotCodesFiltered '{clientCode}' Called");

            MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<MatrixClientCodeModelResponse>();

                    _logger.LogInformation($"HttpApiRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} succes is {result.Response.IsSuccess}");

                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound");

            return result;
        }

        public async Task<NewClientOptionWorkShopModelResponse> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop Called for {newClientModel.CodesPairRF[0].MatrixClientCode}");

            NewClientOptionWorkShopModelResponse result = new NewClientOptionWorkShopModelResponse();
            result.NewOWClient = newClientModel;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string bodyJson = JsonSerializer.Serialize(newClientModel);
                StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop", stringContent);


                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<NewClientOptionWorkShopModelResponse>();

                    _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop success for {newClientModel.CodesPairRF[0].MatrixClientCode}");
                    return result;
                }
            }


            _logger.LogWarning($"HttpApiRepository CreateNewClientOptionWorkshop NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository CreateNewClientOptionWorkshop NotFound");

            return result;
        }
    }
}