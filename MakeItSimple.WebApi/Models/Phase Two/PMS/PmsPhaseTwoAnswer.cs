namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsPhaseTwoAnswer : BaseIdEntity
    {
        public int? PmsId { get; set; }
        public virtual PmsPhaseTwo Pms { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }
        //public int? QuestionId { get; set; }
        public int? SectionQuestionID { get; set; }
        public virtual PmsSectionQuestion SectionQuestion { get; set; }
        public bool? StatusAnswer { get; set; }
        public string RemarksAnswer { get; set; }
        public string AssetTag { get; set; }
        public string Paragraph { get; set; }
        public string Textfield { get; set; }
        public bool? IsActive { get; set; } = true;

        
    }
}
