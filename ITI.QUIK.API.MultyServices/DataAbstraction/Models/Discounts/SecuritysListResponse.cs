using DataAbstraction.Interfaces;

namespace DataAbstraction.Models.Discounts
{
    public class SecuritysListResponse : IResponseDirect
    {
        public SecuritysListResponse()
        {
            IsSuccess = true;
            Messages = new List<string>();

            Securitys = new List<string>();
        }
        public List<string> Securitys { get; set; }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
