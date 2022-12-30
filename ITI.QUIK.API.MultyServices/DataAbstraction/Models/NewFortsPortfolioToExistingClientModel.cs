namespace DataAbstraction.Models
{
    public class NewFortsPortfolioToExistingClientModel
    {
        public MatrixToFortsCodesMappingModel MatrixToFortsCodes { get; set; }
        public int UID { get; set; }
        public bool CheckIsUserHaveEqualsPortfolio { get; set; } = true;
    }
}
