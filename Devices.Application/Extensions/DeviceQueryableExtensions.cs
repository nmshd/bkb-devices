﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devices.Application.Extensions
{
    public static class DeviceQueryableExtensions
    {
        public static async Task<Device> FirstWithId(this IQueryable<Device> query, DeviceId id, CancellationToken cancellationToken)
        {
            var device = await query.WithId(id).FirstOrDefaultAsync(cancellationToken);

            if (device == null)
                throw new NotFoundException(nameof(Device));

            return device;
        }

        public static IQueryable<Device> WithId(this IQueryable<Device> query, DeviceId id)
        {
            return query.Where(d => d.Id == id);
        }

        public static IQueryable<Device> OfIdentity(this IQueryable<Device> query, IdentityAddress address)
        {
            return query.Where(d => d.IdentityAddress == address);
        }

        public static IQueryable<Device> NotDeleted(this IQueryable<Device> query)
        {
            return query.Where(Device.IsNotDeleted);
        }

        public static IQueryable<Device> WithIdIn(this IQueryable<Device> query, IEnumerable<DeviceId> ids)
        {
            return query.Where(d => ids.Contains(d.Id));
        }

        public static IQueryable<Device> IncludeUser(this IQueryable<Device> query)
        {
            return query.Include(d => d.User);
        }
    }
}
