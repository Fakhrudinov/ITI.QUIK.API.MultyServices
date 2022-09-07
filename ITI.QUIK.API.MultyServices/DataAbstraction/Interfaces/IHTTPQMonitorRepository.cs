using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IHTTPQMonitorRepository
    {
        Task<ListStringResponseModel> ReloadSpotBrlMC013820000();
        Task<ListStringResponseModel> ReloadFortsBrlSPBFUT();
    }
}
