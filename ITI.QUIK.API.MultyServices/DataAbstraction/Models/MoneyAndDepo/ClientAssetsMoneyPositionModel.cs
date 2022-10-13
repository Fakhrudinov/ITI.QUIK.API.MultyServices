namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientAssetsMoneyPositionModel
    {
        //MONEY:  FIRM_ID = MC0138200000; TAG = EQTV; CURR_CODE = SUR; CLIENT_CODE = BP3871/01; OPEN_BALANCE = 87311.41; OPEN_LIMIT = 0; LEVERAGE =  2.00; LIMIT_KIND = 2;
        public string Tag { get; set; }//TAG = EQTV;
        public string Currency { get; set; } = "SUR";//CURR_CODE = SUR;
        public decimal Balance { get; set; }//OPEN_BALANCE = 87311.41;
    }
}
