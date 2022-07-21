using DataAbstraction.Models;
using DataValidationService.SingleEntityValidation;
using FluentValidation;

namespace DataValidationService
{
    public class NewClientOptionWorkshopValidationService : AbstractValidator<NewClientOptionWorkShopModel>
    {
        public NewClientOptionWorkshopValidationService()
        {
            RuleFor(x => x.CodesPairRF).Must(x => x.Length > 0)
                    .WithMessage("{PropertyName} Должен быть как минимум 1 портфель/код в массиве CodesPair")
                    .WithErrorCode("PP800");
            RuleForEach(x => x.CodesPairRF).ChildRules(codes =>
            {
                codes.RuleFor(x => x.MatrixClientCode).SetValidator(new MatrixClientPortfolioRfValidator());
            });

            RuleForEach(x => x.CodesPairRF).ChildRules(codes =>
            {
                codes.RuleFor(x => x.FortsClientCode).SetValidator(new MatrixFortsCodeValidator());
            });

            RuleFor(x => x.Client).SetValidator(new NewClientNameEmailValidator());

            RuleFor(x => x.Key).SetValidator(new QAdminPubringKeyValidator());
        }
    }
}