﻿using Devices.Application.Extensions;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Devices.Application.Devices.Commands.ChangePassword;

public class Handler : IRequestHandler<ChangePasswordCommand>
{
    private readonly DeviceId _activeDevice;
    private readonly IDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public Handler(UserManager<ApplicationUser> userManager, IDbContext dbContext, IUserContext userContext, ILogger<Handler> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
        _activeDevice = userContext.GetDeviceId();
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentDevice = await _dbContext
            .SetReadOnly<Device>()
            .NotDeleted()
            .Include(d => d.User)
            .FirstWithId(_activeDevice, cancellationToken);

        var changePasswordResult = await _userManager.ChangePasswordAsync(currentDevice.User, request.OldPassword, request.NewPassword);

        if (!changePasswordResult.Succeeded)
            if (!changePasswordResult.Succeeded)
                throw new OperationFailedException(ApplicationErrors.Devices.ChangePasswordFailed(changePasswordResult.Errors.First().Description));

        _logger.LogTrace($"Successfuly changed password for device with id '{_activeDevice}'.");

        return Unit.Value;
    }
}