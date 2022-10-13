using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICoreSingleServices
    {
        Task<BoolResponse> CheckIsAnyFortsCodesFromOptionWorkshopInEDP();
        Task<BoolResponse> CheckIsFileCorrectLimLim(bool sendReport);
    }
}
