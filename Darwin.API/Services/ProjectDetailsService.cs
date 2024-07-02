using System;
using Alpha.API.Dtos;
using Alpha.API.Models;
using Alpha.API.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace Alpha.API.Services
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

        public ProjectDetailService(IProjectMaterialsRepository projectMaterialRepository, IProjectLaborRepository projectLaborRepository, IRepository<ProjectModule> projectModuleRepository, IRepository<ProjectModuleComposite> projectModuleComposite, IRepository<Project> projectRepository)
        {
            _projectLaborRepository = projectLaborRepository;
            _projectMaterialRepository = projectMaterialRepository;
            _projectModuleRepository = projectModuleRepository;
            _projectModuleComposite = projectModuleComposite;
            _projectRepository = projectRepository;
        }
    
        public async Task<bool> UpsertProjectDetails(ProjectDetailsDto projectDetails, int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (projectId != 0)
            {
                var projectMaterials = await _projectMaterialRepository.GetByProjectId(projectId);
                await _projectMaterialRepository.DeleteRangeAsync(projectMaterials);

                var projectLabor = await _projectLaborRepository.GetByProjectId(projectId);
                await _projectLaborRepository.DeleteRangeAsync(projectLabor);
                
                var projectModules = await _projectModuleRepository.FindAsync(p => p.ProjectId == projectId);
                await _projectModuleRepository.DeleteRangeAsync(projectModules);

                var projectModulesComposite = await _projectModuleComposite.FindAsync(p => p.ProjectId == projectId);
                await _projectModuleComposite.DeleteRangeAsync(projectModulesComposite);
            }

            var newProjectMaterials = MapProjectMaterials(projectDetails.ProjectMaterials ?? new List<ProjectMaterialDto>());
            await _projectMaterialRepository.AddRangeAsync(newProjectMaterials);

            var newProjectLabor = MapProjectLabor(projectDetails.ProjectLabor ?? new List<ProjectLaborDto>());
            await _projectLaborRepository.AddRangeAsync(newProjectLabor);

            var newProjectModules = MapProjectModules(projectDetails.ProjectModules ?? new List<ProjectModuleDto>());
            await _projectModuleRepository.AddRangeAsync(newProjectModules);

            var newProjectModulesComposite = MapProjectModulesComposite(projectDetails.ProjectModuleComposites ?? new List<ProjectModuleCompositesDto>());
            await _projectModuleComposite.AddRangeAsync(newProjectModulesComposite);

            return true;
        }

        public async Task<ProjectDetailsDto> GetProjectDetails(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
            {
                return new ProjectDetailsDto();
            }

            var projectMaterials = await _projectMaterialRepository.GetByProjectId(projectId);
            var projectLabor = await _projectLaborRepository.GetByProjectId(projectId);
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
                UnitPrice = pm.UnitPrice,
                TaxStatus = pm.TaxStatus,
                CifPrice = pm.CifPrice,
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
                Quantity = pl.Quantity,
                LastModified = DateTime.UtcNow,
                HourlyRate = pl.HourlyRate,
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
                ModuleId = pm.ModuleId,
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

