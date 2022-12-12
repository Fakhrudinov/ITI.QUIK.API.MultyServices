using DataAbstraction.Interfaces;
using DataAbstraction.Responses;

namespace DataAbstraction.Models.MoneyAndDepo
{
    public class SingleClientPortfoliosMoneyResponse : IResponseNested
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public List<PortfoliosAndMoneyModel> PortfoliosAndMoney { get; set; } = new List<PortfoliosAndMoneyModel>();
    }
}
