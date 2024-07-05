using System;
using System.Data;
using Darwin.API.Controllers;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.AspNetCore.Mvc;


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
        private readonly IRepository<ProjectModule> _projectModuleRepository;
        private readonly IRepository<ProjectModuleComposite> _projectModuleComposite;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Module> _moduleRepository;

        private readonly IRepository<ModulesComposite> _moduleCompositeRepository;
        private readonly IRepository<Material> _materialRepository;
        private readonly IRepository<Labor> _laborRepository;

        public ProjectDetailService(IProjectMaterialsRepository projectMaterialRepository, IProjectLaborRepository projectLaborRepository, 
                                    IRepository<ProjectModule> projectModuleRepository, IRepository<ProjectModuleComposite> projectModuleComposite, 
                                    IRepository<Project> projectRepository,IRepository<Material> materialRepository, 
                                    IRepository<Labor> laborRepository, IRepository<Module> moduleRepository, IRepository<ModulesComposite> moduleCompositeRepository)
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
        }
            
        public async Task<bool> UpsertProjectDetails(ProjectDetailsDto projectDetails, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return false;
            }

            var existingMaterials = (await _projectMaterialRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId == null)).ToList();
            var existingLabor = (await _projectLaborRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId == null)).ToList();
            var existinModuleMaterials = (await _projectMaterialRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId != null)).ToList();
            var existingModuleLabor = (await _projectLaborRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId != null)).ToList();
            var existingModules = (await _projectModuleRepository.FindAsync(p => p.ProjectId == projectId, includes: [p => p.Module, p=>p.Module.ModulesMaterials, p=>p.Module.ModulesLabors])).ToList();
            var existingModulesComposite = (await _projectModuleComposite.FindAsync(p => p.ProjectId == projectId, includes: [p => p.ModuleComposite, p =>p.ModuleComposite.ModuleCompositeDetails])).ToList();
            var existingCompositeMaterials = (await _projectMaterialRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId != null && p.Module.ModuleCompositeDetails.Count() > 0, includes:[p=>p.Module.ModuleCompositeDetails])).ToList();
            var existingCompositeLabor = (await _projectLaborRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId != null && p.Module.ModuleCompositeDetails.Count() > 0, includes:[p=>p.Module.ModuleCompositeDetails])).ToList();

            var newMaterials = projectDetails.ProjectMaterials ?? new List<ProjectMaterialDto>();
            var newLabor = projectDetails.ProjectLabor ?? new List<ProjectLaborDto>();
            var newModules = projectDetails.ProjectModules ?? new List<ProjectModuleDto>();
            var newModulesComposite = projectDetails.ProjectModuleComposites ?? new List<ProjectModuleCompositesDto>();
            
            // Update or add materials
            foreach (var materialDto in newMaterials)
            {
                var existingMaterial = existingMaterials.FirstOrDefault(m => m.MaterialId == materialDto.MaterialId);
                if (existingMaterial != null)
                {
                    // Update existing material
                    existingMaterial.Quantity = materialDto.Quantity;
                    existingMaterial.LastModified = DateTime.UtcNow;
                    //existingMaterial.ModuleId = materialDto.ModuleId == 0 ? null : materialDto.ModuleId;
                    await _projectMaterialRepository.UpdateAsync(existingMaterial);
                }
                else
                {
                    var material = await _materialRepository.GetByIdAsync(materialDto.MaterialId);
                    // Add new material
                    var newMaterial = new ProjectMaterial
                    {
                        ProjectId = projectId,
                        MaterialId = materialDto.MaterialId,
                        Quantity = materialDto.Quantity,
                        LastModified = DateTime.UtcNow,
                        UnitPrice = material.UnitPrice,
                        TaxStatus = material.TaxStatus,
                        CifPrice = material.CifPrice ?? 0,
                        ModuleId = null
                    };
                    await _projectMaterialRepository.AddAsync(newMaterial);

                    var result = await _projectMaterialRepository.FindAsync(p => p.ProjectMaterialId == newMaterial.ProjectMaterialId);
                    existingMaterials.Add(result.FirstOrDefault());
                }
            }

            // Remove materials that are not in the incoming list
            foreach (var existingMaterial in existingMaterials)
            {
                if (!newMaterials.Any(m => m.MaterialId == existingMaterial.MaterialId))
                {
                    await _projectMaterialRepository.DeleteAsync(existingMaterial.ProjectMaterialId);
                }
            }

            // Update or add labor
            foreach (var laborDto in newLabor)
            {
                var existingLaborItem = existingLabor.FirstOrDefault(l => l.LaborId == laborDto.LaborId);
                if (existingLaborItem != null)
                {
                    // Update existing labor
                    existingLaborItem.Quantity = (int)laborDto.Quantity;
                    existingLaborItem.LastModified = DateTime.UtcNow;
                    //existingLaborItem.ModuleId = null;
                    await _projectLaborRepository.UpdateAsync(existingLaborItem);
                }
                else
                {
                    // Add new labor
                    var newLaborItem = new ProjectLabor
                    {
                        ProjectId = projectId,
                        LaborId = laborDto.LaborId,
                        Quantity = (int)laborDto.Quantity,
                        LastModified = DateTime.UtcNow,
                        HourlyRate = (await _laborRepository.GetByIdAsync(laborDto.LaborId)).HourlyRate,
                        ModuleId = null
                    };
                    await _projectLaborRepository.AddAsync(newLaborItem);

                    var result = await _projectLaborRepository.FindAsync(p=> p.ProjectLaborId == newLaborItem.ProjectLaborId);
                    existingLabor.Add(result.FirstOrDefault());
                }
            }

            // Remove labor that is not in the incoming list
            foreach (var existingLaborItem in existingLabor)
            {
                if (!newLabor.Any(l => l.LaborId == existingLaborItem.LaborId))
                {
                    await _projectLaborRepository.DeleteAsync(existingLaborItem.ProjectLaborId);
                }
            }

            // Update or add modules
            foreach (var moduleDto in newModules)
            {
                var existingModule = existingModules.FirstOrDefault(m => m.ModuleId == moduleDto.ModuleId && m.ModuleId != null);
                if (existingModule != null)
                {
                    // Update existing module
                    existingModule.Quantity = moduleDto.Quantity;
                    await _projectModuleRepository.UpdateAsync(existingModule);
                }
                else
                {
                    // Add new module
                    var newModule = new ProjectModule
                    {
                        ProjectId = projectId,
                        ModuleId = moduleDto.ModuleId ?? throw new DataException("ModuleId is required"),
                        Quantity = moduleDto.Quantity
                    };
                    await _projectModuleRepository.AddAsync(newModule);
                    var result = await _projectModuleRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == newModule.ModuleId, includes: [p => p.Module, p=>p.Module.ModulesMaterials, p=>p.Module.ModulesLabors]); 
                    existingModules.Add(result.FirstOrDefault());
                }

                //var module = await _moduleRepository.FindAsync(m => m.ModuleId == moduleDto.ModuleId, includes: [m => m.ModulesMaterials, m => m.ModulesLabors]);
                var module = existingModules.FirstOrDefault(m => m.ModuleId == moduleDto.ModuleId).Module;

                foreach (var moduleMaterial in module.ModulesMaterials ?? new List<ModulesMaterial>())
                {
                    var material = await _materialRepository.GetByIdAsync(moduleMaterial.MaterialId);
                    var existingProjectMaterial = existinModuleMaterials.FirstOrDefault(m => m.ModuleId == moduleMaterial.ModuleId && m.MaterialId == moduleMaterial.MaterialId);
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
                            TaxStatus = material.TaxStatus,
                            CifPrice = material.CifPrice ?? 0,
                            ModuleId = moduleDto.ModuleId
                        };
                        await _projectMaterialRepository.AddAsync(newProjectMaterial);
                        var result = await _projectMaterialRepository.FindAsync(p => p.ProjectMaterialId == newProjectMaterial.ProjectMaterialId, includes:[p=>p.Module.ModuleCompositeDetails] );
                        existinModuleMaterials.Add(result.FirstOrDefault());
                    }
                }

                foreach (var moduleLabor in module.ModulesLabors ?? new List<ModulesLabor>())
                {
                    var labor = await _laborRepository.GetByIdAsync(moduleLabor.LaborId);
                    var existingProjectLabor = existingModuleLabor.FirstOrDefault(l => l.ModuleId == moduleDto.ModuleId && l.LaborId == moduleLabor.LaborId);
                    if (existingProjectLabor != null)
                    {
                        existingProjectLabor.Quantity = (int)moduleLabor.HoursRequired;
                        existingProjectLabor.LastModified = DateTime.UtcNow;
                        await _projectLaborRepository.UpdateAsync(existingProjectLabor);
                    }
                    else
                    {
                        var newProjectLabor = new ProjectLabor
                        {
                            ProjectId = projectId,
                            LaborId = moduleLabor.LaborId,
                            Quantity = (int)moduleLabor.HoursRequired,
                            LastModified = DateTime.UtcNow,
                            HourlyRate = labor.HourlyRate,
                            ModuleId = moduleDto.ModuleId
                        };
                        await _projectLaborRepository.AddAsync(newProjectLabor);

                        var result = await _projectLaborRepository.FindAsync(p=> p.ProjectLaborId == newProjectLabor.ProjectLaborId, includes:[p=>p.Module.ModuleCompositeDetails]);
                        existingModuleLabor.Add(result.FirstOrDefault());
                    }
                }
            }

            // Remove modules that are not in the incoming list
            foreach (var existingModule in existingModules)
            {
                if (!newModules.Any(m => m.ModuleId == existingModule.ModuleId))
                {
                    await _projectModuleRepository.DeleteAsync(existingModule.ProjectModuleId);
                }
            }


            // Handle composite modules
            foreach (var moduleCompositeDto in newModulesComposite)
            {
                var existingModuleComposite = existingModulesComposite.FirstOrDefault(m => m.ModuleCompositeId == moduleCompositeDto.ModuleCompositeId);
                if (existingModuleComposite != null)
                {
                    // Update existing composite module
                    existingModuleComposite.Quantity = moduleCompositeDto.Quantity;
                    await _projectModuleComposite.UpdateAsync(existingModuleComposite);
                }
                else
                {
                    // Add new composite module
                    var newModuleComposite = new ProjectModuleComposite
                    {
                        ProjectId = projectId,
                        ModuleCompositeId = moduleCompositeDto.ModuleCompositeId,
                        Quantity = moduleCompositeDto.Quantity
                    };
                    await _projectModuleComposite.AddAsync(newModuleComposite);

                    var result = await _projectModuleComposite.FindAsync(p=>p.ProjectModuleCompositeId == newModuleComposite.ProjectModuleCompositeId, includes: [p => p.ModuleComposite, p =>p.ModuleComposite.ModuleCompositeDetails]);
                    existingModulesComposite.Add(result.FirstOrDefault());
                }

                var moduleComp = existingModulesComposite.FirstOrDefault(m => m.ModuleCompositeId == moduleCompositeDto.ModuleCompositeId);

                // Handle modules in composite module
                foreach (var detail in moduleComp?.ModuleComposite.ModuleCompositeDetails ?? new List<ModuleCompositeDetail>())
                {
                    var moduleCompDetailModule = detail.Module;

                    foreach (var compositeModuleMaterial in moduleCompDetailModule?.ModulesMaterials ?? new List<ModulesMaterial>())
                    {
                        var material = await _materialRepository.GetByIdAsync(compositeModuleMaterial.MaterialId);
                        var existingCompositeMaterial = existingCompositeMaterials.FirstOrDefault(m => m.ModuleId == detail.ModuleId&& m.Module?.ModuleCompositeDetails.FirstOrDefault()?.ModuleCompositeId == moduleComp?.ModuleCompositeId && m.MaterialId == material.MaterialId);
                        if (existingCompositeMaterial != null)
                        {
                            existingCompositeMaterial.Quantity = (int)compositeModuleMaterial.Quantity;
                            existingCompositeMaterial.LastModified = DateTime.UtcNow;
                            await _projectMaterialRepository.UpdateAsync(existingCompositeMaterial);
                        }
                        else
                        {
                            var newCompositeMaterial = new ProjectMaterial
                            {
                                ProjectId = projectId,
                                MaterialId = material.MaterialId,
                                Quantity = (int)compositeModuleMaterial.Quantity,
                                LastModified = DateTime.UtcNow,
                                UnitPrice = material.UnitPrice,
                                TaxStatus = material.TaxStatus,
                                CifPrice = material.CifPrice ?? 0,
                                ModuleId = detail.ModuleId
                            };
                            await _projectMaterialRepository.AddAsync(newCompositeMaterial);
                            var result = await _projectMaterialRepository.FindAsync(cm=> cm.ProjectMaterialId == newCompositeMaterial.ProjectMaterialId, includes: [p => p.Module.ModuleCompositeDetails]);
                            existingCompositeMaterials.Add(result.FirstOrDefault());
                        }
                    }

                    foreach (var compositeModuleLabor in moduleCompDetailModule?.ModulesLabors ?? new List<ModulesLabor>())
                    {
                        var labor = await _laborRepository.GetByIdAsync(compositeModuleLabor.LaborId);
                        var existingCompLabor = existingCompositeLabor?.FirstOrDefault(l => l.ModuleId == detail.ModuleId && l.Module.ModuleCompositeDetails.FirstOrDefault()?.ModuleCompositeId == moduleComp?.ModuleCompositeId&& l.LaborId == compositeModuleLabor.LaborId);
                        if (existingCompositeLabor != null)
                        {
                            existingCompLabor.Quantity = (int)compositeModuleLabor.HoursRequired;
                            existingCompLabor.LastModified = DateTime.UtcNow;
                            await _projectLaborRepository.UpdateAsync(existingCompLabor);
                        }
                        else
                        {
                            var newCompositeLabor = new ProjectLabor
                            {
                                ProjectId = projectId,
                                LaborId = compositeModuleLabor.LaborId,
                                Quantity = (int)compositeModuleLabor.HoursRequired,
                                LastModified = DateTime.UtcNow,
                                HourlyRate = labor.HourlyRate,
                                ModuleId = detail.ModuleId
                            };
                            await _projectLaborRepository.AddAsync(newCompositeLabor);
                            var result = await _projectLaborRepository.FindAsync(p=> p.ProjectLaborId == newCompositeLabor.ProjectLaborId, includes:[p=>p.Module.ModuleCompositeDetails]);
                            existingCompositeLabor?.Add(result.FirstOrDefault());
                        }
                    }
                }
            }

            // Remove composite modules that are not in the incoming list
            foreach (var existingModuleComposite in existingModulesComposite)
            {
                if (!newModulesComposite.Any(m => m.ModuleCompositeId == existingModuleComposite.ModuleCompositeId))
                {
                    await _projectModuleComposite.DeleteAsync(existingModuleComposite.ProjectModuleCompositeId);
                }
            }

            return true;
        }



        public async Task<ProjectDetailsDto> GetProjectDetails(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return new ProjectDetailsDto();
            }

            var projectMaterials = await _projectMaterialRepository.FindAsync(p => p.ProjectId ==projectId && p.ModuleId == null);
            var projectLabor = await _projectLaborRepository.FindAsync(p => p.ProjectId == projectId && p.ModuleId == null);
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



        private IList<ProjectMaterial> MapProjectMaterials(IList<ProjectMaterialDto> projectMaterials)
        {
            return projectMaterials.Select(pm => new ProjectMaterial
            {
                ProjectId = pm.ProjectId,
                MaterialId = pm.MaterialId,
                Quantity = pm.Quantity,
                LastModified = DateTime.UtcNow,
                UnitPrice = pm.UnitPrice ?? 0,
                TaxStatus = pm.TaxStatus,
                CifPrice = pm.CifPrice ?? 0,
                ModuleId = pm.ModuleId == 0 ? null : pm.ModuleId,
            }).ToList();
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
                TaxStatus = pm.TaxStatus,
                CifPrice = pm.CifPrice,
                ModuleId = pm.ModuleId == 0 ? null : pm.ModuleId,
            }).ToList();
        }

        private IList<ProjectLabor> MapProjectLabor(IList<ProjectLaborDto> projectLabor)
        {
            return projectLabor.Select(pl => new ProjectLabor
            {
                ProjectLaborId = pl.ProjectLaborId,
                ProjectId = pl.ProjectId,
                LaborId = pl.LaborId,
                Quantity = (int)pl.Quantity,
                LastModified = DateTime.UtcNow,
                HourlyRate = pl.HourlyRate?? 0,
                ModuleId = pl.ModuleId == 0 ? null : pl.ModuleId,
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
                ModuleId = pl.ModuleId == 0 ? null : pl.ModuleId,
            }).ToList();
        }

        private IList<ProjectModule> MapProjectModules(IList<ProjectModuleDto> projectModules)
        {
            return projectModules.Select(pm => new ProjectModule
            {
                ProjectId = pm.ProjectId,
                ModuleId = pm.ModuleId ?? 0,
                Quantity = pm.Quantity,
                ProjectModuleId = pm.ProjectModuleId
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

        private IList<ProjectModuleComposite> MapProjectModulesComposite(IList<ProjectModuleCompositesDto> projectModules)
        {
            return projectModules.Select(pm => new ProjectModuleComposite
            {
                ProjectId = pm.ProjectId,
                Quantity = pm.Quantity,
                ModuleCompositeId = pm.ModuleCompositeId,
                ProjectModuleCompositeId = pm.ProjectModuleCompositeId,
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

