using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IDataBaseRepository
    {
        Task<ListStringResponseModel> CheckConnections();
    }
}
