using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot
{
    public class GetSitePivot
    {

        public class GetSitePivotResult
        {
            public int? SiteId { get; set; }
            public string SiteName { get; set; }
            public bool? IsActive { get; set; }

            public List<BusinessUnits> BusinessUnits { get; set; }
        }

        public class BusinessUnits
        {
            public int? BusinessUnitId { get; set; }
            public string BusinessUnitName { get; set; }
            
        }
        public class GetSitePivotQuery : UserParams, IRequest<PagedList<GetSitePivotResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetSitePivotQuery, PagedList<GetSitePivotResult>>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<GetSitePivotResult>> Handle(GetSitePivotQuery request, CancellationToken cancellationToken)
            {
                IQueryable<SitePivot> pivotQuery = _context.SitesPivot
                    .AsNoTracking()
                    .Include(x => x.Site);


                if (!string.IsNullOrEmpty(request.Search))
                {
                    pivotQuery = pivotQuery.Where(x => x.BusinessUnitName.Contains(request.Search)
                    || x.Site.Site.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    pivotQuery = pivotQuery.Where(x => x.IsActive == request.Status);
                }

                var result = pivotQuery.GroupBy(x => new { x.SiteId, x.Site.Site}).Select(x => new GetSitePivotResult
                {
                    SiteId = x.Key.SiteId,
                    SiteName = x.Key.Site,
                    IsActive =x.First().IsActive,
                    BusinessUnits = x.Select(s => new BusinessUnits
                    {
                        BusinessUnitId = s.BusinessUnitId,
                        BusinessUnitName = s.BusinessUnitName,
                    }).Distinct().ToList()

                });

                return await PagedList<GetSitePivotResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
