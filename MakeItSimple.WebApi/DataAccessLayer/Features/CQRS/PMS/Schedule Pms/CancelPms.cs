using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms
{
    public class CancelPms
    {
        public class CancelPmsResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class CancelPmsCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<CancelPmsCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(CancelPmsCommand command, CancellationToken cancellationToken)
            {

                var type = await _context.Pmss.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (type == null)
                {
                    return Result.Failure(PmsError.TypeDoesNotExist());
                }

                type.IsCanceled = !type.IsCanceled;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new CancelPmsResult
                {
                    Id = type.Id,
                    Status = type.IsCanceled
                };

                return Result.Success(results);



            }
        }
    }
}
