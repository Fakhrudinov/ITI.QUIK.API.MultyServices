using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Models;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace MatrixDataBaseRepository
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private ILogger<DataBaseRepository> _logger;
        private readonly string _connectionString = "";

        //команды не должны содержать ; в конце! 
        private const string _queryChechConnection = "select * from moff.CLIENT_PORTFOLIO where id_client = 'BP17840'";
        private const string _queryGetAllSpotPortfolios = "SELECT ID FROM moff.CLIENT_PORTFOLIO WHERE id_client = :clientCode AND SECBOARD != 'RTS_FUT'";
        private const string _queryGetAllFortsPortfolios = "SELECT ID, ALIAS FROM moff.CLIENT_PORTFOLIO WHERE id_client = :clientCode AND SECBOARD = 'RTS_FUT'";
        private const string _queryGetAllFortsNoEDPPortfolios = "SELECT ID, ALIAS  FROM moff.CLIENT_PORTFOLIO WHERE id_client = :clientCode AND SECBOARD = 'RTS_FUT' AND ID_ACCOUNT NOT LIKE '%-MO-%'";
        private const string _queryGetPortfolioEDPBelongings = "SELECT ID_ACCOUNT FROM moff.CLIENT_PORTFOLIO WHERE ID= :clientportfolio";

        public DataBaseRepository(IOptions<DataBaseConnectionConfiguration> connection, ILogger<DataBaseRepository> logger)
        {
            _logger = logger;
            _connectionString = connection.Value.ConnectionString + " User Id=" + connection.Value.Login + "; Password=" + connection.Value.Password + ";";
        }

        public async Task<ListStringResponseModel> CheckConnections()
        {
            _logger.LogInformation($"DBRepository CheckConnections Called");

            ListStringResponseModel response = new ListStringResponseModel();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    OracleCommand command = new OracleCommand(_queryChechConnection, connection);

                    _logger.LogInformation($"DBRepository CheckConnections try to connect");
                    await connection.OpenAsync();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Messages.Add(reader.GetString(0));
                        }
                    }

                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DBRepository CheckConnections Failed, Exception: " + ex.Message);

                response.IsSuccess = false;
                response.Messages.Add("Exception at DataBase: " + ex.Message);
                return response;
            }

            _logger.LogInformation($"DBRepository CheckConnections Success");
            return response;
        }

        public async Task<MatrixClientCodeModelResponse> GetUserSpotPortfolios(string clientCode)
        {
            _logger.LogInformation($"DBRepository GetUserSpotPortfolios for {clientCode} Called");

            MatrixClientCodeModelResponse result = new MatrixClientCodeModelResponse();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    OracleCommand command = new OracleCommand(_queryGetAllSpotPortfolios, connection);
                    command.Parameters.Add(":clientCode", clientCode);

                    _logger.LogInformation($"DBRepository GetUserSpotPortfolios try to connect");
                    await connection.OpenAsync();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            MatrixClientCodeModel portfolio = new MatrixClientCodeModel();
                            portfolio.MatrixClientCode = reader.GetString(0);

                            result.MatrixClientCodesList.Add(portfolio);
                        }
                    }

                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DBRepository GetUserSpotPortfolios Failed, Exception: " + ex.Message);

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"DBRepository GetUserSpotPortfolios Failed, Exception: " + ex.Message);
            }

            _logger.LogInformation($"DBRepository GetUserSpotPortfolios Success");
            return result;
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetUserFortsPortfolios(string clientCode)
        {
            _logger.LogInformation($"DBRepository GetUserFortsPortfolios for {clientCode} Called");

            MatrixToFortsCodesMappingResponse result = new MatrixToFortsCodesMappingResponse();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    OracleCommand command = new OracleCommand(_queryGetAllFortsPortfolios, connection);
                    command.Parameters.Add(":clientCode", clientCode);

                    _logger.LogInformation($"DBRepository GetUserFortsPortfolios try to connect");
                    await connection.OpenAsync();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            MatrixToFortsCodesMappingModel portfolioMapping = new MatrixToFortsCodesMappingModel();
                            portfolioMapping.MatrixClientCode = reader.GetString(0);
                            portfolioMapping.FortsClientCode = reader.GetString(1);

                            result.MatrixToFortsCodesList.Add(portfolioMapping);
                        }
                    }

                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DBRepository GetUserFortsPortfolios Failed, Exception: " + ex.Message);

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"DBRepository GetUserFortsPortfolios Failed, Exception: " + ex.Message);
            }

            _logger.LogInformation($"DBRepository GetUserFortsPortfolios Success");
            return result;
        }

        public async Task<MatrixToFortsCodesMappingResponse> GetUserFortsPortfoliosNoEDP(string clientCode)
        {
            _logger.LogInformation($"DBRepository GetUserFortsPortfolios for {clientCode} Called");

            MatrixToFortsCodesMappingResponse result = new MatrixToFortsCodesMappingResponse();

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    OracleCommand command = new OracleCommand(_queryGetAllFortsNoEDPPortfolios, connection);
                    command.Parameters.Add(":clientCode", clientCode);

                    _logger.LogInformation($"DBRepository GetUserFortsPortfolios try to connect");
                    await connection.OpenAsync();

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            MatrixToFortsCodesMappingModel portfolioMapping = new MatrixToFortsCodesMappingModel();
                            portfolioMapping.MatrixClientCode = reader.GetString(0);
                            portfolioMapping.FortsClientCode = reader.GetString(1);

                            result.MatrixToFortsCodesList.Add(portfolioMapping);
                        }
                    }

                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DBRepository GetUserFortsPortfolios Failed, Exception: " + ex.Message);

                result.Response.IsSuccess = false;
                result.Response.Messages.Add($"DBRepository GetUserFortsPortfolios Failed, Exception: " + ex.Message);
            }

            _logger.LogInformation($"DBRepository GetUserFortsPortfolios Success");
            return result;
        }

        public async Task<BoolResponse> GetIsPortfolioInEDP(string clientRfPortfolio)
        {
            _logger.LogInformation($"DBRepository GetIsPortfolioInEDP for {clientRfPortfolio} Called");

            BoolResponse result = new BoolResponse();
            string requestResult = null;

            try
            {
                using (OracleConnection connection = new OracleConnection(_connectionString))
                {
                    OracleCommand command = new OracleCommand(_queryGetPortfolioEDPBelongings, connection);
                    command.Parameters.Add(":clientportfolio", clientRfPortfolio);

                    _logger.LogInformation($"DBRepository GetIsPortfolioInEDP try to connect");
                    await connection.OpenAsync();                    

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (await reader.ReadAsync())
                        {
                            requestResult = reader.GetString(0);
                        }
                    }

                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DBRepository GetIsPortfolioInEDP Failed, Exception: " + ex.Message);

                result.IsSuccess = false;
                result.Messages.Add($"DBRepository GetIsPortfolioInEDP Failed, Exception: " + ex.Message);
            }

            _logger.LogInformation($"DBRepository GetIsPortfolioInEDP Success");

            if (requestResult == null)
            {
                result.Messages.Add($"(404)");
                return result;
            }
            
            if (!requestResult.Contains("-MO-"))
            {
                result.IsTrue = true;
            }

            return result;
        }
    }
}