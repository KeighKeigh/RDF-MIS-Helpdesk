namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsPhaseTwoQuestionCategory : BaseIdEntity
    {
        public string QuestionCategory { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
