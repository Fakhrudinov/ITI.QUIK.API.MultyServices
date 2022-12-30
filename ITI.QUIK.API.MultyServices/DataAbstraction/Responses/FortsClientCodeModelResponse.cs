using DataAbstraction.Interfaces;
using DataAbstraction.Models;


namespace DataAbstraction.Responses
{
    public class FortsClientCodeModelResponse : IResponseNested
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public List<FortsClientCodeModel> FortsClientCodesList { get; set; } = new List<FortsClientCodeModel>();
    }
}
