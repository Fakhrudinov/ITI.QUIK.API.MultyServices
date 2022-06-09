using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using DataAbstraction.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace MatrixDataBaseRepository
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private ILogger<DataBaseRepository> _logger;
        private DataBaseConnectionConfiguration _connection;

        private static string _queryChechConnection = "select * from moff.CLIENT_PORTFOLIO where id_client = 'BP17840'";

        public DataBaseRepository(IOptions<DataBaseConnectionConfiguration> connection, ILogger<DataBaseRepository> logger)
        {
            _connection = connection.Value;
            _logger = logger;
        }

        public async Task<ListStringResponseModel> CheckConnections()
        {
            _logger.LogInformation($"DBRepository CheckConnections Called");

            ListStringResponseModel response = new ListStringResponseModel();

            string connectionString = _connection.ConnectionString + " User Id=" + _connection.Login + "; Password=" + _connection.Password + ";";

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
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
                _logger.LogWarning($"QuikDBRepository CheckConnections Failed, Exception: " + ex.Message);

                response.IsSuccess = false;
                response.Messages.Add("Exception at DataBase: " + ex.Message);
                return response;
            }

            _logger.LogInformation($"QuikDBRepository CheckConnections Success");
            return response;
        }
    }
}