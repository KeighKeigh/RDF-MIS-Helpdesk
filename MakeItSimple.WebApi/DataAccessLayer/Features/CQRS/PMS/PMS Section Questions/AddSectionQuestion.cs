using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions
{
    public class AddSectionQuestion
    {
        public class AddSectionQuestionCommand : IRequest<Result>
        {
            public int? ChecklistManagementId { get; set; }
            public int? PmsTypeId { get; set; }
            public List<SectionList> SectionLists { get; set; }
            public Guid? AddedBy { get; set; }

        }

        public class SectionList
        {
            public string Section { get; set; }
            public List<HeaderList> HeaderLists { get; set; }
        }

        public class HeaderList
        {
            public string Header { get; set; }
            public bool? HasHeader { get; set; }
            public List<AddQuestionList> QuestionLists { get; set; }
        }
        public class AddQuestionList
        {
            public int? QuestionId { get; set; }
            public string Questions { get; set; }
            public bool? HasCheckBox { get; set; }
            public bool? HasRemarks { get; set; }
            public bool? HasAssetTag { get; set; }
            public bool? HasParagraph { get; set; }
            public int? OrderBy { get; set; }
            
        }

        public class Handler : IRequestHandler<AddSectionQuestionCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(AddSectionQuestionCommand command, CancellationToken cancellationToken)
            {
                var addSectionQuestions = await _unitOfWork.PmsCreate.AddNewSectionQuestion(command, cancellationToken);

                return Result.Success(PmsConsString.AddedSuccessful);
            }
        }
    }
}
