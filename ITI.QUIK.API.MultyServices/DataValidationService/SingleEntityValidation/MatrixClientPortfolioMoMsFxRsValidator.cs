using FluentValidation;

namespace DataValidationService.SingleEntityValidation
{
    internal class MatrixClientPortfolioMoMsFxRsValidator : AbstractValidator<string>
    {
        internal MatrixClientPortfolioMoMsFxRsValidator()
        {
            RuleFor(x => x)
                .Matches("^(AA|B[PC])[0-9]{4,6}-(MS|MO|FX|RS)-[0-9]{2,3}$")
                    .WithMessage("{PropertyName} '{PropertyValue}' is not in format 'BP12345-XX-01'. Accept only MS|MO|FX|RS portfolio")
                    .WithErrorCode("MS102");
        }
    }
}
