using FluentValidation;


namespace DataValidationService.SingleEntityValidation
{
    internal class SecurityNameValidator : AbstractValidator<string>
    {
        internal SecurityNameValidator()
        {
            RuleFor(x => x)
                .Length(3, 12)
                    .WithMessage("{PropertyName} '{PropertyValue}' lenght of security is not valid. Min=3, max=12")
                    .WithErrorCode("SN100")
                .Matches("(^[A-Z0-9_-]{3,12}$)|(^[A-Za-z0-9]{2}[*]$)")
                    .WithMessage("{PropertyName} '{PropertyValue}' contain invalid characters (^[A-Z0-9_-]{3,12}$)|(^[A-Za-z0-9]{2}[*]$)")
                    .WithErrorCode("SN101");

            // ^[A-Z0-9_-]{3,12}$ для фодндовых и валютных
            // ^[A-Za-z0-9]{2}[*]$ для фьючерсов
        }
    }
}
