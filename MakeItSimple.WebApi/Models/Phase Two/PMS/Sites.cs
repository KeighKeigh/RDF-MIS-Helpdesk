namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class Sites : BaseIdEntity
    {
        public string Site { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
