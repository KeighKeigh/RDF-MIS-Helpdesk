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


namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.ApprovalDateTicket
{
    public partial class ApprovalDateTicket
    {
        public class Handler : IRequestHandler<ApprovalDateTicketCommand, Result>
        {
            private readonly IUnitOfWork unitOfWork;
            private readonly IHubContext<TicketHub> _hubContext;
            private readonly MisDbContext context;
            public Handler(IUnitOfWork unitOfWork, IHubContext<TicketHub> hubContext, MisDbContext context)
            {
                this.unitOfWork = unitOfWork;
                _hubContext = hubContext;
                this.context = context;
            }

            public async Task<Result> Handle(ApprovalDateTicketCommand command, CancellationToken cancellationToken)
            {
                var dateToday = DateTime.Today;

                //var channel = 0;
                var userDetails = await unitOfWork.User
                    .UserExist(command.Transacted_By);

                var approverPermissionList = await unitOfWork.UserRole
                         .UserRoleByPermission(TicketingConString.Approver);

                foreach (var date in command.ApproveDateRequests)
                {

                    var dateApproveTicketExist = await unitOfWork.ApproverDate
                          .ApproverDateExist(date.ApprovalDateTicketId);

                    if (dateApproveTicketExist is null)
                        return Result.Failure(TicketDateError.DateApproverTicketIdNotExist());

                    if (dateApproveTicketExist.IsActive is false)
                        return Result.Failure(TicketDateError.TicketAlreadyCancel());

                    var selectNonDateApproveRequestId = await unitOfWork.ApproverDate
                        .ApproverByMinLevel(dateApproveTicketExist.Id);

                    if (selectNonDateApproveRequestId is not null)
                    {

                        if (dateApproveTicketExist.TicketApprover != command.Users
                            || !approverPermissionList.Any(x => x.Contains(command.Role)))
                            return Result.Failure(TransferTicketError.ApproverUnAuthorized());

                        var userApprovalList = await unitOfWork.ApproverDate
                            .ApproverByDateApprovalTicketList(selectNonDateApproveRequestId.ApproverDateId);

                        //var validateUserApprover = await unitOfWork.ApproverDate
                        //    .ApproverPlusOne(selectNonDateApproveRequestId.ApproverDateId, selectNonDateApproveRequestId.ApproverLevel.Value);

                        await unitOfWork.ApproverDate.ApprovedApproval(selectNonDateApproveRequestId.Id);

                        var ticketHistoryApproval = await unitOfWork.RequestTicket
                             .TicketHistoryMinByForApproval(dateApproveTicketExist.TicketConcernId);

                        var updateHistoryApproval = new TicketHistory
                        {
                            Id = ticketHistoryApproval.Id,
                            TransactedBy = command.Transacted_By,
                            Request = TicketingConString.Approve,
                            Status = $"{TicketingConString.DateApproval} {userDetails.Fullname}"
                        };

                        await unitOfWork.RequestTicket.UpdateTicketHistory(updateHistoryApproval, cancellationToken);

                        //if (validateUserApprover is not null)
                        //{

                        //    await unitOfWork.ApproverDate.NextApproverUser(dateApproveTicketExist.Id, validateUserApprover.UserId);

                        //    var addNewTicketTransactionNotification = new TicketTransactionNotification
                        //    {

                        //        Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} is pending for Target Date approval",
                        //        AddedBy = command.Transacted_By.Value,
                        //        Created_At = DateTime.Now,
                        //        ReceiveBy = validateUserApprover.UserId.Value,
                        //        Modules = PathConString.Approval,
                        //        Modules_Parameter = PathConString.ForDateApproval,
                        //        PathId = dateApproveTicketExist.TicketConcernId

                        //    };

                        //    await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                        //    var addTicketApproveNotification = new TicketTransactionNotification
                        //    {

                        //        Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} was approved by {userDetails.Fullname}",
                        //        AddedBy = command.Transacted_By.Value,
                        //        Created_At = DateTime.Now,
                        //        ReceiveBy = dateApproveTicketExist.TicketConcern.UserId.Value,
                        //        Modules = PathConString.IssueHandlerConcerns,
                        //        Modules_Parameter = PathConString.ForDateApproval,
                        //        PathId = dateApproveTicketExist.TicketConcernId

                        //    };

                        //    await unitOfWork.RequestTicket.CreateTicketNotification(addTicketApproveNotification, cancellationToken);

                        //}
                        //else
                        //{

                            var approvedClosingTicket = new ApproverDate
                            {
                                Id = dateApproveTicketExist.Id,
                                ApprovedDateBy = command.ApprovedDateBy,

                            };

                            await unitOfWork.ApproverDate.ApprovedDateTicket(approvedClosingTicket, cancellationToken);

                            var approvedTicketByApprovingDate = new TicketConcern
                            {
                                Id = dateApproveTicketExist.TicketConcernId,
                                ApprovedDateBy = command.ApprovedDateBy,
                                ConcernStatus = TicketingConString.OnGoing,
                                IsApprove = true,
                                IsAssigned = true,

                            };

                            await unitOfWork.ApproverDate.ApprovedTicketConcernByApprovingDate(approvedTicketByApprovingDate, cancellationToken);

                        //    var approvedRequestByApprovingDate = new RequestConcern
                        //    {
                        //        Id = dateApproveTicketExist.TicketConcern.RequestConcernId.Value,
                        //        ConcernStatus = TicketingConString.OnGoing,
                                

                        //    };

                        //    await unitOfWork.ApproverDate.ApprovedRequestConcernByApprovingDate(approvedRequestByApprovingDate, cancellationToken);

                        //channel = approvedRequestByApprovingDate.ChannelId.Value;

                            var addNewTicketTransactionNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId}, Target Date is approved",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);

                            var addNewTransactionConfirmationNotification = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId}, Target Date is approved",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.UserId.Value,
                                Modules = PathConString.IssueHandlerConcerns,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId

                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTransactionConfirmationNotification, cancellationToken);

                            var addNewTicketTransactionOngoing = new TicketTransactionNotification
                            {

                                Message = $"Ticket number {dateApproveTicketExist.TicketConcernId} is now ongoing",
                                AddedBy = command.Transacted_By.Value,
                                Created_At = DateTime.Now,
                                ReceiveBy = dateApproveTicketExist.TicketConcern.RequestConcern.UserId.Value,
                                Modules = PathConString.ConcernTickets,
                                Modules_Parameter = PathConString.Ongoing,
                                PathId = dateApproveTicketExist.TicketConcernId,


                            };

                            await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionOngoing, cancellationToken);

                        //}
                    }
                    else
                    {
                        return Result.Failure(TicketDateError.ApproverUnAuthorized());

                    }
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);

                //if (channel == 2)
                //{

                //    var totalOpenTickets = await context.TicketConcerns.AsNoTracking()
                //        .Where(x => x.ConcernStatus == TicketingConString.OnGoing && x.AssignTo != null
                //        && x.RequestConcern.IsDone != true && x.OnHold != true
                //        && x.RequestConcern.ChannelId == 2)
                //        .Select(x => new
                //        {
                //            TicketConcernId = x.Id,
                //            EmpId = x.User.EmpId,
                //            IssueHandler = x.User.Fullname,
                //            TargetDate = x.TargetDate,
                //            ClosedDate = x.Closed_At,
                //        }).ToListAsync(cancellationToken);


                //    var totalClosingTickets = await context.ClosingTickets.AsNoTracking()
                //        .Where(x => x.IsClosing == true
                //        && x.TicketConcern.RequestConcern.ChannelId == 2)
                //        .Select(x => new
                //        {
                //            TicketConcernId = x.TicketConcernId,
                //            EmpId = x.AddedByUser.EmpId,
                //            IssueHandler = x.AddedByUser.Fullname,
                //            TargetDate = x.TicketConcern.TargetDate,
                //            ClosedDate = x.ClosingAt,
                //        }).ToListAsync(cancellationToken);

                //    var delayedTickets = totalClosingTickets.Where(x => x.ClosedDate.Value.Date > x.TargetDate.Value.Date).ToList();
                //    var delayedOpenTickets = totalOpenTickets.Where(x => x.TargetDate < DateTime.Today).ToList();

                //    var allDelayedTickets = delayedTickets
                //        .Concat(delayedOpenTickets)
                //        .ToList();

                //    var onTimeTickets = totalClosingTickets.Where(x => x.ClosedDate.Value.Date <= x.TargetDate).ToList();

                //    var openTicketCount = totalOpenTickets.Count();
                //    var closedTicketCount = totalClosingTickets.Count();
                //    var delayedTicketCount = allDelayedTickets.Count();
                //    var onTimeTicketCount = onTimeTickets.Count();

                //    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveClosingTicketCounts", new
                //    {
                //        TotalClosingTickets = closedTicketCount,

                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOpenTicketCounts", new
                //    {
                //        TotalOpenTickets = openTicketCount,

                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveDelayedTicketCounts", new
                //    {
                //        DelayedTickets = delayedTicketCount,

                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ReceiveOnTimeTicketCounts", new
                //    {
                //        OnTimeTickets = onTimeTicketCount,

                //    });


                //    await _hubContext.Clients.Group("Admins").SendAsync("ListOfOpenTickets", new
                //    {
                //        OpenTicketList = totalOpenTickets,

                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ListOfClosedTickets", new
                //    {
                //        ClosedTicketList = totalClosingTickets,

                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ListOfDelayedTickets", new
                //    {
                //        DelayedTicketList = allDelayedTickets,


                //    });

                //    await _hubContext.Clients.Group("Admins").SendAsync("ListOfOnTimeTickets", new
                //    {
                //        OnTimeTicketlist = onTimeTickets,


                //    });
                //}
                return Result.Success();
            }

        }

    }
}
