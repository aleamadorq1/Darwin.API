using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{
    public interface IProjectCostsService
    {
        Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan = false);
        Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId,  bool? orphan = false);
        Task<ProjectCostDetailsDto> GetProjectCosts(int projectId);
        Task<bool> UpdateProjectCosts(ProjectCostDetailsDto projectCostDetailsDto);
    }

    public class ProjectCostsService : IProjectCostsService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectMaterial> _projectMaterialsRepository;
        private readonly IRepository<ProjectLabor> _projectLaborRepository;
        private readonly IRepository<ProjectModule> _projectModuleRepository;
        private readonly IRepository<ProjectModuleComposite> _projectModuleCompositeRepository;
        private readonly IProjectService _projectService;
        private readonly IRepository<Models.System> _systemRepository;
        private readonly GoogleMapsService _googleMapsService;

        public ProjectCostsService(
            IRepository<ProjectMaterial> projectMaterialsRepository, 
            IRepository<ProjectLabor> projectLaborRepository, 
            IRepository<ProjectModule> projectModuleRepository, 
            IRepository<ProjectModuleComposite> projectModuleCompositeRepository, 
            IRepository<Models.System> systemRepository,
            IProjectService projectService, 
            GoogleMapsService googleMapsService,
            IRepository<Project> projectRepository)
        {
            _projectMaterialsRepository = projectMaterialsRepository;
            _projectLaborRepository = projectLaborRepository;
            _projectModuleRepository = projectModuleRepository;
            _projectModuleCompositeRepository = projectModuleCompositeRepository;
            _systemRepository = systemRepository;
            _projectService = projectService;
            _googleMapsService = googleMapsService;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan = false)
        {
            IEnumerable<ProjectMaterial> projectMaterials;
            if (orphan == true)
            {
                projectMaterials = await _projectMaterialsRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId == null, 
                    query => query.Include(pm => pm.Material));
            }
            else
            {
                projectMaterials = await _projectMaterialsRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId != null, 
                    query => query.Include(pm => pm.Material));
            }
            return projectMaterials;
        }

        public async Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId, bool? orphan = false)
        {
            IEnumerable<ProjectLabor> projectLabors;
            if (orphan == true)
            {
                projectLabors = await _projectLaborRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId == null, 
                    query=> query.Include( pm => pm.Labor));
            }
            else
            {
                projectLabors = await _projectLaborRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId != null, 
                    query=>query.Include( pm => pm.Labor));
            }
            return projectLabors;
        }

        public async Task<IList<ProjectModuleDto>> GetProjectModulesByProjectId(int projectId)
        {
            var projectMaterials = await _projectModuleRepository.FindAsync(
                                                                            pm => pm.ProjectId == projectId,
                                                                            query => query.Include(pm => pm.Module)
                                                                                        .ThenInclude(m => m.ModulesMaterials)
                                                                                        .ThenInclude(mm => mm.Material),
                                                                            query => query.Include(pm => pm.Module)
                                                                                        .ThenInclude(m => m.ModulesLabors)
                                                                                        .ThenInclude(ml => ml.Labor)
);

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
                ModuleMaterials = pm.Module.ModulesMaterials.Select(m => new ProjectMaterialDto
                {
                    MaterialId = m.MaterialId,
                    MaterialName = m.Material.MaterialName,
                    Quantity = (int)m.Quantity,
                    UnitPrice = m.Material.UnitPrice,
                    TaxStatus = m.Material.TaxStatus,
                    CifPrice = m.Material.CifPrice,
                    ProjectMaterialId = m.Material.ProjectMaterials.FirstOrDefault(pm => pm.ModuleId == pm.ModuleId)?.ProjectMaterialId ?? 0
                }).ToList(),
                ModuleLabors = pm.Module.ModulesLabors.Select(l => new ProjectLaborDto
                {
                    LaborId = l.LaborId,
                    ProjectLaborId = l.Labor.ProjectLabors.FirstOrDefault(pl => pl.ModuleId == pm.ModuleId)?.ProjectLaborId ?? 0,
                    LaborType = l.Labor.LaborType,
                    Quantity = l.HoursRequired,
                    HourlyRate = l.Labor.HourlyRate
                }).ToList()
            }).ToList();
        }
        public async Task<IEnumerable<ProjectModuleCompositesDto>> GetProjectModuleCompositesByProjectId(int projectId)
        {
            var projectModuleComposites = await _projectModuleCompositeRepository.FindAsync(
                pmc => pmc.ProjectId == projectId,
                query => query.Include(pmc => pmc.ModuleComposite.ModuleCompositeDetails)
                              .ThenInclude(pmc=>pmc.Module)
                              .ThenInclude(pmc=>pmc.ProjectMaterials)
                              .Include(pmc=>pmc.ModuleComposite.ModuleCompositeDetails)
                              .ThenInclude(pmc=>pmc.Module)
                              .ThenInclude(pmc=>pmc.ProjectLabors));

            var systems = await _systemRepository.GetAllAsync();
            var projectMaterials = await GetProjectMaterialByProjectId(projectId);
            var projectLabors = await GetProjectLaborByProjectId(projectId);
            //var projectModules = await GetProjectModulesByProjectId(projectId);

            var compositeDetails = projectModuleComposites.Select(pmc => new ProjectModuleCompositesDto
            {
                ProjectModuleCompositeId = pmc.ProjectModuleCompositeId,
                ProjectId = pmc.ProjectId,
                ModuleCompositeId = pmc.ModuleCompositeId,
                CompositeName = pmc.ModuleComposite.CompositeName,
                Quantity = pmc.Quantity,
                CompositeDetails = pmc.ModuleComposite.ModuleCompositeDetails.Select(mcd => new ModuleCompositeDetailDto
                {

                    ModuleCompositeDetailId = mcd.ModuleCompositeDetailId,
                    ModuleId = mcd.ModuleId,
                    ModuleName = mcd.Module?.ModuleName ?? "No Module",
                    Quantity = mcd.Quantity,
                    Module = mcd.Module != null ? new ProjectModuleDto
                    {
                        ModuleId = mcd.Module.ModuleId,
                        ModuleName = mcd.Module.ModuleName,
                        Description = mcd.Module.Description,
                        SystemName = systems.FirstOrDefault(s => s.SystemId == mcd.Module.SystemId)?.Description,
                        Quantity = (int)mcd.Quantity,
                        Total = projectMaterials.Where(p => p.ModuleId == mcd.ModuleId).Sum(p => p.Quantity * p.UnitPrice)
                              + projectLabors.Where(p => p.ModuleId == mcd.ModuleId).Sum(p => p.Quantity * p.HourlyRate),
                        ModuleMaterials = projectMaterials.Where(p => p.ModuleId == mcd.ModuleId).Select(p => new ProjectMaterialDto
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
                        ModuleLabors = projectLabors.Where(p => p.ModuleId == mcd.ModuleId).Select(p => new ProjectLaborDto
                        {
                            ProjectLaborId = p.ProjectLaborId,
                            ModuleId = p.ModuleId,
                            LaborId = p.LaborId,
                            LaborType = p.Labor.LaborType,
                            Quantity = p.Quantity,
                            HourlyRate = p.HourlyRate
                        }).ToList()
                    } : null
                }).ToList()});
            return compositeDetails;
        }

        public async Task<ProjectCostDetailsDto> GetProjectCosts(int projectId)
        {
            var query = await _projectRepository.FindAsync(p=>p.ProjectId == projectId, query => query.Include(p => p.DistributionCenter));
            var project = query.FirstOrDefault();
            var projectModules = await _projectModuleRepository.FindAsync(pm => pm.ProjectId == projectId, query => query.Include( pm => pm.Module));
            var projectMaterials = await GetProjectMaterialByProjectId(projectId);
            var projectLabors = await GetProjectLaborByProjectId(projectId);
            var projectModuleComposites = await GetProjectModuleCompositesByProjectId(projectId);

            var orphanMaterials = await GetProjectMaterialByProjectId(projectId, true);
            var orphanLabors = await GetProjectLaborByProjectId(projectId, true);
            var systems = await _systemRepository.GetAllAsync();

            var modules = projectModules.Select(pm => new ProjectModuleDto
            {
                ModuleId = pm.ModuleId,
                ProjectModuleId = pm.ProjectModuleId,
                ProjectId = pm.ProjectId,
                ModuleName = pm.Module.ModuleName,
                SystemName = systems.FirstOrDefault(s => s.SystemId == pm.Module.SystemId)?.Description,
                Description = pm.Module.Description,
                Quantity = projectModules.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity),
                Total = projectMaterials.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity * p.UnitPrice) 
                      + projectLabors.Where(p => p.ModuleId == pm.ModuleId).Sum(p => p.Quantity * p.HourlyRate),
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
                    ProjectLaborId = p.ProjectLaborId,
                    ModuleId = p.ModuleId,
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
                Quantity = 1,
                Total = orphanMaterials.Sum(pm => pm.Quantity * pm.UnitPrice),
                ModuleMaterials = orphanMaterials.Select(pm => new ProjectMaterialDto
                {
                    ProjectMaterialId = pm.ProjectMaterialId,
                    MaterialId = pm.MaterialId,
                    MaterialName = pm.Material.MaterialName,
                    Quantity = pm.Quantity,
                    UnitPrice = pm.UnitPrice,
                    TaxStatus = pm.TaxStatus,
                    CifPrice = pm.CifPrice
                }).ToList(),
                ModuleLabors = orphanLabors.Select(pl => new ProjectLaborDto
                {
                    ProjectLaborId = pl.ProjectLaborId,
                    LaborId = pl.LaborId,
                    LaborType = pl.Labor.LaborType,
                    Quantity = pl.Quantity,
                    HourlyRate = pl.HourlyRate
                }).ToList()
            };

            modules.Add(orphanModule);

            var totalCost = modules.Sum(m => m.Total);

            return new ProjectCostDetailsDto
            {
                ProjectId = projectId,
                TotalCost = (decimal)totalCost,
                ProfitMargin = (await _projectService.GetProjectById(projectId)).ProfitMargin,
                Distance = await _googleMapsService.GetDrivingDistanceAsync(project.LocationCoordinates, project.DistributionCenter.LocationCoordinates),
                Modules = modules,
                ModulesComposite = projectModuleComposites.ToList(),
                ParentLessCosts = orphanModule
            };
        }

        public async Task<bool> UpdateProjectCosts(ProjectCostDetailsDto projectCostDetailsDto)
        {
            try
            {
                var project = await _projectService.GetProjectById(projectCostDetailsDto.ProjectId);
                if (project == null)
                {
                    return false;
                }
                else
                {
                    project.ProfitMargin = projectCostDetailsDto.ProfitMargin;
                    await _projectService.UpdateProject(project);
                }

            // Update module materials and labor prices/rates
                foreach (var module in projectCostDetailsDto.Modules)
                {
                    // Update materials
                    foreach (var materialDto in module.ModuleMaterials ?? new List<ProjectMaterialDto>())
                    {
                        var projectMaterial = await _projectMaterialsRepository.GetByIdAsync(materialDto.ProjectMaterialId);
                        if (projectMaterial != null)
                        {
                            projectMaterial.UnitPrice = materialDto.UnitPrice??projectMaterial.UnitPrice;
                            await _projectMaterialsRepository.UpdateAsync(projectMaterial);
                        }
                    }

                    // Update labor
                    foreach (var laborDto in module.ModuleLabors ?? new List<ProjectLaborDto>())
                    {
                        var projectLabor = await _projectLaborRepository.GetByIdAsync(laborDto.ProjectLaborId);
                        if (projectLabor != null)
                        {
                            projectLabor.HourlyRate = laborDto.HourlyRate??projectLabor.HourlyRate;
                            await _projectLaborRepository.UpdateAsync(projectLabor);
                        }
                    }
                }

                // Update composite modules
                foreach (var composite in projectCostDetailsDto.ModulesComposite)
                {
                    foreach (var detail in composite.CompositeDetails ?? new List<ModuleCompositeDetailDto>())
                    {
                        if (detail.Module != null)
                        {
                            // Update materials
                            foreach (var materialDto in detail.Module.ModuleMaterials ?? new List<ProjectMaterialDto>())
                            {
                                var projectMaterial = await _projectMaterialsRepository.GetByIdAsync(materialDto.ProjectMaterialId);
                                if (projectMaterial != null)
                                {
                                    projectMaterial.UnitPrice = materialDto.UnitPrice ?? projectMaterial.UnitPrice;
                                    await _projectMaterialsRepository.UpdateAsync(projectMaterial);
                                }
                            }

                            // Update labor
                            foreach (var laborDto in detail.Module.ModuleLabors ?? new List<ProjectLaborDto>())
                            {
                                var projectLabor = await _projectLaborRepository.GetByIdAsync(laborDto.ProjectLaborId);
                                if (projectLabor != null)
                                {
                                    projectLabor.HourlyRate = laborDto.HourlyRate ?? projectLabor.HourlyRate;
                                    await _projectLaborRepository.UpdateAsync(projectLabor);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception($"Error updating project costs: {ex.Message}", ex);
                return false;
            }
            return true; 
        }

    }
}