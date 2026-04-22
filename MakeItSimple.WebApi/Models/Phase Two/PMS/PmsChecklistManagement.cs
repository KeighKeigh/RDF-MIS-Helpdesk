namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsChecklistManagement : BaseIdEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; } = true;

    }
}
