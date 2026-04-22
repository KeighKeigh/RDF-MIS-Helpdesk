using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequestConcern
{
    public partial class AddConcern
    {
        public class AddConcernCommand : IRequest<Result>
        {

            public int? TicketConcernId { get; set; }
            public Guid? Added_By { get; set; }
            public Guid? Modified_By { get; set; }
            public int? RequestConcernId { get; set; }
            public int? CompanyId { get; set; }
            public int? BusinessUnitId { get; set; }
            public int? DepartmentId { get; set; }
            public int? UnitId { get; set; }
            public int? SubUnitId { get; set; }
            public string Location_Code { get; set; }
            public int? ChannelId { get; set; } = 2;
            public DateTime? TargetDate { get; set; }
            public Guid? AssignTo { get; set; }

            public Guid? RequestorBy { get; set; }
            public string OneChargingCode { get; set; }
            public string OneChargingName { get; set; }


            public int? CategoryId { get; set; } = 249;
            //public List<AddTicketCategory> AddTicketCategories { get; set; }
            //public class AddTicketCategory
            //{
            //    public int? TicketCategoryId { get; set; }
                

            //}
            //public List<AddTicketSubCategory> AddTicketSubCategories { get; set; }
            //public class AddTicketSubCategory
            //{
            //    public int? TicketSubCategoryId { get; set; }
            //    public int? SubCategoryId { get; set; } 
            //}

            public DateTime? DateNeeded { get; set; } = DateTime.Today;
            public Guid? UserId { get; set; } = Guid.Parse("18BA659B-FED1-4A92-2895-08DE73F3C68B");

            public string Concern { get; set; }
            //public List<Concerns> Concern { get; set; }
            //public class Concerns
            //{
                
            //}
            public string Remarks { get; set; }
            public string Notes { get; set; }
            public string Contact_Number { get; set; }
            public string Request_Type { get; set; } = "New Request";
            public string Severity { get; set; } = "Urgent";

            public int? BackJobId { get; set; }

            //public List<AttachmentsFile> AttachmentsFiles { get; set; }

            //public class AttachmentsFile
            //{
            //    public int? TicketAttachmentId { get; set; }
            //    public IFormFile Attachment { get; set; }
            //}
            public int? ServiceProviderId { get; set; } = 1;


        }
    }
}
