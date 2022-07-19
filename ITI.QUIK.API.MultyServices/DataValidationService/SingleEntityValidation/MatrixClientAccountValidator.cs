using FluentValidation;

namespace DataValidationService.SingleEntityValidation
{
    internal class MatrixClientAccountValidator : AbstractValidator<string>
    {
        internal MatrixClientAccountValidator()
        {
            RuleFor(x => x)
                .Matches("^(AA|B[PC])[0-9]{4,6}$")
                    .WithMessage("{PropertyName} '{PropertyValue}' is not in format 'BP12345'.")
                    .WithErrorCode("CA100");
        }
    }
}