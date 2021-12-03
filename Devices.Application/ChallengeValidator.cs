﻿using System.Text.Json;
using System.Threading.Tasks;
using Devices.Application.Devices.DTOs;
using Devices.Application.DTOs;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.Crypto;
using Enmeshed.Crypto.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Devices.Application
{
    public class ChallengeValidator
    {
        private readonly IDbContext _dbContext;
        private readonly ISignatureHelper _signatureHelper;

        public ChallengeValidator(IDbContext dbContext, ISignatureHelper signatureHelper)
        {
            _dbContext = dbContext;
            _signatureHelper = signatureHelper;
        }

        public async Task Validate(SignedChallengeDTO signedChallenge, PublicKey publicKey)
        {
            ValidateSignature(signedChallenge.Challenge, Signature.FromBytes(signedChallenge.Signature).Bytes, publicKey.Key);
            await ValidateChallengeExpiracy(signedChallenge.Challenge);
        }

        private void ValidateSignature(string challenge, byte[] signature, byte[] publicKey)
        {
            var signatureIsValid = _signatureHelper.VerifySignature(
                ConvertibleString.FromUtf8(challenge),
                ConvertibleString.FromByteArray(signature),
                ConvertibleString.FromByteArray(publicKey));

            if (!signatureIsValid)
                throw new OperationFailedException(ApplicationErrors.Devices.InvalidSignature());
        }

        private async Task ValidateChallengeExpiracy(string challengeString)
        {
            var idOfSignedChallenge = JsonSerializer.Deserialize<ChallengeDTO>(challengeString, new JsonSerializerOptions {PropertyNameCaseInsensitive = true}).Id;

            var challenge = await _dbContext.SetReadOnly<Challenge>().FirstOrDefaultAsync(c => c.Id == idOfSignedChallenge);

            if (challenge == null)
                throw new NotFoundException(nameof(Challenge));

            if (challenge.IsExpired())
                throw new OperationFailedException(ApplicationErrors.Devices.ChallengeHasExpired());
        }
    }
}
