using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_two;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites
{
    public class AddSite
    {
        public class SitesNames
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }
        public class AddSiteCommand : IRequest<Result>
        {
            public List<SitesNames> SiteNames { get; set; } = new();

        }

        public class Handler : IRequestHandler<AddSiteCommand, Result>
        {
            private readonly MisDbContext _context;
            private readonly IUnitOfWork _unitOfWork;
            public Handler(MisDbContext context, IUnitOfWork unitOfWork)
            {
                _context = context;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(AddSiteCommand command, CancellationToken cancellationToken)
            {

                var siteExist = await _unitOfWork.PmsValidate.ValidateSites(command.SiteNames);

                if (!siteExist)
                {
                    return Result.Failure(PmsError.SiteAlreadyExist());
                }

                var addSite = await _unitOfWork.PmsCreate.AddNewSite(command.SiteNames, cancellationToken);

                if(command.SiteNames.Count > 1)
                {
                    return Result.Success(PmsConsString.SitesAdded);
                }
                return Result.Success(PmsConsString.SiteAdded);
            }
        }
    }
}
