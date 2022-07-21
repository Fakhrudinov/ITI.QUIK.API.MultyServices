using DataAbstraction.Models;
using DataAbstraction.Responses;
using DataValidationService.SingleEntityValidation;
using FluentValidation.Results;

namespace DataValidationService
{
    public static class ValidateData
    {
        public static ListStringResponseModel ValidateMatrixClientAccount(string matrixClientAccount)
        {
            MatrixClientAccountValidator validator = new MatrixClientAccountValidator();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(matrixClientAccount);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static ListStringResponseModel ValidateMatrixFortsCode(string fortsClientCode)
        {
            MatrixFortsCodeValidator validator = new MatrixFortsCodeValidator();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(fortsClientCode);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static ListStringResponseModel ValidateMatrixClientPortfolioModel(MatrixClientPortfolioModel model)
        {
            MatrixClientPortfolioMoMsFxRsValidator validator = new MatrixClientPortfolioMoMsFxRsValidator();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model.MatrixClientPortfolio);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static ListStringResponseModel ValidateMatrixCodeAndPubringKeyModel(MatrixCodeAndPubringKeyModel model)
        {
            MatrixCodeAndPubringKeyModelValidationService validator = new MatrixCodeAndPubringKeyModelValidationService();
            ListStringResponseModel responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static ListStringResponseModel ValidateFortsCodeAndPubringKeyModel(FortsCodeAndPubringKeyModel model)
        {
            FortsCodeAndPubringKeyModelValidationService validator = new FortsCodeAndPubringKeyModelValidationService();
            ListStringResponseModel responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }
    }
}