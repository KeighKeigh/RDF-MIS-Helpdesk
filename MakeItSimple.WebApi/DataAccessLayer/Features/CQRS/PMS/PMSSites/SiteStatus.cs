using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites
{
    public class SiteStatus
    {

        public class SiteStatusResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class SiteStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<SiteStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SiteStatusCommand command, CancellationToken cancellationToken)
            {

                var site = await _context.Sites.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (site == null)
                {
                    return Result.Failure(PmsError.SiteDoesNotExist());
                }
                var siteInUse = await _context.SitesPivot.AnyAsync(x => x.SiteId == command.Id && x.IsActive == true, cancellationToken);
                if (siteInUse == true)
                {
                    return Result.Failure(PmsError.SiteInUse(site.Site));
                }

                site.IsActive = !site.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new SiteStatusResult
                {
                    Id = site.Id,
                    Status = site.IsActive
                };

                return Result.Success(results);



            }
        }
    }
}
