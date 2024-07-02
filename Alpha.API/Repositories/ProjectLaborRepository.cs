using System;
using Alpha.API.Models;
using Alpha.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Alpha.API.Repositories
{
    public interface IProjectLaborRepository : IRepository<ProjectLabor>
    {
        Task<IEnumerable<ProjectLabor>> GetByProjectId(int projectId);
    }

    public class ProjectLaborRepository : Repository<ProjectLabor>, IProjectLaborRepository
    {
        private readonly AlphaDbContext _context;

        public ProjectLaborRepository(AlphaDbContext context, IServiceProvider serviceProvider) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectLabor>> GetByProjectId(int projectId)
        {
            return await _context.ProjectLabors
                .Where(pl => pl.ProjectId == projectId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

