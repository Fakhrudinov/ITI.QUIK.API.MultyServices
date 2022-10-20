using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;
using DataAbstraction.Models.MoneyAndDepo;
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
        Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientPortfolioModel code);
        Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientPortfolioModel code);
        Task WarmUpBackOfficeDataBase();
        Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixClientAccount(string clientPortfolio);
        Task<MatrixClientCodeModelResponse> GetAllKvalSpotPortfolios();
        Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode);
        Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> SetKvalClientsToComplexProductRestrictions(CodesArrayModel model);
        Task<ListStringResponseModel> BlockUserByUID(int uid);
        Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> GenerateNewFileCurrClnts();
        Task DownloadNewFileCurrClnts();
        Task<ListStringResponseModel> GetIsUserAlreadyExistByCodeArray(string[] clientCodes);
        Task<ClientAndMoneyResponse> GetClientsSpotPortfoliosWhoTradesYesterday(int i);
        Task<InstrTWDataBaseRecords> GetRecordsFromInstrTwDataBase(List<string> allportfolios);
        Task<ListStringResponseModel> GetAllClientsFromTemplatePoKomissii(string templateName);
        Task<ListStringResponseModel> GetAllClientsFromTemplatePoPlechu(string templateName);
        Task<ListStringResponseModel> GetSftpFileLastWriteTime(string nameOrPath);
        Task<BoolResponse> GetIsAllSpotPortfoliosPresentInFileCodesIni(List<string> allportfolios);
        Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentSpotPortfolios();
        //Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersSpotPortfolios();
        Task<ListStringResponseModel> SetClientsToTemplatePoKomissii(TemplateAndMatrixCodesModel templateAndMatrixCodes);
        Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentCdPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentCdPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersCdPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllKvalKsurUsersCdPortfolios();
        Task<PortfoliosAndTestForComplexProductResponse> GetAllNonKvalWithTestsSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersCdPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersCdPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllKvalKpurUsersSpotPortfolios();
        Task<FortsClientCodeModelResponse> GetAllEnemyNonResidentFortsCodes();
        Task<FortsClientCodeModelResponse> GetAllKvalClientsFortsCodes();
        Task<FortsClientCodeModelResponse> GetAllNonKvalWithTest16FortsCodes();
        Task<ListStringResponseModel> SetClientsToFortsTemplatePoKomissii(TemplateAndMatrixFortsCodesModel templateAndMatrixFortsCodesModel);
        Task<ListStringResponseModel> SetNonKvalClientsWithTestsToComplexProductRestrictions(QCodeAndListOfComplexProductsTestsModel[] qCodeAndListOfComplexProductsTestsModels);
        Task<ClientDepoPositionsResponse> GetClientsPositionsByMatrixPortfolioList(string portfoliosToHTTPRequestDepoPositions);
        Task<SecurityAndBoardResponse> GetRestrictedSecuritiesAndBoards();
        Task<ListStringResponseModel> SetRestrictedSecuritiesInTemplatesPoKomissii(RestrictedSecuritiesArraySetForBoardInTemplatesModel board);
        Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentKvalSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentNonKvalSpotPortfolios();
        Task<BoolResponse> GetIsClientHasOptionWorkshop(string clientCode);
        Task<ListStringResponseModel> DownloadLimLimFile();
        Task<MatrixClientCodeModelResponse> GetAllRestrictedCDPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllAllowedCDPortfolios();
        Task<ListStringResponseModel> AddNewMatrixPortfolioToExistingClientByUID(MatrixPortfolioAndUidModel matrixPortfolioAndUid);
    }
}
