﻿using Devices.Application.Devices.DTOs;
using Devices.Application.Extensions;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devices.Application.Identities.CreateIdentity;

public class Handler : IRequestHandler<CreateIdentityCommand, CreateIdentityResponse>
{
    private readonly ApplicationOptions _applicationOptions;
    private readonly ChallengeValidator _challengeValidator;
    private readonly IDbContext _dbContext;
    private readonly ILogger<Handler> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public Handler(IDbContext dbContext, UserManager<ApplicationUser> userManager, ChallengeValidator challengeValidator, ILogger<Handler> logger, IOptions<ApplicationOptions> applicationOptions)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _challengeValidator = challengeValidator;
        _logger = logger;
        _applicationOptions = applicationOptions.Value;
    }

    public async Task<CreateIdentityResponse> Handle(CreateIdentityCommand command, CancellationToken cancellationToken)
    {
        var publicKey = PublicKey.FromBytes(command.IdentityPublicKey);
        await _challengeValidator.Validate(command.SignedChallenge, publicKey);

        _logger.LogTrace("Challenge sucessfully validated.");

        var address = IdentityAddress.Create(publicKey.Key, _applicationOptions.AddressPrefix);

        _logger.LogTrace($"Address created. Result: {address}");

        var existingIdentity = await _dbContext.Set<Identity>().FirstWithAddressOrDefault(address, cancellationToken);

        if (existingIdentity != null)
            throw new OperationFailedException(ApplicationErrors.Devices.AddressAlreadyExists());

        var newIdentity = new Identity(command.ClientId, address, command.IdentityPublicKey);

        var user = new ApplicationUser(newIdentity);

        var createUserResult = await _userManager.CreateAsync(user, command.DevicePassword);

        if (!createUserResult.Succeeded)
            throw new OperationFailedException(ApplicationErrors.Devices.RegistrationFailed(createUserResult.Errors.First().Description));

        _logger.LogTrace($"Identity created. Address: {newIdentity.Address}, Device ID: {user.DeviceId}, Username: {user.UserName}");

        return new CreateIdentityResponse
        {
            Address = address,
            CreatedAt = newIdentity.CreatedAt,
            Device = new CreateIdentityResponseDevice
            {
                Id = user.DeviceId,
                Username = user.UserName,
                CreatedAt = user.Device.CreatedAt
            }
        };
    }
}
