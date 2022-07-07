﻿using DataAbstraction.Models;
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
        Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByMatrixPortfolio(string clientPortfolio);
        Task<FindedQuikQAdminClientResponse> GetIsUserAlreadyExistByFortsCode(string fortsClientCode);
    }
}
