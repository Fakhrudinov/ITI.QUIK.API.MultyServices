using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;
//using System.Net.Http.Headers;
//using System.Net.Http.Json;
//using System.Text;
using System.Text.Json;
using DataAbstraction.Models.Discounts;
using System.Reflection;

namespace ChildHttpMatrixRepository
{
    public class HttpQuikRepository : IHttpQuikRepository
    {
        private ILogger<HttpQuikRepository> _logger;
        private string _connection;
        private HttpApiExecutiveRepository _executiveRepo;

        public HttpQuikRepository(
            ILogger<HttpQuikRepository> logger, 
            IOptions<HttpConfigurations> connections,
            HttpApiExecutiveRepository executiveRepo)
        {
            _logger = logger;
            _connection = connections.Value.QuikAPIConnectionString;
            _executiveRepo= executiveRepo;
        }

        public async Task<ListStringResponseModel> ReloadFortsBrlSPBFUT()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpQuikRepository ReloadFortsBrlSPBFUT Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection, 
                "/api/QuikQMonitor/ReloadDealerLib/Forts");
            return result;

            //return await ReloadDealerLibByLink("/api/QuikQMonitor/ReloadDealerLib/Forts");
        }

        public async Task<ListStringResponseModel> ReloadSpotBrlMC013820000()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpQuikRepository ReloadSpotBrlMC013820000 Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikQMonitor/ReloadDealerLib/Spot");
            return result;

            //return await ReloadDealerLibByLink("/api/QuikQMonitor/ReloadDealerLib/Spot");
        }

        //private async Task<ListStringResponseModel> ReloadDealerLibByLink(string request)
        //{
        //    ListStringResponseModel result = new ListStringResponseModel();

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_connection);
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var response = await client.GetAsync(_connection + request);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

        //                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpQuikRepository ReloadDealerLibByLink '{request}' succes is {result.IsSuccess}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpQuikRepository ReloadDealerLibByLink response is " +
        //                    $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

        //                result.IsSuccess = false;
        //                result.Messages.Add($"HttpQuikRepository ReloadDealerLibByLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpQuikRepository ReloadDealerLibByLink request url '{request}' NotFound; {ex.Message}");

        //        result.IsSuccess = false;
        //        result.Messages.Add($"(404) HttpQuikRepository ReloadDealerLibByLink request url NotFound; {ex.Message}");
        //        result.Messages.Add(_connection + request);
        //    }

        //    return result;
        //}

        public async Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetResultFromQuikSFTPFileUpload '{file}' Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetResultFromQuikSFTPFileUpload succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetResultFromQuikSFTPFileUpload response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetResultFromQuikSFTPFileUpload request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/GetResultOfXMLFileUpload?file=" + file);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> FillCodesIniFile(CodesArrayModel codesArray)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillCodesIniFile Called for {codesArray.MatrixClientPortfolios[0].MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(codesArray);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/AddClientCodesToFileCodesIni");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(codesArray);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PutAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillCodesIniFile success for {codesArray.MatrixClientPortfolios[0].MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository FillCodesIniFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillCodesIniFile request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository FillCodesIniFile request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddClientCodesToFileCodesIni");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixClientAccount(string clientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount Called for {clientAccount}");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/UID/byMatrixClientAccount?MatrixClientAccount=" + clientAccount);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixClientAccount?MatrixClientAccount=" + clientAccount);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetIsUserAlreadyExistByMatrixClientAccount request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixClientAccount?MatrixClientAccount=" + clientAccount);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByCodeArray(string[] clientCodesArray)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByCodeArray Called, first code is {clientCodesArray[0]}");

            string codesAtRequest = "codesArray=" + clientCodesArray[0];
            for (int i = 1; i < clientCodesArray.Length; i++)
            {
                codesAtRequest = codesAtRequest + "&codesArray=" + clientCodesArray[i];
            }

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/UID/byMatrixCodesArray?" + codesAtRequest);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCodesArray?" + codesAtRequest);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByCodeArray succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByCodeArray response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetIsUserAlreadyExistByCodeArray response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByCodeArray request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetIsUserAlreadyExistByCodeArray request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byMatrixCodesArray?" + codesAtRequest);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByFortsCode Called for {fortsClientCode}");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByFortsCode succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetIsUserAlreadyExistByFortsCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetIsUserAlreadyExistByFortsCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/UID/byFortsCode?FortsClientCode=" + fortsClientCode);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByMatrixClientCode Called for {model.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Delete,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/BlockUserBy/MatrixClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Delete,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/MatrixClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByMatrixClientCode success for {model.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository BlockUserByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository BlockUserByMatrixClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/MatrixClientCode");
            //}

            //return result;
        }
        public async Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByFortsClientCode Called for {model.FortsClientCode}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Delete,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/BlockUserBy/FortsClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Delete,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/FortsClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByFortsClientCode success for {model.FortsClientCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository BlockUserByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository BlockUserByFortsClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/FortsClientCode");
            //}

            //return result;
        }
        public async Task<ListStringResponseModel> BlockUserByUID(int uid)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByUID '{uid}' Called");

            string bodyJson = JsonSerializer.Serialize("");
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Delete,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/BlockUserBy/UID/" + uid);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.DeleteAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/UID/" + uid);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByUID '{uid}' succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByUID response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository BlockUserByUID response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository BlockUserByUID request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository BlockUserByUID request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/BlockUserBy/UID/" + uid);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByMatrixClientCode Called for " +
                $"{model.MatrixClientPortfolio.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/SetNewPubringKeyBy/MatrixClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Put,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/MatrixClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByMatrixClientCode success for {model.MatrixClientPortfolio.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetNewPubringKeyByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByMatrixClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetNewPubringKeyByMatrixClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/MatrixClientCode");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByFortsClientCode Called for {model.ClientCode.FortsClientCode}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/SetNewPubringKeyBy/FortsClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Put,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/FortsClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByFortsClientCode success for {model.ClientCode.FortsClientCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetNewPubringKeyByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNewPubringKeyByFortsClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetNewPubringKeyByFortsClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetNewPubringKeyBy/FortsClientCode");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByMatrixClientCode Called" +
                $" for {model.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/SetAllTrades/ByMatrixClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Put,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTrades/ByMatrixClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByMatrixClientCode success for {model.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetAllTradesByMatrixClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByMatrixClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetAllTradesByMatrixClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTrades/ByMatrixClientCode");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByFortsClientCode Called" +
                $" for {model.FortsClientCode}");

            string bodyJson = JsonSerializer.Serialize(model);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/SetAllTradesBy/FortsClientCode");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var request = new HttpRequestMessage
            //        {
            //            Method = HttpMethod.Put,
            //            RequestUri = new Uri(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTradesBy/FortsClientCode"),
            //            Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            //        };
            //        var response = await client.SendAsync(request);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByFortsClientCode success for {model.FortsClientCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetAllTradesByFortsClientCode response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetAllTradesByFortsClientCode request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetAllTradesByFortsClientCode request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/SetAllTradesBy/FortsClientCode");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GenerateNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GenerateNewFileCurrClnts Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/RequestFile/CurrClnts");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/RequestFile/CurrClnts");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GenerateNewFileCurrClnts succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GenerateNewFileCurrClnts response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GenerateNewFileCurrClnts response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GenerateNewFileCurrClnts request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GenerateNewFileCurrClnts request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/RequestFile/CurrClnts");
            //}

            //return result;
        }
        public async Task DownloadNewFileCurrClnts()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadNewFileCurrClnts Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/DownloadFile/CurrClnts");

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/CurrClnts");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadNewFileCurrClnts succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadNewFileCurrClnts response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadNewFileCurrClnts request url NotFound; {ex.Message}");
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString}/api/QuikSftpServer/DownloadFile/CurrClnts");
            //}
        }

        public async Task<ListStringResponseModel> DownloadLimLimFile()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadLimLimFile Called");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/DownloadFile/LimLim");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/LimLim");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadLimLimFile succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadLimLimFile response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository DownloadLimLimFile response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DownloadLimLimFile request url NotFound; {ex.Message}");
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} {_connections.QuikAPIConnectionString}/api/QuikSftpServer/DownloadFile/LimLim");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository DownloadLimLimFile request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/DownloadFile/LimLim");
            //}

            //return result;
        }

        public async Task<InstrTWDataBaseRecords> GetRecordsFromInstrTwDataBase(List<string> allportfolios)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRecordsFromInstrTwDataBase Called for {allportfolios[0]}");

            //InstrTWDataBaseRecords result = new InstrTWDataBaseRecords();

            //http://localhost:5146/api/QuikDataBase/Get/RegisteredCodes?codes=string1&codes=string2
            string codesAtRequest = "codes=" + allportfolios[0];
            for (int i = 1; i < allportfolios.Count; i++)
            {
                codesAtRequest = codesAtRequest + "&codes=" + allportfolios[i];
            }

            InstrTWDataBaseRecords result = await _executiveRepo.GetTDirectResponse<InstrTWDataBaseRecords>(
                _connection,
                "/api/QuikDataBase/Get/RegisteredCodes?" + codesAtRequest);
            return result;

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Get/RegisteredCodes?" + codesAtRequest);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<InstrTWDataBaseRecords>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRecordsFromInstrTwDataBase succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRecordsFromInstrTwDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetRecordsFromInstrTwDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRecordsFromInstrTwDataBase request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetRecordsFromInstrTwDataBase request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Get/RegisteredCodes?" + codesAtRequest);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GetAllClientsFromTemplatePoKomissii(string templateName)
        {
            //http://localhost:8753/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/templateName
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoKomissii Called for {templateName}");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/" + templateName);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/" + templateName);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoKomissii succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetAllClientsFromTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CheckCdPortfolioExistAtTemplatePoKomissii request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository CheckCdPortfolioExistAtTemplatePoKomissii request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoKomissii/" + templateName);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> GetAllClientsFromTemplatePoPlechu(string templateName)
        {
            //http://localhost:8753/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/templateName
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoPlechu Called for {templateName}");

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/" + templateName);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/" + templateName);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoPlechu succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetAllClientsFromTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllClientsFromTemplatePoPlechu request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetAllClientsFromTemplatePoPlechu request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/GetAllClientsFromTemplate/PoPlechu/" + templateName);
            //}

            //return result;
        }
        public async Task<ListStringResponseModel> GetSftpFileLastWriteTime(string fileNameOrPath)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetSftpFileLastWriteTime Called with " + fileNameOrPath);

            ListStringResponseModel result = await _executiveRepo.GetTDirectResponse<ListStringResponseModel>(
                _connection,
                "/api/QuikSftpServer/Get/FileInfo/ByPath/" + fileNameOrPath);
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/Get/FileInfo/ByPath/" + fileNameOrPath);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetSftpFileLastWriteTime " +
            //                $"succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetSftpFileLastWriteTime response is" +
            //                $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetSftpFileLastWriteTime response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetSftpFileLastWriteTime " +
            //        $"request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetSftpFileLastWriteTime request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/Get/FileInfo/ByPath/" + fileNameOrPath);
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> AddNewMatrixPortfolioToExistingClientByUID(MatrixPortfolioAndUidModel matrixPortfolioAndUid)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID Called for " +
                $"UID={matrixPortfolioAndUid.UID} portfolio={matrixPortfolioAndUid.MatrixPortfolio.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(matrixPortfolioAndUid);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/AddNew/MatrixPortfolio/ToExistClient/ByUID");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(matrixPortfolioAndUid);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PutAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddNew/MatrixPortfolio/ToExistClient/ByUID", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID success for " +
            //                $"UID={matrixPortfolioAndUid.UID} portfolio={matrixPortfolioAndUid.MatrixPortfolio.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID " +
            //                $"response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddNew/MatrixPortfolio/ToExistClient/ByUID");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> AddNewFortsPortfolioToExistingClientByUID(FortsCodeAndUidModel fortsCodeAndUid)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewFortsPortfolioToExistingClientByUID Called for " +
                $"UID={fortsCodeAndUid.UID} portfolio={fortsCodeAndUid.MatrixFortsCode}");

            string bodyJson = JsonSerializer.Serialize(fortsCodeAndUid);
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Put,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/AddNew/MatrixFortsCode/ToExistClient/ByUID");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(fortsCodeAndUid);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PutAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddNew/MatrixFortsCode/ToExistClient/ByUID", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewFortsPortfolioToExistingClientByUID success for " +
            //                $"UID={fortsCodeAndUid.UID} portfolio={fortsCodeAndUid.MatrixFortsCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewFortsPortfolioToExistingClientByUID " +
            //                $"response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository AddNewMatrixPortfolioToExistingClientByUID response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddNewFortsPortfolioToExistingClientByUID request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository AddNewFortsPortfolioToExistingClientByUID request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/AddNew/MatrixFortsCode/ToExistClient/ByUID");
            //}

            //return result;
        }
        public async Task<BoolResponse> GetIsAllSpotPortfoliosPresentInFileCodesIni(List<string> allportfolios)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni Called for {allportfolios[0]}");

            //BoolResponse result = new BoolResponse();

            //http://localhost:5146/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?codesArray=string1&codesArray=string2
            string codesAtRequest = "codesArray=" + allportfolios[0];
            for (int i = 1; i < allportfolios.Count; i++)
            {
                codesAtRequest = codesAtRequest + "&codesArray=" + allportfolios[i];
            }

            BoolResponse result = await _executiveRepo.GetTDirectResponse<BoolResponse>(
                _connection,
                "/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?" + codesAtRequest);
            return result;

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?" + codesAtRequest);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<BoolResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetIsAllSpotPortfoliosPresentInFileCodesIni request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/ClientCodes/IsPresentIn/FileCodesIni?" + codesAtRequest);
            //}

            //return result;
        }
        public async Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClientOptionWorkshop Called for {newClientModel.CodesPairRF[0].MatrixClientCode}");

            string bodyJson = JsonSerializer.Serialize(newClientModel);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/NewClient/OptionWorkshop");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(newClientModel);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClientOptionWorkshop success for {newClientModel.CodesPairRF[0].MatrixClientCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository CreateNewClientOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository CreateNewClientOptionWorkshop request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient/OptionWorkshop");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClient Called for {newClientModel.Client.FirstName}");

            string bodyJson = JsonSerializer.Serialize(newClientModel);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikSftpServer/NewClient");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(newClientModel);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClient success for {newClientModel.Client.FirstName}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository CreateNewClient response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository CreateNewClient request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository CreateNewClient request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikSftpServer/NewClient");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newMNPClient)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillDataBaseInstrTW Called for {newMNPClient.Client.FirstName}");

            string bodyJson = JsonSerializer.Serialize(newMNPClient);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikDataBase/Set/NewClient/ToMNP");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(newMNPClient);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillDataBaseInstrTW success for {newMNPClient.Client.FirstName}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository FillDataBaseInstrTW response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository FillDataBaseInstrTW request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikDataBase/Set/NewClient/ToMNP");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientPortfolioModel code)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplateKomissii Called " +
                $"for {code.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(code);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(code);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplateKomissii success for {code.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository AddCdPortfolioToTemplateKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository AddCdPortfolioToTemplateKomissii request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/KomissiiTemplate/CD_portfolio");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientPortfolioModel code)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu Called " +
                $"for {code.MatrixClientPortfolio}");

            string bodyJson = JsonSerializer.Serialize(code);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(code);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu success for {code.MatrixClientPortfolio}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository AddCdPortfolioToTemplatePoPlechu request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/AddMatrixClientPortfolioTo/PoPlechuTemplate/CD_portfolio");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetClientsToTemplatePoKomissii(TemplateAndMatrixCodesModel templateAndMatrixCodes)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToTemplatePoKomissii Called " +
                $"for {templateAndMatrixCodes.Template}");

            string bodyJson = JsonSerializer.Serialize(templateAndMatrixCodes);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikQAdminSpotApi/ReplaceAllCodesMatrixInTemplate/PoKomisii");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(templateAndMatrixCodes);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAllCodesMatrixInTemplate/PoKomisii", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToTemplatePoKomissii success for {templateAndMatrixCodes.Template}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToTemplatePoKomissii response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetClientsToTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToTemplatePoKomissii request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetClientsToTemplatePoKomissii request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAllCodesMatrixInTemplate/PoKomisii");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetClientsToFortsTemplatePoKomissii(TemplateAndMatrixFortsCodesModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToFortsTemplatePoKomissii Called " +
                $"for {model.Template}");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikQAdminFortsApi/ReplaceAll/MatrixFortsCode/InTemplate/PoKomisii");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(model);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminFortsApi/ReplaceAll/MatrixFortsCode/InTemplate/PoKomisii", stringContent);


            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToFortsTemplatePoKomissii success for {model.Template}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToFortsTemplatePoKomissii response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetClientsToFortsTemplatePoKomissii response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetClientsToFortsTemplatePoKomissii request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetClientsToFortsTemplatePoKomissii request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminFortsApi/ReplaceAll/MatrixFortsCode/InTemplate/PoKomisii");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetKvalClientsToComplexProductRestrictions(CodesArrayModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetKvalClientsToComplexProductRestrictions Called");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/KvalAndTests/Replace/KvalInvestorsList");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(model);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/KvalInvestorsList", stringContent);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetKvalClientsToComplexProductRestrictions success");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetKvalClientsToComplexProductRestrictions response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetKvalClientsToComplexProductRestrictions response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetKvalClientsToComplexProductRestrictions request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetKvalClientsToComplexProductRestrictions request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/KvalInvestorsList");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetNonKvalClientsWithTestsToComplexProductRestrictions(QCodeAndListOfComplexProductsTestsModel[] model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions Called");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/KvalAndTests/Replace/NonKvalInvestorsWithTestsArray");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(model);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/NonKvalInvestorsWithTestsArray", stringContent);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions success");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetNonKvalClientsWithTestsToComplexProductRestrictions request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/KvalAndTests/Replace/NonKvalInvestorsWithTestsArray");
            //}

            //return result;
        }

        public async Task<ListStringResponseModel> SetRestrictedSecuritiesInTemplatesPoKomissii(RestrictedSecuritiesArraySetForBoardInTemplatesModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii Called");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/QuikQAdminSpotApi/ReplaceAll/RestrictedSecurities/InTemplate/PoKomisii");
            return result;

            //ListStringResponseModel result = new ListStringResponseModel();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.QuikAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        string bodyJson = JsonSerializer.Serialize(model);
            //        StringContent stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            //        var response = await client.PostAsync(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAll/RestrictedSecurities/InTemplate/PoKomisii", stringContent);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ListStringResponseModel>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii success");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository SetRestrictedSecuritiesInTemplatesPoKomissii request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.QuikAPIConnectionString + "/api/QuikQAdminSpotApi/ReplaceAll/RestrictedSecurities/InTemplate/PoKomisii");
            //}

            //return result;
        }

        public async Task<DiscountSingleResponse> GetDiscountSingleFromGlobal(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetDiscountSingleFromGlobal Called for {security}");

            DiscountSingleResponse result = await _executiveRepo.GetTDirectResponse<DiscountSingleResponse>(
                _connection,
                "/api/Discounts/Get/SingleDiscount/FromGlobal/" + security);
            return result;
        }

        public async Task<DiscountSingleResponse> GetDiscountSingleFromMarginTemplate(string template, string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetDiscountSingleFromMarginTemplate " +
                $"Called for {template}/{security}");

            DiscountSingleResponse result = await _executiveRepo.GetTDirectResponse<DiscountSingleResponse>(
                _connection,
                $"/api/Discounts/Get/SingleDiscount/FromMarginTemplate/{template}/{security}");
            return result;
        }

        public async Task<ListStringResponseModel> PostSingleDiscountToGlobal(DiscountAndSecurityModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository PostSingleDiscountToGlobal {model.Security} Called");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/Discounts/Post/SingleDiscount/ToGlobal");
            return result;
        }

        public async Task<ListStringResponseModel> PostSingleDiscountToTemplate(string template, DiscountAndSecurityModel model)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository PostSingleDiscountToTemplate {model.Security} Called");

            string bodyJson = JsonSerializer.Serialize(model);

            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Post,
                bodyJson,
                _connection,
                "/api/Discounts/Post/SingleDiscount/ToMarginTemplate/" + template);
            return result;
        }

        public async Task<ListStringResponseModel> DeleteDiscountFromGlobal(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DeleteDiscountFromGlobal {security} Called");

            string bodyJson = JsonSerializer.Serialize("");
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Delete,
                bodyJson,
                _connection,
                "/api/Discounts/Delete/SingleDiscount/FromGlobal/" + security);
            return result;
        }

        public async Task<ListStringResponseModel> DeleteDiscountFromTemplate(string template, string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository DeleteDiscountFromTemplate {template} {security} Called");

            string bodyJson = JsonSerializer.Serialize("");
            ListStringResponseModel result = await _executiveRepo.DoActionTDirectResponse<ListStringResponseModel>(
                EnumHttpActions.Delete,
                bodyJson,
                _connection,
                $"/api/Discounts/Delete/SingleDiscount/{security}/FromMarginTemplate/{template}");
            return result;
        }
    }
}
