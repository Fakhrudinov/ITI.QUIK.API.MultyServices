using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class NewClientOptionWorkShopModelResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public NewClientOptionWorkShopModel NewOWClient { get; set; } = new NewClientOptionWorkShopModel();
    }
}
