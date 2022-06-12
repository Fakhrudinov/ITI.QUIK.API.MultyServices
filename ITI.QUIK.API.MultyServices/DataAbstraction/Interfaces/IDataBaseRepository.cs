using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IDataBaseRepository
    {
        Task<ListStringResponseModel> CheckConnections();
        Task<MatrixClientCodeModelResponse> GetUserSpotPortfolios(string clientCode);
    }
}
