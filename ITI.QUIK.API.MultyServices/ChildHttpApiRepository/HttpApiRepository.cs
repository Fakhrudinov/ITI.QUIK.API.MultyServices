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

                        _logger.LogInformation($"HttpApiRepository GetClientInformation '{clientCode}' succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetClientInformation request url NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientInformation request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

                return result;
            }
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

                        _logger.LogInformation($"HttpApiRepository GetClientsFortsCodes '{request}' succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetClientsFortsCodes request url '{request}' NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientsFortsCodes request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + request);

                return result;
            }
        }

        public async Task WarmUpBackOfficeDataBase()
        {
            _logger.LogInformation($"HttpApiRepository WarmUpBackOfficeDataBase Called");

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogInformation($"HttpApiRepository WarmUpBackOfficeDataBase succes status is {response.StatusCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository WarmUpBackOfficeDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository WarmUpBackOfficeDataBase request url NotFound: " + _connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");
                _logger.LogWarning($"HttpApiRepository WarmUpBackOfficeDataBase request Exception is : {ex.Message}");
            }
        }

        public async Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientBOInformation '{clientCode}' Called");

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

                        _logger.LogInformation($"HttpApiRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetClientBOInformation request url /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientBOInformation request url NotFound;  {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);

                return result;
            }
        }

        public async Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetClientAllSpotCodesFiltered '{clientCode}' Called");

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

                        _logger.LogInformation($"HttpApiRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} succes is {result.Response.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.Response.IsSuccess = false;
                        result.Response.Messages.Add($"HttpApiRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetClientAllSpotCodesFiltered request url /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound; {ex.Message}");

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"(404) HttpApiRepository GetClientAllSpotCodesFiltered request url NotFound; {ex.Message}");
                result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);

                return result;
            }
        }

        public async Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop Called for {newClientModel.CodesPairRF[0].MatrixClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository CreateNewClientOptionWorkshop success for {newClientModel.CodesPairRF[0].MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop");

                return result;
            }
        }

        public async Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"HttpApiRepository CreateNewClient Called for {newClientModel.Client.FirstName}");

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

                        _logger.LogInformation($"HttpApiRepository CreateNewClient success for {newClientModel.Client.FirstName}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository CreateNewClient request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository CreateNewClient request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient");

                return result;
            }
        }

        public async Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file)
        {
            _logger.LogInformation($"HttpApiRepository GetResultFromQuikSFTPFileUpload '{file}' Called");

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

                        _logger.LogInformation($"HttpApiRepository GetResultFromQuikSFTPFileUpload succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);

                return result;
            }
        }

        public async Task<ListStringResponseModel> FillCodesIniFile(CodesArrayModel codesArray)
        {
            _logger.LogInformation($"HttpApiRepository FillCodesIniFile Called for {codesArray.ClientCodes[0].MatrixClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository FillCodesIniFile success for {codesArray.ClientCodes[0].MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository FillCodesIniFile request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository FillCodesIniFile request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni");

                return result;
            }
        }

        public async Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newMNPClient)
        {
            _logger.LogInformation($"HttpApiRepository FillDataBaseInstrTW Called for {newMNPClient.Client.FirstName}");

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

                        _logger.LogInformation($"HttpApiRepository FillDataBaseInstrTW success for {newMNPClient.Client.FirstName}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP");

                return result;
            }
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientCodeModel code)
        {
            _logger.LogInformation($"HttpApiRepository AddCdPortfolioToTemplateKomissii Called for {code.MatrixClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository AddCdPortfolioToTemplateKomissii success for {code.MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio");

                return result;
            }
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientCodeModel code)
        {
            _logger.LogInformation($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu Called for {code.MatrixClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu success for {code.MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio");

                return result;
            }
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixPortfolio(string clientPortfolio)
        {
            _logger.LogInformation($"HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio Called for {clientPortfolio}");

            ListStringResponseModel result = new ListStringResponseModel();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCode?MatrixClientCode=" + clientPortfolio);

                    if (response.IsSuccessStatusCode)
                    {
                        result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

                        _logger.LogInformation($"HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsUserAlreadyExistByMatrixPortfolio request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCode?MatrixClientCode=" + clientPortfolio);

                return result;
            }
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"HttpApiRepository GetIsUserAlreadyExistByFortsCode Called for {fortsClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository GetIsUserAlreadyExistByFortsCode succes is {result.IsSuccess}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);

                return result;
            }
        }

        public async Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientCodeModel model)
        {
            _logger.LogInformation($"HttpApiRepository BlockUserByMatrixClientCode Called for {model.MatrixClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository BlockUserByMatrixClientCode success for {model.MatrixClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/MatrixClientCode");

                return result;
            }
        }

        public async Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model)
        {
            _logger.LogInformation($"HttpApiRepository BlockUserByFortsClientCode Called for {model.FortsClientCode}");

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

                        _logger.LogInformation($"HttpApiRepository BlockUserByFortsClientCode success for {model.FortsClientCode}");
                    }
                    else
                    {
                        _logger.LogWarning($"HttpApiRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

                        result.IsSuccess = false;
                        result.Messages.Add($"HttpApiRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpApiRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");

                result.IsSuccess = false;
                result.Messages.Add($"(404) HttpApiRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");
                result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/FortsClientCode");

                return result;
            }
        }
    }
}