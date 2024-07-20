using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{
    public interface IProjectCostsService
    {
        Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan = false);
        Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId, bool? orphan = false);
        Task<ProjectCostDetailsDto?> GetProjectCosts(int projectId);
        Task<bool> UpdateProjectCosts(ProjectCostDetailsDto projectCostDetailsDto);
    }

    public class ProjectCostsService : IProjectCostsService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectMaterial> _projectMaterialsRepository;
        private readonly IRepository<ProjectLabor> _projectLaborRepository;
        private readonly IRepository<ProjectModule> _projectModuleRepository;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IRepository<HandlingCost> _handlingCostRepository;
        private readonly IRepository<ProjectModuleComposite> _projectModuleCompositeRepository;
        private readonly IProjectService _projectService;
        private readonly IRepository<Models.System> _systemRepository;
        private readonly IRepository<ProjectAllowance> _projectAllowanceRepository;
        private readonly GoogleMapsService _googleMapsService;

        public ProjectCostsService(
            IRepository<ProjectMaterial> projectMaterialsRepository,
            IRepository<ProjectLabor> projectLaborRepository,
            IRepository<ProjectModule> projectModuleRepository,
            IRepository<ProjectModuleComposite> projectModuleCompositeRepository,
            IRepository<Models.System> systemRepository,
            IProjectService projectService,
            GoogleMapsService googleMapsService,
            IRepository<Project> projectRepository,
            IRepository<ProjectAllowance> projectAllowanceRepository,
            IRepository<TaxRate> taxRateRepository,
            IRepository<HandlingCost> handlingCostRepository)
        {
            _projectMaterialsRepository = projectMaterialsRepository;
            _projectLaborRepository = projectLaborRepository;
            _projectModuleRepository = projectModuleRepository;
            _projectModuleCompositeRepository = projectModuleCompositeRepository;
            _systemRepository = systemRepository;
            _projectService = projectService;
            _googleMapsService = googleMapsService;
            _projectRepository = projectRepository;
            _projectAllowanceRepository = projectAllowanceRepository;
            _taxRateRepository = taxRateRepository;
            _handlingCostRepository = handlingCostRepository;
        }

        public async Task<IEnumerable<ProjectMaterial>> GetProjectMaterialByProjectId(int projectId, bool? orphan = false)
        {
            return orphan == true ?
                await _projectMaterialsRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId == null,
                    query => query.Include(pm => pm.Material).ThenInclude(m => m.TaxRate)
                        .Include(pm => pm.Material).ThenInclude(m => m.HandlingCost)) :
                await _projectMaterialsRepository.FindAsync(
                    pm => pm.ProjectId == projectId && pm.ModuleId != null,
                    query => query.Include(pm => pm.Material).ThenInclude(m => m.TaxRate)
                        .Include(pm => pm.Material).ThenInclude(m => m.HandlingCost));
        }

        public async Task<IEnumerable<ProjectLabor>> GetProjectLaborByProjectId(int projectId, bool? orphan = false)
        {
            return orphan == true ?
                await _projectLaborRepository.FindAsync(
                    pl => pl.ProjectId == projectId && pl.ModuleId == null,
                    query => query.Include(pl => pl.Labor),
                    query => query.Include(pl => pl.ProjectAllowance)) :
                await _projectLaborRepository.FindAsync(
                    pl => pl.ProjectId == projectId && pl.ModuleId != null,
                    query => query.Include(pl => pl.Labor),
                    query => query.Include(pl => pl.ProjectAllowance));
        }

        public async Task<IList<ProjectModuleDto>> GetProjectModulesByProjectId(int projectId)
        {
            var taxRates = await _taxRateRepository.GetAllAsync();
            var handlingCosts = await _handlingCostRepository.GetAllAsync();

            var projectModules = await _projectModuleRepository.FindAsync(
                pm => pm.ProjectId == projectId,
                query => query.Include(pm => pm.Module)
                              .ThenInclude(m => m.ModulesMaterials)
                              .ThenInclude(mm => mm.Material),
                query => query.Include(pm => pm.Module)
                              .ThenInclude(m => m.ModulesLabors)
                              .ThenInclude(ml => ml.Labor));

            var systems = await _systemRepository.GetAllAsync();
            return projectModules.Select(pm => new ProjectModuleDto
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
                    TaxRate = taxRates.FirstOrDefault(t => t.TaxRateId == m.Material.TaxRateId)?.Rate * 100,
                    HandlingCost = handlingCosts.FirstOrDefault(h => h.HandlingCostId == m.Material.HandlingCostId)?.Cost,
                    CifPrice = m.Material.CifPrice,
                    ProjectMaterialId = m.Material.ProjectMaterials.FirstOrDefault(pm => pm.ModuleId == pm.ModuleId)?.ProjectMaterialId ?? 0
                }).ToList(),
                ModuleLabors = pm.Module.ModulesLabors.Select(l => new ProjectLaborDto
                {
                    LaborId = l.LaborId,
                    ProjectLaborId = l.Labor.ProjectLabors.FirstOrDefault(pl => pl.ModuleId == pm.ModuleId)?.ProjectLaborId ?? 0,
                    LaborType = l.Labor.LaborType,
                    Quantity = l.Quantity,
                    HourlyRate = l.Labor.HourlyRate,
                    HoursRequired = l.HoursRequired,
                    AllowanceAmount = l.Labor.ProjectLabors.FirstOrDefault(pl => pl.ModuleId == pm.ModuleId)?.ProjectAllowance?.Amount ?? 0,
                    AllowanceQuantity = l.Labor.ProjectLabors.FirstOrDefault(pl => pl.ModuleId == pm.ModuleId)?.ProjectAllowance?.Quantity ?? 0
                }).ToList()
            }).ToList();
        }

        public async Task<IEnumerable<ProjectModuleCompositesDto>> GetProjectModuleCompositesByProjectId(int projectId)
        {
            var taxRates = await _taxRateRepository.GetAllAsync();
            var handlingCosts = await _handlingCostRepository.GetAllAsync();

            var projectModuleComposites = await _projectModuleCompositeRepository.FindAsync(
                pmc => pmc.ProjectId == projectId,
                query => query.Include(pmc => pmc.ModuleComposite.ModuleCompositeDetails)
                              .ThenInclude(pmc => pmc.Module)
                              .ThenInclude(pmc => pmc.ProjectMaterials)
                              .Include(pmc => pmc.ModuleComposite.ModuleCompositeDetails)
                              .ThenInclude(pmc => pmc.Module)
                              .ThenInclude(pmc => pmc.ProjectLabors)
                              .ThenInclude(pmc => pmc.ProjectAllowance));

            var systems = await _systemRepository.GetAllAsync();
            var projectMaterials = await GetProjectMaterialByProjectId(projectId);
            var projectLabors = await GetProjectLaborByProjectId(projectId);

            return projectModuleComposites.Select(pmc => new ProjectModuleCompositesDto
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
                            TaxRate = p.TaxRate * 100,
                            HandlingCost = p.HandlingCost,
                            CifPrice = p.CifPrice
                        }).ToList(),
                        ModuleLabors = projectLabors.Where(p => p.ModuleId == mcd.ModuleId).Select(p => new ProjectLaborDto
                        {
                            ProjectLaborId = p.ProjectLaborId,
                            ModuleId = p.ModuleId,
                            LaborId = p.LaborId,
                            LaborType = p.Labor.LaborType,
                            Quantity = p.Quantity,
                            HourlyRate = p.HourlyRate,
                            HoursRequired = p.HoursRequired,
                            AllowanceAmount = p.ProjectAllowance?.Amount ?? 0,
                            AllowanceQuantity = p.ProjectAllowance?.Quantity ?? 0
                        }).ToList()
                    } : null
                }).ToList()
            }).ToList();
        }

        public async Task<ProjectCostDetailsDto?> GetProjectCosts(int projectId)
        {
            var query = await _projectRepository.FindAsync(p => p.ProjectId == projectId, query => query.Include(p => p.DistributionCenter));
            var project = query.FirstOrDefault();
            if (project == null) return null;

            var drivingDistance = await _googleMapsService.GetDrivingDistanceAsync(project.LocationCoordinates, project.DistributionCenter.LocationCoordinates);

            var projectModules = await _projectModuleRepository.FindAsync(pm => pm.ProjectId == projectId, query => query.Include(pm => pm.Module));
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
                    TaxRate = p.TaxRate * 100,
                    HandlingCost = p.HandlingCost,
                    CifPrice = p.CifPrice
                }).ToList(),
                ModuleLabors = projectLabors.Where(p => p.ModuleId == pm.ModuleId).Select(p => new ProjectLaborDto
                {
                    ProjectLaborId = p.ProjectLaborId,
                    ModuleId = p.ModuleId,
                    LaborId = p.LaborId,
                    LaborType = p.Labor.LaborType,
                    Quantity = p.Quantity,
                    HourlyRate = p.HourlyRate,
                    HoursRequired = p.HoursRequired,
                    AllowanceAmount = p.ProjectAllowance?.Amount ?? 0,
                    AllowanceQuantity = p.ProjectAllowance?.Quantity ?? 0
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
                    TaxRate = pm.Material.TaxRate.Rate * 100,
                    CifPrice = pm.CifPrice
                }).ToList(),
                ModuleLabors = orphanLabors.Select(pl => new ProjectLaborDto
                {
                    ProjectLaborId = pl.ProjectLaborId,
                    LaborId = pl.LaborId,
                    LaborType = pl.Labor.LaborType,
                    Quantity = pl.Quantity,
                    HourlyRate = pl.HourlyRate,
                    HoursRequired = pl.HoursRequired,
                    AllowanceAmount = pl.ProjectAllowance?.Amount ?? 0,
                    AllowanceQuantity = pl.ProjectAllowance?.Quantity ?? 0
                }).ToList()
            };

            if (orphanModule.ModuleMaterials.Count > 0 && orphanModule.ModuleLabors.Count > 0)
            {
                modules.Add(orphanModule);
            }

            var totalCost = modules.Sum(m => m.Total);

            return new ProjectCostDetailsDto
            {
                ProjectId = projectId,
                TotalCost = (decimal)(totalCost??0),
                ProfitMargin = (await _projectService.GetProjectById(projectId)).ProfitMargin,
                Distance = drivingDistance,
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

                project.ProfitMargin = projectCostDetailsDto.ProfitMargin;
                await _projectService.UpdateProject(project);

                foreach (var module in projectCostDetailsDto.Modules ?? new List<ProjectModuleDto>())
                {
                    await UpdateModuleMaterialsAsync(module.ModuleMaterials ?? new List<ProjectMaterialDto>());
                    await UpdateModuleLaborsAsync(module.ModuleLabors ?? new List<ProjectLaborDto>());
                }

                foreach (var composite in projectCostDetailsDto.ModulesComposite ?? new List<ProjectModuleCompositesDto>())
                {
                    foreach (var detail in composite.CompositeDetails ?? new List<ModuleCompositeDetailDto>())
                    {
                        if (detail.Module != null)
                        {
                            await UpdateModuleMaterialsAsync(detail.Module.ModuleMaterials ?? new List<ProjectMaterialDto>());
                            await UpdateModuleLaborsAsync(detail.Module.ModuleLabors ?? new List<ProjectLaborDto>());
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception($"Error updating project costs: {ex.Message}", ex);
            }
        }

        private async Task UpdateModuleMaterialsAsync(IEnumerable<ProjectMaterialDto> materials)
        {
            foreach (var materialDto in materials ?? new List<ProjectMaterialDto>())
            {
                var projectMaterial = await _projectMaterialsRepository.GetByIdAsync(materialDto.ProjectMaterialId);
                if (projectMaterial != null)
                {
                    projectMaterial.UnitPrice = materialDto.UnitPrice ?? projectMaterial.UnitPrice;
                    await _projectMaterialsRepository.UpdateAsync(projectMaterial);
                }
            }
        }

        private async Task UpdateModuleLaborsAsync(IEnumerable<ProjectLaborDto> labors)
        {
            foreach (var laborDto in labors ?? new List<ProjectLaborDto>())
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
