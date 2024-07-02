using System;
using Alpha.API.Models;
using Alpha.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Alpha.API.Repositories
{
    public interface IProjectMaterialsRepository : IRepository<ProjectMaterial>
    {
        Task<IEnumerable<ProjectMaterial>> GetByProjectId(int projectId);
    }

    public class ProjectMaterialsRepository : Repository<ProjectMaterial>, IProjectMaterialsRepository
    {
        private readonly AlphaDbContext _context;

        public ProjectMaterialsRepository(AlphaDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<ProjectMaterial>> GetByProjectId(int projectId)
        {
            return await _context.ProjectMaterials
                .Where(pm => pm.ProjectId == projectId)
                .ToListAsync();
        }
    }
}

