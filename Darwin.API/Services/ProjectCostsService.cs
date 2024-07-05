using System.Runtime.InteropServices;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{
    public interface IProjectCostsService
    {
        Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan = false);
        Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId,  bool? orphan = false);
        //Task<ProjectModuleDto> GetOrphanProjectMaterials(int projectId,  bool? ophan = false);

        Task<IEnumerable<ProjectModuleDto>> GetProjectModules(int projectId);
    }

    public class ProjectCostsService : IProjectCostsService
    {
        private readonly IRepository<ProjectMaterial> _projectMaterialsRepository;
        private readonly IRepository<ProjectLabor> _projectLaborRepository;
        private readonly IRepository<ProjectModule> _projectModuleRepository;
        private readonly IRepository<ProjectModuleComposite> _projectModuleCompositeRepository;

        private readonly IRepository<Models.System> _systemRepository;

        public ProjectCostsService(IRepository<ProjectMaterial> projectMaterialsRepository, IRepository<ProjectLabor> projectLaborRepository, IRepository<ProjectModule> projectModuleRepository, IRepository<ProjectModuleComposite> projectModuleCompositeRepository, IRepository<Models.System> systemRepository)
        {
            _projectMaterialsRepository = projectMaterialsRepository;
            _projectLaborRepository = projectLaborRepository;
            _projectModuleRepository = projectModuleRepository;
            _projectModuleCompositeRepository = projectModuleCompositeRepository;
            _systemRepository = systemRepository;
        }

        public async Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan =false)
        {
            IEnumerable<ProjectMaterial> projectMaterials = new List<ProjectMaterial>();
            if (orphan == true)
            {
                projectMaterials = await _projectMaterialsRepository.FindAsync(
                                                pm => pm.ProjectId == projectId && pm.ModuleId == null, 
                                                includes: pm => pm.Material);

            }
            else
            {
                projectMaterials = await _projectMaterialsRepository.FindAsync(pm => pm.ProjectId == projectId && pm.ModuleId != null, includes: pm => pm.Material);
            }
            
            
            return projectMaterials;
        }

        public async Task<IEnumerable<ProjectModuleDto>> GetProjectModulesByProjectId(int projectId)
        {
            var projectMaterials = await _projectModuleRepository.FindAsync(pm => pm.ProjectId == projectId , pm => pm.Module, pm => pm.Module.ModulesMaterials, pm => pm.Module.ModulesMaterials.Select(m => m.Material));
            var systems = await _systemRepository.GetAllAsync();
            return projectMaterials.Select(pm => new ProjectModuleDto
            {
                ProjectId = pm.ProjectId,
                ModuleId = pm.ModuleId,
                ProjectModuleId = pm.ProjectModuleId,
                ModuleName = pm.Module.ModuleName,
                SystemName = systems.FirstOrDefault(s => s.SystemId == pm.Module.SystemId)?.Description,
                Description = pm.Module.Description,
                Quantity = pm.Quantity,
                Total = pm.Module.ModulesMaterials.Sum(m => m.Quantity * m.Material.UnitPrice),
            }).ToList();
        }

        public async Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId, bool? orphan=false)
        {
            IEnumerable<ProjectLabor> projectLabors = new List<ProjectLabor>();

            if (orphan == true)
            {
                projectLabors = await _projectLaborRepository.FindAsync(pm => pm.ProjectId == projectId && pm.ModuleId == null, includes: pm => pm.Labor);
            }
            else
            {
                projectLabors = await _projectLaborRepository.FindAsync(pm => pm.ProjectId == projectId &&  pm.ModuleId != null, includes: pm => pm.Labor);
            }

            return projectLabors;
        }

        public async Task<IEnumerable<ProjectModuleDto>> GetProjectModules(int projectId)
        {
            var projectModules = await _projectModuleRepository.FindAsync(pm => pm.ProjectId == projectId, includes: pm => pm.Module);
            var projectMaterials = await GetProjectMaterialByProjectId(projectId);
            var projectLabors = await GetProjectLaborByProjectId(projectId);

            var orphanMaterials = await GetProjectMaterialByProjectId(projectId, true);
            var orphanLabors = await GetProjectLaborByProjectId(projectId, true);

            var systems = await _systemRepository.GetAllAsync();

            var result = projectModules.Select(pm => new ProjectModuleDto
            {
                ModuleId = pm.ModuleId,
                ProjectModuleId = pm.ProjectModuleId,
                ProjectId = pm.ProjectId,
                ModuleName = pm.Module.ModuleName,
                SystemName = systems.FirstOrDefault(s => s.SystemId == pm.Module.SystemId)?.Description,
                Description = pm.Module.Description,
                Quantity = projectModules.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity),
                Total = projectMaterials.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity * p.UnitPrice) + projectLabors.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity * p.HourlyRate),
                ModuleMaterials = projectMaterials.Where(p => p.ModuleId == pm.ModuleId).Select(p => new ProjectMaterialDto
                {
                    ModuleId = p.ModuleId,
                    ProjectMaterialId = p.ProjectMaterialId,
                    LastModified = p.LastModified,
                    MaterialId = p.MaterialId,
                    MaterialName = p.Material.MaterialName,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    TaxStatus = p.TaxStatus,
                    CifPrice = p.CifPrice
                }).ToList(),
                ModuleLabors = projectLabors.Where(p => p.ModuleId == pm.ModuleId).Select(p => new ProjectLaborDto
                {
                    ModuleId = p.ModuleId,
                    //ModuleName = p.Module.ModuleName,
                    LaborId = p.LaborId,
                    LaborType = p.Labor.LaborType,
                    Quantity = p.Quantity,
                    HourlyRate = p.HourlyRate
                }).ToList()
            }).ToList();

            var orphanModule = new ProjectModuleDto
            {
                ProjectId = projectId,
                ModuleName = "Costs without module",
                
                Description = "Materials that are not assigned to any module",
                SystemName = "No System",
                Quantity = orphanMaterials.Sum(pm => pm.Quantity),
                Total = orphanMaterials.Sum(pm => pm.Quantity * pm.UnitPrice),
                ModuleMaterials = orphanMaterials.Select(pm => new ProjectMaterialDto
                {
                    MaterialId = pm.MaterialId,
                    MaterialName = pm.Material.MaterialName,
                    Quantity = pm.Quantity,
                    UnitPrice = pm.UnitPrice,
                    TaxStatus = pm.TaxStatus,
                    CifPrice = pm.CifPrice
                }).ToList(),
                ModuleLabors = orphanLabors.Select(pl => new ProjectLaborDto
                {
                    LaborId = pl.LaborId,
                    LaborType = pl.Labor.LaborType,
                    Quantity = pl.Quantity,
                    HourlyRate = pl.HourlyRate
                }).ToList()
            };

            result.Add(orphanModule);

            return result;
        }


    }
}

