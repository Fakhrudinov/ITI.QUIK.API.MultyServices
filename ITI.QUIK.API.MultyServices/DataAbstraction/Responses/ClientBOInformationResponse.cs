using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class ClientBOInformationResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public ClientBOInformationModel ClientBOInformation { get; set; } = new ClientBOInformationModel();
    }
}
