namespace DataAbstraction.Models
{
    public class NewMatrixPortfolioToExistingClientModel
    {
        public MatrixClientPortfolioModel MatrixPortfolio { get; set; }
        public int UID { get; set; }
        public bool checkIsUserHaveEqualsPortfolio { get; set; } = true;
    }
}
