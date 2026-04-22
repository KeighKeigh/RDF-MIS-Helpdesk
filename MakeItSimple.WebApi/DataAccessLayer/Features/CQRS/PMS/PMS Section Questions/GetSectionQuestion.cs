using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions.AddSectionQuestion;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions.GetSectionQuestion;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.SubCategorySetup.GetSubCategory;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions
{
    public class GetSectionQuestion
    {

        public class GetSectionQuestionResult
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public int? PmsTypeId { get; set; }
            public List<GetSectionList> SectionLists { get; set; }
            public string AddedBy { get; set; }

        }

        public class GetSectionList
        {
            public string Section { get; set; }
            public List<GetHeaderList> HeaderLists { get; set; }
        }

        public class GetHeaderList
        {
            public string Header { get; set; }
            public bool? HasHeader { get; set; }
            public List<GetQuestionList> QuestionLists { get; set; }
        }
        public class GetQuestionList
        {
            public int? QuestionId { get; set; }
            public string Questions { get; set; }
            public bool? HasCheckBox { get; set; }
            public bool? HasRemarks { get; set; }
            public bool? HasAssetTag { get; set; }
            public bool? HasParagraph { get; set; }
            public int? OrderBy { get; set; }

        }

        public class GetSectionQuestionQuery : UserParams, IRequest<PagedList<GetSectionQuestionResult>>
        {
            public int? ChecklistId { get; set; }
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class Handler : IRequestHandler<GetSectionQuestionQuery, PagedList<GetSectionQuestionResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }
            public async Task<PagedList<GetSectionQuestionResult>> Handle(GetSectionQuestionQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsSectionQuestion> pmsSectionQuestions = _context.PmsSectionQuestions
                    .AsNoTracking()
                    .Include(x => x.ChecklistManagement);

                if (request.ChecklistId != null)
                {
                    pmsSectionQuestions = pmsSectionQuestions.Where(x => x.ChecklistManagementId == request.ChecklistId);
                }

                if (!string.IsNullOrEmpty(request.Search))
                {
                    pmsSectionQuestions = pmsSectionQuestions.Where(x => x.Questions.Contains(request.Search));
                }

                if (request.Status != null)
                {
                    pmsSectionQuestions = pmsSectionQuestions.Where(x => x.IsActive == request.Status);
                }

                var results = pmsSectionQuestions
                    .GroupBy(x => new { x.PmsTypeId, x.ChecklistManagement.Title, x.ChecklistManagement.Description, x.AddedByUser.Fullname } )
                    .Select(mainGroup => new GetSectionQuestionResult
                    {
                        Title = mainGroup.Key.Title,
                        Description = mainGroup.Key.Description,
                        PmsTypeId = mainGroup.Key.PmsTypeId,
                        AddedBy = mainGroup.Key.Fullname,
                        SectionLists = mainGroup
                        .GroupBy(x => x.Section)
                        .Select(sectionGroup => new GetSectionList
                        {
                            Section = sectionGroup.Key,
                            HeaderLists = sectionGroup
                            .GroupBy(x => new { x.Headers, x.HasHeader })
                            .Select(headerGroup => new GetHeaderList
                            {
                                Header = headerGroup.Key.Headers,
                                HasHeader = headerGroup.Key.HasHeader,
                                QuestionLists = headerGroup.OrderBy(x => x.OrderBy)
                                .Select(q => new GetQuestionList
                                {
                                    QuestionId = q.Id,
                                    Questions = q.Questions,
                                    HasCheckBox = q.HasCheckBox,
                                    HasRemarks = q.HasRemarks,
                                    HasAssetTag = q.HasAssetTag,
                                    HasParagraph = q.HasParagraph,
                                    OrderBy = q.OrderBy
                                   
                                }).ToList()
                            }).ToList()


                        }).ToList()



                    });

                return await PagedList<GetSectionQuestionResult>.CreateAsync(results, request.PageNumber, request.PageSize);
            }
        }
    }
}
