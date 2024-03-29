﻿using DataAbstraction.Models;
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
        Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel);
        Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel);
        Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file);
        Task<ListStringResponseModel> FillCodesIniFile(CodesArrayModel codesArray);
        Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newClientModel);
        Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientCodeModel code);
        Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientCodeModel code);
        Task WarmUpBackOfficeDataBase();
        Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixPortfolio(string clientPortfolio);
        Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode);
        Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientCodeModel model);
        Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> BlockUserByUID(int uid);
        Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientCodeModel model);
        Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> GenerateNewFileCurrClnts();
        Task DownloadNewFileCurrClnts();
    }
}
