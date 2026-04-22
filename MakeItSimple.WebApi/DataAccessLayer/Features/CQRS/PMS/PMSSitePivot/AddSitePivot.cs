using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot
{
    public class AddSitePivot
    {

        public class AddSitePivotCommand : IRequest<Result>
        {
            public int? SiteId { get; set; }
            public int? BusinessUnitId { get; set; }
            public string BusinessUnitName { get; set; }

        }

        public class Handler : IRequestHandler<AddSitePivotCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;

            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }


            public async Task<Result> Handle(AddSitePivotCommand command, CancellationToken cancellationToken)
            {
                var siteDoesNotExist = await _unitOfWork.PmsValidate.ExistingSiteById(command.SiteId);

                if(!siteDoesNotExist)
                {
                    return Result.Failure(PmsError.SiteDoesNotExist());
                }

                var businessUnitAlreadyExist = await _unitOfWork.PmsValidate.ExistingBusinessUnitById(command.BusinessUnitId);

                if (businessUnitAlreadyExist)
                {
                    return Result.Failure(PmsError.BusinessAlreadyExist());
                }

                var pivotExist = await _unitOfWork.PmsValidate.ExistingPivot(command.SiteId ,command.BusinessUnitId);

                if (businessUnitAlreadyExist)
                {
                    return Result.Failure(PmsError.DataAlreadyExist());
                }

                var upsertSite = await _unitOfWork.PmsCreate.AddPivotSite(command, cancellationToken);

                return Result.Success(PmsConsString.BusinessUnitTagged);
            }
        }

    }
}
