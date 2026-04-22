using Azure.Core;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport.SLAReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Reports.SADSLAReport
{
    public class SADSLAReport
    {

        public class SADSLAReportQuery : UserParams, IRequest<PagedList<SADSLAReportResult>>
        {
            public string Search { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }


        public class SADSLAReportResult
        {
            public int? TicketNo { get; set; }
            public string Assign { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public string OpenDate { get; set; }
            public string TargetDate { get; set; }
            public string ActualDate { get; set; }
            public string Remarks { get; set; }
            public string Solution { get; set; }
            public DateTime? RequestedAt { get; set; }
        }


        public class Handler : IRequestHandler<SADSLAReportQuery, PagedList<SADSLAReportResult>>
        {
            private readonly MisDbContext _context;
            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<SADSLAReportResult>> Handle(SADSLAReportQuery request, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<SADSLAReportResult>();
                var dateToday = DateTime.Now;

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .AsSplitQuery()
                .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                    .Where(t => t.DateApprovedAt.Value.Date >= request.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= request.Date_To.Value.Date && t.RequestConcern.ChannelId == 8)
                    .Select(o => new SADSLAReportResult
                    {
                        TicketNo = o.Id,
                        Assign = o.User.Fullname,
                        Category = string.Join(", ", o.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        Description = o.RequestConcern.Concern,
                        OpenDate = o.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        TargetDate = o.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ActualDate = o.Closed_At.ToString(),
                        Remarks = o.TargetDate.Value.Date >= dateToday.Date ? "On Time" : "Delay",
                        Solution = o.RequestConcern.Resolution,
                        RequestedAt = o.CreatedAt,



                    }).ToListAsync();


                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                .AsSplitQuery()
                .Where(x => x.IsClosing == true && x.IsActive == true)
                .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date && t.TicketConcern.RequestConcern.ChannelId == 8)
                    .Select(ct => new SADSLAReportResult
                    {
                        TicketNo = ct.TicketConcernId,
                        Assign = ct.TicketConcern.User.Fullname,
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        Description = ct.TicketConcern.RequestConcern.Concern,
                        OpenDate = ct.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        TargetDate = ct.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        ActualDate = ct.ForClosingAt.Value.ToString("MM/dd/yyyy hh:tt:mm"),
                        Remarks = ct.TicketConcern.TargetDate.Value.Date >= ct.ForClosingAt.Value.Date ? "On Time" : "Delay",
                        Solution = ct.TicketConcern.RequestConcern.Resolution,
                        RequestedAt = ct.TicketConcern.CreatedAt,

                    }).ToListAsync();



                if (!string.IsNullOrEmpty(request.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(request.Search.ToLower().Trim(), @"\s+", " ");

                    openTicketQuery = openTicketQuery
                    .Where(x => x.TicketNo.ToString().ToLower().Contains(request.Search)
                        || x.Assign.ToLower().Contains(request.Search)
                        || x.Remarks.ToLower().Contains(request.Search)
                        || x.Solution.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();



                    closingTicketQuery = closingTicketQuery
                   .Where(x => x.TicketNo.ToString().ToLower().Contains(request.Search)
                        || x.Assign.ToLower().Contains(request.Search)
                        || x.Remarks.ToLower().Contains(request.Search)
                        || x.Solution.ToLower().Contains(request.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();

                }

                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                var results = combineTicketReports
                    .OrderBy(x => x.OpenDate)
                    .ThenBy(x => x.TicketNo)
                    .Select(r => new SADSLAReportResult
                    {

                        TicketNo = r.TicketNo,
                        Assign = r.Assign,
                        Category = r.Category,
                        Description = r.Description,
                        OpenDate = r.OpenDate,
                        TargetDate = r.TargetDate,
                        ActualDate = r.ActualDate,
                        Remarks = r.Remarks,
                        Solution = r.Solution,
                        RequestedAt = r.RequestedAt
                        
                    }).AsQueryable();
                return PagedList<SADSLAReportResult>.Create(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
