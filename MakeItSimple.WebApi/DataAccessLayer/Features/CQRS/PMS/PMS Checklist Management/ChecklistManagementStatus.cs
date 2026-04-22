using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management
{
    public class ChecklistManagementStatus
    {
        public class ChecklistManagementStatusResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class ChecklistManagementStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<ChecklistManagementStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(ChecklistManagementStatusCommand command, CancellationToken cancellationToken)
            {

                var checklist = await _context.PmsChecklistManagements.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (checklist == null)
                {
                    return Result.Failure(PmsError.CheckListDoesNotExist());
                }

                checklist.IsActive = !checklist.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new ChecklistManagementStatusResult
                {
                    Id = checklist.Id,
                    Status = checklist.IsActive
                };

                return Result.Success(results);



            }
        }
    }
}
