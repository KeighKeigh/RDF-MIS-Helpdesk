using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType
{
    public class GetType
    {

        public class GetTypeResult
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool? IsActive { get; set; }

        }

        public class GetTypeQuery : UserParams, IRequest<PagedList<GetTypeResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetTypeQuery, PagedList<GetTypeResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetTypeResult>> Handle(GetTypeQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsPhaseTwoType> types = _context.PmsType;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    types = types.Where(x => x.PmsType.ToLower().Contains(request.Search));
                }

                if (request.Status != null)
                {
                    types = types.Where(x => x.IsActive == request.Status);

                }

                var allsites = types.Select(x => new GetTypeResult
                {
                    Id = x.Id,
                    Name = x.PmsType,
                    IsActive = x.IsActive
                });

                return await PagedList<GetTypeResult>.CreateAsync(allsites, request.PageNumber, request.PageSize);
            }
        }
    }
}
