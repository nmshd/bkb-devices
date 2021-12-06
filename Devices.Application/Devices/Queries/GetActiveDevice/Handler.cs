using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Devices.Application.Devices.DTOs;
using Devices.Application.Extensions;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Devices.Application.Devices.Queries.GetActiveDevice
{
    public class Handler : IRequestHandler<GetActiveDeviceQuery, DeviceDTO>
    {
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public Handler(IDbContext dbContext, IMapper mapper, IUserContext userContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<DeviceDTO> Handle(GetActiveDeviceQuery request, CancellationToken cancellationToken)
        {
            var device = await _dbContext
                .SetReadOnly<Device>()
                .NotDeleted()
                .WithId(_userContext.GetDeviceId())
                .IncludeUser()
                .ProjectTo<DeviceDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return device;
        }
    }
}
