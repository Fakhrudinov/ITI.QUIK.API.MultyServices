﻿using DataAbstraction.Responses;

namespace DataAbstraction.Interfaces
{
    public interface ICoreDiscounts
    {
        Task<BoolResponse> CheckSingleDiscount(string security);
    }
}
