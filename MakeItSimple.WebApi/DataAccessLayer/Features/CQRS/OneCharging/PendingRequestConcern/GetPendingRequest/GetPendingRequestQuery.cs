using MakeItSimple.WebApi.Common.Pagination;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.GetPendingRequest
{
    public partial class GetPendingRequestHandler
    {
        public class GetPendingRequestQuery : UserParams , IRequest<PagedList<GetPendingRequestResult>>
        {
            public string Search { get; set; }
        }
            
    }
}
