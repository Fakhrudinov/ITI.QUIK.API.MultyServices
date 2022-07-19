using DataAbstraction.Models;
using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICore
    {
        Task<NewClientCreationResponse> PostNewClient(NewClientModel newClientModel);
        Task<NewClientModelResponse> GetInfoNewUserNonEDP(string clientCode);
        Task<NewClientOptionWorkShopModelResponse> GetInfoNewUserOptionWorkShop(string clientCode);
        PubringKeyModelResponse GetKeyFromString(string keyText);
        PubringKeyModelResponse GetKeyFromFile(string filePath);
        Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string fileName);
        Task<ListStringResponseModel> PostNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel);
        void RenewAllClientFile();
        Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByMatrixClientAccount(string clientPortfolio);
        Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByFortsCode(string fortsClientCode);
        Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> BlockUserByUID(int uid);
        Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model);
    }
}
