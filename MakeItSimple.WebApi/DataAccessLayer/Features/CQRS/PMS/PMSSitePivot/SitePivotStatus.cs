using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.SitePivotStatus;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot
{
    public class SitePivotStatus
    {
        public class SitePivotStatusResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class SitePivotStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<SitePivotStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SitePivotStatusCommand command, CancellationToken cancellationToken)
            {

                var site = await _context.SitesPivot.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (site == null)
                {
                    return Result.Failure(PmsError.SiteBusinessUnitDoesNotExist());
                }
                var siteInUse = await _context.Sites.FirstOrDefaultAsync(x => x.Id == site.SiteId && x.IsActive == false, cancellationToken);
                if (siteInUse != null)
                {
                    return Result.Failure(PmsError.SiteIsInactive(siteInUse.Site));
                }

                site.IsActive = !site.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new SitePivotStatusResult
                {
                    Id = site.Id,
                    Status = site.IsActive
                };

                return Result.Success(results);



            }
        }
    }
}
