using Darwin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Repositories
{
    public interface IModuleMaterialsRepository : IRepository<ModulesMaterial>
    {
        Task<IEnumerable<ModulesMaterial>> GetByModuleIdAsync(int moduleId);
    }

    public class ModuleMaterialsRepository : Repository<ModulesMaterial>, IModuleMaterialsRepository
    {
        private readonly AlphaDbContext _context;

        public ModuleMaterialsRepository(AlphaDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ModulesMaterial>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.ModulesMaterials
                .Include(mm => mm.Module)
                .Include(mm => mm.Material)
                .Where(mm => mm.ModuleId == moduleId)
                .ToListAsync();
        }
    }
}

