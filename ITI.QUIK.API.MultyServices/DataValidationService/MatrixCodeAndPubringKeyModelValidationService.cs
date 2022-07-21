using DataAbstraction.Models;
using DataValidationService.SingleEntityValidation;
using FluentValidation;

namespace DataValidationService
{
    public class MatrixCodeAndPubringKeyModelValidationService : AbstractValidator<MatrixCodeAndPubringKeyModel>
    {
        public MatrixCodeAndPubringKeyModelValidationService()
        {
            RuleFor(x => x.MatrixClientPortfolio.MatrixClientPortfolio).SetValidator(new MatrixClientPortfolioMoMsFxRsValidator());
            RuleFor(x => x.Key).SetValidator(new QAdminPubringKeyValidator());
        }
    }
}
