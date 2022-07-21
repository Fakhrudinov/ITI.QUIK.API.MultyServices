using DataAbstraction.Models;
using FluentValidation;

namespace DataValidationService.SingleEntityValidation
{
    internal class QAdminPubringKeyValidator : AbstractValidator<PubringKeyModel>
    {
        internal QAdminPubringKeyValidator()
        {
            RuleFor(x => x.KeyID)
                .Length(16)
                    .WithMessage("{PropertyName} '{PropertyValue}' must be 16 symbols lenght")
                    .WithErrorCode("QK100");
            RuleFor(x => x.Time)
                .GreaterThan(0)
                    .WithMessage("{PropertyName} '{PropertyValue}' must be Greater Than 0")
                    .WithErrorCode("QK101");
            RuleFor(x => x.RSAKey)
                .Length(64)
                    .WithMessage("{PropertyName} '{PropertyValue}' must be 64 symbols lenght")
                    .WithErrorCode("QK102");
        }
    }
}
