using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICoreKval
    {
        Task<ListStringResponseModel> RenewClients(bool sendReport);
    }
}
