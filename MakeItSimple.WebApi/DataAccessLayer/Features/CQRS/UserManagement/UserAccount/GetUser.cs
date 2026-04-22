using MakeItSimple.WebApi.DataAccessLayer.Errors;
using MakeItSimple.WebApi.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Common.ConstantString;
using System.Data;
using Dapper;
using CloudinaryDotNet;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.UserManagement.UserAccount
{
    public class GetUser
    {

        public class GetUserResult
        {
            public Guid Id { get; set; }
            public string EmpId { get; set; }
            public string Fullname { get; set; }
            public string Username { get; set; }
            public string Added_By { get; set; }
            public DateTime Created_At { get; set; }
            public bool Is_Active { get; set; }
            public string Modified_By { get; set; }
            public DateTime? Update_At { get; set; }

            public string Profile_Pic { get; set; }
            public string FileName { get; set; }
            public decimal? FileSize { get; set; }

            public int? UserRoleId { get; set; }
            public string User_Role_Name { get; set; }

            public int? DepartmentId { get; set; }
            public string Department_Code { get; set; }
            public string Department_Name { get; set; }

            public int? CompanyId { get; set; }
            public string Company_Code { get; set; }
            public string Company_Name { get; set; }

            public int? LocationId { get; set; }
            public string Location_Code { get; set; }
            public string Location_Name { get; set; }

            public int? BusinessUnitId { get; set; }
            public string BusinessUnit_Code { get; set; }
            public string BusinessUnit_Name { get; set; }

            public int? UnitId { get; set; }
            public string Unit_Code { get; set; }
            public string Unit_Name { get; set; }

            public int? SubUnitId { get; set; }
            public string SubUnit_Code { get; set; }
            public string SubUnit_Name { get; set; }
            
            public string OneChargingCode { get; set; }
            public string OneChargingName { get; set; }
            public ICollection<string> Permission { get; set; }

            //public string PermissionJson { get; set; }
            //public List<string>  Permission 
            //{
            //    get
            //    {
            //        if (!string.IsNullOrWhiteSpace(PermissionJson))
            //        {
            //            return JsonConvert.DeserializeObject<List<string>>(PermissionJson);
            //        }
            //        return new List<string>();
            //    }
            //}

            public bool Is_Use { get; set; }

            public bool? Is_Store { get; set; }


        }

        public class GetUsersQuery : UserParams, IRequest<PagedList<GetUserResult>>
        {
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetUsersQuery, PagedList<GetUserResult>>
        {

            private readonly MisDbContext _context;
            private readonly IDbConnection _dbConnection;

            public Handler(MisDbContext context, IDbConnection dbConnection)
            {
                _context = context;
                _dbConnection = dbConnection;
            }

            public async Task<PagedList<GetUserResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {

                if (request == null)
                {

                }

                IQueryable<User> userQuery = _context.Users
                    .Include(x => x.AddedByUser)
                    .Include(x => x.ModifiedByUser)
                    .Include(x => x.UserRole);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    userQuery = userQuery.Where(x => x.Fullname.Contains(request.Search)
                    || x.UserRole.UserRoleName.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    userQuery = userQuery.Where(x => x.IsActive == request.Status);

                }

                var userPermissions = new List<string>();
                var users = userQuery.Select(x => new GetUserResult
                {

                    Id = x.Id,
                    EmpId = x.EmpId,
                    Fullname = x.Fullname,
                    Username = x.Username,
                    Added_By = x.AddedByUser.Fullname,
                    Created_At = x.CreatedAt,
                    Is_Active = x.IsActive,
                    Modified_By = x.ModifiedByUser.Fullname,
                    Profile_Pic = x.ProfilePic,
                    FileName = x.FileName,
                    FileSize = x.FileSize,
                    Update_At = x.UpdatedAt,
                    UserRoleId = x.UserRoleId,
                    User_Role_Name = x.UserRole.UserRoleName,
                    DepartmentId = x.DepartmentId,
                    Department_Code = x.OneChargingMIS.department_code,
                    Department_Name = x.OneChargingMIS.department_name,
                    SubUnitId = x.SubUnitId,
                    SubUnit_Code = x.OneChargingMIS.sub_unit_code,
                    SubUnit_Name = x.OneChargingMIS.sub_unit_name,
                    CompanyId = x.CompanyId,
                    Company_Code = x.OneChargingMIS.company_code,
                    Company_Name = x.OneChargingMIS.company_name,
                    LocationId = x.LocationId,
                    Location_Code = x.OneChargingMIS.location_code,
                    Location_Name = x.OneChargingMIS.location_name,
                    BusinessUnitId = x.BusinessUnitId,
                    BusinessUnit_Code = x.OneChargingMIS.business_unit_code,
                    BusinessUnit_Name = x.OneChargingMIS.business_unit_name,
                    UnitId = x.UnitId,
                    Unit_Code = x.OneChargingMIS.department_unit_code,
                    Unit_Name = x.OneChargingMIS.department_unit_name,
                    OneChargingCode = x.OneChargingCode,
                    OneChargingName = x.OneChargingName,
                    Permission = x.UserRole.Permissions != null ? x.UserRole.Permissions : userPermissions,
                    Is_Use = x.Approvers.Any() || x.Receivers.Any() ||
                    x.ApproversTickets.Any(x => x.IsApprove == null) ||
                    x.UserRole.UserRoleName.Contains(TicketingConString.IssueHandler)
                    && x.TicketConcerns.Any(x => x.IsApprove == true && x.IsClosedApprove == null) ?
                      true : false,
                    Is_Store = x.IsStore,

                });

                return await PagedList<GetUserResult>.CreateAsync(users, request.PageNumber, request.PageSize);


            }


        }



    }
}
