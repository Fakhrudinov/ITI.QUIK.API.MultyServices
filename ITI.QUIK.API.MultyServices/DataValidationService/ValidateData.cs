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
        public static FindedQuikClientResponse ValidateMatrixClientAccountToFindedQuik(string matrixClientAccount)
        {
            MatrixClientAccountValidator validator = new MatrixClientAccountValidator();
            var result = new FindedQuikClientResponse();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(matrixClientAccount);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);

                result.Messages.AddRange(responseList.Messages);
                result.IsSuccess = false;
            }

            return result;
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

        public static ListStringResponseModel ValidateNewClientOptionWorkShopModel(NewClientOptionWorkShopModel model)
        {
            NewClientOptionWorkshopValidationService validator = new NewClientOptionWorkshopValidationService();
            ListStringResponseModel responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static ListStringResponseModel ValidateNewClientModel(NewClientModel model)
        {
            NewClientValidationService validator = new NewClientValidationService();
            ListStringResponseModel responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            return responseList;
        }

        public static NewClientCreationResponse ValidateNewMatrixPortfolioToExistingClientModel(MatrixClientPortfolioModel model)
        {
            MatrixClientPortfolioMoMsFxRsCdValidator validator = new MatrixClientPortfolioMoMsFxRsCdValidator();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model.MatrixClientPortfolio);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            NewClientCreationResponse response = new NewClientCreationResponse();
            response.IsNewClientCreationSuccess = responseList.IsSuccess;
            response.NewClientCreationMessages.AddRange(responseList.Messages);

            return response;
        }

        public static NewClientCreationResponse ValidateNewFortsPortfolioToExistingClientModel(MatrixToFortsCodesMappingModel model)
        {
            MatrixToFortsCodesMappingModelValidationService validator = new MatrixToFortsCodesMappingModelValidationService();
            var responseList = new ListStringResponseModel();

            ValidationResult validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                responseList = SetResponseFromValidationResult.SetResponse(validationResult, responseList);
            }

            NewClientCreationResponse response = new NewClientCreationResponse();
            response.IsNewClientCreationSuccess = responseList.IsSuccess;
            response.NewClientCreationMessages.AddRange(responseList.Messages);

            return response;
        }
    }
}