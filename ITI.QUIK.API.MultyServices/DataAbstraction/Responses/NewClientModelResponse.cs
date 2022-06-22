using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class NewClientModelResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public NewClientModel NewClient { get; set; } = new NewClientModel();
    }
}
