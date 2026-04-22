using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.CategoryConcernSetup
{
    public class GetCategoryConcern
    {
        public class GetCategoryConcernResult
        {
            public int Id { get; set; }
            public string categoryConcern { get; set; }

            public bool? Is_Active { get; set; }
        }

        public class GetCategoryConcernQuery : UserParams, IRequest<PagedList<GetCategoryConcernResult>>
        {
            public string Search {  get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetCategoryConcernQuery, PagedList<GetCategoryConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }


            public async Task<PagedList<GetCategoryConcernResult>> Handle(GetCategoryConcernQuery request, CancellationToken cancellationToken)
            {
                IQueryable<CategoryConcern> categoryConcernsQuery = _context.CategoryConcerns
                .AsNoTracking().AsQueryable();

                if(!string.IsNullOrEmpty(request.Search))
                {
                    categoryConcernsQuery = categoryConcernsQuery.Where(x => x.CategoryConcernName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    categoryConcernsQuery = categoryConcernsQuery.Where(x => x.IsActive == request.Status);
                }

                var result = categoryConcernsQuery.Select(x => new GetCategoryConcernResult
                {
                    Id = x.Id,
                    categoryConcern = x.CategoryConcernName,
                    Is_Active = x.IsActive,
                }).OrderBy(x => x.categoryConcern != "N/A").ThenBy(x => x.Id);

                return await PagedList<GetCategoryConcernResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }

            
            
        }
    }
}
