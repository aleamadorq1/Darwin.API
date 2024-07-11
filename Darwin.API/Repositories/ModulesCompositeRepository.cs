using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darwin.API.Repositories
{
    public interface IModulesCompositeRepository : IRepository<ModulesComposite>
    {
        Task<IEnumerable<ModulesComposite>> GetAllWithDetailsAsync();
        Task<ModulesComposite> GetWithDetailsByIdAsync(int id);
        Task<IEnumerable<ModuleCompositeDetail>> GetByModuleCompositeIdAsync(int moduleCompositeId);
    }

    public class ModulesCompositeRepository : Repository<ModulesComposite>, IModulesCompositeRepository
    {
        private readonly AlphaDbContext _context;

        public ModulesCompositeRepository(AlphaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModulesComposite>> GetAllWithDetailsAsync()
        {
            return await _context.ModulesComposites
                .Include(mc => mc.ModuleCompositeDetails)
                    /*.ThenInclude(mcd => mcd.Module)*/
                .ToListAsync();
        }

        public async Task<ModulesComposite> GetWithDetailsByIdAsync(int id)
        {
            return await _context.ModulesComposites
                .Include(mc => mc.ModuleCompositeDetails)
                    .ThenInclude(mcd => mcd.Module)
                .FirstOrDefaultAsync(mc => mc.ModuleCompositeId == id) ?? new ModulesComposite();
        }

        public async Task<IEnumerable<ModuleCompositeDetail>> GetByModuleCompositeIdAsync(int moduleCompositeId)
        {
            return await _context.ModuleCompositeDetails
                .Include(mm => mm.Module)
                .Where(mm => mm.ModuleCompositeId == moduleCompositeId)
                .ToListAsync();
        }
    }
}

