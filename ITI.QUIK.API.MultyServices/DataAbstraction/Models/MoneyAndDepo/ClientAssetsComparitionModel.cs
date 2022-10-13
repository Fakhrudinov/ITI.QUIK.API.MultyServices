namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientAssetsComparitionModel
    {
        public MatrixClientPortfolioModel ClientPortfolio { get; set; }
        public List<ClientMoneyComparitionModel> Money { get; set; } = new List<ClientMoneyComparitionModel>();
        public List<ClientPositionComparitionModel> Position { get; set; } = new List<ClientPositionComparitionModel>();

    }

    public class ClientPositionComparitionModel
    {
        public string Seccode { get; set; } // SECCODE = RU000A1034X1;
        public List<ClientPositionComparitionPositionModel> PositionAtTradeSystem { get; set; } = new List<ClientPositionComparitionPositionModel>();
    }

    public class ClientPositionComparitionPositionModel
    {
        public string TradeSystem { get; set; } //limlim or matrix
        public decimal OpenBalance { get; set; } // OPEN_BALANCE = 13;
        public decimal AveragePrice { get; set; } // WA_POSITION_PRICE=78.659231;
        public string TKS { get; set; } // TRDACCID =L01+00000F00;
    }

    public class ClientMoneyComparitionModel
    {
        public string Currency { get; set; }//CURR_CODE = SUR;
        public List<ClientMoneyComparitionMoneyModel> MoneyAtTradeSystem { get; set; } = new List<ClientMoneyComparitionMoneyModel>();
    }

    public class ClientMoneyComparitionMoneyModel
    {
        public string TradeSystem { get; set; } //limlim or matrix
        public string Tag { get; set; }//TAG = EQTV;
        public decimal Balance { get; set; }//OPEN_BALANCE = 87311.41;
    }
}
