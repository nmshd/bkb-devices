using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.Devices.Commands.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(c => c.OldPassword).DetailedNotEmpty();
            RuleFor(c => c.NewPassword).DetailedNotEmpty();
        }
    }
}
