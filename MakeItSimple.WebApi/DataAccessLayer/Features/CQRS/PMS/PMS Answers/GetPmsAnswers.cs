using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.GetSite;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers
{
    public class GetPmsAnswers
    {
        public class GetPmsAnswersResult
        {
            public int? PmsId { get; set; }
            
            public List<QuestionSection> QuestionSections { get; set; }

        }
        public class QuestionSection
        {
            public string Section { get; set; }
            public List<QuestionHeader> questionHeaders { get; set; }
        }

        public class QuestionHeader
        {
            public string Header { get; set; }
            public List<QuestionAndAnswers> QuestionAndAnswers { get; set; }
        }
        public class QuestionAndAnswers
        {
            public int? QuestionId { get; set; }
            public string Question { get; set; }
            public bool? Status { get; set; }
            public string AssetTag { get; set; }
            public string Remarks { get; set; }
            public string Textfield { get; set; }
            public string Paragraph { get; set; }
        }

        public class GetPmsAnswersQuery : UserParams, IRequest<PagedList<GetPmsAnswersResult>>
        {
            public int? PmsId { get; set; }
            public bool? Status { get; set; }
            public string Search { get; set; }
        }

        public class Handler : IRequestHandler<GetPmsAnswersQuery, PagedList<GetPmsAnswersResult>>
        {
            private readonly MisDbContext _context;

            public Handler(MisDbContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetPmsAnswersResult>> Handle(GetPmsAnswersQuery request, CancellationToken cancellationToken)
            {
                IQueryable<PmsPhaseTwoAnswer> answers = _context.PmsAnswers
                    .AsNoTracking()
                    .Include(x => x.SectionQuestion)
                    .Where(x => x.PmsId == request.PmsId);


                if (request.Status != null)
                {
                    answers = answers.Where(x => x.Pms.IsCanceled != request.Status);

                }

                var allAnswers = answers.GroupBy(x => x.PmsId).Select(mainData => new GetPmsAnswersResult
                {
                    PmsId = mainData.Key,
                    QuestionSections = mainData.GroupBy(x => x.SectionQuestion.Section).Select(section => new QuestionSection
                    {
                        Section = section.Key,
                        questionHeaders = section.GroupBy(x => x.SectionQuestion.Headers ?? PmsConsString.NoHeader).Select(header => new QuestionHeader
                        {
                            Header = header.Key == PmsConsString.NoHeader ? null : header.Key,
                            QuestionAndAnswers = header.OrderBy(x => x.SectionQuestion.OrderBy).Select(x => new QuestionAndAnswers
                            {
                                QuestionId = x.SectionQuestion.Id,
                                Question = x.SectionQuestion.Questions,
                                Status = x.StatusAnswer,
                                AssetTag = x.AssetTag,
                                Remarks = x.RemarksAnswer,
                                Textfield = x.Textfield,
                                Paragraph = x.Paragraph

                            }).ToList()
                        }).ToList()
                    }).ToList()
                });

                return await PagedList<GetPmsAnswersResult>.CreateAsync(allAnswers, request.PageNumber, request.PageSize);
            }
        }
    }
}
