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

            _logger.LogWarning($"HttpApiRepository GetClientInformation request url NotFound");
            
            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientInformation request url NotFound");
            result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

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

            _logger.LogWarning($"HttpApiRepository GetClientsFortsCodes request url '{request}' NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientsFortsCodes request url NotFound");
            result.Response.Messages.Add(_connections.MatrixAPIConnectionString + request);

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

            _logger.LogWarning($"HttpApiRepository GetClientBOInformation request url /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientBOInformation request url NotFound");
            result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);

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

            _logger.LogWarning($"HttpApiRepository GetClientAllSpotCodesFiltered request url /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound");

            result.Response.IsSuccess = false;
            result.Response.Messages.Add($"(404) HttpApiRepository GetClientAllSpotCodesFiltered request url NotFound");
            result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);

            return result;
        }

        public async Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop Called for {newClientModel.CodesPairRF[0].MatrixClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string bodyJson = JsonSerializer.Serialize(newClientModel);
                StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop", stringContent);


                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop success for {newClientModel.CodesPairRF[0].MatrixClientCode}");
                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository CreateNewClientOptionWorkshop request url NotFound");

            result.IsSuccess = false;
            result.Messages.Add($"(404) HttpApiRepository CreateNewClientOptionWorkshop request url NotFound");
            result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop");

            return result;
        }

        public async Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository CreateNewClient Called for {newClientModel.Client.FirstName}");

            ListStringResponseModel result = new ListStringResponseModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string bodyJson = JsonSerializer.Serialize(newClientModel);
                StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient", stringContent);


                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation($"HttpApiRepository CreateNewClient success for {newClientModel.Client.FirstName}");
                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository CreateNewClient request url NotFound");

            result.IsSuccess = false;
            result.Messages.Add($"(404) HttpApiRepository CreateNewClient request url NotFound");
            result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient");

            return result;
        }

        public async Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file)
        {
            _logger.LogInformation($"HttpApiRepository GetResultFromQuikSFTPFileUpload '{file}' Called");

            ListStringResponseModel result = new ListStringResponseModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation($"HttpApiRepository GetResultFromQuikSFTPFileUpload succes is {result.IsSuccess}");

                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound");

            result.IsSuccess = false;
            result.Messages.Add($"(404) HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound");
            result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);

            return result;
        }

        public async Task<ListStringResponseModel> FillCodesIniFile(NewClientModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository FillCodesIniFile Called for {newClientModel.Client.FirstName}");

            ListStringResponseModel result = new ListStringResponseModel();

            if(newClientModel.CodesMatrix != null)
            {
                CodesArrayModel codesArray = new CodesArrayModel();
                codesArray.ClientCodes = new MatrixClientCodeModel[newClientModel.CodesMatrix.Length];

                for (int i = 0; i < newClientModel.CodesMatrix.Length; i++)
                {
                    codesArray.ClientCodes[i] = newClientModel.CodesMatrix[i];
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(codesArray);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PutAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni", stringContent);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"HttpApiRepository FillCodesIniFile success for {newClientModel.Client.FirstName}");
                        return result;
                    }
                }

                _logger.LogWarning($"HttpApiRepository FillCodesIniFile request url NotFound");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository FillCodesIniFile request url NotFound");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni");
            }
            else
            {
                result.Messages.Add($"No action requared - there is no MS FX RS CD portfolios");
            }

            return result;
        }

        public async Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newMNPClient)
        {
            _logger.LogInformation($"HttpApiRepository FillDataBaseInstrTW Called for {newMNPClient.Client.FirstName}");

            ListStringResponseModel result = new ListStringResponseModel();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string bodyJson = JsonSerializer.Serialize(newMNPClient);
                StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP", stringContent);


                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                    _logger.LogInformation($"HttpApiRepository FillDataBaseInstrTW success for {newMNPClient.Client.FirstName}");
                    return result;
                }
            }

            _logger.LogWarning($"HttpApiRepository FillDataBaseInstrTW request url NotFound");

            result.IsSuccess = false;
            result.Messages.Add($"(404) HttpApiRepository FillDataBaseInstrTW request url NotFound");
            result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP");

            return result;
        }
    }
}