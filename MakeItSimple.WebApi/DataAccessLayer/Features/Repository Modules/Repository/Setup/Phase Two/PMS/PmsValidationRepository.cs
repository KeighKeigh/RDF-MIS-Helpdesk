using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Policy;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSSites.AddSite;
using static MakeItSimple.WebApi.DataAccessLayer.Features.CQRS.PMS.PMSType.AddType;


namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two.PMS
{
    public class PmsValidationRepository : IPmsValidationRepository
    {
        private readonly MisDbContext context;

        public PmsValidationRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistingSite(string site)
        {
            var sites = await context.Sites.AnyAsync(x => x.Site == site);

            if (sites)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ExistingSiteById(int? id)
        {
            var sites = await context.Sites.AnyAsync(x => x.Id == id);

            if (sites)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ValidateSites(List<SitesNames> newSites)
        {

            foreach (var site in newSites) {
                var validate = await context.Sites.AnyAsync(x => x.Site == site.Name);
                if (validate)
                {
                    return false;
                }

            }

            return true;
            
        }

        public async Task<bool> ValidateTypes(List<TypeNames> newTypes)
        {

            foreach (var type in newTypes)
            {
                var validate = await context.PmsType.AnyAsync(x => x.PmsType == type.Name);
                if (validate)
                {
                    return false;
                }

            }

            return true;

        }

        public async Task<bool> ExistingBusinessUnitById(int? id)
        {
            var sites = await context.SitesPivot.AnyAsync(x => x.BusinessUnitId == id && x.IsActive == true);

            if (sites)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> ExistingPivot(int? siteId, int? businessUnitId)
        {
            var sites = await context.SitesPivot.AnyAsync(x => x.BusinessUnitId == businessUnitId && x.SiteId == siteId);

            if (sites)
            {
                return true;
            }
            return false;
        }


    }
}
