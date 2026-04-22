using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms
{
    public class GetReminderPms
    {
        public class GetReminderPmsResult
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public DateTime? Date { get; set; }
            public string Area { get; set; }
            public string Status { get; set; }
            public string Completion { get; set; }
            public int? PmsTypeId { get; set; }
            public int? SiteId { get; set; }
            public int? BusinessUnitId { get; set; }
            public string AddedBy { get; set; }

        }

        public class GetReminderPmsQuery : UserParams, IRequest<PagedList<GetReminderPmsResult>>
        {
            public bool? IsCanceled { get; set; }
            public string Search { get; set; }
            public int? SiteId { get; set; }
            public int? PmsTypeId { get; set; }
            public string Status { get; set; }
            public string TimePeriod { get; set; }
        }

        public class Handler : IRequestHandler<GetReminderPmsQuery, PagedList<GetReminderPmsResult>>
        {
            private readonly MisDbContext _context;
            private readonly IDbConnection _dbConnection;

            public Handler(MisDbContext context, IDbConnection dbConnection)
            {
                _context = context;
                _dbConnection = dbConnection;
            }

            public async Task<PagedList<GetReminderPmsResult>> Handle(GetReminderPmsQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsPhaseTwo> pms = _context.Pmss
                    .AsNoTracking()
                    .Include(x => x.OneChargingMIS)
                    .Where(x => x.SiteId == request.SiteId && x.PmsTypeId == request.PmsTypeId);

                var dateToday = DateTime.Today;
                if (!string.IsNullOrEmpty(request.Search))
                {
                    pms = pms.Where(x => x.OneChargingMIS.name.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    if(request.Status == PmsConsString.Pending)
                    {
                        pms = pms.Where(x => x.IsCompleted == false);
                    }
                    else if (request.Status == PmsConsString.Done)
                    {
                        pms = pms.Where(x => x.IsCompleted == true);
                    }
                    else if (request.Status == PmsConsString.Done)
                    {
                        pms = pms.Where(x => x.ScheduleDate < dateToday);
                    }

                }

                if (request.TimePeriod != null)
                {
                    if (request.TimePeriod == PmsConsString.Today)
                    {
                        pms = pms.Where(x => x.ScheduleDate == dateToday);
                    }
                    else if (request.TimePeriod == PmsConsString.ThisWeek)
                    {
                        pms = pms.Where(x => x.ScheduleDate <= dateToday.AddDays(7));
                    }


                }

                if (request.IsCanceled != null)
                {
                    pms = pms.Where(x => x.IsCanceled == request.IsCanceled);

                }

                var pmsCount = pms.Where(x => x.IsCompleted == true).Count();

                var result = pms.Select(x => new GetReminderPmsResult
                {
                    Id = x.Id,
                    Name = x.OneChargingMIS.name,
                    Date = x.ScheduleDate,
                    Area = x.OneChargingMIS.department_unit_name + " - " + x.OneChargingMIS.sub_unit_name,
                    Status = x.IsCompleted == true ? PmsConsString.Done : PmsConsString.Pending,
                    Completion = $"{pmsCount} out of 9",
                    PmsTypeId = x.Id,
                    SiteId = x.SiteId,
                    BusinessUnitId = x.OneChargingMIS.business_unit_id,
                    AddedBy = x.AddedByUser.Fullname,
                });


                return await PagedList<GetReminderPmsResult>.CreateAsync(result, request.PageNumber, request.PageSize);
            }
        }
    }
}
