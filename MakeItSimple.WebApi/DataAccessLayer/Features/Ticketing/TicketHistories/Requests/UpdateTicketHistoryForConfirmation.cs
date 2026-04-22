using MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketHistories.Dtos;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Ticketing.TicketHistories.Requests
{
    public class UpdateTicketHistoryForConfirmation : IRequest<Unit>
    {
        public int TicketConcernId { get; set; }
        
    }
}
