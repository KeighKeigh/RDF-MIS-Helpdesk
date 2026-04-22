using DocumentFormat.OpenXml.InkML;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Hubs;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.ClosedTicketConcern.ApprovalClosing
{
    public partial class ApprovalClosingTicket
    {

        public class countClosing
        {
            //public int? TicketConcernId { get; set; }
            public int? ChannelId { get; set; }

        }
        public class Handler : IRequestHandler<ApproveClosingTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            //private readonly IHubCaller hubCaller;
            private readonly MisDbContext context;
            private readonly IHubContext<TicketHub> _hubContext;
            public Handler(IUnitOfWork unitOfWork, MisDbContext context, IHubContext<TicketHub> hubContext)
            {
                this.unitOfWork = unitOfWork;
                this.context = context;
                this._hubContext = hubContext;

            }
            public async Task<Result> Handle(ApproveClosingTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;
                var oneTicket = 0;
                var closingCount = new countClosing();

                var userDetails = await unitOfWork.User
                    .UserExist(command.Transacted_By);

                var approverPermissionList = await unitOfWork.UserRole
                         .UserRoleByPermission(TicketingConString.Approver);

                foreach (var close in command.ApproveClosingRequests)
                {

                    var closingTicketExist = await unitOfWork.ClosingTicket
                          .ClosingTicketExist(close.ClosingTicketId);

                    

                    if (closingTicketExist is null)
                        return Result.Failure(ClosingTicketError.ClosingTicketIdNotExist());

                    if(closingTicketExist.IsActive is false)
                        return Result.Failure(ClosingTicketError.TicketAlreadyCancel());

                    var selectClosedRequestId = await unitOfWork.ClosingTicket
                        .ApproverByMinLevel(closingTicketExist.Id);


                    if (oneTicket == 0)
                    {
                        if (closingTicketExist.TicketConcern.RequestConcern.ChannelId == 2)
                        {
                            closingCount = new countClosing
                            {
                                ChannelId = closingTicketExist.TicketConcern.RequestConcern.ChannelId,
                            };
                        }
                        oneTicket = 1;
                    }
                    if (selectClosedRequestId is not null)
                    {

                        if (closingTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))                    
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                        var userApprovalList = await unitOfWork.ClosingTicket
                            .ApproverByClosingTicketList(selectClosedRequestId.ClosingTicketId);

                        //var validateUserApprover = await unitOfWork.ClosingTicket
                        //    .ApproverPlusOne(selectClosedRequestId.ClosingTicketId);

                        await unitOfWork.ClosingTicket.ApprovedApproval(selectClosedRequestId.Id);

                        var ticketHistoryApproval = await unitOfWork.RequestTicket
                             .TicketHistoryMinByForApproval(closingTicketExist.TicketConcernId);

                        var updateHistoryApproval = new TicketHistory
                        {
                            Id = ticketHistoryApproval.Id,
                            TransactedBy = command.Transacted_By,
                            Request = TicketingConString.Approve,
                            Status = $"{TicketingConString.CloseApprove} {userDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        //var resolutionHIstory = new TicketHistory
                        //{
                        //    Id = ticketHistoryApproval.Id,
                        //    TransactedBy = command.Transacted_By,
                        //    Request = "Resolution",
                        //    Status = $"Resolution : {closingTicketExist.Resolution}"
                        //};

                        //await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        var resolutionHIstory = new TicketHistory
                        {
                            TicketConcernId = closingTicketExist.TicketConcernId,
                            TransactedBy = closingTicketExist.TicketConcern.AssignTo,
                            TransactionDate = DateTime.Now,
                            Request = "Resolution",
                            Status = $"Resolution : {closingTicketExist.Resolution}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(resolutionHIstory, cancellationToken);


                     

                            var approvedClosingTicket = new ClosingTicket
                            {
                                Id = closingTicketExist.Id,
                                ClosedBy = command.Closed_By,
                                IsClosing = true
                            };

                            await unitOfWork.ClosingTicket.ApprovedClosingTicket(approvedClosingTicket,cancellationToken);



                            var approvedTicketByClosingTicket = new TicketConcern
                            {
                                Id = closingTicketExist.TicketConcernId,
                                ClosedApproveBy = command.Closed_By,
                                IsDone = true,
                                ConcernStatus = TicketingConString.NotConfirm,
                                IsClosedApprove = true,
                            };

                            await unitOfWork.ClosingTicket.ApprovedTicketConcernByClosing(approvedTicketByClosingTicket,cancellationToken);

                            var approvedRequestByClosingTicket = new RequestConcern
                            {
                                Id = closingTicketExist.TicketConcern.RequestConcernId.Value,
                                Resolution = closingTicketExist.Resolution,
                                ConcernStatus = TicketingConString.NotConfirm,
                                CategoryConcernName = closingTicketExist.CategoryConcernName,

                            };

                            await unitOfWork.ClosingTicket.ApprovedRequestConcernByClosing(approvedRequestByClosingTicket,cancellationToken);

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is pending for closing Confirmation",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = closingTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification,cancellationToken);

                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {closingTicketExist.TicketConcernId} is waiting for Confirmation",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = closingTicketExist.TicketConcern.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.ForConfirmation,
                                PathId = closingTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);


                        
                    }
                    else
                    {
                        return Result.Failure(ClosingTicketError.ApproverUnAuthorized());

                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                if (closingCount.ChannelId == 2)
                {

                    var totalOpenTickets = await context.TicketConcerns.AsNoTracking()
                        .Where(x => x.ConcernStatus == TicketingConString.OnGoing && x.AssignTo != null
                        && x.RequestConcern.IsDone != true && x.OnHold != true
                        && x.RequestConcern.ChannelId == 2)
                        .Select(x => new
                        {
                            TicketConcernId = x.Id,
                            EmpId = x.User.EmpId,
                            IssueHandler = x.User.Fullname,
                            TargetDate = x.TargetDate,
                            ClosedDate = x.Closed_At,
                        }).ToListAsync(cancellationToken);


                    var totalClosingTickets = await context.ClosingTickets.AsNoTracking()
                        .Where(x => x.IsClosing == true
                        && x.TicketConcern.RequestConcern.ChannelId == 2)
                        .Select(x => new
                        {
                            TicketConcernId = x.TicketConcernId,
                            EmpId = x.AddedByUser.EmpId,
                            IssueHandler = x.AddedByUser.Fullname,
                            TargetDate = x.TicketConcern.TargetDate,
                            ClosedDate = x.ClosingAt,
                        }).ToListAsync(cancellationToken);

                    var delayedTickets = totalClosingTickets.Where(x => x.ClosedDate.Value.Date > x.TargetDate).ToList();
                    var delayedOpenTickets = totalOpenTickets.Where(x => DateTime.Today.Date > x.TargetDate).ToList();

                    var allDelayedTickets = delayedTickets
                        .Concat(delayedOpenTickets)
                        .ToList();

                    var onTimeTickets = totalClosingTickets.Where(x => x.ClosedDate.Value.Date <= x.TargetDate).ToList();

                    var openTicketCount = totalOpenTickets.Count();
                    var closedTicketCount = totalClosingTickets.Count();
                    var delayedTicketCount = allDelayedTickets.Count();
                    var onTimeTicketCount = onTimeTickets.Count();

                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveClosingTicketCounts", new
                    {
                        TotalClosingTickets = closedTicketCount,

                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOpenTicketCounts", new
                    {
                        TotalOpenTickets = openTicketCount,

                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDelayedTicketCounts", new
                    {
                        DelayedTickets = delayedTicketCount,

                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOnTimeTicketCounts", new
                    {
                        OnTimeTickets = onTimeTicketCount,

                    });


                    await _hubContext.Clients.Group("Admins").SendAsync("ListOfOpenTickets", new
                    {
                        OpenTicketList = totalOpenTickets,

                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ListOfClosedTickets", new
                    {
                        ClosedTicketList = totalClosingTickets,

                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ListOfDelayedTickets", new
                    {
                        DelayedTicketList = allDelayedTickets,


                    });

                    await _hubContext.Clients.Group("Admins").SendAsync("ListOfOnTimeTickets", new
                    {
                        OnTimeTicketlist = onTimeTickets,


                    });
                }
                return Result.Success();
            }

        }
    }
}
