using MakeItSimple.WebApi.Common.ConstantString;
using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketHistories.Dtos;
using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketHistories.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketHistories.Handlers
{
    public class UpdateTicketHistoryForConfirmationHandler : IRequestHandler<UpdateTicketHistoryForConfirmation, Unit>
    {
        private readonly MisDbContext _context;
        public UpdateTicketHistoryForConfirmationHandler(MisDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(UpdateTicketHistoryForConfirmation request, CancellationToken cancellationToken)
        {

            
            
            var ticketHistory = await _context.TicketHistories
    .Where(x => x.TicketConcernId == request.TicketConcernId)
    .Where(x => x.IsApprove == null && x.Request.Contains(TicketingConString.NotConfirm))
    .FirstOrDefaultAsync();

            if (ticketHistory != null)
            {
                ticketHistory.TicketConcernId = request.TicketConcernId;
                ticketHistory.TransactedBy = null;
                ticketHistory.TransactionDate = DateTime.Now;
                ticketHistory.Request = "Auto Confirm";
                ticketHistory.Status = "Ticket has been auto confirmed by the system";
            }


            return Unit.Value;

        }
    }
}
