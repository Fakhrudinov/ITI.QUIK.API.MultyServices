namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientAssetsDepoPositionModel
    {
        //DEPO:  FIRM_ID = MC0138200000; SECCODE = RU000A1034X1; CLIENT_CODE =BP3871/01; OPEN_BALANCE = 13; OPEN_LIMIT = 0; TRDACCID =L01+00000F00; WA_POSITION_PRICE=78.659231; LIMIT_KIND = 2;
        public string Seccode { get; set; } // SECCODE = RU000A1034X1;
        public decimal OpenBalance { get; set; } // OPEN_BALANCE = 13;
        public decimal AveragePrice { get; set; } // WA_POSITION_PRICE=78.659231;
        public string TKS { get; set; } // TRDACCID =L01+00000F00;
    }
}
