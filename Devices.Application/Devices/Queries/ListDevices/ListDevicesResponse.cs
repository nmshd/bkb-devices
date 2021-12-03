using System.Collections.Generic;
using Enmeshed.BuildingBlocks.Application.Pagination;
using Devices.Application.Devices.DTOs;

namespace Devices.Application.Devices.Queries.ListDevices
{
    public class ListDevicesResponse : PagedResponse<DeviceDTO>
    {
        public ListDevicesResponse(IEnumerable<DeviceDTO> items, PaginationFilter previousPaginationFilter, int totalRecords) : base(items, previousPaginationFilter, totalRecords) { }
    }
}
