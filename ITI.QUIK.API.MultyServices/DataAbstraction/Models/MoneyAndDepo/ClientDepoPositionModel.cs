namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientDepoPositionModel
    {
        public string MatrixClientPortfolio { get; set; }//BP12345-MS-01
        public string SecCode { get; set; }//GAZP
        public decimal OpenBalance { get; set; }//pieces
        public decimal AveragePrice { get; set; }
        public string SecBoard { get; set; }//EQ/CETS
        public string TKS { get; set; } //L01+00000F00;
    }
}
