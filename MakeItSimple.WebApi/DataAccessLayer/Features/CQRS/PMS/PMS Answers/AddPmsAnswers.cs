using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.AddSitePivot;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers
{
    public class AddPmsAnswers
    {
        public class AddPmsAnswersCommand : IRequest<Result>
        { 
            public int? PmsId { get; set; }
            public Guid? AnsweredBy { get; set; }
            public List<PmsAnswers> PmsAnswers { get; set; }

        }

        public class PmsAnswers
        {
            public int? QuestionId { get; set; }
            public bool? StatusAnswer { get; set; }
            public string AssetTagAnswer { get; set; }
            public string RemarksAnswer { get; set; }
            public string ParagraphAnswer { get; set; }
            public string Textfield { get;set; }
        }
        public class Handler : IRequestHandler<AddPmsAnswersCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }


            public async Task<Result> Handle(AddPmsAnswersCommand command, CancellationToken cancellationToken)
            {
                var addAnswers = await _unitOfWork.PmsCreate.AddAnswers(command, cancellationToken);

                if(command.PmsAnswers.Count > 1)
                {
                    return Result.Success(PmsConsString.AnswersAdded);
                }
                return Result.Success(PmsConsString.AnswerAdded);
            }
        }

    }
}
