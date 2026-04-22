namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob
{
    public partial class TicketBackJob
    {
        public record TicketBackJobResult
        {
            public int TicketConcernId { get; set; }
            public int Days_Closed { get; set; }
            public string Concern { get; set; }
            public int? ServiceProviderId { get; set; }
            public int? ChannelId { get; set; }
            public List<GetBackjobCategory> GetBackjobCategories { get; set; }

            public class GetBackjobCategory
            {
                public int? TicketCategoryId { get; set; }
                public int? CategoryId { get; set; }
                public string Category_Description { get; set; }

            }

            public List<GetBackjobSubCategory> GetBackjobSubCategories { get; set; }

            public class GetBackjobSubCategory
            {
                public int? TicketSubCategoryId { get; set; }
                public int? SubCategoryId { get; set; }
                public string SubCategory_Description { get; set; }
            }

        }
    }
}
