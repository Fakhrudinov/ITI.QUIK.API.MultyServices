using DataAbstraction.Models;
using DataValidationService.SingleEntityValidation;
using FluentValidation;

namespace DataValidationService
{
    public class FortsCodeAndPubringKeyModelValidationService : AbstractValidator<FortsCodeAndPubringKeyModel>
    {
        public FortsCodeAndPubringKeyModelValidationService()
        {
            RuleFor(x => x.ClientCode.FortsClientCode).SetValidator(new MatrixFortsCodeValidator());
            RuleFor(x => x.Key).SetValidator(new QAdminPubringKeyValidator());
        }
    }
}