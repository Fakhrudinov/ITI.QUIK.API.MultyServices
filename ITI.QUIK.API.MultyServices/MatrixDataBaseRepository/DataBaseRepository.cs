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
        private static string _queryChechConnection = "select * from moff.CLIENT_PORTFOLIO where id_client = 'BP17840'";
        private static string _queryGetAllSpotPortfolios = "SELECT DISTINCT ID_ACCOUNT FROM moff.CLIENT_PORTFOLIO WHERE id_client = :clientCode AND SECBOARD != 'RTS_FUT'";

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
    }
}