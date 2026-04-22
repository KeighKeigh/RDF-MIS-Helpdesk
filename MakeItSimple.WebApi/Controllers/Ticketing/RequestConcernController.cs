using DocumentFormat.OpenXml.Bibliography;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Extension;
using MakeItSimple.WebApi.Controllers.Authentication;

//using MakeItSimple.WebApi.Common.SignalR;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddCommentNotificationValidator;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequest.UpdateRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequestConcern.AddConcern;

//using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AssignAndApprovalTicket.AssignAndApprovalConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.CherryPicking.TakeCherryPicking;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.DownloadImageTicketing;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.GetTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.RemoveTicketComment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ViewMultipleSubCategories.ViewMultipleSubCategory;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ApprovalTicket.RequestApprovalReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AssignTicket.AddRequestConcernReceiver;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.BackJob.TicketBackJob;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.CancelTicket.CancelRequestConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetAttachment.GetRequestAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.GetConcernTicket.GetRequestorTicketConcern;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.RemoveTicketAttachment.RemoveTicketAttachment;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.ViewImage.ViewTicketImage;



namespace MakeItSimple.WebApi.Controllers.Ticketing
{
    [Route("api/request-concern")]
    [ApiController]
    public class RequestConcernController : ControllerBase
    {
        private readonly IMediator _mediator;
        //private readonly TimerControl _timerControl;
        private readonly MisDbContext context; 

        public RequestConcernController(IMediator mediator, MisDbContext context)
        {
            _mediator = mediator;

            this.context = context;

        }


        [HttpPost("add-request-concern")]
        public async Task<IActionResult> AddRequestConcern([FromForm]AddRequestConcernCommand command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    //command.UserId = userId;
                   
                }
                var result = await _mediator.Send(command);
                if(result.IsFailure)
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

        [AllowAnonymous]
        [ApiKeyAuth]
        [HttpPost("add-concern")]
        public async Task<IActionResult> AddConcern([FromForm] AddConcernCommand command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                    command.Modified_By = command.UserId;
                    command.Added_By = command.UserId;

                var result = await _mediator.Send(command);
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



        [HttpGet("backjob")]
        public async Task<IActionResult> TicketBackJob([FromQuery] TicketBackJobQuery command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

                }
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("multiple-sub-category")]
        public async Task<IActionResult> ViewMultipleSubCategory([FromQuery] ViewMultipleSubCategoryQuery command)
        {
            try
            {

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("cancel-request")]
        public async Task<IActionResult> CancelRequestConcern([FromBody] CancelRequestConcernCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("remove-attachment")]
        public async Task<IActionResult> RemoveTicketAttachment([FromBody] RemoveTicketAttachmentCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        // addticket
        [HttpPost("add-ticket-concern")]
        public async Task<IActionResult> AddRequestConcernReceiver([FromForm]AddRequestConcernReceiverCommand command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.Modified_By = userId;
                        command.Added_By = userId;

                    }
                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result);
                }

                await transaction.CommitAsync();
                return Ok(result);


            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                return Conflict(ex.Message);
            }
        }


        [HttpGet("request-attachment")]
        public async Task<IActionResult> GetRequestAttachment([FromQuery] GetRequestAttachmentQuery query)
        {
            try
            {
                var results = await _mediator.Send(query);
                if (results.IsFailure)
                {
                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }  
        }

        [HttpGet("requestor-page")]
        public async Task<IActionResult> GetRequestorTicketConcern([FromQuery] GetRequestorTicketConcernQuery query)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        query.UserId = userId;
                        
                    }
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        query.Role = userRole.Value;
                    }
                }

                var requestConcern = await _mediator.Send(query);

                Response.AddPaginationHeader(

                requestConcern.CurrentPage,
                requestConcern.PageSize,
                requestConcern.TotalCount,
                requestConcern.TotalPages,
                requestConcern.HasPreviousPage,
                requestConcern.HasNextPage

                );

                var result = new
                {
                    requestConcern,
                    requestConcern.CurrentPage,
                    requestConcern.PageSize,
                    requestConcern.TotalCount,
                    requestConcern.TotalPages,
                    requestConcern.HasPreviousPage,
                    requestConcern.HasNextPage
                }; 

                var successResult = Result.Success(result);

                return Ok(successResult);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        //approvalrequest
        [AllowAnonymous]

        [HttpPut("approval-request-receiver")]
        public async Task<IActionResult> RequestApprovalReceiver([FromBody] RequestApprovalReceiverCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity)
                {
                    var userRole = identity.FindFirst(ClaimTypes.Role);
                    if (userRole != null)
                    {
                        command.Role = userRole.Value;
                    }

                    if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                    {
                        command.UserId = userId;
                        command.Approved_By = userId;
                        

                    }
                    
                }

                var results = await _mediator.Send(command);
                if (results.IsFailure)
                {

                    return BadRequest(results);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [AllowAnonymous]

        //[HttpPut("approval-request-receiver-add-ticket-concern")]
        //public async Task<IActionResult> AssignAndRequestConcern([FromForm] AssignAndApprovalConcernCommand command)
        //{
        //    try
        //    {
        //        if (User.Identity is ClaimsIdentity identity)
        //        {
        //            var userRole = identity.FindFirst(ClaimTypes.Role);
        //            if (userRole != null)
        //            {
        //                command.Role = userRole.Value;
        //            }

        //            if (Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
        //            {
        //                command.UserId = userId;
        //                command.Approved_By = userId;
        //                command.Modified_By = userId;
        //                command.Added_By = userId;

        //            }

        //        }

        //        var results = await _mediator.Send(command);
        //        if (results.IsFailure)
        //        {

        //            return BadRequest(results);
        //        }
        //        return Ok(results);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }
        //}

        //[HttpPost("add-comment")]
        //public async Task<IActionResult> AddTicketComment([FromForm] AddTicketCommentCommand command)
        //{
        //    try
        //    {
        //        if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
        //        {
        //            command.Modified_By = userId;
        //            command.Added_By = userId;
        //            command.UserId = userId;

        //        }
        //        var result = await _mediator.Send(command);
        //        if (result.IsFailure)
        //        {
        //            return BadRequest(result);
        //        }
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Conflict(ex.Message);
        //    }
        //}

        [HttpPut("remove-comment")]
        public async Task<IActionResult> RemoveTicketComment([FromBody] RemoveTicketCommentCommand command)
        {
            try
            {
                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;

                }
                var result = await _mediator.Send(command);
                if (result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("view-comment")]
        public async Task<IActionResult> GetTicketComment([FromQuery]GetTicketCommentQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("add-comment-view")]
        public async Task<IActionResult> AddCommentNotificationValidator([FromBody] AddCommentNotificationValidatorCommand command)
        {
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.UserId = userId;
                    command.Added_By = userId;  

                }    
                var result = await _mediator.Send(command);
                if(result.IsFailure)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadImageTicketing(int id)
        {
            try
            {
                var query = new DownloadImageTicketingCommand
                {
                    TicketAttachmentId = id
                };

                var result = await _mediator.Send(query);
                if (result.IsSuccess)
                {
                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

                    if (fileResult != null)
                    {
                        return fileResult;
                    }

                    return BadRequest(result);
                }

                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("view/{id}")]
        public async Task<IActionResult> ViewTicketImage(int id)
        {
            try
            {
                var query = new ViewTicketImageCommand
                {
                    TicketAttachmentId = id
                };

                var result = await _mediator.Send(query);
                if (result.IsSuccess)
                {
                    var fileResult = result is Result<FileStreamResult> fileStreamResult ? fileStreamResult.Value : null;

                    if (fileResult != null)
                    {
                        Response.Headers.Add("Content-Disposition", "inline");
                        return fileResult;
                    }

                    return BadRequest(result);
                }

                return BadRequest(new { ErrorCode = result.Error.Code, ErrorMessage = result.Error.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("update-request-concern")]
        public async Task<IActionResult> UpdateRequestConcern( [FromForm] UpdateRequestConcernCommand command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                if (User.Identity is ClaimsIdentity identity && Guid.TryParse(identity.FindFirst("id")?.Value, out var userId))
                {
                    command.Modified_By = userId;
                    command.Added_By = userId;
                    //command.UserId = userId;

                }
                var result = await _mediator.Send(command);
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


        [HttpPost("cherry-pick-concern")]
        public async Task<IActionResult> CherryPickConcern([FromBody] TakeCherryPickingCommand command)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var result = await _mediator.Send(command);
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

    }
}
