using DataAbstraction.Interfaces;
using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class PortfoliosAndTestForComplexProductResponse : IResponseNested
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public List<PortfoliosAndTestForComplexProductModel> TestForComplexProductList { get; set; } = new List<PortfoliosAndTestForComplexProductModel>();
    }
}
