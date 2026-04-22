using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers.AddPmsAnswers;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management.AddChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions.AddSectionQuestion;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.AddSitePivot;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.AddType;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms.SchedulePms;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS
{
    public interface IPmsCreatingRepository
    {
        Task<bool> AddNewSite(List<SitesNames> newSites, CancellationToken cancellationToken);

        Task<bool> AddPivotSite(AddSitePivotCommand pivot, CancellationToken cancellationToken);

        Task<bool> AddSchedulePMS(SchedulePmsCommand schedule, CancellationToken cancellationToken);

        //Types
        Task<bool> AddNewType(List<TypeNames> newTypes, CancellationToken cancellationToken);
        Task<bool> AddNewSectionQuestion(AddSectionQuestionCommand question, CancellationToken cancellationToken);

        //ChecklistManagement
        Task<bool> AddChecklistManagement(AddChecklistManagementCommand checklist, CancellationToken cancellationToken);

        //Answers
        Task<bool> AddAnswers(AddPmsAnswersCommand answers, CancellationToken cancellationToken);
    }
}
