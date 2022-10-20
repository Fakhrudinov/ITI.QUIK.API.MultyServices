namespace DataAbstraction.Models
{
    public class NewPortfolioToExistingClientModel
    {
        public MatrixClientPortfolioModel MatrixPortfolio { get; set; }
        public int UID { get; set; }
        public bool checkIsUserHaveEqualsPortfolio { get; set; } = true;
    }
}
