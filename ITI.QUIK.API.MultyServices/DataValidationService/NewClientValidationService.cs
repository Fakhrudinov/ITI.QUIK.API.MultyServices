using DataAbstraction.Models;
using DataValidationService.SingleEntityValidation;
using FluentValidation;


namespace DataValidationService
{
    public class NewClientValidationService : AbstractValidator<NewClientModel>
    {
        public NewClientValidationService()
        {
            RuleForEach(x => x.MatrixClientPortfolios).ChildRules(codes =>
            {
                codes.RuleFor(x => x.MatrixClientPortfolio).SetValidator(new MatrixClientPortfolioMoMsFxRsCdValidator());
            });


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

            RuleFor(x => x.MatrixClientPortfolios)
                .NotEmpty()
                    .When(x => x.CodesPairRF == null)
                    .WithMessage("MO MS FX CD RS Portfolios array 'CodesMatrix' must be Greater Than 0 when RF portfolios array 'CodesPairRF' is not set")
                    .WithErrorCode("PP910");
            RuleFor(x => x.CodesPairRF)
                .NotEmpty()
                    .When(x => x.MatrixClientPortfolios == null)
                    .WithMessage("RF portfolios array 'CodesPairRF' must be Greater Than 0 when MO MS FX CD RS Portfolios array 'CodesMatrix' is not set")
                    .WithErrorCode("PP911");

        }
    }
}