using DataAbstraction.Interfaces;

namespace DataAbstraction.Models.Discounts
{
    public class DiscountMatrixSingleResponse : IResponseDirect
    {
        public DiscountMatrixSingleResponse()
        {
            Discount = new DiscountMatrixModel();

            IsSuccess = true;
            Messages = new List<string>();
        }

        public DiscountMatrixModel Discount { get; set; }

        public bool IsSuccess { get; set; }
        public List<string> Messages { get; set; }
    }
}
