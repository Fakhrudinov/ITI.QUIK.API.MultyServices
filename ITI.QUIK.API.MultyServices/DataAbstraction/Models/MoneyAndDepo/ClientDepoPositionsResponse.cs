using DataAbstraction.Responses;

namespace DataAbstraction.Models.MoneyAndDepo
{
    public class ClientDepoPositionsResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();

        public List<ClientDepoPositionModel> Clients { get; set; } = new List<ClientDepoPositionModel>();
    }
}
