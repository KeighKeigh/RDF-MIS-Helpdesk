using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.GetSitePivot;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms.GetReminderPms;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms.GetSchedulePms;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.Schedule_Pms.SchedulePms;

namespace MakeItSimple.WebApi.Controllers.PMS
{
    [Route("api/pms")]
    [ApiController]
    public class PmssController : ControllerBase
    {
        private readonly IMediator mediator;
        public PmssController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> UpsertPms([FromForm] SchedulePmsCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                }
                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetPms([FromQuery] GetSchedulePmsQuery query)
        {
            try
            {
                var pms = await mediator.Send(query);

                Response.AddPaginationHeader(

                pms.CurrentPage,
                pms.PageSize,
                pms.TotalCount,
                pms.TotalPages,
                pms.HasPreviousPage,
                pms.HasNextPage

                );
                var result = new
                {
                    pms,
                    pms.CurrentPage,
                    pms.PageSize,
                    pms.TotalCount,
                    pms.TotalPages,
                    pms.HasPreviousPage,
                    pms.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("reminder")]
        public async Task<IActionResult> GetReminderPms([FromQuery] GetReminderPmsQuery query)
        {
            try
            {
                var pms = await mediator.Send(query);

                Response.AddPaginationHeader(

                pms.CurrentPage,
                pms.PageSize,
                pms.TotalCount,
                pms.TotalPages,
                pms.HasPreviousPage,
                pms.HasNextPage

                );
                var result = new
                {
                    pms,
                    pms.CurrentPage,
                    pms.PageSize,
                    pms.TotalCount,
                    pms.TotalPages,
                    pms.HasPreviousPage,
                    pms.HasNextPage
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
