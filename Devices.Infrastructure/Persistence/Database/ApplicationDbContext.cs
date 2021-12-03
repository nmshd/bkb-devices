using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Infrastructure.Persistence.Database.ValueConverters;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Devices.Domain.Entities;
using Devices.Infrastructure.Persistence.Database.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Devices.Infrastructure.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        private const int MAX_RETRY_COUNT = 50000;
        private static readonly TimeSpan MAX_RETRY_DELAY = TimeSpan.FromSeconds(1);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Identity> Identities { get; set; }

        public DbSet<Challenge> Challenges { get; set; }

        public IQueryable<T> SetReadOnly<T>() where T : class
        {
            return Set<T>().AsNoTracking();
        }

        public async Task RunInTransaction(Func<Task> action, List<int> errorNumbersToRetry,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var executionStrategy =
                new SqlServerRetryingExecutionStrategy(this, MAX_RETRY_COUNT, MAX_RETRY_DELAY, errorNumbersToRetry);

            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var transaction = await Database.BeginTransactionAsync(isolationLevel);
                await action();
                await transaction.CommitAsync();
            });
        }

        public async Task RunInTransaction(Func<Task> action, IsolationLevel isolationLevel)
        {
            await RunInTransaction(action, null, isolationLevel);
        }

        public async Task<T> RunInTransaction<T>(Func<Task<T>> func, List<int> errorNumbersToRetry,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var response = default(T);

            await RunInTransaction(async () => { response = await func(); }, errorNumbersToRetry, isolationLevel);

            return response;
        }

        public async Task<T> RunInTransaction<T>(Func<Task<T>> func, IsolationLevel isolationLevel)
        {
            return await RunInTransaction(func, null, isolationLevel);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(DeviceEntityTypeConfiguration).Assembly);


            builder.UseValueConverter(
                new UsernameValueConverter(
                    new ConverterMappingHints(Username.MAX_LENGTH)));

            builder.UseValueConverter(
                new IdentityAddressValueConverter(new ConverterMappingHints(IdentityAddress.MAX_LENGTH)));

            builder.UseValueConverter(new DeviceIdValueConverter(new ConverterMappingHints(DeviceId.MAX_LENGTH)));
        }
    }
}
