using DataAbstraction.Models;
using DataAbstraction.Models.InstrTw;
using DataAbstraction.Models.MoneyAndDepo;
using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface IHttpMatrixRepository
    {
        Task<ClientInformationResponse> GetClientInformation(string clientCode);
        Task<MatrixToFortsCodesMappingResponse> GetClientNonEdpFortsCodes(string clientCode);
        Task<MatrixToFortsCodesMappingResponse> GetClientAllFortsCodes(string clientCode);
        Task<ClientBOInformationResponse> GetClientBOInformation(string clientCode);
        Task<MatrixClientCodeModelResponse> GetClientAllSpotCodesFiltered(string clientCode);
        Task WarmUpBackOfficeDataBase();        
        Task<MatrixClientCodeModelResponse> GetAllKvalSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllEnemyNonResidentSpotPortfolios();
        //Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKsurUsersSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllNonKvalKpurUsersSpotPortfolios();
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
        Task<ClientDepoPositionsResponse> GetClientsPositionsByMatrixPortfolioList(string portfoliosToHTTPRequestDepoPositions);
        Task<SecurityAndBoardResponse> GetRestrictedSecuritiesAndBoards();
        Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentKvalSpotPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllFrendlyNonResidentNonKvalSpotPortfolios();
        Task<BoolResponse> GetIsClientHasOptionWorkshop(string clientCode);
        Task<MatrixClientCodeModelResponse> GetAllRestrictedCDPortfolios();
        Task<MatrixClientCodeModelResponse> GetAllAllowedCDPortfolios();
        Task<SingleClientPortfoliosMoneyResponse> GetClientSpotPortfoliosAndMoneyForLimLim(string matrixClientAccount);
        Task<ClientDepoPositionsResponse> GetClientActualSpotPositionsForLimLim(string matrixClientAccount);
        Task<ClientDepoPositionsResponse> GetClientInitialDepoToTksSpotPositionsForLimLim(string matrixClientAccount);
        Task<BoolResponse> GetBoolIsClientTradeDaysAgoByClientAccountAndDays(string matrixClientAccount, int dayShift);
        Task<ClientDepoPositionsResponse> GetClientZeroedClosedSpotPositionsForLimLim(string matrixClientAccount, int dayShift);
        Task<ClientAndMoneyResponse> GetClientsSpotPortfoliosWhoTradesYesterday(int i);
    }
}
