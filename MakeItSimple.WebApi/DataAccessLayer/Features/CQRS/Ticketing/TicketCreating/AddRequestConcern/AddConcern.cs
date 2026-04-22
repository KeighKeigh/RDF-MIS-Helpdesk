using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Errors.Ticketing;
using MakeItSimple.WebApi.DataAccessLayer.Errors.UserManagement.UserAccount;
using MakeItSimple.WebApi.DataAccessLayer.Unit_Of_Work;
using MakeItSimple.WebApi.Hubs;
using MakeItSimple.WebApi.Models.Ticketing;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketCreating.AddRequest.AddRequestConcern;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.Ticketing.TicketCreating.AddRequestConcern
{
    public partial class AddConcern
    {

        //public class returnRequests
        //{
        //    public string TicketConcernId { get; set; }
        //    public string RequestorName { get; set; }
        //    public string RequestorDepartment { get; set; }
        //    public string ChannelName { get; set; }
        //    public string Concern { get; set; }
        //    public DateTime? RequestedDate { get; set; }
        //}
        public class Handler : IRequestHandler<AddConcernCommand, Result>
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

            public async Task<Result> Handle(AddConcernCommand command, CancellationToken cancellationToken)
            {
                var oneTicket = 0;
                //var requestorData = new returnRequests();
                //var requestorData =

                var requestorDetails = await unitOfWork.User
                             .UserExist(command.UserId);

                
                    var ticketConcernId = new int(); //????kkkk
                    var requestConcernId = new int();


                    //var userDetails = await unitOfWork.User
                    //    .UserExist(command.Added_By);

                    var userIdExist = await unitOfWork.User
                        .UserExist(command.UserId);


                    if (userIdExist is null)
                        return Result.Failure(UserError.UserNotExist());

                    //var channelExist = await unitOfWork.Channel
                    //  .ChannelExist(command.ChannelId);

                    //if (channelExist is null)
                    //    return Result.Failure(TicketRequestError.ChannelNotExist());



                    var userDepartment = await unitOfWork.User.UserDepartment(userIdExist.DepartmentId);

                    //KK
                    //foreach (var concern in command.Concern)
                    

                        
                            var ticketCategoryExist = await unitOfWork.Category
                              .CategoryExist(command.CategoryId);

                    if (ticketCategoryExist is null)
                        return Result.Failure(TicketRequestError.CategoryNotExist());


                    //foreach (var subCategory in command.AddTicketSubCategories)
                    //{
                    //    var ticketSubCategoryExist = await unitOfWork.SubCategory
                    //        .SubCategoryExist(subCategory.SubCategoryId);

                    //    //if (ticketSubCategoryExist is null)
                    //    //    return Result.Failure(TicketRequestError.SubCategoryNotExist());
                    //}
                    var ticketCategoryList = new List<int?>();
                        var ticketSubCategoryList = new List<int?>();


                        var addRequestConcern = new RequestConcern
                        {
                            UserId = command.UserId,
                            Concern = command.Concern,
                            AddedBy = command.Added_By,
                            ConcernStatus = TicketingConString.ForApprovalTicket,
                            CompanyId = userIdExist.CompanyId,
                            BusinessUnitId = userIdExist.BusinessUnitId,
                            DepartmentId = userIdExist.DepartmentId,
                            ReqSubUnitId = userIdExist.SubUnitId.ToString() == "" ? null : userIdExist.SubUnitId,
                            ReqUnitId = userIdExist.UnitId.ToString() == "" ? null : userIdExist.UnitId,
                            LocationId = userIdExist.LocationId,
                            DateNeeded = command.DateNeeded,
                            ChannelId = command.ChannelId == 0 ? null : command.ChannelId,
                            Notes = command.Notes,
                            IsDone = false,
                            ContactNumber = command.Contact_Number,
                            RequestType = command.Request_Type,
                            BackJobId = command.BackJobId,
                            Severity = command.Severity,
                            TargetDate = command.TargetDate.ToString() == "" ? null : command.TargetDate,
                            AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,
                            ServiceProviderId = command.ServiceProviderId,
                            OneChargingCode = userIdExist.OneChargingCode,
                            OneChargingName = userIdExist.OneChargingName,
                            //TicketCategories = 

                        };

                        await unitOfWork.RequestTicket.CreateRequestConcern(addRequestConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);
                        requestConcernId = addRequestConcern.Id;


                        //kk

                        var addTicketConcern = new TicketConcern
                        {
                            RequestConcernId = requestConcernId,
                            RequestorBy = command.UserId,
                            IsApprove = false,
                            AddedBy = command.Added_By,
                            ConcernStatus = TicketingConString.ForApprovalTicket,
                            IsAssigned = false,
                            AssignTo = command.AssignTo.ToString() == "" ? null : command.AssignTo,



                        };

                        await unitOfWork.RequestTicket.CreateTicketConcern(addTicketConcern, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);


                        //if (oneTicket == 0)
                        //{
                        //    var channelName = await context.Channels.Where(x => x.Id == addRequestConcern.ChannelId).Select(x => x.ChannelName).FirstOrDefaultAsync();
                        //    requestorData = new returnRequests
                        //    {
                        //        TicketConcernId = addTicketConcern.Id.ToString(),
                        //        RequestorName = addTicketConcern.RequestorByUser.Fullname,
                        //        RequestorDepartment = userDepartment.department_name,
                        //        ChannelName = channelName,
                        //        Concern = addRequestConcern.Concern,
                        //        RequestedDate = addRequestConcern.CreatedAt,

                        //    };
                        //    oneTicket = 1;
                        //}

                        ticketConcernId = addTicketConcern.Id;

                        var addTicketHistory = new TicketHistory
                        {
                            TicketConcernId = ticketConcernId,
                            TransactedBy = command.Added_By,
                            TransactionDate = DateTime.Now,
                            Request = TicketingConString.Request,
                            Status = $"{TicketingConString.RequestCreated} {userIdExist.Fullname}"
                        };

                        await unitOfWork.RequestTicket.CreateTicketHistory(addTicketHistory, cancellationToken);

                        //kk


                        var addNewTicketTransactionNotification = new TicketTransactionNotification
                        {

                            Message = $"New request concern number {requestConcernId} has received",
                            AddedBy = command.Added_By.Value,
                            Created_At = DateTime.Now,
                            Modules = PathConString.ReceiverConcerns,
                            PathId = requestConcernId

                        };

                        await unitOfWork.RequestTicket.CreateTicketNotification(addNewTicketTransactionNotification, cancellationToken);



                        
                            var ticketCategoryExists = await unitOfWork.RequestTicket
                                .TicketCategoryExist(command.CategoryId, command.RequestConcernId);
                            if (command.CategoryId != null)
                            {
                                if (ticketCategoryExists is null)
                                {
                                    var addTicketCategory = new TicketCategory
                                    {
                                        RequestConcernId = requestConcernId,
                                        CategoryId = command.CategoryId,
                                    };
                                    await unitOfWork.RequestTicket.CreateTicketCategory(addTicketCategory, cancellationToken);
                                }
                            }
                        

                        //foreach (var subCategory in command.AddTicketSubCategories)
                        //{
                        //    var ticketSubCategoryExist = await unitOfWork.RequestTicket
                        //        .TicketSubCategoryExist(subCategory.SubCategoryId, command.RequestConcernId);
                        //    if (subCategory.SubCategoryId != null)
                        //    {
                        //        if (ticketSubCategoryExist is null)
                        //        {
                        //            var addTicketSubCategory = new TicketSubCategory
                        //            {
                        //                RequestConcernId = requestConcernId,
                        //                SubCategoryId = subCategory.SubCategoryId,
                        //            };
                        //            await unitOfWork.RequestTicket.CreateTicketSubCategory(addTicketSubCategory, cancellationToken);
                        //        }
                        //    }
                        //}

                        if (ticketCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketCategory(requestConcernId, ticketCategoryList, cancellationToken);

                        if (ticketSubCategoryList.Any())
                            await unitOfWork.RequestTicket.RemoveTicketSubCategory(requestConcernId, ticketSubCategoryList, cancellationToken);


                        //if (!Directory.Exists(TicketingConString.AttachmentPath))
                        //{
                        //    Directory.CreateDirectory(TicketingConString.AttachmentPath);
                        //}

                        //if (command.AttachmentsFiles.Count(x => x.Attachment != null) > 0)
                        //{
                        //    foreach (var attachments in command.AttachmentsFiles.Where(a => a.Attachment.Length > 0))
                        //    {

                        //        if (attachments.Attachment.Length > 10 * 1024 * 1024)
                        //        {
                        //            return Result.Failure(TicketRequestError.InvalidAttachmentSize());
                        //        }

                        //        var allowedFileTypes = new[] { ".jpeg", ".jpg", ".png", ".docx", ".pdf", ".xlsx" };
                        //        var extension = Path.GetExtension(attachments.Attachment.FileName)?.ToLowerInvariant();

                        //        if (extension == null || !allowedFileTypes.Contains(extension))
                        //        {
                        //            return Result.Failure(TicketRequestError.InvalidAttachmentType());
                        //        }

                        //        var fileName = $"{Guid.NewGuid()}{extension}";
                        //        var filePath = Path.Combine(TicketingConString.AttachmentPath, fileName);

                        //        var ticketAttachment = await unitOfWork.RequestTicket
                        //            .TicketAttachmentExist(attachments.TicketAttachmentId);

                        //        if (ticketAttachment != null)
                        //        {
                        //            var updateTicketAttachment = new TicketAttachment
                        //            {
                        //                Attachment = filePath,
                        //                FileName = attachments.Attachment.FileName,
                        //                FileSize = attachments.Attachment.Length,
                        //                UpdatedAt = DateTime.Now,

                        //            };

                        //            await unitOfWork.RequestTicket.UpdateTicketAttachment(updateTicketAttachment, cancellationToken);
                        //        }
                        //        else
                        //        {
                        //            var addAttachment = new TicketAttachment
                        //            {
                        //                TicketConcernId = ticketConcernId, //kk
                        //                Attachment = filePath,
                        //                FileName = attachments.Attachment.FileName,
                        //                FileSize = attachments.Attachment.Length,
                        //                AddedBy = command.Added_By,
                        //            };

                        //            await unitOfWork.RequestTicket.CreateTicketAttachment(addAttachment, cancellationToken);

                        //        }

                        //        await using (var stream = new FileStream(filePath, FileMode.Create))
                        //        {
                        //            await attachments.Attachment.CopyToAsync(stream);
                        //        }
                        //    }

                        //}


                    

                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    if (command.AssignTo != null && command.ChannelId == 2)
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

                        var delayedTickets = totalClosingTickets.Where(x => x.ClosedDate.Value.Date > x.TargetDate.Value.Date).ToList();
                        var delayedOpenTickets = totalOpenTickets.Where(x => x.TargetDate < DateTime.Today).ToList();

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
