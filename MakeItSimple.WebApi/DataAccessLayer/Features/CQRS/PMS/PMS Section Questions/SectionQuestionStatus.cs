using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions
{
    public class SectionQuestionStatus
    {
        public class SectionQuestionStatusResult
        {
            public int Id { get; set; }
            public bool? Status { get; set; }
        }

        public class SectionQuestionStatusCommand : IRequest<Result>
        {
            public int Id { get; set; }

        }

        public class Handler : IRequestHandler<SectionQuestionStatusCommand, Result>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(SectionQuestionStatusCommand command, CancellationToken cancellationToken)
            {

                var checklist = await _context.PmsSectionQuestions.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

                if (checklist == null)
                {
                    return Result.Failure(PmsError.QuestionDoesNotExist());
                }

                checklist.IsActive = !checklist.IsActive;

                await _context.SaveChangesAsync(cancellationToken);

                var results = new SectionQuestionStatusResult
                {
                    Id = checklist.Id,
                    Status = checklist.IsActive
                };

                return Result.Success(results);



            }
        }
    }
}
