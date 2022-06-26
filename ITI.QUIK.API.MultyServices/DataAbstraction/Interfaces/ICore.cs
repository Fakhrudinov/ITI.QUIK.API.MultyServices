using DataAbstraction.Models;
using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICore
    {
        Task<NewClientCreationResponse> PostNewClient(NewClientModel newClientModel);
    }
}
