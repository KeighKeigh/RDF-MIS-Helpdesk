using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.AddType;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS
{
    public interface IPmsValidationRepository
    {
        Task<bool> ExistingSite(string site);

        Task<bool> ExistingSiteById(int? id);

        Task<bool> ValidateSites( List<SitesNames> newSites);
        Task<bool> ValidateTypes(List<TypeNames> newTypes);
        Task<bool> ExistingBusinessUnitById(int? id);
        Task<bool> ExistingPivot(int? siteId, int? businessUnitId);
    }
}
