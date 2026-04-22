using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS
{
    public interface IPmsGetRepository
    {
        Task<User> UserExist(Guid? id);
        Task<List<Sites>> GetSites();
    }
}
