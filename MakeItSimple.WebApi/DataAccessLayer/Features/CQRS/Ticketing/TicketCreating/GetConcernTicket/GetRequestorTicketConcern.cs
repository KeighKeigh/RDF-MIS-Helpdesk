using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern.GetRequestorTicketConcernResult;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket
{
    public partial class GetRequestorTicketConcern
    {

        public class Handler : IRequestHandler<GetRequestorTicketConcernQuery, PagedList<GetRequestorTicketConcernResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetRequestorTicketConcernResult>> Handle(GetRequestorTicketConcernQuery request, CancellationToken cancellationToken)
            {

                var dateToday = DateTime.Now;

                IQueryable<TicketConcern> requestConcernsQuery = _context.TicketConcerns
                     .AsNoTrackingWithIdentityResolution()
                     .Include(x => x.RequestConcern)
                     .OrderBy(x => x.Id)
                     .AsSplitQuery();
                
                if (requestConcernsQuery.Any())
                {

                    var allUserList = await _context.UserRoles
                        .AsNoTrackingWithIdentityResolution()
                        .Select(x => new
                        {
                            x.Id,
                            x.UserRoleName,
                            x.Permissions

                        }).ToListAsync();

                    var requestorPermissionList = allUserList
                    .Where(x => x.Permissions
                    .Contains(TicketingConString.Requestor))
                    .Select(x => x.UserRoleName)
                    .ToList();

                    if (!string.IsNullOrEmpty(request.Search))
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.RequestConcern.User.Fullname
                            .Contains(request.Search)
                            || x.Id.ToString().Contains(request.Search)
                            || x.RequestConcern.Concern.ToLower().Contains(request.Search.ToLower()));
                    }

                    if (request.Status != null)
                    {
                        requestConcernsQuery = requestConcernsQuery
                            .Where(x => x.IsActive == request.Status);
                    }
                    // ETO
                    if (request.Is_Approve != null)
                    {

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => x.IsApprove == request.Is_Approve);
                    }
                    // ETO YON
                    if (request.Ascending is not null)
                    {
                        
                        requestConcernsQuery = request.Ascending.Value is true
                            ? requestConcernsQuery
                            .OrderBy(x => x.Id)
                            : requestConcernsQuery
                            .OrderByDescending(x => x.Id);
                    }

                    if (request.DepartmentId != null)
                    {

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => x.RequestConcern.DepartmentId == request.DepartmentId);
                    }

                    if (request.LocationId != null)
                    {

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => x.RequestConcern.LocationId == request.LocationId);
                    }

                    if (request.CategoryId != null)
                    {

                        requestConcernsQuery = requestConcernsQuery
                           .Where(x => x.RequestConcern.TicketCategories.Any(c => c.CategoryId == request.CategoryId));
                    }





                    if (request.Concern_Status is not null)
                    {

                        switch (request.Concern_Status)
                        {
                            case TicketingConString.Approval:
                                
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.RequestConcern.ConcernStatus == TicketingConString.ForApprovalTicket);
                                break;
                            case TicketingConString.OnGoing:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.RequestConcern.ConcernStatus == TicketingConString.CurrentlyFixing);
                                break;

                            case TicketingConString.NotConfirm:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.RequestConcern.Is_Confirm == null && x.ConcernStatus == TicketingConString.NotConfirm);
                                break;

                            case TicketingConString.Done:
                                requestConcernsQuery = requestConcernsQuery
                                    .Where(x => x.RequestConcern.ConcernStatus == TicketingConString.Done && x.RequestConcern.Is_Confirm == true);
                                break;
                            default:
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);

                        }
                    }

                    if (!string.IsNullOrEmpty(request.UserType))
                    {
                        if (request.UserType == TicketingConString.Requestor)
                        {
                            if (requestorPermissionList.Any(x => x.Contains(request.Role)))
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => x.RequestorBy == request.UserId);
                            }
                            else
                            {
                                return new PagedList<GetRequestorTicketConcernResult>(new List<GetRequestorTicketConcernResult>(), 0, request.PageNumber, request.PageSize);
                            }
                        }
                        if(request.UserType == TicketingConString.Receiver)
                        {
                            var userChannelId = await _context.ChannelUsers
                                 .Where(cu => cu.UserId == request.UserId)
                                 .Select(cu => cu.ChannelId)
                                 .ToListAsync();


                            if (userChannelId.Any())
                            {
                                var serviceProviderIds = await _context.ServiceProviderChannels
                                    .Where(spc => userChannelId.Contains(spc.ChannelId.Value))
                                    .Select(spc => spc.ServiceProviderId)
                                    .Distinct()
                                    .ToListAsync();

                                

                                requestConcernsQuery = requestConcernsQuery
                                    .Where(rc =>
                                        (rc.RequestConcern.ChannelId.HasValue && userChannelId.Contains(rc.RequestConcern.ChannelId.Value)) && rc.AssignTo == null
                                        || (!rc.RequestConcern.ChannelId.HasValue && rc.RequestConcern.ServiceProviderId != null && serviceProviderIds.Contains(rc.RequestConcern.ServiceProviderId.Value) && rc.AssignTo == null)
                                    );
                            }
                            else
                            {
                                requestConcernsQuery = requestConcernsQuery.Where(x => false);
                            }
                        }
                    }

                    if (request.DateFrom != null && request.DateTo != null)
                    {
                        requestConcernsQuery = requestConcernsQuery.Where(x => x.CreatedAt.Date >= request.DateFrom && x.CreatedAt.Date <= request.DateTo);
                    }



                }

                var results = requestConcernsQuery
                    .Select(g => new GetRequestorTicketConcernResult
                    {

                        RequestConcernId = g.RequestConcernId,
                        Concern = g.RequestConcern.Concern,
                        Resolution = g.RequestConcern.Resolution,
                        CompanyId = g.RequestConcern.CompanyId,
                        Company_Code = g.RequestConcern.OneChargingMIS.company_code,
                        Company_Name = g.RequestConcern.OneChargingMIS.company_name,
                        BusinessUnitId = g.RequestConcern.BusinessUnitId,
                        BusinessUnit_Code = g.RequestConcern.OneChargingMIS.business_unit_code,
                        BusinessUnit_Name = g.RequestConcern.OneChargingMIS.business_unit_name,
                        DepartmentId = g.RequestConcern.DepartmentId,
                        Department_Code = g.RequestConcern.OneChargingMIS.department_code,
                        Department_Name = g.RequestConcern.OneChargingMIS.department_name,
                        UnitId = g.RequestConcern.ReqUnitId,
                        Unit_Code = g.RequestConcern.OneChargingMIS.department_unit_code,
                        Unit_Name = g.RequestConcern.OneChargingMIS.department_unit_name,
                        SubUnitId = g.RequestConcern.ReqSubUnitId,
                        SubUnit_Code = g.RequestConcern.OneChargingMIS.sub_unit_code,
                        SubUnit_Name = g.RequestConcern.OneChargingMIS.sub_unit_name,
                        LocationId = g.RequestConcern.LocationId,
                        Location_Code = g.RequestConcern.OneChargingMIS.location_code,
                        Location_Name = g.RequestConcern.OneChargingMIS.location_name,
                        RequestorId = g.RequestConcern.UserId,
                        FullName = g.RequestConcern.User.Fullname,
                        ChannelId = g.RequestConcern.ChannelId,
                        Channel_Name = g.RequestConcern.Channel.ChannelName,
                        OneCharginCode = g.RequestConcern.OneChargingCode,
                        OneChargingName = g.RequestConcern.OneChargingName,
                        RequestedDate = g.CreatedAt,
                        TargetDate = g.RequestConcern.TargetDate,
                        AssignTo = g.RequestConcern.AssignTo,
                        AssignToName = _context.Users.Where(u => u.Id == g.RequestConcern.AssignTo).Select(u => u.Fullname).FirstOrDefault(),
                        ServiceProviderId = g.RequestConcern.ServiceProviderId,
                        ServiceProviderName = g.RequestConcern.ServiceProvider.ServiceProviderName,
                        GetRequestTicketCategories = g.RequestConcern.TicketCategories
                        .Select(t => new GetRequestorTicketConcernResult.GetRequestTicketCategory
                        {
                            TicketCategoryId = t.Id,
                            CategoryId = t.CategoryId,
                            Category_Description = t.Category.CategoryDescription,

                        }).ToList(),

                        GetRequestSubTicketCategories = g.RequestConcern.TicketSubCategories
                        .Select(t => new GetRequestorTicketConcernResult.GetRequestSubTicketCategory
                        {
                            TicketSubCategoryId = t.Id,
                            Category_Id = t.SubCategory.CategoryId,
                            SubCategoryId = t.SubCategoryId,
                            SubCategory_Description = t.SubCategory.SubCategoryDescription,
                        }).ToList(),

                        Concern_Status = g.RequestConcern.ConcernStatus,
                        Severity = g.RequestConcern.Severity,
                        Is_Done = g.RequestConcern.IsDone,
                        Remarks = g.RequestConcern.Remarks,
                        Notes = g.RequestConcern.Notes,
                        Contact_Number = g.RequestConcern.ContactNumber,
                        Request_Type = g.RequestConcern.RequestType,
                        BackJobId = g.RequestConcern.BackJobId,
                        Back_Job_Concern = g.RequestConcern.BackJob.Concern,
                        Added_By = g.RequestConcern.AddedByUser.Fullname,
                        Date_Needed = g.RequestConcern.DateNeeded,
                        Created_At = g.RequestConcern.CreatedAt,
                        Modified_By = g.RequestConcern.ModifiedByUser.Fullname,
                        updated_At = g.RequestConcern.UpdatedAt,
                        Is_Confirmed = g.RequestConcern.Is_Confirm,
                        Confirmed_At = g.RequestConcern.Confirm_At,
                        TicketRequestConcerns = g.RequestConcern.TicketConcerns
                            .Select(tc => new TicketRequestConcern
                            {
                                TicketConcernId = /*g.ConcernStatus == "For Approval" ? null :*/ tc.Id,
                                UserId = tc.UserId,
                                Issue_Handler = tc.User.Fullname,
                                Target_Date = tc.TargetDate,
                                Remarks = tc.Remarks,
                                Is_Assigned = tc.IsAssigned,
                                Added_By = tc.AddedByUser.Fullname,
                                Created_At = tc.CreatedAt,
                                Modified_By = tc.ModifiedByUser.Fullname,
                                Updated_At = tc.UpdatedAt,
                                Is_Active = tc.IsActive,
                                OnHold = tc.OnHold,
                                OnHold_At = tc.OnHoldAt,
                                OnHold_Reasons = tc.OnHoldAt != null ? tc.TicketOnHolds
                                .OrderByDescending(x => x.CreatedAt)
                                .First().Reason : null,
                                Resume_At = tc.Resume_At,
                                Closed_At = tc.Closed_At,
                                Closing_Notes = tc.IsClosedApprove == true ?
                                tc.ClosingTickets.Where(x => x.IsClosing == true)
                                .OrderByDescending(x => x.ClosingAt)
                                .First().Notes : null,
                                Is_Transfer = tc.IsTransfer,
                                Transfer_At = tc.TransferAt,
                                Transfer_By = tc.TransferByUser.Fullname,

                            }).ToList()

                    }) ;

                

                return await PagedList<GetRequestorTicketConcernResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}