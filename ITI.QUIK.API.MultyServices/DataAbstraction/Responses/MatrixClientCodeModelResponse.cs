﻿using DataAbstraction.Models;

namespace DataAbstraction.Responses
{
    public class MatrixClientCodeModelResponse
    {
        public ListStringResponseModel Response { get; set; } = new ListStringResponseModel();
        public List<MatrixClientCodeModel> MatrixClientCodesList { get; set; } = new List<MatrixClientCodeModel>();
    }
}
