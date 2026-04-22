using Azure;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management.AddChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Checklist_Management.ChecklistManagementStatus;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMS_Questions.PMS_Checklist_Management.GetChecklistManagement;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.GetSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.SiteStatus;

namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Checklist_Management_Controller
{
    [Route("api/pms-checklist-management")]
    [ApiController]
    public class PmsChecklistManagementController : ControllerBase
    {
        private readonly IMediator mediator;

        public PmsChecklistManagementController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost()]
        public async Task<IActionResult> AddChecklistManagement([FromBody] AddChecklistManagementCommand command)
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
        public async Task<IActionResult> GetChecklistManagement([FromQuery] GetChecklistManagementQuery query)
        {
            try
            {
                var checklistManagement = await mediator.Send(query);

                Response.AddPaginationHeader(

                checklistManagement.CurrentPage,
                checklistManagement.PageSize,
                checklistManagement.TotalCount,
                checklistManagement.TotalPages,
                checklistManagement.HasPreviousPage,
                checklistManagement.HasNextPage

                );
                var result = new
                {
                    checklistManagement,
                    checklistManagement.CurrentPage,
                    checklistManagement.PageSize,
                    checklistManagement.TotalCount,
                    checklistManagement.TotalPages,
                    checklistManagement.HasPreviousPage,
                    checklistManagement.HasNextPage
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
        public async Task<IActionResult> SiteChecklistManagement([FromRoute] int id)
        {
            try
            {
                var command = new ChecklistManagementStatusCommand
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
