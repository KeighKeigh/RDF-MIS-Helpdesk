using ClosedXML.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {

        public class Handler : IRequestHandler<ClosingTicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(ClosingTicketExportCommand request, CancellationToken cancellationToken)
            {

                var closing =  await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsActive == true && x.IsClosing == true)
                    .Where(t => t.ClosingAt.Value.Date >= request.Date_From.Value.Date && t.ClosingAt.Value.Date <= request.Date_To.Value.Date)
                    .Select(x => new ClosingTicketExportResult
                    {
                        Year = x.TicketConcern.TargetDate.Value.Year.ToString(),
                        Month = x.TicketConcern.TargetDate.Value.Month.ToString(),
                        RequestedYear = x.TicketConcern.CreatedAt.Year.ToString(),
                        Personnel = x.TicketConcern.User.Fullname,
                        Ticket_Number = x.TicketConcernId,
                        Description = x.TicketConcern.RequestConcern.Concern,
                        Target_Date = x.TicketConcern.TargetDate.Value.ToString("MM/dd/yyyy"),
                        Actual =  x.ClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        Varience = x.ClosingAt.Value.Date > x.TicketConcern.TargetDate.Value.Date  ? EF.Functions.DateDiffDay(x.TicketConcern.TargetDate.Value.Date, x.ClosingAt.Value.Date) : 0,
                        Efficeincy = x.ClosingAt.Value.Date <= x.TicketConcern.TargetDate.Value.Date ? "100 %" : "50 %",
                        Status = TicketingConString.Closed,
                        Remarks = x.ClosingAt.Value.Date  <= x.TicketConcern.TargetDate.Value.Date ? TicketingConString.OnTime : TicketingConString.Delay,
                        Category = string.Join(", ", x.TicketConcern.RequestConcern.TicketCategories
                          .Select(x => x.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", x.TicketConcern.RequestConcern.TicketSubCategories
                          .Select(x => x.SubCategory.SubCategoryDescription)),
                        Aging_Days = EF.Functions.DateDiffDay(x.TicketConcern.DateApprovedAt.Value.Date, x.ClosingAt.Value.Date),
                        Start_Date = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy"),
                        ForClosedDate = x.ForClosingAt.Value.ToString("MM/dd/yyyy hh:mm:tt") ?? "",
                        ServiceProvider = x.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = x.TicketConcern.RequestConcern.ChannelId,
                        ServiceProviderName = x.TicketConcern.RequestConcern.ServiceProvider.ServiceProviderName,
                        ChannelName = x.TicketConcern.RequestConcern.Channel.ChannelName,
                        UserId = x.TicketConcern.AssignTo,
                        CreatedAt = x.TicketConcern.CreatedAt.ToString("MM/dd/yyyy hh:mm:tt"),
                        ConfirmedAt = x.TicketConcern.RequestConcern.Confirm_At.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        OpenDate = x.TicketConcern.DateApprovedAt.Value.ToString("MM/dd/yyyy hh:mm:tt"),
                        //Technician1 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(0).Take(1).FirstOrDefault(),
                        //Technician2 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(1).Take(1).FirstOrDefault(),
                        //Technician3 = x.ticketTechnicians.Select(t => t.TechnicianByUser.Fullname).Skip(2).Take(1).FirstOrDefault(),

                        //Requestor = x.TicketConcern.RequestorByUser.Fullname,
                        CategoryConcern = x.CategoryConcernName,
                        //Department = x.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        Notes =x.Notes

                    }).ToListAsync();


                if (request.ServiceProvider is not null)
                {
                    closing = closing.Where(x => x.ServiceProvider == request.ServiceProvider).ToList();

                    if (request.Channel is not null)
                    {
                        closing = closing.Where(x => x.ChannelId == request.Channel).ToList();

                        if (request.UserId is not null)
                        {
                            closing = closing.Where(x => x.UserId == request.UserId).ToList();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.Remarks))
                {
                    switch (request.Remarks)
                    {
                        case TicketingConString.OnTime:
                            closing = closing
                                .Where(x => x.Actual != null && x.Target_Date_DateTime.Value.Date > x.Actual_Date_DateTime.Value.Date)
                                .ToList();
                            break;

                        case TicketingConString.Delay:
                            closing = closing
                                .Where(x => x.Actual != null && x.Target_Date_DateTime.Value.Date < x.Actual_Date_DateTime.Value.Date)
                                .ToList();
                            break;

                        default:
                            return Unit.Value;

                    }
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    closing = closing
                        .Where(x => x.Ticket_Number.ToString().Contains(request.Search)
                        || x.Personnel.Contains(request.Search))
                        .ToList();
                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"Closing Ticket Report");
                    var headers = new List<string>
                    {
                        "Year",
                        "Month",
                        "Start Date",
                        "End Date",
                        "Personnel",
                        "Ticket Number",
                        "Description",
                        "Category",
                        "Sub Category",
                        "Concern Category",
                        "Requested Date",
                        "Open Date",
                        "Target Date",
                        "For Closing Date",
                        "Approved Date",
                        "Confirmed Date",
                        "Variance",
                        "Efficiency",
                        "Remarks",
                        "Channel Name",
                        "Service Provider",
                        

                    };

                    var range = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                    range.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                    range.Style.Font.Bold = true;
                    range.Style.Font.FontColor = XLColor.Black;
                    range.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    for (var index = 1; index <= headers.Count; index++)
                    {
                        worksheet.Cell(1, index).Value = headers[index - 1];
                    }
                    for (var index = 1; index <= closing.Count; index++)
                    {
                        var row = worksheet.Row(index + 1);

                        row.Cell(1).Value = closing[index - 1].Year;
                        row.Cell(2).Value = closing[index - 1].Month;
                        row.Cell(3).Value = closing[index - 1].Start_Date;
                        row.Cell(4).Value = closing[index - 1].End_Date;
                        row.Cell(5).Value = closing[index - 1].Personnel;
                        row.Cell(6).Value = $"{closing[index - 1].RequestedYear} - {closing[index - 1].Ticket_Number}";
                        row.Cell(7).Value = closing[index - 1].Description;
                        row.Cell(8).Value = closing[index - 1].Category;
                        row.Cell(9).Value = closing[index - 1].SubCategory;
                        row.Cell(10).Value = closing[index - 1].CategoryConcern;
                        row.Cell(11).Value = closing[index - 1].CreatedAt;
                        row.Cell(12).Value = closing[index - 1].OpenDate;
                        row.Cell(13).Value = closing[index - 1].Target_Date;
                        row.Cell(14).Value = closing[index - 1].ForClosedDate;
                        row.Cell(15).Value = closing[index - 1].Actual;
                        row.Cell(16).Value = closing[index - 1].ConfirmedAt;
                        row.Cell(17).Value = closing[index - 1].Varience;
                        row.Cell(18).Value = closing[index - 1].Efficeincy;
                        row.Cell(19).Value = closing[index - 1].Remarks;
                        row.Cell(20).Value = closing[index - 1].ChannelName;
                        row.Cell(21).Value = closing[index - 1].ServiceProviderName;




                    }

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs($"ClosingTicketReports {request.Date_From:MM-dd-yyyy} - {request.Date_To:MM-dd-yyyy}.xlsx");

                }

                return Unit.Value;
            }
        }
    }
}
