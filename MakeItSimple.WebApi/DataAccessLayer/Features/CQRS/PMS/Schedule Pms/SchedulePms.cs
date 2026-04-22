using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms
{
    public class SchedulePms
    {

        public class SchedulePmsCommand : IRequest<Result>
        {
            public string Charging { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? ScheduleDate { get; set; }
            public int? PmsTypeId { get; set; }
            public int? SiteId { get; set; }
            public int? BusinessUnitId { get; set; }

        }


        public class Handler : IRequestHandler<SchedulePmsCommand, Result>
        {

            private readonly IUnitOfWork unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                this.unitOfWork = unitOfWork;
            }


            public async Task<Result> Handle(SchedulePmsCommand command, CancellationToken cancellationToken)
            {

                var addSchedulePms = await unitOfWork.PmsCreate.AddSchedulePMS(command, cancellationToken);

                return Result.Success();

            }
        }


    }
}
