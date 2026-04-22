using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.AddSitePivot;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.GetSitePivot;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSitePivot.SitePivotStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.GetSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.SiteStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Business_Unit_Controller
{
    [Route("api/pms-site-business-unit")]
    [ApiController]
    public class SitesBusinessUnitController : ControllerBase
    {
        private readonly IMediator mediator;

        public SitesBusinessUnitController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> AddSitePivot([FromBody] AddSitePivotCommand command)
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
        public async Task<IActionResult> GetSitePivot([FromQuery] GetSitePivotQuery query)
        {
            try
            {
                var businessUnit = await mediator.Send(query);

                Response.AddPaginationHeader(

                businessUnit.CurrentPage,
                businessUnit.PageSize,
                businessUnit.TotalCount,
                businessUnit.TotalPages,
                businessUnit.HasPreviousPage,
                businessUnit.HasNextPage

                );
                var result = new
                {
                    businessUnit,
                    businessUnit.CurrentPage,
                    businessUnit.PageSize,
                    businessUnit.TotalCount,
                    businessUnit.TotalPages,
                    businessUnit.HasPreviousPage,
                    businessUnit.HasNextPage
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
        public async Task<IActionResult> SiteStatusPivot([FromRoute] int id)
        {
            try
            {
                var command = new SitePivotStatusCommand
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
