using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Questions.PMS_Checklist_Management
{
    public class GetChecklistManagement
    {
        public class GetChecklistManagementResult
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool? IsActive { get; set; }

        }

        public class GetChecklistManagementQuery : UserParams, IRequest<PagedList<GetChecklistManagementResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetChecklistManagementQuery, PagedList<GetChecklistManagementResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetChecklistManagementResult>> Handle(GetChecklistManagementQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsChecklistManagement> checklistmanagement = _context.PmsChecklistManagements;

                if (!string.IsNullOrEmpty(request.Search))
                {
                    checklistmanagement = checklistmanagement.Where(x => x.Title.Contains(request.Search)
                    && x.Description.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    checklistmanagement = checklistmanagement.Where(x => x.IsActive == request.Status);

                }

                var allchecklist = checklistmanagement.Select(x => new GetChecklistManagementResult
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsActive = x.IsActive,
                });

                return await PagedList<GetChecklistManagementResult>.CreateAsync(allchecklist, request.PageNumber, request.PageSize);
            }
        }
    }
}
