using System;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Repositories
{
    public interface IModulesLaborRepository : IRepository<ModulesLabor>
    {
        Task<IEnumerable<ModulesLabor>> GetByModuleIdAsync(int moduleId);
    }

    public class ModulesLaborRepository : Repository<ModulesLabor>, IModulesLaborRepository
    {
        private readonly AlphaDbContext _context;

        public ModulesLaborRepository(AlphaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModulesLabor>> GetByModuleIdAsync(int moduleId)
        {
            return await _context.ModulesLabors
                .Include(ml => ml.Module)
                .Include(ml => ml.Labor)
                .Where(ml => ml.ModuleId == moduleId)
                .ToListAsync();
        }
    }
}

