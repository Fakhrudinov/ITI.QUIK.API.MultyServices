using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IHttpApiRepository
    {
        Task<ClientInformationResponse> GetClientInformation(string clientCode);
        Task<MatrixToFortsCodesMappingResponse> GetClientNonEdpFortsCodes(string clientCode);
        Task<MatrixToFortsCodesMappingResponse> GetClientAllFortsCodes(string clientCode);
        Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode);
        Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode);
    }
}
