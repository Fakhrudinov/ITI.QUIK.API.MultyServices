
using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class PubringKeyModelResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public PubringKeyModel Key { get; set; } = new PubringKeyModel();
    }
}
