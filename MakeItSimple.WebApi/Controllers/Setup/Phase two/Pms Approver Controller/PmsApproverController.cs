//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Create_Pms_Form.CreatePmsForm;
//using System.Security.Claims;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Upsert_Pms_Approver.UpsertPmsApprover;
//using MakeItSimple.WebApi.Common.Extension;
//using MakeItSimple.WebApi.Common;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.Setup.Phase_Two.Pms_Form_Setup.Get_Pms_Form.GetPmsForm;
//using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Get_Pms_Approver.GetPmsApprover;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Setup.Phase_Two.Pms_Approver_Setup.Update_Pms_Approver_Status.UpdatePmsApproverStatus;

//namespace MakeItSimple.WebApi.Controllers.Setup.Phase_two.Pms_Approver_Controller
//{
//    [Route("api/pms-approver")]
//    [ApiController]
//    public class PmsApproverController : ControllerBase
//    {
//        private readonly IMediator mediator;

//        public PmsApproverController(IMediator mediator)
//        {
//            this.mediator = mediator;
//        }

//        [HttpPost("upsert")]
//        public async Task<IActionResult> UpsertPmsApprover([FromBody] UpsertPmsApproverCommand command)
//        {
//            try
//            {
//                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//                {
//                    command.Added_By = userId;
//                    command.Modified_By = userId;
//                }
//                var result = await mediator.Send(command);

//                return result.IsFailure ? BadRequest(result) : Ok(result);

//            }
//            catch (Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }

//        [HttpGet("page")]
//        public async Task<IActionResult> GetPmsApprover([FromQuery] GetPmsApproverQuery query)
//        {
//            try
//            {
//                var pmsApprover = await mediator.Send(query);

//                Response.AddPaginationHeader(

//                pmsApprover.CurrentPage,
//                pmsApprover.PageSize,
//                pmsApprover.TotalCount,
//                pmsApprover.TotalPages,
//                pmsApprover.HasPreviousPage,
//                pmsApprover.HasNextPage

//                );

//                var result = new
//                {
//                    pmsApprover,
//                    pmsApprover.CurrentPage,
//                    pmsApprover.PageSize,
//                    pmsApprover.TotalCount,
//                    pmsApprover.TotalPages,
//                    pmsApprover.HasPreviousPage,
//                    pmsApprover.HasNextPage
//                };

//                var successResult = Result.Success(result);
//                return Ok(successResult);

//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPatch("update-status/{id}")]
//        public async Task<IActionResult> UpdatePmsApproverStatus([FromRoute] int id)
//        {
//            try
//            {
//                var command = new UpdatePmsApproverStatusCommand
//                {
//                    Id = id

//                };

//                var result = await mediator.Send(command);

//                return result.IsFailure ? BadRequest(result) : Ok(result);

//            }
//            catch (Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }


//    }
//}
