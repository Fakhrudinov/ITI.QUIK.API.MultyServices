namespace DataAbstraction.Models
{
    public class NewMatrixPortfolioToExistingClientModel
    {
        public MatrixClientPortfolioModel MatrixPortfolio { get; set; }
        public int UID { get; set; }
        public bool CheckIsUserHaveEqualsPortfolio { get; set; } = true;
    }
}
