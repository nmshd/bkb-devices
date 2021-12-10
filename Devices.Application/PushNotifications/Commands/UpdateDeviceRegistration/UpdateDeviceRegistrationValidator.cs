using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.FluentValidation;
using FluentValidation;

namespace Devices.Application.PushNotifications.Commands.UpdateDeviceRegistration;

public class UpdateDeviceRegistrationValidator : AbstractValidator<UpdateDeviceRegistrationCommand>
{
    public UpdateDeviceRegistrationValidator()
    {
        RuleFor(dto => dto.Platform).DetailedNotNull().In("fcm", "apns");

        RuleFor(dto => dto.Handle)
            .DetailedNotNull()
            .Length(10, 500).WithErrorCode(GenericApplicationErrors.Validation.InvalidPropertyValue().Code);

        RuleFor(dto => dto.InstallationId).DetailedNotEmpty();
    }
}
