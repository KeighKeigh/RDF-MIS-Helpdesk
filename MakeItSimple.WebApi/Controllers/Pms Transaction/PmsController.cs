//using MediatR;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.UpsertPms.UpsertPms;
//using MakeItSimple.WebApi.Common.Extension;
//using MakeItSimple.WebApi.Common;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Get_Pms.GetPms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Approved_Pms.ApprovedPms;
//using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Reject_Pms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Reject_Pms.RejectPms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Cancel_Pms.CancelPms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.View_Image_Pms.ViewImagePms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.DownloadImageTicketing;
//using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Download_Attachment_Pms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Download_Attachment_Pms.DownloadAttachmentPms;
//using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Remove_Attachment_Pms;
//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Pms_Transaction.Remove_Attachment_Pms.RemoveAttachmentPms;

//namespace MakeItSimple.WebApi.Controllers.Pms_Transaction
//{
//    [Route("api/pms")]
//    [ApiController]
//    public class PmsController : ControllerBase
//    {
//        private readonly IMediator mediator;

//        public PmsController(IMediator mediator)
//        {
//            this.mediator = mediator;
//        }


//        [HttpPost("upsert")]
//        public async Task<IActionResult> UpsertPms([FromForm] UpsertPmsCommand command)
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
//        public async Task<IActionResult> GetPms([FromQuery] GetPmsQuery query)
//        {
//            try
//            {
//                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//                {
//                    query.UserId = userId;
//                }

//                var pms = await mediator.Send(query);

//                Response.AddPaginationHeader(

//                pms.CurrentPage,
//                pms.PageSize,
//                pms.TotalCount,
//                pms.TotalPages,
//                pms.HasPreviousPage,
//                pms.HasNextPage

//                );

//                var result = new
//                {
//                    pms,
//                    pms.CurrentPage,
//                    pms.PageSize,
//                    pms.TotalCount,
//                    pms.TotalPages,
//                    pms.HasPreviousPage,
//                    pms.HasNextPage
//                };

//                var successResult = Result.Success(result);
//                return Ok(successResult);

//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        [HttpPut("approved")]
//        public async Task<IActionResult> ApprovedPms([FromBody] ApprovedPmsCommand command)
//        {
//            try
//            {
//                if (User.Identity is ClaimsIdentity identity)
//                {
//                    var userRole = identity.FindFirst(ClaimTypes.Role);
//                    if (userRole != null)
//                    {
//                        command.Role = userRole.Value;
//                    }

//                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//                    {
//                        command.UserId = userId;
//                    }
//                }
//                var result = await mediator.Send(command);

//                return result.IsSuccess ? Ok(result) : BadRequest(result);

//            }
//            catch(Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }

//        [HttpPut("rejected")]
//        public async Task<IActionResult> RejectPms([FromBody] RejectPmsCommand command)
//        {
//            try
//            {
//                if (User.Identity is ClaimsIdentity identity)
//                {

//                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
//                    {
//                        command.Rejected_By = userId;
//                    }
//                }
//                var result = await mediator.Send(command);

//                return result.IsSuccess ? Ok(result) : BadRequest(result);

//            }
//            catch (Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }


//        [HttpPut("cancelled/{id}")]
//        public async Task<IActionResult> CancelPms([FromRoute] int id)
//        {
//            try
//            {
//                var command = new CancelPmsCommand
//                {
//                    Id = id
//                };

//                var result = await mediator.Send(command);

//                return result.IsSuccess ? Ok(result) : BadRequest(result);

//            }
//            catch (Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }

//        [HttpGet("view-image/{id}")]
//        public async Task<IActionResult> ViewImagePms([FromRoute] int id)
//        {
//            try
//            {
//                var query = new ViewImagePmsCommand
//                {
//                    Id = id
//                };

//                var result = await mediator.Send(query);
//                if (result.IsSuccess)
//                {
//                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

//                    if (fileResult != null)
//                    {
//                        Response.Headers.Add("Content-Disposition", "inline");
//                        return fileResult;
//                    }

//                    return BadRequest(result);
//                }

//                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });

//            }
//            catch(Exception ex)
//            {
//                return Conflict(ex.Message);
//            }
//        }

//        [HttpGet("download-attachment/{id}")]
//        public async Task<IActionResult> DownloadAttachmentPms(int id)
//        {
//            try
//            {
//                var query = new DownloadAttachmentPmsCommand
//                {
//                    Id = id
//                };

//                var result = await mediator.Send(query);
//                if (result.IsSuccess)
//                {
//                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

//                    if (fileResult != null)
//                    {
//                        return fileResult;
//                    }

//                    return BadRequest(result);
//                }

//                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
//            }
//        }

//        [HttpPut("remove-attachment/{id}")]
//        public async Task<IActionResult> RemoveAttachmentPms([FromRoute]int id)
//        {
//            try
//            {
//                var command = new RemoveAttachmentPmsCommand
//                {
//                    Id = id
//                };

//                var result = await mediator.Send(command);

//                return result.IsSuccess ? Ok(result) : BadRequest(result);

//            }
//            catch(Exception ex)
//            {
//                return Conflict(ex.Message);
//            }




//        }




//    }
//}
