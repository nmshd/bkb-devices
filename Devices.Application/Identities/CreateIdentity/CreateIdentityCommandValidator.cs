using Enmeshed.BuildingBlocks.Application.FluentValidation;
using Devices.Application.Devices.DTOs.Validators;
using FluentValidation;

namespace Devices.Application.Identities.CreateIdentity
{
    public class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
    {
        public CreateIdentityCommandValidator()
        {
            RuleFor(c => c.IdentityPublicKey).DetailedNotEmpty();
            RuleFor(c => c.DevicePassword).DetailedNotEmpty();
            RuleFor(c => c.SignedChallenge).DetailedNotEmpty().SetValidator(new SignedChallengeDTOValidator());
        }
    }
}
