using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.OneCharging;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging
{
    public class GetOneChargingByBusinessUnit
    {
        public class GetOneChargingByBusinessUnitResult
        {
            public string ChargingCode { get; set; }
            public string ChargingName { get; set; }
            public string UnitCode { get; set; }
            public string UnitName { get; set; }
            public string SubunitCode { get; set; }
            public string SubunitName { get; set; }
            public int? BusinessUnitId { get; set; }
            public string BusinessUnitCode { get; set; }
            public string BusinessUnitName { get; set; }
            public DateTime? deleted_at { get; set; }
            public bool? IsActive { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }

        public class GetOneChargingByBusinessUnitQuery : UserParams, IRequest<PagedList<GetOneChargingByBusinessUnitResult>>
        {
            public List<int?> Ids { get; set; }
            public string Search { get; set; }

            public bool? Status { get; set; }
        }


        public class Handler : IRequestHandler<GetOneChargingByBusinessUnitQuery, PagedList<GetOneChargingByBusinessUnitResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetOneChargingByBusinessUnitResult>> Handle(GetOneChargingByBusinessUnitQuery request, CancellationToken cancellationToken)
            {
                IQueryable<OneChargingMIS> oneChargingList = _context.OneChargings;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    oneChargingList = oneChargingList.Where(x => x.business_unit_name.ToLower().Contains(request.Search.ToLower())
                    || x.business_unit_code.ToLower().Contains(request.Search.ToLower()));
                }

                if(request.Ids != null && request.Ids.Any())
                {
                    oneChargingList = oneChargingList.Where(x => request.Ids.Contains(x.business_unit_id));
                }
                if (request.Status != null)
                {
                    oneChargingList = oneChargingList.Where(x => x.IsActive == request.Status);
                }

                var result = oneChargingList.Select(x => new GetOneChargingByBusinessUnitResult
                {
                    ChargingCode = x.code,
                    ChargingName = x.name,
                    UnitCode = x.department_unit_code,
                    UnitName = x.department_unit_name,
                    SubunitCode = x.sub_unit_code,
                    SubunitName = x.sub_unit_name,
                    BusinessUnitId = x.business_unit_id,
                    BusinessUnitCode = x.business_unit_code,
                    BusinessUnitName = x.business_unit_name,
                    deleted_at = x.deleted_at,
                    IsActive = x.IsActive,
                    UpdatedAt = x.UpdatedAt,
                });

                return await PagedList<GetOneChargingByBusinessUnitResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}

