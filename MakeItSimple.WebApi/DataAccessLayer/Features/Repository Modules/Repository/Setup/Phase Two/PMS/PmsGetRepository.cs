using MakeItSimple.WebApi.DataAccessLayer.Data.DataContext;
using MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository_Interface.Setup.Phase_Two.PMS;
using MakeItSimple.WebApi.Models;
using MakeItSimple.WebApi.Models.Phase_Two.PMS;
using Microsoft.EntityFrameworkCore;

namespace MakeItSimple.WebApi.DataAccessLayer.Features.Repository_Modules.Repository.Setup.Phase_Two.PMS
{
    public class PmsGetRepository : IPmsGetRepository
    {
        private readonly MisDbContext context;

        public PmsGetRepository(MisDbContext context)
        {
            this.context = context;
        }

        public async Task<User> UserExist(Guid? id)
        {
            var userExist = await context.Users.FirstOrDefaultAsync(u => u.Id == id);

            return userExist;
        }

        public async Task<List<Sites>> GetSites()
        {
            var sites = await context.Sites.Where(x => x.IsActive == true).ToListAsync();

            return sites;
        }
    }
}
