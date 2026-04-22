using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.SiteStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.AddType;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.GetType;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.TypeStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Type_Controller
{
    [Route("api/pms-types")]
    [ApiController]
    public class PmsTypeController : ControllerBase
    {
        private readonly IMediator mediator;

        public PmsTypeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> AddType([FromBody] TypeNamesCommand command)
        {
            try
            {
                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetType([FromQuery] GetTypeQuery query)
        {
            try
            {
                var type = await mediator.Send(query);

                Response.AddPaginationHeader(

                type.CurrentPage,
                type.PageSize,
                type.TotalCount,
                type.TotalPages,
                type.HasPreviousPage,
                type.HasNextPage

                );
                var result = new
                {
                    type,
                    type.CurrentPage,
                    type.PageSize,
                    type.TotalCount,
                    type.TotalPages,
                    type.HasPreviousPage,
                    type.HasNextPage
                };

                var successResult = Result.Success(result);
                return Ok(successResult);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> TypeStatus([FromRoute] int id)
        {
            try
            {
                var command = new TypeStatusCommand
                {
                    Id = id
                };

                var result = await mediator.Send(command);

                return result.IsFailure ? BadRequest(result) : Ok(result);

            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
