using MakeItSimple.WebApi.Models.OneCharging;
using System.ComponentModel.DataAnnotations.Schema;

namespace MakeItSimple.WebApi.Models.Phase_Two.PMS
{
    public class PmsPhaseTwo : BaseIdEntity
    {
        [ForeignKey("OneChargingMIS")]
        public string ChargingCode { get; set; }
        public virtual OneChargingMIS OneChargingMIS { get; set; }
        public Guid? RequestorId { get; set; }
        public virtual User Requestor { get; set; }
        public Guid? AddedBy { get; set; }
        public virtual User AddedByUser { get; set; }

        public DateTime? ScheduleDate { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool? IsCompleted { get; set; }
        public bool? IsCanceled { get; set; } = false;
        public int? PmsTypeId { get; set; }
        public virtual PmsPhaseTwoType PmsType { get; set; }
        public int? SiteId { get; set; }
        public virtual Sites Site { get; set; }



    }
}
