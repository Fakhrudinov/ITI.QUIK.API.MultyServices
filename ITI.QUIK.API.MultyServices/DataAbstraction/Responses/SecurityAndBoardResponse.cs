using DataAbstraction.Interfaces;
using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class SecurityAndBoardResponse : IResponseNested
    {
        public List<SecurityAndBoardModel> SecurityAndBoardList { get; set; } = new List<SecurityAndBoardModel>();
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
    }
}
