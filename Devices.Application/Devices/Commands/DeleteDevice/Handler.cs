﻿using System.Threading;
using System.Threading.Tasks;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Devices.Application.Devices.DTOs;
using Devices.Application.Extensions;
using Devices.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Devices.Application.Devices.Commands.DeleteDevice
{
    public class Handler : IRequestHandler<DeleteDeviceCommand>
    {
        private readonly ChallengeValidator _challengeValidator;
        private readonly IDbContext _dbContext;
        private readonly ILogger<Handler> _logger;
        private readonly IUserContext _userContext;

        public Handler(IDbContext dbContext, IUserContext userContext, ChallengeValidator challengeValidator, ILogger<Handler> logger)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _challengeValidator = challengeValidator;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            var device = await _dbContext.Set<Device>()
                .OfIdentity(_userContext.GetAddress())
                .NotDeleted()
                .Include(d => d.Identity)
                .FirstWithId(request.DeviceId, cancellationToken);

            await _challengeValidator.Validate(request.SignedChallenge, PublicKey.FromBytes(device.Identity.PublicKey));

            _logger.LogTrace("Challenge successfully validated.");

            device.MarkAsDeleted(request.DeletionCertificate, _userContext.GetDeviceId());

            _dbContext.Set<Device>().Update(device);

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogTrace($"Successfully marked device with id '{request.DeviceId}' as deleted.");

            return Unit.Value;
        }
    }
}
