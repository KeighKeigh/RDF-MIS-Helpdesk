using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers.AddPmsAnswers;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management.AddChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions.AddSectionQuestion;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.AddSitePivot;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.AddType;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms.SchedulePms;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two.PMS
{
    public class PmsCreatingRepository : IPmsCreatingRepository
    {

        private readonly MisDbContext context;

        public PmsCreatingRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AddNewSite(List<SitesNames> newSites, CancellationToken cancellationToken)
        {


            foreach (var site in newSites)
            {
                if(site.Id == null)
                {
                    var sites = new Sites
                    {
                        Site = site.Name,
                    };
                    await context.Sites.AddAsync(sites, cancellationToken);
                }
                else
                {
                    var existingSites = await context.Sites.Where(x => x.IsActive == true && x.Id == site.Id).FirstOrDefaultAsync();

                    existingSites.Site = site.Name;
                }
                
            }
            await context.SaveChangesAsync();

            return true;
        }


        public async Task<bool> AddPivotSite(AddSitePivotCommand pivot, CancellationToken cancellationToken)
        {

            var addPivotSite = new SitePivot
            {
               SiteId = pivot.SiteId,
               BusinessUnitId = pivot.BusinessUnitId,
               BusinessUnitName = pivot.BusinessUnitName,
            };

            await context.SitesPivot.AddAsync(addPivotSite, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> AddSchedulePMS(SchedulePmsCommand schedule, CancellationToken cancellationToken)
        {
            var schedulePms = new PmsPhaseTwo
            {
                ChargingCode = schedule.Charging,
                ScheduleDate = schedule.ScheduleDate,
                PmsTypeId = schedule.PmsTypeId,
                AddedBy = schedule.UserId,
                SiteId = schedule.SiteId,
            };

            await context.Pmss.AddAsync(schedulePms, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return true;
        }


        //Types

        public async Task<bool> AddNewType(List<TypeNames> newTypes, CancellationToken cancellationToken)
        {
            foreach (var type in newTypes)
            {
                if (type.Id == null)
                {
                    var types = new PmsPhaseTwoType
                    {
                        PmsType = type.Name,
                    };
                    await context.PmsType.AddAsync(types, cancellationToken);
                }
                else
                {
                    var existingTypes = await context.PmsType.Where(x => x.IsActive == true && x.Id == type.Id).FirstOrDefaultAsync();

                    existingTypes.PmsType = type.Name;
                }

            } 
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddNewSectionQuestion(AddSectionQuestionCommand question, CancellationToken cancellationToken)
        {
            var existingQuestions = await context.PmsSectionQuestions.Where(x => x.IsActive == true).ToListAsync(cancellationToken);
            foreach (var section in question.SectionLists)
            {
                foreach (var header in section.HeaderLists)
                {
                    foreach (var questions in header.QuestionLists)
                    {

                        if (questions.QuestionId != null)
                        {
                            var existing = existingQuestions.FirstOrDefault(x => x.Id == questions.QuestionId);

                            if (existing != null)
                            {
                                existing.Section = section.Section;
                                existing.Questions = questions.Questions;
                                existing.HasHeader = header.HasHeader;
                                existing.Headers = header.Header;
                                existing.HasCheckBox = questions.HasCheckBox;
                                existing.HasAssetTag = questions.HasAssetTag;
                                existing.HasRemarks = questions.HasRemarks;
                                existing.HasParagraph = questions.HasParagraph;
                                existing.OrderBy = questions.OrderBy;
                                existing.UpdatedBy = question.AddedBy;
                                existing.UpdatedAt = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            var questionnaires = new PmsSectionQuestion
                            {
                                ChecklistManagementId = question.ChecklistManagementId,
                                PmsTypeId = question.PmsTypeId,
                                Section = section.Section,
                                Questions = questions.Questions,
                                HasHeader = header.HasHeader,
                                Headers = header.Header,
                                HasCheckBox = questions.HasCheckBox,
                                HasAssetTag = questions.HasAssetTag,
                                HasRemarks = questions.HasRemarks,
                                HasParagraph = questions.HasParagraph,
                                AddedBy = question.AddedBy,
                                OrderBy = questions.OrderBy
                            };
                            await context.PmsSectionQuestions.AddAsync(questionnaires, cancellationToken);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();

            return true;
        }

        //Checklist Management
        public async Task<bool> AddChecklistManagement(AddChecklistManagementCommand checklist, CancellationToken cancellationToken)
        {
            if (checklist.Id == null)
            {
                var chchecklists = new PmsChecklistManagement
                {
                    Title = checklist.Title,
                    Description = checklist.Description,

                };
                await context.PmsChecklistManagements.AddAsync(chchecklists, cancellationToken);
            }
            else
            {
                var existingchecklist = await context.PmsChecklistManagements.Where(x => x.IsActive == true && x.Id == checklist.Id).FirstOrDefaultAsync();

                existingchecklist.Title = checklist.Title;
                existingchecklist.Description = checklist.Description;
            }

            await context.SaveChangesAsync();

            return true;
        }


        //Answers
        public async Task<bool> AddAnswers(AddPmsAnswersCommand answers, CancellationToken cancellationToken)
        {
            foreach(var answer in answers.PmsAnswers)
            {
                var allAnswers = new PmsPhaseTwoAnswer
                {
                    PmsId = answers.PmsId,
                    AddedBy = answers.AnsweredBy,
                    SectionQuestionID = answer.QuestionId,
                    StatusAnswer = answer.StatusAnswer,
                    AssetTag = answer.AssetTagAnswer,
                    RemarksAnswer = answer.RemarksAnswer,
                    Paragraph = answer.ParagraphAnswer,
                    Textfield = answer.Textfield,
                };

                await context.PmsAnswers.AddAsync(allAnswers, cancellationToken);
            }
            await context.SaveChangesAsync();
            return true;
        }
    }
}
