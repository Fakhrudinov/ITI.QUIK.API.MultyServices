namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientAssetsModel
    {
        public MatrixClientPortfolioModel ClientPortfolio { get; set; }
        public List<ClientAssetsMoneyPositionModel> Moneys { get; set; } = new List<ClientAssetsMoneyPositionModel>();
        public List<ClientAssetsDepoPositionModel> Positions { get; set; } = new List<ClientAssetsDepoPositionModel>();
    }
}
