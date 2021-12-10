using Devices.Domain.Entities;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Devices.Infrastructure.Persistence.Database.EntityConfigurations;

public class DeviceEntityTypeConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("Devices");

        builder.Property(x => x.Id).HasColumnType($"char({DeviceId.MAX_LENGTH})");
        builder.Property(x => x.DeletedByDevice).HasColumnType($"char({DeviceId.MAX_LENGTH})");
        builder.Property(x => x.IdentityAddress).HasColumnType($"char({IdentityAddress.MAX_LENGTH})");
    }
}
