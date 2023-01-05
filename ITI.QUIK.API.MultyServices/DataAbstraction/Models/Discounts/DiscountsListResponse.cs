using DataAbstraction.Interfaces;

namespace DataAbstraction.Models.Discounts
{
    public class DiscountsListResponse : IResponseDirect
    {
        public DiscountsListResponse()
        {
            Discounts = new List<DiscountMatrixModel>();

            IsSuccess = true;
            Messages = new List<string>();
        }

        public List<DiscountMatrixModel> Discounts { get; set; }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
