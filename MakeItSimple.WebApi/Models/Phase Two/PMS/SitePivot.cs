namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class SitePivot :BaseIdEntity
    {
        public int? SiteId { get; set; }
        public virtual Sites Site { get; set; }
        public int? BusinessUnitId { get; set; }
        public string BusinessUnitName { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
