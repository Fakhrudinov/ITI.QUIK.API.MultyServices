using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class MatrixToFortsCodesMappingResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public List<MatrixToFortsCodesMappingModel> MatrixToFortsCodesList { get; set; } = new List<MatrixToFortsCodesMappingModel>();
    }
}
