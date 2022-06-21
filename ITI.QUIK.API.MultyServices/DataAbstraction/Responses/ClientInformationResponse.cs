using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class ClientInformationResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public ClientInformationModel ClientInformation { get; set; } = new ClientInformationModel();
    }
}
