using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType
{
    public class TypeStatus
    {

        public class TypeStatusResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class TypeStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<TypeStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(TypeStatusCommand command, CancellationToken cancellationToken)
            {

                var type = await _context.PmsType.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (type == null)
                {
                    return Result.Failure(PmsError.TypeDoesNotExist());
                }

                type.IsActive = !type.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new TypeStatusResult
                {
                    Id = type.Id,
                    Status = type.IsActive
                };

                return Result.Success(results);



            }
        }
    }
}
