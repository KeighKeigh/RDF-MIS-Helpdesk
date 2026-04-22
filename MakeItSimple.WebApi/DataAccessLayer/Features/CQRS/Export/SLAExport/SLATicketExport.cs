using Azure.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.AllTicketExport.AllTicketExport;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport.SLAReport;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Export.SLAExport
{
    public class SLATicketExport
    {

        public class SLATicketExportCommand : UserParams, IRequest<Unit>
        {
            public string Search { get; set; }
            public int? Channel { get; set; }
            public int? ServiceProvider { get; set; }
            public Guid? UserId { get; set; }
            public string Remarks { get; set; }
            public DateTime? Date_From { get; set; }
            public DateTime? Date_To { get; set; }
        }

        public class SLATicketExportResult
        {
            public string Year { get; set; }
            public string Month { get; set; }
            public string RequestedYear { get; set; }
            public int? TicketNo { get; set; }
            public string Assign { get; set; }
            public string Store { get; set; }
            public string Description { get; set; }
            public DateTime? OpenDate { get; set; }
            public string OpenTime { get; set; }
            public DateTime? ForClosingDate { get; set; }
            public string ForClosingTime { get; set; }
            public DateTime? TargetDate { get; set; }
            public DateTime? ClosedDate { get; set; }
            public string ClosedTime { get; set; }
            public string Solution { get; set; }
            public string RequestType { get; set; }
            public string Status { get; set; }
            public string Rating { get; set; }
            public string Remarks { get; set; }
            public string Category { get; set; }


            public string SubCategory { get; set; }
            public string Position { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public Guid? AssignTo { get; set; }

            public string CompanyCode { get; set; }
            public string CompanyName { get; set; }
            public string LocationCode { get; set; }
            public string LocationName { get; set; }
            public string DeparmentCode { get; set; }
            public string DepartmentName { get; set; }
            public string BusinessUnitCode { get; set; }
            public string BusinessUnitName { get; set; }
            public string UnitCode { get; set; }
            public string UnitName { get; set; }
            public string SubUnitCode { get; set; }
            public string SubUnitName { get; set; }

        }

        public class Handler : IRequestHandler<SLATicketExportCommand, Unit>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(SLATicketExportCommand command, CancellationToken cancellationToken)
            {
                var combineTicketReports = new List<SLATicketExportResult>();
                var dateToday = DateTime.Now;

                var openTicketQuery = await _context.TicketConcerns
                    .AsNoTrackingWithIdentityResolution()
                    .Include(t => t.RequestConcern)
                    .ThenInclude(x => x.OneChargingMIS)
                    .AsSplitQuery()
                .Where(t => t.IsApprove == true && t.IsTransfer != true && t.IsClosedApprove != true && t.OnHold != true && t.IsDone != true)
                .Where(t => t.DateApprovedAt.Value.Date >= command.Date_From.Value.Date && t.DateApprovedAt.Value.Date <= command.Date_To.Value.Date)
                    .Select(o => new SLATicketExportResult
                    {


                        Year = o.DateApprovedAt.Value.ToString("yyyy"),
                        Month = o.DateApprovedAt.Value.ToString("MMMM"),
                        TicketNo = o.Id,
                        Assign = o.User.Fullname,
                        Store = o.RequestorByUser.Fullname,
                        Description = o.RequestConcern.Concern,
                        OpenDate = o.DateApprovedAt.Value.Date,
                        OpenTime = o.DateApprovedAt.Value.ToString("HH:mm tt"),
                        TargetDate = o.TargetDate,
                        ClosedDate = o.Closed_At.Value.Date,
                        ClosedTime = o.Closed_At.Value.ToString("HH:mm tt"),
                        Solution = o.RequestConcern.Resolution,
                        RequestType = o.RequestConcern.RequestType,
                        Status = o.RequestConcern.ConcernStatus,
                        Rating = o.TargetDate.Value.Date < dateToday ? "Delay" : "On Time",
                        Category = string.Join(", ", o.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", o.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Position = "",
                        ServiceProviderId = o.RequestConcern.ServiceProviderId,
                        ChannelId = o.RequestConcern.ChannelId,
                        AssignTo = o.AssignTo,
                        CompanyCode = o.RequestConcern.OneChargingMIS.company_code,
                        CompanyName = o.RequestConcern.OneChargingMIS.company_name,
                        LocationCode = o.RequestConcern.OneChargingMIS.location_code,
                        LocationName = o.RequestConcern.OneChargingMIS.location_name,
                        DeparmentCode = o.RequestConcern.OneChargingMIS.department_code,
                        DepartmentName = o.RequestConcern.OneChargingMIS.department_name,
                        BusinessUnitCode = o.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnitName = o.RequestConcern.OneChargingMIS.business_unit_name,
                        UnitCode = o.RequestConcern.OneChargingMIS.department_unit_code,
                        UnitName = o.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnitCode = o.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnitName = o.RequestConcern.OneChargingMIS.sub_unit_name,
                        RequestedYear = o.CreatedAt.Year.ToString()





                    }).ToListAsync();


                var closingTicketQuery = await _context.ClosingTickets
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                .AsSplitQuery()
                .Where(x => x.IsClosing == true && x.IsActive == true)
                .Where(t => t.ClosingAt.Value.Date >= command.Date_From.Value.Date && t.ClosingAt.Value.Date <= command.Date_To.Value.Date)
                    .Select(ct => new SLATicketExportResult
                    {
                        Year = ct.TicketConcern.DateApprovedAt.Value.ToString("yyyy"),
                        Month = ct.TicketConcern.DateApprovedAt.Value.ToString("MMMM"),
                        TicketNo = ct.TicketConcernId,
                        Assign = ct.TicketConcern.User.Fullname,
                        Store = ct.TicketConcern.RequestorByUser.Fullname,
                        Description = ct.TicketConcern.RequestConcern.Concern,
                        OpenDate = ct.TicketConcern.DateApprovedAt.Value.Date,
                        OpenTime = ct.TicketConcern.DateApprovedAt.Value.ToString("HH:mm tt"),
                        TargetDate = ct.TicketConcern.TargetDate,
                        ForClosingDate = ct.ForClosingAt.Value.Date,
                        ForClosingTime = ct.ForClosingAt.Value.ToString("HH:mm tt"),
                        ClosedDate = ct.TicketConcern.Closed_At.Value.Date,
                        ClosedTime = ct.TicketConcern.Closed_At.Value.ToString("HH:mm tt"),
                        Solution = ct.TicketConcern.RequestConcern.Resolution,
                        RequestType = ct.TicketConcern.RequestConcern.RequestType,
                        Status = ct.TicketConcern.RequestConcern.ConcernStatus,
                        Rating = ct.TicketConcern.TargetDate.Value.Date >= ct.TicketConcern.Closed_At.Value.Date ? "On Time" : "Delay",
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        Position = "",
                        ServiceProviderId = ct.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = ct.TicketConcern.AssignTo,
                        Remarks = ct.TicketConcern.TargetDate.Value.Date >= ct.ForClosingAt.Value.Date ? "On Time" : "Delay",
                        RequestedYear = ct.TicketConcern.CreatedAt.Year.ToString(),
                        CompanyCode = ct.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        CompanyName = ct.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        LocationCode = ct.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        LocationName = ct.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        DeparmentCode = ct.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        DepartmentName = ct.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        BusinessUnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        UnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        UnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,

                    }).ToListAsync();

                var onHoldTicketQuery = await _context.TicketOnHolds
                    .AsNoTrackingWithIdentityResolution()
                    .Include(c => c.TicketConcern)
                    .ThenInclude(c => c.RequestConcern)
                    .AsSplitQuery()
                    .Where(x => x.IsHold == true && x.IsActive == true)
                    .Where(t => t.TicketConcern.CreatedAt.Date >= command.Date_From.Value.Date && t.TicketConcern.CreatedAt.Date <= command.Date_To.Value.Date)
                    .Select(ct => new SLATicketExportResult
                    {
                        Year = ct.TicketConcern.DateApprovedAt.Value.ToString("yyyy"),
                        Month = ct.TicketConcern.DateApprovedAt.Value.ToString("MMMM"),
                        TicketNo = ct.TicketConcernId,
                        Assign = ct.TicketConcern.User.Fullname,
                        Store = ct.TicketConcern.RequestorByUser.Fullname,
                        Description = ct.TicketConcern.RequestConcern.Concern,
                        OpenDate = ct.TicketConcern.DateApprovedAt.Value.Date,
                        OpenTime = ct.TicketConcern.DateApprovedAt.Value.ToString("HH:mm tt"),
                        TargetDate = ct.TicketConcern.TargetDate,
                        ClosedDate = ct.TicketConcern.Closed_At.Value.Date,
                        ClosedTime = ct.TicketConcern.Closed_At.Value.ToString("HH:mm tt"),
                        Solution = ct.TicketConcern.RequestConcern.Resolution,
                        RequestType = ct.TicketConcern.RequestConcern.RequestType,
                        Status = ct.TicketConcern.RequestConcern.ConcernStatus,
                        Category = string.Join(", ", ct.TicketConcern.RequestConcern.TicketCategories.Select(rc => rc.Category.CategoryDescription)),
                        SubCategory = string.Join(", ", ct.TicketConcern.RequestConcern.TicketSubCategories.Select(rc => rc.SubCategory.SubCategoryDescription)),
                        ServiceProviderId = ct.TicketConcern.RequestConcern.ServiceProviderId,
                        ChannelId = ct.TicketConcern.RequestConcern.ChannelId,
                        AssignTo = ct.TicketConcern.AssignTo,
                        RequestedYear = ct.TicketConcern.CreatedAt.Year.ToString(),
                        CompanyCode = ct.TicketConcern.RequestConcern.OneChargingMIS.company_code,
                        CompanyName = ct.TicketConcern.RequestConcern.OneChargingMIS.company_name,
                        LocationCode = ct.TicketConcern.RequestConcern.OneChargingMIS.location_code,
                        LocationName = ct.TicketConcern.RequestConcern.OneChargingMIS.location_name,
                        DeparmentCode = ct.TicketConcern.RequestConcern.OneChargingMIS.department_code,
                        DepartmentName = ct.TicketConcern.RequestConcern.OneChargingMIS.department_name,
                        BusinessUnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.business_unit_name,
                        UnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_code,
                        UnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnitCode = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnitName = ct.TicketConcern.RequestConcern.OneChargingMIS.sub_unit_name,

                    }).ToListAsync();


                if (command.ServiceProvider is not null)
                {
                    openTicketQuery = openTicketQuery
                           .Where(x => x.ServiceProviderId == command.ServiceProvider)
                    .ToList();



                    closingTicketQuery = closingTicketQuery
                       .Where(x => x.ServiceProviderId == command.ServiceProvider)
                    .ToList();

                    onHoldTicketQuery = onHoldTicketQuery
                       .Where(x => x.ServiceProviderId == command.ServiceProvider)
                    .ToList();

                    if (command.Channel is not null)
                    {
                        openTicketQuery = openTicketQuery
                           .Where(x => x.ChannelId == command.Channel)
                        .ToList();



                        closingTicketQuery = closingTicketQuery
                           .Where(x => x.ChannelId == command.Channel)
                           .ToList();

                        onHoldTicketQuery = onHoldTicketQuery
                           .Where(x => x.ChannelId == command.Channel)
                           .ToList();

                        if (command.UserId is not null)
                        {
                            openTicketQuery = openTicketQuery
                                .Where(x => x.AssignTo == command.UserId)
                            .ToList();

                            closingTicketQuery = closingTicketQuery
                                    .Where(x => x.AssignTo == command.UserId)
                                .ToList();

                            onHoldTicketQuery = onHoldTicketQuery
                                    .Where(x => x.AssignTo == command.UserId)
                                .ToList();
                        }
                    }
                }



                foreach (var list in openTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in closingTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                foreach (var list in onHoldTicketQuery)
                {
                    combineTicketReports.Add(list);
                }

                var results = combineTicketReports
                    .OrderBy(x => x.OpenDate)
                    .ThenBy(x => x.TicketNo)
                    .Select(r => new SLATicketExportResult
                    {
                        Year = r.Year,
                        Month = r.Month,
                        TicketNo = r.TicketNo,
                        Assign = r.Assign,
                        Store = r.Store,
                        Description = r.Description,
                        OpenDate = r.OpenDate,
                        OpenTime = r.OpenTime,
                        TargetDate = r.TargetDate,
                        ForClosingDate = r.ForClosingDate,
                        ForClosingTime = r.ForClosingTime,
                        ClosedDate = r.ClosedDate,
                        ClosedTime = r.ClosedTime,
                        Solution = r.Solution,
                        RequestType = r.RequestType,
                        Status = r.Status,
                        Rating = r.Rating,
                        Category = r.Category,
                        SubCategory = r.SubCategory,
                        Position = r.Position,
                        Remarks = r.Remarks,
                        CompanyCode = r.CompanyCode,
                        CompanyName = r.CompanyName,
                        LocationCode = r.LocationCode,
                        LocationName = r.LocationName,
                        DeparmentCode = r.DeparmentCode,
                        DepartmentName = r.DepartmentName,
                        BusinessUnitCode = r.BusinessUnitCode,
                        BusinessUnitName = r.BusinessUnitName,
                        UnitCode = r.UnitCode,
                        UnitName = r.UnitName,
                        SubUnitCode = r.UnitName,
                        SubUnitName = r.UnitName,
                        RequestedYear = r.RequestedYear,
                    }).ToList();


                //if (!string.IsNullOrEmpty(command.Remarks))
                //{
                //    switch (command.Remarks)
                //    {
                //        case TicketingConString.OnTime:
                //            results = results
                //                .Where(x => x.TargetDate > x.ClosedDate)
                //            .ToList();
                //            break;

                //        case TicketingConString.Delay:
                //            results = results
                //                .Where(x => x.TargetDate < x.ClosedDate)
                //                .ToList();
                //            break;

                //        default:
                //            return Unit.Value;

                //    }
                //}

                if (!string.IsNullOrEmpty(command.Search))
                {
                    var normalizedSearch = System.Text.RegularExpressions.Regex.Replace(command.Search.ToLower().Trim(), @"\s+", " ");

                    results = results
                    .Where(x => x.TicketNo.ToString().ToLower().Contains(command.Search)
                        || x.Assign.ToLower().Contains(command.Search)
                        || x.Store.ToLower().Contains(command.Search)
                        || x.Solution.ToLower().Contains(command.Search)
                        || x.RequestType.ToLower().Contains(command.Search)
                        || x.Status.ToLower().Contains(command.Search)
                        || x.Rating.ToLower().Contains(command.Search)
                        || System.Text.RegularExpressions.Regex.Replace(x.Description.ToLower(), @"\s+", " ").Contains(normalizedSearch)).ToList();


                }


                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add($"SLA Report");
                    var headers = new List<string>
                    {
                        "Year",
                        "Month",
                        "Ticket No",
                        "Assign",
                        "Store",
                        "Description",
                        "Open Date",
                        "Open Time",
                        "Target Date",
                        "For Closing Date",
                        "For Closing Time",
                        "Closed Date",
                        "Closed Time",
                        "Solution",
                        "Remarks",
                        "Request Type",
                        "Status",
                        "Rating",
                        "Category",
                        "Sub Category",
                        "Company",
                        "Location",
                        "Department",
                        "Business Unit",
                        "Unit",
                        "Sub Unit",
                        "Position"

                    };

                    if (command.Channel == 3)
                    {
                        var smurange = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                        smurange.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                        smurange.Style.Font.Bold = true;
                        smurange.Style.Font.FontColor = XLColor.Black;
                        smurange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        smurange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        for (var index = 1; index <= headers.Count; index++)
                        {
                            worksheet.Cell(1, index).Value = headers[index - 1];
                        }
                        for (var index = 1; index <= results.Count; index++)
                        {
                            var row = worksheet.Row(index + 1);

                            row.Cell(1).Value = results[index - 1].Year;
                            row.Cell(2).Value = results[index - 1].Month;
                            row.Cell(3).Value = $"{results[index - 1].RequestedYear} - {results[index - 1].TicketNo}";
                            row.Cell(4).Value = results[index - 1].Assign;
                            row.Cell(5).Value = results[index - 1].Store;
                            row.Cell(6).Value = results[index - 1].Description;
                            row.Cell(7).Value = results[index - 1].OpenDate;
                            row.Cell(8).Value = results[index - 1].OpenTime;
                            row.Cell(9).Value = results[index - 1].TargetDate;
                            row.Cell(10).Value = results[index - 1].ForClosingDate;
                            row.Cell(11).Value = results[index - 1].ForClosingTime;
                            row.Cell(12).Value = results[index - 1].ClosedDate;
                            row.Cell(13).Value = results[index - 1].ClosedTime;
                            row.Cell(14).Value = results[index - 1].Solution;
                            //row.Cell(15).Value = results[index - 1].Remarks;
                            row.Cell(16).Value = results[index - 1].RequestType;
                            row.Cell(17).Value = results[index - 1].Status;
                            row.Cell(18).Value = results[index - 1].Rating;
                            row.Cell(19).Value = results[index - 1].Category;
                            row.Cell(20).Value = results[index - 1].SubCategory;
                            //row.Cell(21).Value = $"{results[index - 1].CompanyCode} - {results[index - 1].CompanyName}";
                            //row.Cell(22).Value = $"{results[index - 1].LocationCode} - {results[index - 1].LocationName}";
                            //row.Cell(23).Value = $"{results[index - 1].DeparmentCode} - {results[index - 1].DepartmentName}"; 
                            //row.Cell(24).Value = $"{results[index - 1].BusinessUnitCode} - {results[index - 1].BusinessUnitName}";
                            //row.Cell(25).Value = $"{results[index - 1].UnitCode} - {results[index - 1].UnitName}";
                            //row.Cell(26).Value = $"{results[index - 1].SubUnitCode} - {results[index - 1].SubUnitName}";
                            row.Cell(27).Value = results[index - 1].Position;

                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs($"SLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");
                    }

                    if (command.Channel == 8)
                    {
                        var smurange = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                        smurange.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                        smurange.Style.Font.Bold = true;
                        smurange.Style.Font.FontColor = XLColor.Black;
                        smurange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        smurange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        for (var index = 1; index <= headers.Count; index++)
                        {
                            worksheet.Cell(1, index).Value = headers[index - 1];
                        }
                        for (var index = 1; index <= results.Count; index++)
                        {
                            var row = worksheet.Row(index + 1);

                            row.Cell(1).Value = results[index - 1].Year;
                            row.Cell(2).Value = results[index - 1].Month;
                            row.Cell(3).Value = $"{results[index - 1].RequestedYear} - {results[index - 1].TicketNo}";
                            row.Cell(4).Value = results[index - 1].Assign;
                            //row.Cell(5).Value = results[index - 1].Store;
                            row.Cell(6).Value = results[index - 1].Description;
                            row.Cell(7).Value = results[index - 1].OpenDate;
                            row.Cell(8).Value = results[index - 1].OpenTime;
                            row.Cell(9).Value = results[index - 1].TargetDate;
                            row.Cell(10).Value = results[index - 1].ForClosingDate;
                            row.Cell(11).Value = results[index - 1].ForClosingTime;
                            //row.Cell(12).Value = results[index - 1].ClosedDate;
                            //row.Cell(13).Value = results[index - 1].ClosedTime;
                            row.Cell(14).Value = results[index - 1].Solution;
                            row.Cell(15).Value = results[index - 1].Remarks;
                            //row.Cell(16).Value = results[index - 1].RequestType;
                            //row.Cell(17).Value = results[index - 1].Status;
                            //row.Cell(18).Value = results[index - 1].Rating;
                            row.Cell(19).Value = results[index - 1].Category;
                            //row.Cell(20).Value = results[index - 1].SubCategory;
                            //row.Cell(21).Value = $"{results[index - 1].CompanyCode} - {results[index - 1].CompanyName}";
                            //row.Cell(22).Value = $"{results[index - 1].LocationCode} - {results[index - 1].LocationName}";
                            //row.Cell(23).Value = $"{results[index - 1].DeparmentCode} - {results[index - 1].DepartmentName}";
                            //row.Cell(24).Value = $"{results[index - 1].BusinessUnitCode} - {results[index - 1].BusinessUnitName}";
                            //row.Cell(25).Value = $"{results[index - 1].UnitCode} - {results[index - 1].UnitName}";
                            //row.Cell(26).Value = $"{results[index - 1].SubUnitCode} - {results[index - 1].SubUnitName}";
                            //row.Cell(27).Value = results[index - 1].Position;

                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs($"SLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");
                    }


                    if (command.Channel == 6)
                    {
                        var smurange = worksheet.Range(worksheet.Cell(1, 1), worksheet.Cell(1, headers.Count));

                        smurange.Style.Fill.BackgroundColor = XLColor.LavenderPurple;
                        smurange.Style.Font.Bold = true;
                        smurange.Style.Font.FontColor = XLColor.Black;
                        smurange.Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        smurange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        for (var index = 1; index <= headers.Count; index++)
                        {
                            worksheet.Cell(1, index).Value = headers[index - 1];
                        }
                        for (var index = 1; index <= results.Count; index++)
                        {
                            var row = worksheet.Row(index + 1);

                            row.Cell(1).Value = results[index - 1].Year;
                            row.Cell(2).Value = results[index - 1].Month;
                            row.Cell(3).Value = $"{results[index - 1].RequestedYear} - {results[index - 1].TicketNo}";
                            row.Cell(4).Value = results[index - 1].Assign;
                            //row.Cell(5).Value = results[index - 1].Store;
                            row.Cell(6).Value = results[index - 1].Description;
                            row.Cell(7).Value = results[index - 1].OpenDate;
                            row.Cell(8).Value = results[index - 1].OpenTime;
                            row.Cell(9).Value = results[index - 1].TargetDate;
                            row.Cell(10).Value = results[index - 1].ForClosingDate;
                            row.Cell(11).Value = results[index - 1].ForClosingTime;
                            row.Cell(12).Value = results[index - 1].ClosedDate;
                            row.Cell(13).Value = results[index - 1].ClosedTime;
                            row.Cell(14).Value = results[index - 1].Solution;
                            //row.Cell(15).Value = results[index - 1].Remarks;
                            //row.Cell(16).Value = results[index - 1].RequestType;
                            //row.Cell(17).Value = results[index - 1].Status;
                            row.Cell(18).Value = results[index - 1].Rating;
                            row.Cell(19).Value = results[index - 1].Category;
                            row.Cell(20).Value = results[index - 1].SubCategory;
                            row.Cell(21).Value = $"{results[index - 1].CompanyCode} - {results[index - 1].CompanyName}";
                            row.Cell(22).Value = $"{results[index - 1].LocationCode} - {results[index - 1].LocationName}";
                            row.Cell(23).Value = $"{results[index - 1].DeparmentCode} - {results[index - 1].DepartmentName}";
                            row.Cell(24).Value = $"{results[index - 1].BusinessUnitCode} - {results[index - 1].BusinessUnitName}";
                            row.Cell(25).Value = $"{results[index - 1].UnitCode} - {results[index - 1].UnitName}";
                            row.Cell(26).Value = $"{results[index - 1].SubUnitCode} - {results[index - 1].SubUnitName}";
                            //row.Cell(27).Value = results[index - 1].Position;

                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs($"SLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");
                    }

                    else
                    {
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
                        for (var index = 1; index <= results.Count; index++)
                        {
                            var row = worksheet.Row(index + 1);

                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs($"SLATicketReports {command.Date_From:MM-dd-yyyy} - {command.Date_To:MM-dd-yyyy}.xlsx");

                    }

                }

                return Unit.Value;

            }
        }
    }
}
