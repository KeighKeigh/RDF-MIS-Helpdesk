using DocumentFormat.OpenXml.ExtendedProperties;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Export.ClosingExport
{
    public partial class ClosingTicketExport
    {
        public record class ClosingTicketExportResult
        {
            public Guid? UserId { get; set; }
            public int? Unit { get; set; }
            public string RequestedYear { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Start_Date { get; set; }
            public string End_Date { get; set; }
            public string Personnel { get; set; }
            public int? Ticket_Number { get; set; }
            public string Description { get; set; }
            public DateTime? Target_Date_DateTime { get; set; }
            public DateTime? Actual_Date_DateTime { get; set; }
            public string Target_Date { get; set; }
            public string Actual { get; set; }
            public int Varience { get; set; }
            public string Efficeincy { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public int? ChannelId { get; set; }
            public string ChannelName { get; set; }
            public int Aging_Days { get; set; }
            public int? ServiceProvider { get; set; }
            public string ServiceProviderName { get; set; }
            public string OpenDate { get; set; }
            public string ForClosedDate { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string Notes { get; set; }
            public string CreatedAt { get; set; }
            public string ConfirmedAt { get; set; }
            public string CategoryConcern { get; set; }

        }
    }
}
