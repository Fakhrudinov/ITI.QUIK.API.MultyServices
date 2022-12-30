using DataAbstraction.Interfaces;
using DataAbstraction.Responses;

namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientAndMoneyResponse : IResponseNested
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();

        public List<ClientAndMoneyModel> Clients { get; set; } = new List<ClientAndMoneyModel>();
    }
}
