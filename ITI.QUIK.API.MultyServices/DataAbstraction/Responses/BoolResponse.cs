using DataAbstraction.Interfaces;

namespace DataAbstraction.Responses
{
    public class BoolResponse : IResponseDirect
    {
        public BoolResponse()
        {
            IsSuccess = true;
            Messages = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }

        public bool IsTrue { get; set; } = false;
    }
}
