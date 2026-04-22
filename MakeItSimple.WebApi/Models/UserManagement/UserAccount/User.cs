using MakeItSimple.WebApi.Models.OneCharging;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MakeItSimple.WebApi.Models.Setup.ApproverSetup;
using MakeItSimple.WebApi.Models.Setup.BusinessUnitSetup;
using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.CompanySetup;
using MakeItSimple.WebApi.Models.Setup.DepartmentSetup;
using MakeItSimple.WebApi.Models.Setup.LocationSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ApproverUsersSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;
using MakeItSimple.WebApi.Models.Setup.ReceiverSetup;
using MakeItSimple.WebApi.Models.Setup.SubUnitSetup;

using MakeItSimple.WebApi.Models.Setup.UnitSetup;
using MakeItSimple.WebApi.Models.Ticketing;
using MakeItSimple.WebApi.Models.UserManagement.UserRoleAccount;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeItSimple.WebApi.Models
{
    public partial class User 
    {
        public Guid Id { get; set; }
        public bool IsActive { get ; set; } = true;
        public string EmpId {  get; set; } 
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ? IsPasswordChange { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ? UpdatedAt { get; set;}

        
        [ForeignKey("AddedByUser")]
        public Guid ? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }


        [ForeignKey("ModifiedByUser")]
        public Guid? ModifiedBy { get; set; }
        public virtual User ModifiedByUser { get; set; }

        public int UserRoleId { get; set; }
        public virtual UserRole UserRole { get; set; }

        public int? CompanyId { get; set; }

        public int BusinessUnitId { get; set; }

        public int ? DepartmentId { get; set; }

        public int? UnitId { get; set; }

        public int ? SubUnitId { get; set; }

        public int? LocationId { get; set; }



        public string ProfilePic { get; set; }
        public string FileName { get; set; }
        public decimal? FileSize { get; set; }
        public bool? IsStore { get; set; }

        [ForeignKey("OneChargingMIS")]
        public string OneChargingCode { get; set; }
        public virtual OneChargingMIS OneChargingMIS { get; set; }


        public string OneChargingName { get; set; }
        public ICollection<TicketConcern> TicketConcerns { get; set;}
        public ICollection<Channel> Channels { get; set; }
        public ICollection<Approver> Approvers { get; set; }
        public ICollection<ApproverTicketing> ApproversTickets { get; set; }
        public ICollection<RequestConcern> RequestConcerns { get; set;}
        public ICollection<ServiceProviders> SeviceProviders { get; set; }
        public ICollection<Receiver> Receivers { get; set; }
        public ICollection<ApproverUser> ApproverUsers { get; set; }
        public ICollection<ApproverUser> IssueHandlerUsers { get; set; }
        public ICollection<PmsPhaseTwo> PmsPhaseTwo { get; set; }





    }
}
