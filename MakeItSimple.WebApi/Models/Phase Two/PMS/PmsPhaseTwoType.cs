namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsPhaseTwoType : BaseIdEntity
    {
        public string PmsType { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PmsPhaseTwo> Pms { get; set; }
        public ICollection<PmsSectionQuestion> SectionQuestions { get; set; }

        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
    }
}
