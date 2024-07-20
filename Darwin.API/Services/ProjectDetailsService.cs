using System.Data;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Darwin.API.Services
{
    public interface IProjectDetailsService
    {
        Task<ProjectDetailsDto> GetProjectDetails(int projectId);
        Task<bool> UpsertProjectDetails(ProjectDetailsDto projectDetails, int projectId);
    }

    public class ProjectDetailService : IProjectDetailsService
    {
        private readonly IProjectMaterialsRepository _projectMaterialRepository;
        private readonly IProjectLaborRepository _projectLaborRepository;

        private readonly IRepository<ProjectAllowance> _projectAllowanceRepository;
        private readonly IRepository<ProjectModule> _projectModuleRepository;
        private readonly IRepository<ProjectModuleComposite> _projectModuleComposite;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Module> _moduleRepository;

        private readonly IRepository<ModulesComposite> _moduleCompositeRepository;
        private readonly IRepository<Material> _materialRepository;
        private readonly IRepository<Labor> _laborRepository;
        private readonly GoogleMapsService _googleMapsService;

        public ProjectDetailService(IProjectMaterialsRepository projectMaterialRepository, IProjectLaborRepository projectLaborRepository, 
                                    IRepository<ProjectModule> projectModuleRepository, IRepository<ProjectModuleComposite> projectModuleComposite, 
                                    IRepository<Project> projectRepository,IRepository<Material> materialRepository, 
                                    IRepository<Labor> laborRepository, IRepository<Module> moduleRepository, IRepository<ModulesComposite> moduleCompositeRepository, 
                                    IRepository<ProjectAllowance> projectAllowanceRepository, GoogleMapsService googleMapsService)
        {
            _projectLaborRepository = projectLaborRepository;
            _projectMaterialRepository = projectMaterialRepository;
            _projectModuleRepository = projectModuleRepository;
            _projectModuleComposite = projectModuleComposite;
            _projectRepository = projectRepository;
            _moduleRepository = moduleRepository;
            _materialRepository = materialRepository;
            _laborRepository = laborRepository;
            _moduleCompositeRepository = moduleCompositeRepository;
            _projectAllowanceRepository = projectAllowanceRepository;
            _googleMapsService = googleMapsService;
        }
        private async Task<Project?> GetProjectWithDetailsAsync(int projectId)
        {
            var projectQuery = await _projectRepository.FindAsync(p => p.ProjectId == projectId, query => query.Include(p => p.DistributionCenter));
            return projectQuery.FirstOrDefault();
        }

        public async Task<bool> UpsertProjectDetails(ProjectDetailsDto projectDetails, int projectId)
        {
            var project = await GetProjectWithDetailsAsync(projectId);
            if (project == null) return false;

            var drivingDistance = await _googleMapsService.GetDrivingDistanceAsync(project.LocationCoordinates, project.DistributionCenter.LocationCoordinates);
            
            var newMaterials = projectDetails.ProjectMaterials ?? new List<ProjectMaterialDto>();
            var newLabor = projectDetails.ProjectLabor ?? new List<ProjectLaborDto>();
            var newModules = projectDetails.ProjectModules ?? new List<ProjectModuleDto>();
            var newModulesComposite = projectDetails.ProjectModuleComposites ?? new List<ProjectModuleCompositesDto>();

            await UpsertMaterialsAsync(newMaterials, projectId);
            await UpsertLaborAsync(newLabor, projectId, drivingDistance);
            await UpsertModulesAsync(newModules, projectId, drivingDistance);
            await UpsertCompositeModulesAsync(newModulesComposite, projectId, drivingDistance);

            return true;
        }

        private async Task UpsertMaterialsAsync(IEnumerable<ProjectMaterialDto> newMaterials, int projectId)
        {
            var existingMaterials = (await _projectMaterialRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == null)).ToList();
            
            foreach (var materialDto in newMaterials)
            {
                var existingMaterial = existingMaterials.FirstOrDefault(m => m.MaterialId == materialDto.MaterialId);
                if (existingMaterial != null)
                {
                    existingMaterial.Quantity = materialDto.Quantity;
                    existingMaterial.LastModified = DateTime.UtcNow;
                    await _projectMaterialRepository.UpdateAsync(existingMaterial);
                }
                else
                {
                    var query = await _materialRepository.FindAsync(m => m.MaterialId == materialDto.MaterialId, q => q.Include(m => m.TaxRate).Include(m => m.HandlingCost));
                    var material = query.FirstOrDefault();
                    var newMaterial = new ProjectMaterial
                    {
                        ProjectId = projectId,
                        MaterialId = materialDto.MaterialId,
                        Quantity = materialDto.Quantity,
                        LastModified = DateTime.UtcNow,
                        UnitPrice = material.UnitPrice,
                        TaxRate = material.TaxRate.Rate,
                        HandlingCost = material.HandlingCost.Cost,
                        CifPrice = material.CifPrice,
                        ModuleId = null
                    };
                    await _projectMaterialRepository.AddAsync(newMaterial);
                    existingMaterials.Add(newMaterial);
                }
            }

            var materialsToDelete = existingMaterials.Where(m => !newMaterials.Any(nm => nm.MaterialId == m.MaterialId)).ToList();
            foreach (var material in materialsToDelete)
            {
                await _projectMaterialRepository.DeleteAsync(material.ProjectMaterialId);
            }
        }

        private async Task UpsertLaborAsync(IEnumerable<ProjectLaborDto> newLabor, int projectId, double drivingDistance)
        {
            var existingLabor = (await _projectLaborRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == null)).ToList();

            foreach (var laborDto in newLabor)
            {
                var existingLaborItem = existingLabor.FirstOrDefault(l => l.LaborId == laborDto.LaborId);
                var labor = await _laborRepository.GetByIdAsync(laborDto.LaborId);

                if (existingLaborItem != null)
                {
                    var existingAllowanceItem = (await _projectAllowanceRepository.FindAsync(p => p.ProjectLaborId == existingLaborItem.ProjectLaborId)).FirstOrDefault();
                    existingLaborItem.Quantity = (int)laborDto.Quantity;
                    existingLaborItem.HoursRequired = laborDto.HoursRequired;
                    existingLaborItem.LastModified = DateTime.UtcNow;
                    if (existingAllowanceItem != null)
                    {
                        UpdateAllowance(existingAllowanceItem, labor, drivingDistance, laborDto.Quantity);
                        await _projectAllowanceRepository.UpdateAsync(existingAllowanceItem);
                    }
                    await _projectLaborRepository.UpdateAsync(existingLaborItem);
                }
                else
                {
                    var newLaborItem = new ProjectLabor
                    {
                        ProjectId = projectId,
                        LaborId = laborDto.LaborId,
                        Quantity = (int)laborDto.Quantity,
                        LastModified = DateTime.UtcNow,
                        HourlyRate = labor.HourlyRate,
                        ModuleId = null
                    };
                    await _projectLaborRepository.AddAsync(newLaborItem);

                    var newAllowance = CreateNewAllowance(newLaborItem, labor, drivingDistance, laborDto.Quantity);
                    await _projectAllowanceRepository.AddAsync(newAllowance);
                    existingLabor.Add(newLaborItem);
                }
            }

            var laborToDelete = existingLabor.Where(l => !newLabor.Any(nl => nl.LaborId == l.LaborId)).ToList();
            foreach (var labor in laborToDelete)
            {
                await _projectLaborRepository.DeleteAsync(labor.ProjectLaborId);
            }
        }

        private ProjectAllowance CreateNewAllowance(ProjectLabor laborItem, Labor labor, double drivingDistance, double quantity)
        {
            return new ProjectAllowance
            {
                ProjectLaborId = laborItem.ProjectLaborId,
                Amount = CalculateAllowanceAmount(labor, drivingDistance),
                Quantity = quantity / 8,
                LastModified = DateTime.UtcNow,
            };
        }

        private void UpdateAllowance(ProjectAllowance allowance, Labor labor, double drivingDistance, double quantity)
        {
            allowance.Amount = CalculateAllowanceAmount(labor, drivingDistance);
            allowance.LastModified = DateTime.UtcNow;
            allowance.Quantity = quantity / 8;
        }

        private double CalculateAllowanceAmount(Labor labor, double drivingDistance)
        {
            if (drivingDistance > 300) return labor.MinAllowance * 3;
            if (drivingDistance > 200) return labor.MinAllowance * 2;
            if (drivingDistance > 100) return labor.MinAllowance * 1.5;
            if (drivingDistance > 50) return labor.MinAllowance * 1.25;
            if (drivingDistance > 25) return labor.MinAllowance;
            return 0;
        }

        private async Task UpsertModulesAsync(IEnumerable<ProjectModuleDto> newModules, int projectId, double drivingDistance)
        {
            var existingModules = (await _projectModuleRepository.FindAsync(p => p.ProjectId == projectId, query => query.Include(p => p.Module).ThenInclude(m => m.ModulesMaterials).Include(p => p.Module).ThenInclude(m => m.ModulesLabors))).ToList();

            foreach (var moduleDto in newModules)
            {
                var existingModule = existingModules.FirstOrDefault(m => m.ModuleId == moduleDto.ModuleId && m.ModuleId != null);
                if (existingModule != null)
                {
                    existingModule.Quantity = moduleDto.Quantity;
                    await _projectModuleRepository.UpdateAsync(existingModule);
                }
                else
                {
                    var newModule = new ProjectModule
                    {
                        ProjectId = projectId,
                        ModuleId = moduleDto.ModuleId ?? throw new DataException("ModuleId is required"),
                        Quantity = moduleDto.Quantity
                    };
                    await _projectModuleRepository.AddAsync(newModule);
                    existingModules.Add(newModule);
                }

                var module = existingModules.FirstOrDefault(m => m.ModuleId == moduleDto.ModuleId)?.Module;

                if (module != null)
                {
                    await UpsertModuleMaterialsAsync(module.ModulesMaterials, moduleDto.ModuleId, projectId);
                    await UpsertModuleLaborAsync(module.ModulesLabors, moduleDto.ModuleId, projectId, drivingDistance);
                }
            }

            var modulesToDelete = existingModules.Where(m => !newModules.Any(nm => nm.ModuleId == m.ModuleId)).ToList();
            foreach (var module in modulesToDelete)
            {
                await _projectModuleRepository.DeleteAsync(module.ProjectModuleId);
            }
        }

        private async Task UpsertModuleMaterialsAsync(IEnumerable<ModulesMaterial> moduleMaterials, int? moduleId, int projectId)
        {
            var existingModuleMaterials = (await _projectMaterialRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == moduleId)).ToList() ?? new List<ProjectMaterial>();

            moduleMaterials = moduleMaterials ?? new List<ModulesMaterial>();

            foreach (var moduleMaterial in moduleMaterials)
            {
                var query = await _materialRepository.FindAsync(m => m.MaterialId == moduleMaterial.MaterialId, q => q.Include(m => m.TaxRate).Include(m => m.HandlingCost));
                var material = query.FirstOrDefault();
                var existingProjectMaterial = existingModuleMaterials.FirstOrDefault(m => m.ModuleId == moduleMaterial.ModuleId && m.MaterialId == moduleMaterial.MaterialId);

                if (existingProjectMaterial != null)
                {
                    existingProjectMaterial.Quantity = (int)moduleMaterial.Quantity;
                    existingProjectMaterial.LastModified = DateTime.UtcNow;
                    await _projectMaterialRepository.UpdateAsync(existingProjectMaterial);
                }
                else
                {
                    var newProjectMaterial = new ProjectMaterial
                    {
                        ProjectId = projectId,
                        MaterialId = material.MaterialId,
                        Quantity = (int)moduleMaterial.Quantity,
                        LastModified = DateTime.UtcNow,
                        UnitPrice = material.UnitPrice,
                        TaxRate = material.TaxRate.Rate,
                        HandlingCost = material.HandlingCost.Cost,
                        CifPrice = material.CifPrice,
                        ModuleId = moduleId
                    };
                    await _projectMaterialRepository.AddAsync(newProjectMaterial);
                    existingModuleMaterials.Add(newProjectMaterial);
                }
            }

            var materialsToDelete = existingModuleMaterials.Where(m => !moduleMaterials.Any(nm => nm.MaterialId == m.MaterialId)).ToList();
            foreach (var material in materialsToDelete)
            {
                await _projectMaterialRepository.DeleteAsync(material.ProjectMaterialId);
            }
        }


        private async Task UpsertModuleLaborAsync(IEnumerable<ModulesLabor> moduleLabors, int? moduleId, int projectId, double drivingDistance)
        {
            var existingModuleLabor = (await _projectLaborRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == moduleId)).ToList();

            foreach (var moduleLabor in moduleLabors ?? new List<ModulesLabor>())
            {
                var labor = await _laborRepository.GetByIdAsync(moduleLabor.LaborId);
                var existingLaborItem = existingModuleLabor.FirstOrDefault(l => l.LaborId == moduleLabor.LaborId && l.ModuleId == moduleLabor.ModuleId);
                if (existingLaborItem != null)
                {
                    var existingAllowanceItem = (await _projectAllowanceRepository.FindAsync(p => p.ProjectLaborId == existingLaborItem.ProjectLaborId)).FirstOrDefault();
                    existingLaborItem.Quantity = moduleLabor.Quantity;
                    existingLaborItem.HoursRequired = moduleLabor.HoursRequired;
                    existingLaborItem.LastModified = DateTime.UtcNow;
                    await _projectLaborRepository.UpdateAsync(existingLaborItem);
                    
                    if (existingAllowanceItem == null)
                    {
                        var newAllowance = CreateNewAllowance(existingLaborItem, labor, drivingDistance, moduleLabor.HoursRequired);
                        await _projectAllowanceRepository.AddAsync(newAllowance);
                    }
                    else
                    {
                        UpdateAllowance(existingAllowanceItem, labor, drivingDistance, moduleLabor.HoursRequired);
                        await _projectAllowanceRepository.UpdateAsync(existingAllowanceItem);
                    }
                    
                }
                else
                {
                    var newLaborItem = new ProjectLabor
                    {
                        ProjectId = projectId,
                        LaborId = moduleLabor.LaborId,
                        Quantity = moduleLabor.Quantity,
                        HoursRequired = moduleLabor.HoursRequired,
                        LastModified = DateTime.UtcNow,
                        HourlyRate = labor.HourlyRate,
                        ModuleId = moduleId
                    };
                    await _projectLaborRepository.AddAsync(newLaborItem);

                    var newAllowance = CreateNewAllowance(newLaborItem, labor, drivingDistance, moduleLabor.HoursRequired);
                    await _projectAllowanceRepository.AddAsync(newAllowance);
                    existingModuleLabor.Add(newLaborItem);
                }
            }

            var laborToDelete = existingModuleLabor.Where(l => !moduleLabors.Any(nl => nl.LaborId == l.LaborId)).ToList();
            foreach (var labor in laborToDelete)
            {
                await _projectLaborRepository.DeleteAsync(labor.ProjectLaborId);
            }
        }

        private async Task UpsertCompositeModulesAsync(IEnumerable<ProjectModuleCompositesDto> newModulesComposite, int projectId, double drivingDistance)
        {
            var existingModulesComposite = (await _projectModuleComposite.FindAsync(p => p.ProjectId == projectId, query => query.Include(p => p.ModuleComposite).ThenInclude(mc => mc.ModuleCompositeDetails))).ToList() ?? new List<ProjectModuleComposite>();

            foreach (var moduleCompositeDto in newModulesComposite)
            {
                var existingModuleComposite = existingModulesComposite.FirstOrDefault(m => m.ModuleCompositeId == moduleCompositeDto.ModuleCompositeId);
                if (existingModuleComposite != null)
                {
                    existingModuleComposite.Quantity = moduleCompositeDto.Quantity;
                    await _projectModuleComposite.UpdateAsync(existingModuleComposite);
                }
                else
                {
                    var newModuleComposite = new ProjectModuleComposite
                    {
                        ProjectId = projectId,
                        ModuleCompositeId = moduleCompositeDto.ModuleCompositeId,
                        Quantity = moduleCompositeDto.Quantity
                    };
                    await _projectModuleComposite.AddAsync(newModuleComposite);
                    existingModulesComposite.Add(newModuleComposite);
                }

                var moduleComp = existingModulesComposite.FirstOrDefault(m => m.ModuleCompositeId == moduleCompositeDto.ModuleCompositeId);

                foreach (var detail in moduleComp?.ModuleComposite?.ModuleCompositeDetails ?? new List<ModuleCompositeDetail>())
                {
                    var moduleCompDetailModule = detail.Module;

                    if (moduleCompDetailModule != null)
                    {
                        await UpsertModuleMaterialsAsync(moduleCompDetailModule.ModulesMaterials, detail.ModuleId, projectId);
                        await UpsertModuleLaborAsync(moduleCompDetailModule.ModulesLabors ?? new List<ModulesLabor>(), detail.ModuleId, projectId, drivingDistance);
                    }
                }
            }

            var modulesCompositeToDelete = existingModulesComposite.Where(mc => !newModulesComposite.Any(nmc => nmc.ModuleCompositeId == mc.ModuleCompositeId)).ToList();
            foreach (var moduleComposite in modulesCompositeToDelete)
            {
                await _projectModuleComposite.DeleteAsync(moduleComposite.ProjectModuleCompositeId);
            }
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return new ProjectDetailsDto();
            }

            var projectMaterials = await _projectMaterialRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId == null);
            var projectLabor = await _projectLaborRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == null, query => query.Include(p => p.Labor), query => query.Include(p => p.ProjectAllowance));
            var projectModules = await _projectModuleRepository.FindAsync(p => p.ProjectId == projectId);
            var projectModulesComposite = await _projectModuleComposite.FindAsync(p => p.ProjectId == projectId);

            var projectDetails = new ProjectDetailsDto{
                ProjectId = projectId,
                ProjectMaterials = MapProjectMaterialsDto(projectMaterials),
                ProjectLabor = MapProjectLaborDto(projectLabor),
                ProjectModules = MapProjectModulesDto(projectModules),
                ProjectModuleComposites = MapProjectModulesCompositeDto(projectModulesComposite)
            };

            return projectDetails;
        }

        private IList<ProjectMaterialDto> MapProjectMaterialsDto(IEnumerable<ProjectMaterial>? projectMaterials)
        {
            return projectMaterials.Select(pm => new ProjectMaterialDto
            {
                ProjectId = pm.ProjectId,
                ProjectMaterialId = pm.ProjectMaterialId,
                MaterialId = pm.MaterialId,
                Quantity = pm.Quantity,
                LastModified = pm.LastModified,
                UnitPrice = pm.UnitPrice,
                TaxRate = pm.TaxRate,
                HandlingCost = pm.HandlingCost,
                CifPrice = pm.CifPrice,
                ModuleId = pm.ModuleId == 0 ? null : pm.ModuleId,
            }).ToList();
        }

        private IList<ProjectLaborDto> MapProjectLaborDto(IEnumerable<ProjectLabor>? projectLabor)
        {
            return projectLabor.Select(pl => new ProjectLaborDto
            {
                ProjectId = pl.ProjectId,
                LaborId = pl.LaborId,
                Quantity = pl.Quantity,
                HourlyRate = pl.HourlyRate,
                ProjectLaborId = pl.ProjectLaborId,
                AllowanceAmount = pl.ProjectAllowance?.Amount,
                AllowanceQuantity = pl.ProjectAllowance?.Quantity,
                ModuleId = pl.ModuleId == 0 ? null : pl.ModuleId,
            }).ToList();
        }

        private IList<ProjectModuleDto> MapProjectModulesDto(IEnumerable<ProjectModule>? projectModules)
        {
            return projectModules.Select(pm => new ProjectModuleDto
            {
                ProjectId = pm.ProjectId,
                ModuleId = pm.ModuleId,
                Quantity = pm.Quantity,
                ProjectModuleId = pm.ProjectModuleId
            }).ToList();
        }

        private IList<ProjectModuleCompositesDto> MapProjectModulesCompositeDto(IEnumerable<ProjectModuleComposite>? projectModules)
        {
            return projectModules.Select(pm => new ProjectModuleCompositesDto
            {
                ProjectId = pm.ProjectId,
                Quantity = pm.Quantity,
                ModuleCompositeId = pm.ModuleCompositeId,
                ProjectModuleCompositeId = pm.ProjectModuleCompositeId,
            }).ToList();
        }

    }

}

