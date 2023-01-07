using DataAbstraction.Models;
using DataAbstraction.Models.Discounts;
using DataAbstraction.Models.InstrTw;
using DataAbstraction.Models.MoneyAndDepo;
using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IHttpQuikRepository
    {
        Task<ListStringResponseModel> ReloadSpotBrlMC013820000();
        Task<ListStringResponseModel> ReloadFortsBrlSPBFUT();
        Task<ListStringResponseModel> GetResultFromQuikSFTPFileUpload(string file);
        Task<ListStringResponseModel> FillCodesIniFile(CodesArrayModel codesArray);
        Task<ListStringResponseModel> GetIsUserAlreadyExistByMatrixClientAccount(string clientPortfolio);
        Task<ListStringResponseModel> GetIsUserAlreadyExistByFortsCode(string fortsClientCode);
        Task<ListStringResponseModel> BlockUserByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> BlockUserByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> BlockUserByUID(int uid);
        Task<ListStringResponseModel> SetNewPubringKeyByMatrixClientCode(MatrixCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetNewPubringKeyByFortsClientCode(FortsCodeAndPubringKeyModel model);
        Task<ListStringResponseModel> SetAllTradesByMatrixClientCode(MatrixClientPortfolioModel model);
        Task<ListStringResponseModel> SetAllTradesByFortsClientCode(FortsClientCodeModel model);
        Task<ListStringResponseModel> GenerateNewFileCurrClnts();
        Task<ListStringResponseModel> GetIsUserAlreadyExistByCodeArray(string[] clientCodes);
        Task<InstrTWDataBaseRecords> GetRecordsFromInstrTwDataBase(List<string> allportfolios);
        Task<ListStringResponseModel> GetAllClientsFromTemplatePoKomissii(string templateName);
        Task<ListStringResponseModel> GetAllClientsFromTemplatePoPlechu(string templateName);
        Task<ListStringResponseModel> DownloadLimLimFile();
        Task DownloadNewFileCurrClnts();
        Task<ListStringResponseModel> CreateNewClientOptionWorkshop(NewClientOptionWorkShopModel newClientModel);
        Task<ListStringResponseModel> CreateNewClient(NewClientModel newClientModel);
        Task<ListStringResponseModel> FillDataBaseInstrTW(NewMNPClientModel newClientModel);
        Task<ListStringResponseModel> AddCdPortfolioToTemplateKomissii(MatrixClientPortfolioModel code);
        Task<ListStringResponseModel> AddCdPortfolioToTemplatePoPlechu(MatrixClientPortfolioModel code);
        Task<ListStringResponseModel> GetSftpFileLastWriteTime(string nameOrPath);
        Task<BoolResponse> GetIsAllSpotPortfoliosPresentInFileCodesIni(List<string> allportfolios);
        Task<ListStringResponseModel> SetClientsToFortsTemplatePoKomissii(TemplateAndMatrixFortsCodesModel templateAndMatrixFortsCodesModel);
        Task<ListStringResponseModel> SetNonKvalClientsWithTestsToComplexProductRestrictions(QCodeAndListOfComplexProductsTestsModel[] qCodeAndListOfComplexProductsTestsModels);
        Task<ListStringResponseModel> SetClientsToTemplatePoKomissii(TemplateAndMatrixCodesModel templateAndMatrixCodes);
        Task<ListStringResponseModel> AddNewMatrixPortfolioToExistingClientByUID(MatrixPortfolioAndUidModel matrixPortfolioAndUid);
        Task<ListStringResponseModel> AddNewFortsPortfolioToExistingClientByUID(FortsCodeAndUidModel fortsCodeAndUid);
        Task<ListStringResponseModel> SetRestrictedSecuritiesInTemplatesPoKomissii(RestrictedSecuritiesArraySetForBoardInTemplatesModel board);
        Task<ListStringResponseModel> SetKvalClientsToComplexProductRestrictions(CodesArrayModel model);
        Task<DiscountSingleResponse> GetDiscountSingleFromGlobal(string security);
        Task<DiscountSingleResponse> GetDiscountSingleFromMarginTemplate(string template, string security);
        Task<ListStringResponseModel> PostSingleDiscountToGlobal(DiscountAndSecurityModel modelToQuik);
        Task<ListStringResponseModel> PostSingleDiscountToTemplate(string template, DiscountAndSecurityModel modelToQuik);
        Task<ListStringResponseModel> DeleteDiscountFromTemplate(string template, string security);
        Task<ListStringResponseModel> DeleteDiscountFromGlobal(string security);
    }
}
