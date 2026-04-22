using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management
{
    public class AddChecklistManagement
    {
        public class AddChecklistManagementCommand : IRequest<Result>
        {
            public int? Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }

        }

        public class Handler : IRequestHandler<AddChecklistManagementCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(AddChecklistManagementCommand command, CancellationToken cancellationToken)
            {
                var addQuestion = await _unitOfWork.PmsCreate.AddChecklistManagement(command, cancellationToken);

                return Result.Success(PmsConsString.ChecklistAdded);
            }
        }

    }


}
