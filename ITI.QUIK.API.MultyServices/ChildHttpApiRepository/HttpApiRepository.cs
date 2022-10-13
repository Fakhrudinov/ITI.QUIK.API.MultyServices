using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;
using DataAbstraction.Models.MoneyAndDepo;
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
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientInformation '{clientCode}' Called");

            ClientInformationResponse result = new ClientInformationResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ClientInformationResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientInformation '{clientCode}' succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientInformation request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientInformation request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);
            }

            return result;
        }

        public async Task<BoolResponse> GetIsClientHasOptionWorkshop(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsClientHasOptionWorkshop '{clientCode}' Called");

            BoolResponse result = new BoolResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientBOServices/Get/IsUserHave/OptionWorkshop/" + clientCode);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<BoolResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsClientHasOptionWorkshop '{clientCode}' succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsClientHasOptionWorkshop response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsClientHasOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsClientHasOptionWorkshop request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsClientHasOptionWorkshop request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientBOServices/Get/IsUserHave/OptionWorkshop/" + clientCode);
            }

            return result;
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientNonEdpFortsCodes(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientNonEdpFortsCodes '{clientCode}' Called");

            return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/NoEDP/" + clientCode);
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientAllFortsCodes(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientAllFortsCodes '{clientCode}' Called");

            return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/" + clientCode);
        }

        private async Task<MatrixToFortsCodesMappingResponse> GetClientsFortsCodes(string request)
        {
            MatrixToFortsCodesMappingResponse result = new MatrixToFortsCodesMappingResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<MatrixToFortsCodesMappingResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsFortsCodes '{request}' succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsFortsCodes request url '{request}' NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientsFortsCodes request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + request);
            }

            return result;
        }

        public async Task WarmUpBackOfficeDataBase()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository WarmUpBackOfficeDataBase Called");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository WarmUpBackOfficeDataBase succes status is {response.StatusCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository WarmUpBackOfficeDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository WarmUpBackOfficeDataBase request url NotFound: " + _connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository WarmUpBackOfficeDataBase request Exception is : {ex.Message}");
            }
        }

        public async Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientBOInformation '{clientCode}' Called");

            ClientBOInformationResponse result = new ClientBOInformationResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ClientBOInformationResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientBOInformation request url /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientBOInformation request url NotFound;  {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);
            }

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientAllSpotCodesFiltered '{clientCode}' Called");

            MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<MatrixClientCodeModelResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientAllSpotCodesFiltered request url /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientAllSpotCodesFiltered request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);
            }

            return result;
        }

        public async Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClientOptionWorkshop Called for {newClientModel.CodesPairRF[0].MatrixClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
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

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClientOptionWorkshop success for {newClientModel.CodesPairRF[0].MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop");
            }

            return result;
        }

        public async Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClient Called for {newClientModel.Client.FirstName}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
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

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClient success for {newClientModel.Client.FirstName}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CreateNewClient request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository CreateNewClient request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient");
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetResultFromQuikSFTPFileUpload '{file}' Called");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetResultFromQuikSFTPFileUpload succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);
            }

            return result;
        }

        public async Task<ListStringResponseModel> FillCodesIniFile(CodesArrayModel codesArray)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillCodesIniFile Called for {codesArray.MatrixClientPortfolios[0].MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
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

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillCodesIniFile success for {codesArray.MatrixClientPortfolios[0].MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillCodesIniFile request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository FillCodesIniFile request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni");
            }

            return result;
        }

        public async Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newMNPClient)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillDataBaseInstrTW Called for {newMNPClient.Client.FirstName}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
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

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillDataBaseInstrTW success for {newMNPClient.Client.FirstName}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP");
            }

            return result;
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientPortfolioModel code)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplateKomissii Called for {code.MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(code);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio", stringContent);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplateKomissii success for {code.MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio");
            }

            return result;
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientPortfolioModel code)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplatePoPlechu Called for {code.MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(code);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio", stringContent);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplatePoPlechu success for {code.MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio");
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixClientAccount(string clientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount Called for {clientAccount}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixClientAccount?MatrixClientAccount=" + clientAccount);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsUserAlreadyExistByMatrixClientAccount request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixClientAccount?MatrixClientAccount=" + clientAccount);
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByCodeArray(string [] clientCodesArray)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByCodeArray Called, first code is {clientCodesArray[0]}");

            string codesAtRequest = "codesArray=" + clientCodesArray[0];
            for (int i = 1; i < clientCodesArray.Length; i++)
            {
                codesAtRequest = codesAtRequest + "&codesArray=" + clientCodesArray[i];
            }

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCodesArray?" + codesAtRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByCodeArray succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByCodeArray response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsUserAlreadyExistByCodeArray response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByCodeArray request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsUserAlreadyExistByCodeArray request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCodesArray?" + codesAtRequest);
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByFortsCode Called for {fortsClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByFortsCode succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);
            }

            return result;
        }

        public async Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByMatrixClientCode Called for {model.MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/MatrixClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByMatrixClientCode success for {model.MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/MatrixClientCode");
            }

            return result;
        }
        public async Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByFortsClientCode Called for {model.FortsClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/FortsClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByFortsClientCode success for {model.FortsClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/FortsClientCode");
            }

            return result;
        }
        public async Task<ListStringResponseModel> BlockUserByUID(int uid)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByUID '{uid}' Called");

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.DeleteAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/UID/" + uid);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByUID '{uid}' succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByUID response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository BlockUserByUID response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository BlockUserByUID request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository BlockUserByUID request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/UID/" + uid);
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByMatrixClientCode Called for {model.MatrixClientPortfolio.MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/MatrixClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByMatrixClientCode success for {model.MatrixClientPortfolio.MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetNewPubringKeyByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByMatrixClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetNewPubringKeyByMatrixClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/MatrixClientCode");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByFortsClientCode Called for {model.ClientCode.FortsClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/FortsClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByFortsClientCode success for {model.ClientCode.FortsClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetNewPubringKeyByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNewPubringKeyByFortsClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetNewPubringKeyByFortsClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/FortsClientCode");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByMatrixClientCode Called for {model.MatrixClientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTrades/ByMatrixClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByMatrixClientCode success for {model.MatrixClientPortfolio}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetAllTradesByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByMatrixClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetAllTradesByMatrixClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTrades/ByMatrixClientCode");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByFortsClientCode Called for {model.FortsClientCode}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Put,
                        RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTradesBy/FortsClientCode"),
                        Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                    };
                    var response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByFortsClientCode success for {model.FortsClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetAllTradesByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetAllTradesByFortsClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetAllTradesByFortsClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTradesBy/FortsClientCode");
            }

            return result;
        }

        public async Task<ListStringResponseModel> GenerateNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GenerateNewFileCurrClnts Called");

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/RequestFile/CurrClnts");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GenerateNewFileCurrClnts succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GenerateNewFileCurrClnts response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GenerateNewFileCurrClnts response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GenerateNewFileCurrClnts request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GenerateNewFileCurrClnts request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/RequestFile/CurrClnts");
            }

            return result;
        }

        public async Task DownloadNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadNewFileCurrClnts Called");

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/CurrClnts");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadNewFileCurrClnts succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadNewFileCurrClnts response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadNewFileCurrClnts request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString}/api/QuikSftpServer/DownloadFile/CurrClnts");
            }
        }

        public async Task<ListStringResponseModel> DownloadLimLimFile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadLimLimFile Called");

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/LimLim");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadLimLimFile succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadLimLimFile response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository DownloadLimLimFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository DownloadLimLimFile request url NotFound; {ex.Message}");
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString}/api/QuikSftpServer/DownloadFile/LimLim");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository DownloadLimLimFile request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/LimLim");
            }

            return result;
        }

        public async Task<InstrTWDataBaseRecords> GetRecordsFromInstrTwDataBase(List<string> allportfolios)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRecordsFromInstrTwDataBase Called for {allportfolios[0]}");

            InstrTWDataBaseRecords result = new InstrTWDataBaseRecords();

            //http://localhost:5146/api/QuikDataBase/Get/RegisteredCodes?codes=string1&codes=string2
            string codesAtRequest = "codes=" + allportfolios[0];
            for (int i = 1; i < allportfolios.Count; i++)
            {
                codesAtRequest = codesAtRequest + "&codes=" + allportfolios[i];
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Get/RegisteredCodes?" + codesAtRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<InstrTWDataBaseRecords>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRecordsFromInstrTwDataBase succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRecordsFromInstrTwDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetRecordsFromInstrTwDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRecordsFromInstrTwDataBase request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetRecordsFromInstrTwDataBase request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Get/RegisteredCodes?" + codesAtRequest);
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetAllClientsFromTemplatePoKomissii(string templateName)
        {
            //http://localhost:8753/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/templateName
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoKomissii Called for {templateName}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/" + templateName);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoKomissii succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetAllClientsFromTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository CheckCdPortfolioExistAtTemplatePoKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository CheckCdPortfolioExistAtTemplatePoKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/" + templateName);
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetAllClientsFromTemplatePoPlechu(string templateName)
        {
            //http://localhost:8753/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/templateName
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoPlechu Called for {templateName}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/" + templateName);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoPlechu succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetAllClientsFromTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllClientsFromTemplatePoPlechu request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetAllClientsFromTemplatePoPlechu request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/" + templateName);
            }

            return result;
        }

        public async Task<BoolResponse> GetIsAllSpotPortfoliosPresentInFileCodesIni(List<string> allportfolios)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni Called for {allportfolios[0]}");

            BoolResponse result = new BoolResponse();

            //http://localhost:5146/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?codesArray=string1&codesArray=string2
            string codesAtRequest = "codesArray=" + allportfolios[0];
            for (int i = 1; i < allportfolios.Count; i++)
            {
                codesAtRequest = codesAtRequest + "&codesArray=" + allportfolios[i];
            }

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?" + codesAtRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<BoolResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsAllSpotPortfoliosPresentInFileCodesIni request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?" + codesAtRequest);
            }

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllEnemyNonResidentSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Spot/Portfolios");

            return result;
        }

        //public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentSpotPortfolios()
        //{
        //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllFrendlyNonResidentSpotPortfolios Called");

        //    MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Spot/Portfolios");

        //    return result;
        //}

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllFrendlyNonResidentKvalSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Kval/Spot/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentNonKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllFrendlyNonResidentKvalSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/NonKval/Spot/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalKsurUsersSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Spot/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalKpurUsersSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Spot/Portfolios");

            return result;           
        }

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllFrendlyNonResidentCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllEnemyNonResidentCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllKvalKpurUsersSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KPUR/Spot/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllKvalKpurUsersCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KPUR/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKsurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllKvalKsurUsersCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KSUR/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalKpurUsersCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalKsurUsersCdPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Cd/Portfolios");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllRestrictedCDPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllRestrictedCDPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/TemplatesPoKomissii/GetAll/Restricted/CD/Portfolios/ForCD_Restrict");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllAllowedCDPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllAllowedCDPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/TemplatesPoKomissii/GetAll/Allowed/CD/Portfolios/ForCD_portfolio");

            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllKvalSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/Spot/Portfolios");

            return result;
        }

        private async Task<MatrixClientCodeModelResponse> GetPortfoliosByApiLink(string apiLink)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetPortfoliosByApiLink Called for {apiLink}");

            MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + apiLink);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<MatrixClientCodeModelResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetPortfoliosByApiLink {apiLink} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetMsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetMsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + apiLink);
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetClientsToTemplatePoKomissii(TemplateAndMatrixCodesModel templateAndMatrixCodes)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToTemplatePoKomissii Called for {templateAndMatrixCodes.Template}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(templateAndMatrixCodes);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAllCodesMatrixInTemplate/PoKomisii", stringContent);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToTemplatePoKomissii success for {templateAndMatrixCodes.Template}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToTemplatePoKomissii response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetClientsToTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToTemplatePoKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetClientsToTemplatePoKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAllCodesMatrixInTemplate/PoKomisii");
            }

            return result;
        }

        public async Task<FortsClientCodeModelResponse> GetAllEnemyNonResidentFortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllEnemyNonResidentFortsCodes Called");

            FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Forts/Codes");

            return result;
        }

        public async Task<FortsClientCodeModelResponse> GetAllKvalClientsFortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllKvalClientsFortsCodes Called");

            FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/Forts/Codes");

            return result;
        }

        public async Task<FortsClientCodeModelResponse> GetAllNonKvalWithTest16FortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalWithTest16FortsCodes Called");

            FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/WithTest16/Forts/Codes");

            return result;
        }

        private async Task<FortsClientCodeModelResponse> GetFortsPortfoliosByApiLink(string apiLink)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetFortsPortfoliosByApiLink Called for {apiLink}");

            FortsClientCodeModelResponse result = new FortsClientCodeModelResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + apiLink);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<FortsClientCodeModelResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetFortsPortfoliosByApiLink {apiLink} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetFortsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetFortsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetFortsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetFortsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + apiLink);
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetClientsToFortsTemplatePoKomissii(TemplateAndMatrixFortsCodesModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToFortsTemplatePoKomissii Called for {model.Template}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(model);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminFortsApi/ReplaceAll/MatrixFortsCode/InTemplate/PoKomisii", stringContent);


                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToFortsTemplatePoKomissii success for {model.Template}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToFortsTemplatePoKomissii response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetClientsToFortsTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetClientsToFortsTemplatePoKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetClientsToFortsTemplatePoKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminFortsApi/ReplaceAll/MatrixFortsCode/InTemplate/PoKomisii");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetKvalClientsToComplexProductRestrictions(CodesArrayModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetKvalClientsToComplexProductRestrictions Called");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(model);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/KvalInvestorsList", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetKvalClientsToComplexProductRestrictions success");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetKvalClientsToComplexProductRestrictions response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetKvalClientsToComplexProductRestrictions response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetKvalClientsToComplexProductRestrictions request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetKvalClientsToComplexProductRestrictions request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/KvalInvestorsList");
            }

            return result;
        }

        public async Task<PortfoliosAndTestForComplexProductResponse> GetAllNonKvalWithTestsSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios Called");

            PortfoliosAndTestForComplexProductResponse result = new PortfoliosAndTestForComplexProductResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/KvalInvestors/GetAll/NonKvalUsers/SpotPortfolios/and/TestForComplexProduct");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<PortfoliosAndTestForComplexProductResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetAllNonKvalWithTestsSpotPortfolios request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/KvalInvestors/GetAll/NonKvalUsers/SpotPortfolios/and/TestForComplexProduct");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetNonKvalClientsWithTestsToComplexProductRestrictions(QCodeAndListOfComplexProductsTestsModel[] model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions Called");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(model);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/NonKvalInvestorsWithTestsArray", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions success");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetNonKvalClientsWithTestsToComplexProductRestrictions request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/NonKvalInvestorsWithTestsArray");
            }

            return result;
        }

        public async Task<SecurityAndBoardResponse> GetRestrictedSecuritiesAndBoards()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRestrictedSecuritiesAndBoards Called");

            SecurityAndBoardResponse result = new SecurityAndBoardResponse();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/Securities/Get/Securities/SpotBlackList/ForNekval");

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<SecurityAndBoardResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRestrictedSecuritiesAndBoards succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRestrictedSecuritiesAndBoards response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetRestrictedSecuritiesAndBoards response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetRestrictedSecuritiesAndBoards request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetRestrictedSecuritiesAndBoards request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/Securities/Get/Securities/SpotBlackList/ForNekval");
            }

            return result;
        }

        public async Task<ListStringResponseModel> SetRestrictedSecuritiesInTemplatesPoKomissii(RestrictedSecuritiesArraySetForBoardInTemplatesModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii Called");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string bodyJson = JsonSerializer.Serialize(model);
                    StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAll/RestrictedSecurities/InTemplate/PoKomisii", stringContent);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii success");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository SetRestrictedSecuritiesInTemplatesPoKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAll/RestrictedSecurities/InTemplate/PoKomisii");
            }

            return result;
        }

        public async Task<ClientAndMoneyResponse> GetClientsSpotPortfoliosWhoTradesYesterday(int daysShift)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday '{daysShift}' Called");

            ClientAndMoneyResponse result = new ClientAndMoneyResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/WhoTrade/SpotPortfoliosAndMoney/" + daysShift);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ClientAndMoneyResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday '{daysShift}' " +
                            $"succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday response is" +
                            $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday " +
                    $"request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientsSpotPortfoliosWhoTradesYesterday request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/WhoTrade/SpotPortfoliosAndMoney/" + daysShift);
            }

            return result;
        }

        public async Task<ClientDepoPositionsResponse> GetClientsPositionsByMatrixPortfolioList(string portfoliosToRequest)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsPositionsByMatrixPortfolioList Called with " + portfoliosToRequest);

            ClientDepoPositionsResponse result = new ClientDepoPositionsResponse();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/Positions/ByMatrixPortfolioList?" + portfoliosToRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ClientDepoPositionsResponse>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsPositionsByMatrixPortfolioList " +
                            $"succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsPositionsByMatrixPortfolioList response is" +
                            $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientsPositionsByMatrixPortfolioList response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetClientsPositionsByMatrixPortfolioList " +
                    $"request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientsPositionsByMatrixPortfolioList request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/Positions/ByMatrixPortfolioList?" + portfoliosToRequest);
            }

            return result;
        }

        public async Task<ListStringResponseModel> GetSftpFileLastWriteTime(string fileNameOrPath)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetSftpFileLastWriteTime Called with " + fileNameOrPath);

            ListStringResponseModel result = new ListStringResponseModel();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/Get/FileInfo/ByPath/" + fileNameOrPath);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetSftpFileLastWriteTime " +
                            $"succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetSftpFileLastWriteTime response is" +
                            $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetSftpFileLastWriteTime response is " +
                            $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpApiRepository GetSftpFileLastWriteTime " +
                    $"request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetSftpFileLastWriteTime request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/Get/FileInfo/ByPath/" + fileNameOrPath);
            }

            return result;
        }
    }
}