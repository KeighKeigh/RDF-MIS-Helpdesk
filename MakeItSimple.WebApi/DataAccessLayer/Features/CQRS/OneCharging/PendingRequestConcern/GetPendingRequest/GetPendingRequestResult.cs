namespace MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.OneCharging.PendingRequestConcern.GetPendingRequest
{
    public partial class GetPendingRequestHandler
    {
        public class GetPendingRequestResult
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
