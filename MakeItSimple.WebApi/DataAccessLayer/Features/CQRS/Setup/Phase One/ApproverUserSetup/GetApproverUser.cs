using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_One.ApproverUserSetup
{
    public class GetApproverUser
    {
        public class GetApproverUserResult
        {

            public int Id { get; set; }
            public Guid? ApproverId { get; set; }
            public string ApproverName { get; set; }
            public List<UserForApprover> UserForApprovers { get; set; }

            public class UserForApprover
            {
                public Guid? UserId { get; set; }
                public string UserName { get; set; }
            }
        }

        public class GetApproverUserQuery : UserParams, IRequest<PagedList<GetApproverUserResult>>
        {
            public string Search { get; set; }
            
        }

        public class Handler : IRequestHandler<GetApproverUserQuery, PagedList<GetApproverUserResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetApproverUserResult>> Handle(GetApproverUserQuery request, CancellationToken cancellationToken)
            {
                IQueryable<ApproverUser> approverUserQuery = _context.ApproverUsers
                    .AsNoTrackingWithIdentityResolution();

                if(!string.IsNullOrEmpty(request.Search))
                {
                    approverUserQuery = approverUserQuery
                        .Where(x => x.Approver.Fullname.ToLower().Contains(request.Search));
                }

                var result = approverUserQuery.GroupBy(x => x.ApproverId)
                    .Select(x => new GetApproverUserResult
                    {
                        Id = x.First().Id,
                        ApproverId = x.Key,
                        ApproverName = x.First().Approver.Fullname,
                        UserForApprovers = x.Select(x => new GetApproverUserResult.UserForApprover
                        {
                            UserId = x.UserId,
                            UserName = x.User.Fullname,
                        }).ToList(),
                    });

                return await PagedList<GetApproverUserResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
