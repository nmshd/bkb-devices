using AutoMapper;
using AutoMapper.QueryableExtensions;
using Devices.Application.Devices.DTOs;
using Devices.Application.Extensions;
using Devices.Domain.Entities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.Persistence.Database;
using Enmeshed.BuildingBlocks.Application.Abstractions.Infrastructure.UserContext;
using Enmeshed.BuildingBlocks.Application.Extensions;
using Enmeshed.DevelopmentKit.Identity.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Devices.Application.Devices.Queries.ListDevices;

public class Handler : IRequestHandler<ListDevicesQuery, ListDevicesResponse>
{
    private readonly IdentityAddress _activeIdentity;
    private readonly IDbContext _dbContext;
    private readonly IMapper _mapper;

    public Handler(IDbContext dbContext, IMapper mapper, IUserContext userContext)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _activeIdentity = userContext.GetAddress();
    }

    public async Task<ListDevicesResponse> Handle(ListDevicesQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .SetReadOnly<Device>()
            .NotDeleted()
            .OfIdentity(_activeIdentity);

        if (request.Ids.Any())
            query = query.WithIdIn(request.Ids);
        
        var items = await query
            .OrderBy(d => d.CreatedAt)
            .Paged(request.PaginationFilter)
            .ProjectTo<DeviceDTO>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var totalNumberOfItems = items.Count < request.PaginationFilter.PageSize ? items.Count : await query.CountAsync(cancellationToken);

        return new ListDevicesResponse(items, request.PaginationFilter, totalNumberOfItems);
    }
}
