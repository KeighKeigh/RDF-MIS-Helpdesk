using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Reports.CloseReport.TicketReports;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.TargetDateClosingReport
{
    public class TargetDateClosingReport
    {
        public class TargetDateClosingReportQuery : UserParams, IRequest<PagedList<TargetDateClosingReportReports>>
        {

            public string Search { get; set; }
            public int? ServiceProvider { get; set; }
            public int? Channel { get; set; }
            public Guid? UserId { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }

        }
        public record class TargetDateClosingReportReports
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string Personnel { get; set; }
            public int Ticket_Number { get; set; }
            public string Description { get; set; }
            public string Target_Date { get; set; }
            public string Actual { get; set; }
            public int Varience { get; set; }
            public string Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public int Aging_Day { get; set; }
            public string StartDate { get; set; }
            public string ClosedDate { get; set; }
            public string ForClosedDate { get; set; }
            public string Department { get; set; }
            public bool? IsStore { get; set; }
            public string Technician1 { get; set; }
            public string Technician2 { get; set; }
            public string Technician3 { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public Guid? AssignTo { get; set; }
            public string Requestor { get; set; }
            public string CategoryConcern { get; set; }
            public string Notes { get; set; }
            public DateTime? RequestedAt { get; set; }

        }


        public class Handler : IRequestHandler<TargetDateClosingReportQuery, PagedList<TargetDateClosingReportReports>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<TargetDateClosingReportReports>> Handle(TargetDateClosingReportQuery request, CancellationToken cancellationToken)
            {



                var closingTicket =  _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsActive == true && x.IsClosing == true)
                    .Where(t => t.TicketConcern.TargetDate.Value.Date >= request.Date_From.Value.Date && t.TicketConcern.TargetDate.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new TargetDateClosingReportReports
                    {
                        Year = x.TicketConcern.TargetDate.Value.Year,
                        Month = x.TicketConcern.TargetDate.Value.Month,
                        Personnel = x.TicketConcern.User.Fullname,
                        Ticket_Number = x.TicketConcernId,
                        Description = x.TicketConcern.RequestConcern.Concern,
                        Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Actual = x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        Varience = x.ClosingAt.Value.Date > x.TicketConcern.TargetDate.Value.Date ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) : 0,
                        Efficeincy = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? "100 %" : "50 %",
                        Status = TicketingConString.Closed,
                        Remarks = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = string.Join(", ", x.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", x.TicketConcern.RequestConcern.TicketSubCategories
                          .Select(x => x.SubCategory.SubCategoryDescription)),
                        Aging_Day = EF.Functions.DateDiffDay(x.TicketConcern.DateApprovedAt.Value.Date, x.ClosingAt.Value.Date),
                        StartDate = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy"),
                        ClosedDate = x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        ForClosedDate = x.ForClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt") ?? "",
                        ServiceProviderId = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = x.TicketConcern.AssignTo,
                        ChannelName = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        Technician1 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(0).Take(1).FirstOrDefault(),
                        Technician2 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(1).Take(1).FirstOrDefault(),
                        Technician3 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(2).Take(1).FirstOrDefault(),
                        IsStore = x.TicketConcern.RequestConcern.User.IsStore,
                        Requestor = x.TicketConcern.RequestorByUser.Fullname,
                        CategoryConcern = x.CategoryConcernName,
                        Department = x.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Notes = x.Notes,
                        RequestedAt = x.TicketConcern.CreatedAt,

                    });


                if (request.ServiceProvider is not null)
                {
                    closingTicket = closingTicket.Where(x => x.ServiceProviderId == request.ServiceProvider);

                    if (request.Channel is not null)
                    {
                        closingTicket = closingTicket.Where(x => x.ChannelId == request.Channel);

                        if (request.UserId is not null)
                        {
                            closingTicket = closingTicket.Where(x => x.AssignTo == request.UserId);
                        }
                    }
                }


                if (!string.IsNullOrEmpty(request.Search))
                {
                    closingTicket = closingTicket
                        .Where(x => x.Ticket_Number.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search)
                        || x.Description.Contains(request.Search)
                        || x.ChannelName.Contains(request.Search));
                }

                var results = closingTicket.Select(x => new TargetDateClosingReportReports
                {
                    Year = x.Year,
                    Month = x.Month,
                    Start_Date = $"{x.Month}-01-{x.Year}",
                    End_Date = $"{x.Month}-{DateTime.DaysInMonth(x.Year, x.Month)}-{x.Year}",
                    Personnel = x.Personnel,
                    Ticket_Number = x.Ticket_Number,
                    Description = x.Description,
                    Target_Date = x.Target_Date,
                    Actual = x.Actual,
                    Varience = x.Varience,
                    Efficeincy = x.Efficeincy,
                    Status = x.Status,
                    Remarks = x.Remarks,
                    Category = x.Category,
                    SubCategory = x.SubCategory,
                    Aging_Day = x.Aging_Day,
                    StartDate = x.StartDate,
                    ClosedDate = x.ClosedDate,
                    Technician1 = x.Technician1,
                    Technician2 = x.Technician2,
                    Technician3 = x.Technician3,
                    Department = x.Department,
                    AssignTo = x.AssignTo,
                    IsStore = x.IsStore,
                    CategoryConcern = x.CategoryConcern,
                    ForClosedDate = x.ForClosedDate,
                    Notes = x.Notes,
                    RequestedAt = x.RequestedAt,



                }).OrderBy(x => x.Ticket_Number);

                return await PagedList<TargetDateClosingReportReports>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
