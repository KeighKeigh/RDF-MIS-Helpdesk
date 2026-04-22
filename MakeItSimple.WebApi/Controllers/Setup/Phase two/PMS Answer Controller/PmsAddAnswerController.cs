using Azure;
using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers.AddPmsAnswers;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Answers.GetPmsAnswers;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management.AddChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Questions.PMS_Checklist_Management.GetChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Section_Questions.AddSectionQuestion;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.PMS_Answer_Controller
{
    [Route("api/pms-answer")]
    [ApiController]
    public class PmsAddAnswerController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly MisDbContext context;

        public PmsAddAnswerController(IMediator mediator, MisDbContext context)
        {
            this.mediator = mediator;
            this.context = context;
        }

        [HttpPost()]
        public async Task<IActionResult> AddAnswers([FromBody] AddPmsAnswersCommand command)
        {

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.AnsweredBy = userId;
                }
                var result = await mediator.Send(command);

                if (result.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result);
                }

                await transaction.CommitAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Conflict(ex.Message);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetAnswers([FromQuery] GetPmsAnswersQuery query)
        {
            try
            {
                var answers = await mediator.Send(query);

                Response.AddPaginationHeader(

                answers.CurrentPage,
                answers.PageSize,
                answers.TotalCount,
                answers.TotalPages,
                answers.HasPreviousPage,
                answers.HasNextPage

                );
                var result = new
                {
                    answers,
                    answers.CurrentPage,
                    answers.PageSize,
                    answers.TotalCount,
                    answers.TotalPages,
                    answers.HasPreviousPage,
                    answers.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
