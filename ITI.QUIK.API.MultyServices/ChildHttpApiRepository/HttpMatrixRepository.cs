using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models.Discounts;
using DataAbstraction.Models.MoneyAndDepo;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace ChildHttpMatrixRepository
{
    public class HttpMatrixRepository : IHttpMatrixRepository
    {
        private ILogger<HttpMatrixRepository> _logger;
        private string _connection;
        private HttpApiExecutiveRepository _executiveRepo;

        public HttpMatrixRepository(
            ILogger<HttpMatrixRepository> logger, 
            IOptions<HttpConfigurations> connections,
            HttpApiExecutiveRepository executiveRepo)
        {
            _logger = logger;
            _connection = connections.Value.MatrixAPIConnectionString;
            _executiveRepo= executiveRepo;
        }

         public async Task WarmUpBackOfficeDataBase()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository WarmUpBackOfficeDataBase Called");

            await _executiveRepo.VoidGet(
                _connection,
                "/api/DBClient/WarmUp/BackOfficeDataBase");

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository WarmUpBackOfficeDataBase succes status is {response.StatusCode}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository WarmUpBackOfficeDataBase response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository WarmUpBackOfficeDataBase request url NotFound: " + _connections.MatrixAPIConnectionString + "/api/DBClient/WarmUp/BackOfficeDataBase");
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository WarmUpBackOfficeDataBase request Exception is : {ex.Message}");
            //}
        }

        public async Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientBOInformation '{clientCode}' Called");

            ClientBOInformationResponse result = await _executiveRepo.GetTNestedResponse<ClientBOInformationResponse>(
                _connection,
                "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);
            return result;


            //ClientBOInformationResponse result = new ClientBOInformationResponse();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ClientBOInformationResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientBOInformation /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetClientBOInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientBOInformation request url /api/DBClient/GetUser/PersonalInfo/BackOffice/{clientCode} NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientBOInformation request url NotFound;  {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/BackOffice/" + clientCode);
            //}

            //return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientAllSpotCodesFiltered '{clientCode}' Called");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);
            return result;

            //MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<MatrixClientCodeModelResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientAllSpotCodesFiltered /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetClientAllSpotCodesFiltered response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientAllSpotCodesFiltered request url /api/DBClient/GetUser/SpotPortfolios/Filtered/{clientCode} NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientAllSpotCodesFiltered request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/SpotPortfolios/Filtered/" + clientCode);
            //}

            //return result;
        }



 

        public async Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllEnemyNonResidentSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Enemy/NonResident/Spot/Portfolios");
            return result;
        }

        //public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentSpotPortfolios()
        //{
        //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllFrendlyNonResidentSpotPortfolios Called");

        //    MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Spot/Portfolios");

        //    return result;
        //}

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllFrendlyNonResidentKvalSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Kval/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Frendly/NonResident/Kval/Spot/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentNonKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllFrendlyNonResidentKvalSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/NonKval/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Frendly/NonResident/NonKval/Spot/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalKsurUsersSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Spot/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalKpurUsersSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Spot/Portfolios");
            return result;           
        }

        public async Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllFrendlyNonResidentCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Frendly/NonResident/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Frendly/NonResident/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllEnemyNonResidentCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Enemy/NonResident/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllKvalKpurUsersSpotPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KPUR/Spot/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/KvalUsers/KPUR/Spot/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllKvalKpurUsersCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KPUR/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/KvalUsers/KPUR/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalKsurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllKvalKsurUsersCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/KSUR/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/KvalUsers/KSUR/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalKpurUsersCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/KPUR/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersCdPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalKsurUsersCdPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Cd/Portfolios");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/KSUR/Cd/Portfolios");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllRestrictedCDPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllRestrictedCDPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/TemplatesPoKomissii/GetAll/Restricted/CD/Portfolios/ForCD_Restrict");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/TemplatesPoKomissii/GetAll/Restricted/CD/Portfolios/ForCD_Restrict");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllAllowedCDPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllAllowedCDPortfolios Called");

            //MatrixClientCodeModelResponse result = await GetPortfoliosByApiLink("/api/TemplatesPoKomissii/GetAll/Allowed/CD/Portfolios/ForCD_portfolio");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/TemplatesPoKomissii/GetAll/Allowed/CD/Portfolios/ForCD_portfolio");
            return result;
        }

        public async Task<MatrixClientCodeModelResponse> GetAllKvalSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllKvalSpotPortfolios Called");

            MatrixClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<MatrixClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/KvalUsers/Spot/Portfolios");
            return result;
        }

        //private async Task<MatrixClientCodeModelResponse> GetPortfoliosByApiLink(string apiLink)
        //{
        //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetPortfoliosByApiLink Called for {apiLink}");

        //    MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var response = await client.GetAsync(_connections.MatrixAPIConnectionString + apiLink);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                result = await response.Content.ReadFromJsonAsync<MatrixClientCodeModelResponse>();

        //                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetPortfoliosByApiLink {apiLink} succes is {result.Response.IsSuccess}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

        //                result.Response.IsSuccess = false;
        //                result.Response.Messages.Add($"HttpMatrixRepository GetMsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");

        //        result.Response.IsSuccess = false;
        //        result.Response.Messages.Add($"(404) HttpMatrixRepository GetMsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");
        //        result.Response.Messages.Add(_connections.MatrixAPIConnectionString + apiLink);
        //    }

        //    return result;
        //}

        public async Task<FortsClientCodeModelResponse> GetAllEnemyNonResidentFortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllEnemyNonResidentFortsCodes Called");

            //FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/Enemy/NonResident/Forts/Codes");

            FortsClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<FortsClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/Enemy/NonResident/Forts/Codes");
            return result;
        }

        public async Task<FortsClientCodeModelResponse> GetAllKvalClientsFortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllKvalClientsFortsCodes Called");

            //FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/KvalUsers/Forts/Codes");

            FortsClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<FortsClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/KvalUsers/Forts/Codes");
            return result;
        }

        public async Task<FortsClientCodeModelResponse> GetAllNonKvalWithTest16FortsCodes()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalWithTest16FortsCodes Called");

            //FortsClientCodeModelResponse result = await GetFortsPortfoliosByApiLink("/api/KvalInvestors/GetAll/NonKvalUsers/WithTest16/Forts/Codes");

            FortsClientCodeModelResponse result = await _executiveRepo.GetTNestedResponse<FortsClientCodeModelResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/WithTest16/Forts/Codes");
            return result;
        }

        //private async Task<FortsClientCodeModelResponse> GetFortsPortfoliosByApiLink(string apiLink)
        //{
        //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetFortsPortfoliosByApiLink Called for {apiLink}");

        //    FortsClientCodeModelResponse result = new FortsClientCodeModelResponse();

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var response = await client.GetAsync(_connections.MatrixAPIConnectionString + apiLink);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                result = await response.Content.ReadFromJsonAsync<FortsClientCodeModelResponse>();

        //                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetFortsPortfoliosByApiLink {apiLink} succes is {result.Response.IsSuccess}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetFortsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

        //                result.Response.IsSuccess = false;
        //                result.Response.Messages.Add($"HttpMatrixRepository GetFortsPortfoliosByApiLink response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetFortsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");

        //        result.Response.IsSuccess = false;
        //        result.Response.Messages.Add($"(404) HttpMatrixRepository GetFortsPortfoliosByApiLink request url {apiLink} NotFound; {ex.Message}");
        //        result.Response.Messages.Add(_connections.MatrixAPIConnectionString + apiLink);
        //    }

        //    return result;
        //}

        public async Task<PortfoliosAndTestForComplexProductResponse> GetAllNonKvalWithTestsSpotPortfolios()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios Called");

            PortfoliosAndTestForComplexProductResponse result = await _executiveRepo.GetTNestedResponse<PortfoliosAndTestForComplexProductResponse>(
                _connection,
                "/api/KvalInvestors/GetAll/NonKvalUsers/SpotPortfolios/and/TestForComplexProduct"
                );
            return result;

            //PortfoliosAndTestForComplexProductResponse result = new PortfoliosAndTestForComplexProductResponse();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/KvalInvestors/GetAll/NonKvalUsers/SpotPortfolios/and/TestForComplexProduct");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<PortfoliosAndTestForComplexProductResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios request url NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetAllNonKvalWithTestsSpotPortfolios request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/KvalInvestors/GetAll/NonKvalUsers/SpotPortfolios/and/TestForComplexProduct");
            //}

            //return result;
        }

        public async Task<SecurityAndBoardResponse> GetRestrictedSecuritiesAndBoards()
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRestrictedSecuritiesAndBoards Called");

            SecurityAndBoardResponse result = await _executiveRepo.GetTNestedResponse<SecurityAndBoardResponse>(
                _connection,
                "/api/Securities/Get/Securities/SpotBlackList/ForNekval");
            return result;

            //SecurityAndBoardResponse result = new SecurityAndBoardResponse();

            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/Securities/Get/Securities/SpotBlackList/ForNekval");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<SecurityAndBoardResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRestrictedSecuritiesAndBoards succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRestrictedSecuritiesAndBoards response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetRestrictedSecuritiesAndBoards response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetRestrictedSecuritiesAndBoards request url NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetRestrictedSecuritiesAndBoards request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/Securities/Get/Securities/SpotBlackList/ForNekval");
            //}

            //return result;
        }

        public async Task<ClientAndMoneyResponse> GetClientsSpotPortfoliosWhoTradesYesterday(int daysShift)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday '{daysShift}' Called");

            ClientAndMoneyResponse result = await _executiveRepo.GetTNestedResponse<ClientAndMoneyResponse>(
                _connection,
                "/api/ClientMoney/GetClients/WhoTrade/SpotPortfoliosAndMoney/" + daysShift);
            return result;

            //ClientAndMoneyResponse result = new ClientAndMoneyResponse();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/WhoTrade/SpotPortfoliosAndMoney/" + daysShift);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ClientAndMoneyResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday '{daysShift}' " +
            //                $"succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday response is" +
            //                $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday " +
            //        $"request url NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientsSpotPortfoliosWhoTradesYesterday request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientMoney/GetClients/WhoTrade/SpotPortfoliosAndMoney/" + daysShift);
            //}

            //return result;
        }

        public async Task<ClientDepoPositionsResponse> GetClientsPositionsByMatrixPortfolioList(string portfoliosToRequest)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsPositionsByMatrixPortfolioList Called " +
                $"with " + portfoliosToRequest);

            ClientDepoPositionsResponse result = await _executiveRepo.GetTNestedResponse<ClientDepoPositionsResponse>(
                _connection,
                "/api/ClientMoney/GetClients/Positions/ByMatrixPortfolioList?" + portfoliosToRequest);
            return result;

            //string httpRequest = "/api/ClientMoney/GetClients/Positions/ByMatrixPortfolioList?" + portfoliosToRequest;

            //return await GetClientDepoPositionsResponseByRequest(httpRequest);
        }

        public async Task<ClientDepoPositionsResponse> GetClientActualSpotPositionsForLimLim(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientActualSpotPositionsForLimLim Called " +
                $"with " + matrixClientAccount);

            ClientDepoPositionsResponse result = await _executiveRepo.GetTNestedResponse<ClientDepoPositionsResponse>(
                _connection,
                "/api/ClientMoney/Get/SingleClient/ActualPositionsLimits/ByMatrixAccount/" + matrixClientAccount);
            return result;


        //    string httpRequest = "/api/ClientMoney/Get/SingleClient/ActualPositionsLimits/ByMatrixAccount/" + matrixClientAccount;

        //    return await GetClientDepoPositionsResponseByRequest(httpRequest);
        }

        public async Task<ClientDepoPositionsResponse> GetClientInitialDepoToTksSpotPositionsForLimLim(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientInitialDepoToTksSpotPositionsForLimLim Called " +
                $"with " + matrixClientAccount);

            ClientDepoPositionsResponse result = await _executiveRepo.GetTNestedResponse<ClientDepoPositionsResponse>(
                _connection,
                "/api/ClientMoney/Get/SingleClient/ZeroPositionToTKSLimits/ByMatrixAccount/" + matrixClientAccount);
            return result;

            //string httpRequest = "/api/ClientMoney/Get/SingleClient/ZeroPositionToTKSLimits/ByMatrixAccount/" + matrixClientAccount;

            //return await GetClientDepoPositionsResponseByRequest(httpRequest);
        }

        public async Task<ClientDepoPositionsResponse> GetClientZeroedClosedSpotPositionsForLimLim(string matrixClientAccount, int dayShift)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientZeroedClosedSpotPositionsForLimLim Called " +
                $"with {matrixClientAccount} days={dayShift}");

            ClientDepoPositionsResponse result = await _executiveRepo.GetTNestedResponse<ClientDepoPositionsResponse>(
                _connection,
                $"/api/ClientMoney/Get/SingleClient/ClosedPositionsLimits/ByMatrixAccount/{matrixClientAccount}/daysShift/{dayShift}");
            return result;

            //string httpRequest = $"/api/ClientMoney/Get/SingleClient/ClosedPositionsLimits/ByMatrixAccount/{matrixClientAccount}/daysShift/{dayShift}";

            //return await GetClientDepoPositionsResponseByRequest(httpRequest);
        }

        //private async Task<ClientDepoPositionsResponse> GetClientDepoPositionsResponseByRequest(string httpRequest)
        //{
        //    _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientDepoPositionsResponseByRequest Called " +
        //        $"with httpRequest " + httpRequest);

        //    ClientDepoPositionsResponse result = new ClientDepoPositionsResponse();
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var response = await client.GetAsync(_connections.MatrixAPIConnectionString + httpRequest);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                result = await response.Content.ReadFromJsonAsync<ClientDepoPositionsResponse>();

        //                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientDepoPositionsResponseByRequest " +
        //                    $"succes is {result.Response.IsSuccess}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientDepoPositionsResponseByRequest response is" +
        //                    $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

        //                result.Response.IsSuccess = false;
        //                result.Response.Messages.Add($"HttpMatrixRepository GetClientDepoPositionsResponseByRequest response is " +
        //                    $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientDepoPositionsResponseByRequest " +
        //            $"request url NotFound; {ex.Message}");

        //        result.Response.IsSuccess = false;
        //        result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientDepoPositionsResponseByRequest request url NotFound; {ex.Message}");
        //        result.Response.Messages.Add(_connections.MatrixAPIConnectionString + httpRequest);
        //    }

        //    return result;
        //}

 

        public async Task<SingleClientPortfoliosMoneyResponse> GetClientSpotPortfoliosAndMoneyForLimLim(string matrixClientAccount)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim Called for " + matrixClientAccount);

            SingleClientPortfoliosMoneyResponse result = await _executiveRepo.GetTNestedResponse<SingleClientPortfoliosMoneyResponse>(
                _connection,
                "/api/ClientMoney/Get/SingleClient/Money/SpotLimits/ByMatrixAccount/" + matrixClientAccount);
            return result;

            //SingleClientPortfoliosMoneyResponse result = new SingleClientPortfoliosMoneyResponse();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientMoney/Get/SingleClient/Money/SpotLimits/ByMatrixAccount/" + matrixClientAccount);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<SingleClientPortfoliosMoneyResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim " +
            //                $"succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim response is" +
            //                $" {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim " +
            //        $"request url NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientSpotPortfoliosAndMoneyForLimLim request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientMoney/Get/SingleClient/Money/SpotLimits/ByMatrixAccount/" + matrixClientAccount);
            //}

            //return result;
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientNonEdpFortsCodes(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientNonEdpFortsCodes '{clientCode}' Called");

            MatrixToFortsCodesMappingResponse result = await _executiveRepo.GetTNestedResponse<MatrixToFortsCodesMappingResponse>(
                _connection,
                "/api/DBClient/GetUser/FortsPortfolios/NoEDP/" + clientCode);
            return result;

            //return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/NoEDP/" + clientCode);
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetClientAllFortsCodes(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientAllFortsCodes '{clientCode}' Called");

            MatrixToFortsCodesMappingResponse result = await _executiveRepo.GetTNestedResponse<MatrixToFortsCodesMappingResponse>(
                _connection,
                "/api/DBClient/GetUser/FortsPortfolios/" + clientCode);
            return result;

            //return await GetClientsFortsCodes("/api/DBClient/GetUser/FortsPortfolios/" + clientCode);
        }

        //private async Task<MatrixToFortsCodesMappingResponse> GetClientsFortsCodes(string request)
        //{
        //    MatrixToFortsCodesMappingResponse result = new MatrixToFortsCodesMappingResponse();

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //            var response = await client.GetAsync(_connections.MatrixAPIConnectionString + request);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                result = await response.Content.ReadFromJsonAsync<MatrixToFortsCodesMappingResponse>();

        //                _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsFortsCodes '{request}' succes is {result.Response.IsSuccess}");
        //            }
        //            else
        //            {
        //                _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

        //                result.Response.IsSuccess = false;
        //                result.Response.Messages.Add($"HttpMatrixRepository GetClientsFortsCodes response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientsFortsCodes request url '{request}' NotFound; {ex.Message}");

        //        result.Response.IsSuccess = false;
        //        result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientsFortsCodes request url NotFound; {ex.Message}");
        //        result.Response.Messages.Add(_connections.MatrixAPIConnectionString + request);
        //    }

        //    return result;
        //}

        public async Task<ClientInformationResponse> GetClientInformation(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientInformation '{clientCode}' Called");

            ClientInformationResponse result = await _executiveRepo.GetTNestedResponse<ClientInformationResponse>(
                _connection,
                "/api/DBClient/GetUser/PersonalInfo/" + clientCode);
            return result;

            //ClientInformationResponse result = new ClientInformationResponse();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<ClientInformationResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientInformation '{clientCode}' succes is {result.Response.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.Response.IsSuccess = false;
            //            result.Response.Messages.Add($"HttpMatrixRepository GetClientInformation response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetClientInformation request url NotFound; {ex.Message}");

            //    result.Response.IsSuccess = false;
            //    result.Response.Messages.Add($"(404) HttpMatrixRepository GetClientInformation request url NotFound; {ex.Message}");
            //    result.Response.Messages.Add(_connections.MatrixAPIConnectionString + "/api/DBClient/GetUser/PersonalInfo/" + clientCode);
            //}

            //return result;
        }


        public async Task<BoolResponse> GetBoolIsClientTradeDaysAgoByClientAccountAndDays(string matrixClientAccount, int dayShift)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays Called " +
                $"with {matrixClientAccount} days={dayShift}");

            BoolResponse result = await _executiveRepo.GetTDirectResponse<BoolResponse>(
                _connection,
                $"/api/ClientMoney/Get/SingleClient/DoTrades/ByMatrixAccount/{matrixClientAccount}/daysAgoShift/{dayShift}");
            return result;

            //BoolResponse result = new BoolResponse();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + 
            //            $"/api/ClientMoney/Get/SingleClient/DoTrades/ByMatrixAccount/{matrixClientAccount}/daysAgoShift/{dayShift}");

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<BoolResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays " +
            //                $"'{matrixClientAccount}' succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays request " +
            //        $"url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetBoolIsClientTradeDaysAgoByClientAccountAndDays request " +
            //        $"url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.MatrixAPIConnectionString + 
            //        $"/api/ClientMoney/Get/SingleClient/DoTrades/ByMatrixAccount/{matrixClientAccount}/daysAgoShift/{dayShift}");
            //}

            //return result;
        }

        public async Task<BoolResponse> GetIsClientHasOptionWorkshop(string clientCode)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsClientHasOptionWorkshop '{clientCode}' Called");

            BoolResponse result = await _executiveRepo.GetTDirectResponse<BoolResponse>(
                _connection,
                "/api/ClientBOServices/Get/IsUserHave/OptionWorkshop/" + clientCode);
            return result;
            //BoolResponse result = new BoolResponse();
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        client.BaseAddress = new Uri(_connections.MatrixAPIConnectionString);
            //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //        var response = await client.GetAsync(_connections.MatrixAPIConnectionString + "/api/ClientBOServices/Get/IsUserHave/OptionWorkshop/" + clientCode);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            result = await response.Content.ReadFromJsonAsync<BoolResponse>();

            //            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsClientHasOptionWorkshop '{clientCode}' succes is {result.IsSuccess}");
            //        }
            //        else
            //        {
            //            _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsClientHasOptionWorkshop response is " +
            //                $"{response.StatusCode} {response.ReasonPhrase} {response.Content}");

            //            result.IsSuccess = false;
            //            result.Messages.Add($"HttpMatrixRepository GetIsClientHasOptionWorkshop response is {response.StatusCode} {response.ReasonPhrase} {response.Content}");
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetIsClientHasOptionWorkshop request url NotFound; {ex.Message}");

            //    result.IsSuccess = false;
            //    result.Messages.Add($"(404) HttpMatrixRepository GetIsClientHasOptionWorkshop request url NotFound; {ex.Message}");
            //    result.Messages.Add(_connections.MatrixAPIConnectionString + "/api/ClientBOServices/Get/IsUserHave/OptionWorkshop/" + clientCode);
            //}

            //return result;
        }

        public async Task<DiscountMatrixSingleResponse> GetDiscountSingle(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetDiscountSingle '{security}' Called");

            DiscountMatrixSingleResponse result = await _executiveRepo.GetTDirectResponse<DiscountMatrixSingleResponse>(
                _connection,
                "/api/Discounts/Get/SingleDiscount/" + security);
            return result;
        }

        public async Task<DiscountMatrixSingleResponse?> GetDiscountSingleForts(string security)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetDiscountSingleForts '{security}' Called");

            DiscountMatrixSingleResponse result = await _executiveRepo.GetTDirectResponse<DiscountMatrixSingleResponse>(
                _connection,
                "/api/Discounts/Get/SingleDiscountForts/" + security);
            return result;
        }

        public async Task<DiscountsListResponse> GetDiscountsListFromMarket(string market)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("HH:mm:ss:fffff")} HttpMatrixRepository GetDiscountsListFromMarket '{market}' Called");

            DiscountsListResponse result = await _executiveRepo.GetTDirectResponse<DiscountsListResponse>(
                _connection,
                "/api/Discounts/Get/DiscountsList/" + market);
            return result;
        }
    }
}