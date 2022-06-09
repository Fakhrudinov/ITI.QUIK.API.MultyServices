using DataAbstraction.Connections;
using DataAbstraction.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatrixDataBaseRepository
{
    public class DataBaseRepository : IDataBaseRepository
    {
        private ILogger<DataBaseRepository> _logger;
        private DataBaseConnectionConfiguration _connection;

        public DataBaseRepository(IOptions<DataBaseConnectionConfiguration> connection, ILogger<DataBaseRepository> logger)
        {
            _connection = connection.Value;
            _logger = logger;
        }
        //
        //connectionStringOra + LogonData.loginOracle + "; Password=" + LogonData.passwordOracle + ";";
    }
}