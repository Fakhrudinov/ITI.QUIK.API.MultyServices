namespace CommonServices
{
    public static class PortfoliosConvertingService
    {
        public static string GetQuikCdPortfolio(string portfolio)
        {
            var portfolioParts = portfolio.Split("-");

            string result = portfolioParts[0]
                        + "/D"
                        + portfolioParts[2];

            return result;
        }

        public static string GetQuikSpotPortfolio(string portfolio)
        {
            var portfolioParts = portfolio.Split("-");

            string result = portfolioParts[0]
                        + "/"
                        + portfolioParts[2];

            return result;
        }

        public static string GetQuikFortsCode(string code)
        {
            return "SPBFUT" + code.Substring(2);
        }

        public static string GetMatrixFortsCode(string code)
        {
            if (code.Contains("SPBFUT"))
            {
                return "C0" + code.Substring(6);
            }
            else
            {
                return code;
            }

        }

        public static string GetMatrixMOCode(string code)
        {
            //case of test contur
            if (!code.Contains('/'))
            {
                return code;
            }


            var portfolioParts = code.Split("/");

            string result = portfolioParts[0]
                        + "-MO-"
                        + portfolioParts[1];

            return result;
        }
    }
}