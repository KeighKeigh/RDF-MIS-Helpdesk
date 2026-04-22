using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.GetSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.SiteStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.CreatePmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Get_Pms_Questionaire_Module.GetPmsQuestionaireModule;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Questionaire_Module_Setup.Update_Pms_Questionaire_Module.UpdatePmsQuestionaireModuleStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.PMS_Sites_Controller
{
    [Route("api/pms-sites")]
    [ApiController]
    public class PmsSitesController : ControllerBase
    {
        private readonly IMediator mediator;

        public PmsSitesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> AddSite([FromBody] AddSiteCommand command)
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
        public async Task<IActionResult> GetSite([FromQuery] GetSiteQuery query)
        {
            try
            {
                var sites = await mediator.Send(query);

                Response.AddPaginationHeader(

                sites.CurrentPage,
                sites.PageSize,
                sites.TotalCount,
                sites.TotalPages,
                sites.HasPreviousPage,
                sites.HasNextPage

                );
                var result = new
                {
                    sites,
                    sites.CurrentPage,
                    sites.PageSize,
                    sites.TotalCount,
                    sites.TotalPages,
                    sites.HasPreviousPage,
                    sites.HasNextPage
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
        public async Task<IActionResult> SiteStatus([FromRoute] int id)
        {
            try
            {
                var command = new SiteStatusCommand
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
