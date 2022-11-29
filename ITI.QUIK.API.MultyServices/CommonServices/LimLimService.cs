namespace CommonServices
{
    public static class LimLimService
    {
        public static string GetTagByPortfolio(string portfolio)
        {
            if (portfolio.Contains("-FX-") || portfolio.Contains("-CD-"))
            {
                return "EUSR";
            }
            else
            {
                return "EQTV";
            }
        }
    }
}
