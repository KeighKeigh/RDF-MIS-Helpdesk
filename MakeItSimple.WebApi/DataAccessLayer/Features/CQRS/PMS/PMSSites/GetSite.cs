using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount.GetUser;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites
{
    public class GetSite
    {

        public class GetSiteResult
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool? IsActive { get; set; }

        }

        public class GetSiteQuery : UserParams, IRequest<PagedList<GetSiteResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetSiteQuery, PagedList<GetSiteResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetSiteResult>> Handle(GetSiteQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Sites> sites  = _context.Sites.AsNoTracking();

                if(!string.IsNullOrEmpty(request.Search))
                {
                    sites = sites.Where(x => x.Site.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    sites = sites.Where(x => x.IsActive == request.Status);

                }

                var allsites = sites.Select(x => new GetSiteResult
                {
                    Id = x.Id,
                    Name = x.Site,
                    IsActive = x.IsActive,
                });

                return await PagedList<GetSiteResult>.CreateAsync(allsites, request.PageNumber, request.PageSize);
            }
        }
    }
}
