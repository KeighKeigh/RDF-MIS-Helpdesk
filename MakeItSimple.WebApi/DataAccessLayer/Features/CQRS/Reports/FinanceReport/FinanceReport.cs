using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.FinanceReports
{
    public class FinanceReport
    {
        public class FinanceReportQuery : UserParams, IRequest<PagedList<FinanceReportResult>>
        {
            public string Search { get; set; }
            public int? ServiceProvider { get; set; }
            public int? Channel { get; set; }
            public Guid? UserId { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

        public class FinanceReportResult
        {

            public int? TicketNumber { get; set; }
            public string AssignTo { get; set; }
            public string Requestor { get; set; }
            public string Description { get; set; }
            public string Open_Date { get; set; }
            public string Target_Date { get; set; }
            public string ForClosingDate { get; set; }
            public string ClosingDate { get; set; }
            public string Remarks { get; set; }
            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string DepartmentCode { get; set; }
            public string DepartmentName { get; set; }
            public string LocationCode { get; set; }
            public string LocationName { get; set; }
            public string BusinessUnitCode { get; set; }
            public string BusinessUnitName { get; set; }
            public string UnitCode { get; set; }
            public string UnitName { get; set; }
            public string SubUnitCode { get; set; }
            public string SubUnitName { get; set; }
            public int Month { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public Guid? IssueHandler { get; set; }
            public DateTime? RequestedAt { get; set; }
        }


        public class Handler : IRequestHandler<FinanceReportQuery, PagedList<FinanceReportResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<FinanceReportResult>> Handle(FinanceReportQuery request, CancellationToken cancellationToken)
            {
                var closingReport = _context.ClosingTickets
                    .AsNoTracking()
                    .Where(x => x.IsActive == true && x.IsClosing == true
                    && x.ClosingAt.Value.Date >= request.Date_From.Value.Date && x.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new FinanceReportResult
                    {
                        TicketNumber = x.TicketConcernId,
                        AssignTo = x.TicketConcern.User.Fullname,
                        Requestor = x.TicketConcern.RequestorByUser.Fullname,
                        Description = x.TicketConcern.RequestConcern.Concern,
                        Open_Date = x.TicketConcern.DateApprovedAt.Value.Date.ToString("MM/dd/yyyy"),
                        Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ForClosingDate = x.ForClosingAt.Value.Date.ToString("MM/dd/yyyy"),
                        ClosingDate = x.TicketConcern.Closed_At.Value.Date.ToString("MM/dd/yyyy"),
                        Remarks = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                        CompanyCode = x.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        CompanyName = x.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        DepartmentCode = x.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        DepartmentName = x.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        LocationCode = x.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        LocationName = x.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        BusinessUnitCode = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnitName = x.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        UnitCode = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        UnitName = x.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnitCode = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnitName = x.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,
                        Month = x.TicketConcern.Closed_At.Value.Month,
                        ServiceProviderId = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        IssueHandler = x.TicketConcern.AssignTo,
                        RequestedAt = x.TicketConcern.CreatedAt
                    });

                if (request.ServiceProvider is not null)
                {
                    closingReport = closingReport.Where(x => x.ServiceProviderId == request.ServiceProvider);

                    if (request.Channel is not null)
                    {
                        closingReport = closingReport.Where(x => x.ChannelId == request.Channel);

                        if (request.UserId is not null)
                        {
                            closingReport = closingReport.Where(x => x.IssueHandler == request.UserId);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    closingReport = closingReport
                        .Where(x => x.TicketNumber.ToString().Contains(request.Search)
                        || x.AssignTo.Contains(request.Search)
                        || x.Description.Contains(request.Search));
                }

                var results = closingReport.Select(x => new FinanceReportResult
                {
                    TicketNumber = x.TicketNumber,
                    AssignTo = x.AssignTo,
                    Requestor = x.Requestor,
                    Description = x.Description,
                    Open_Date = x.Open_Date,
                    Target_Date = x.Target_Date,
                    ForClosingDate = x.ForClosingDate,
                    ClosingDate = x.ClosingDate,
                    Remarks = x.Remarks,
                    CompanyCode = x.CompanyCode,
                    CompanyName = x.CompanyName,
                    DepartmentCode = x.DepartmentCode,
                    DepartmentName = x.DepartmentName,
                    LocationCode = x.LocationCode,
                    LocationName = x.LocationName,
                    BusinessUnitCode = x.BusinessUnitCode,
                    BusinessUnitName = x.BusinessUnitName,
                    UnitCode = x.UnitCode,
                    UnitName = x.UnitName,
                    SubUnitCode = x.SubUnitCode,
                    SubUnitName = x.SubUnitName,
                    Month = x.Month,
                    RequestedAt = x.RequestedAt
                }).OrderBy(x => x.TicketNumber);


                return await PagedList<FinanceReportResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
