namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsSectionQuestion :BaseIdEntity
    {
        public int? ChecklistManagementId { get; set; }
        public virtual PmsChecklistManagement ChecklistManagement { get; set; }
        public string Section { get; set; }
        public string Headers { get; set; }
        public bool? HasHeader { get; set; }
        public string Questions { get; set; }
        public bool? HasCheckBox { get; set; }
        public bool? HasRemarks { get; set; }
        public bool? HasAssetTag { get; set; }
        public bool? HasParagraph { get; set; }
        public bool? HasTextfield { get; set; }
        public int? PmsTypeId { get; set; }
        public virtual PmsPhaseTwoType PmsType { get; set; }
        public Guid? AddedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public virtual User UpdatedByUser { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual User AddedByUser { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? OrderBy { get; set; }

    }
}
