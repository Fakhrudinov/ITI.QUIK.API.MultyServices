using DataAbstraction.Interfaces;
using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class ClientBOInformationResponse : IResponseNested
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public ClientBOInformationModel ClientBOInformation { get; set; } = new ClientBOInformationModel();
    }
}
