using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICoreDiscounts
    {
        Task<BoolResponse> CheckSingleDiscount(string security);
        Task<BoolResponse> DeleteSingleDiscount(string security);
        Task<BoolResponse> PostSingleDiscount(string security);
    }
}
