using DataAbstraction.Models;
using DataValidationService.SingleEntityValidation;
using FluentValidation;

namespace DataValidationService
{
    internal class MatrixToFortsCodesMappingModelValidationService : AbstractValidator<MatrixToFortsCodesMappingModel>
    {
        public MatrixToFortsCodesMappingModelValidationService()
        {
            RuleFor(x => x.MatrixClientCode).SetValidator(new MatrixClientPortfolioRfValidator());
            RuleFor(x => x.FortsClientCode).SetValidator(new MatrixFortsCodeValidator());
        }
    }
}