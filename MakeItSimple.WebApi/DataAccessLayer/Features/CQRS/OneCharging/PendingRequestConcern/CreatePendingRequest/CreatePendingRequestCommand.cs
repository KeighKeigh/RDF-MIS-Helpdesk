using MakeItSimple.WebApi.Common;
using MediatR;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.CreatePendingRequest
{
    public partial class CreatePendingRequestHandler
    {
        public class CreatePendingRequestCommand : IRequest<Result>
        {
            public int Id { get; set; }
            public string Id_Prefix { get; set; }
            public string Id_No { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string First_Name { get; set; }
            public string Last_Name { get; set; }
            public string Middle_Name { get; set; }
            public string Suffix { get; set; }
        }
    }
}
