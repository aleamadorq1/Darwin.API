using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{

    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjects();
        Task<ProjectDto?> GetProjectById(int id);
        Task<Project> AddProject(ProjectDto project);
        Task<ProjectDto?> UpdateProject(ProjectDto project);
        Task<bool> DeleteProject(int id);
    }

    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Client> _clientRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IProjectDetailsService _projectDetailsService;

        public ProjectService(IRepository<Project> projectRepository, IRepository<Client> clientRepository, IRepository<Organization> organizationRepository, IProjectDetailsService projectDetailsService)
        {
            _projectRepository = projectRepository;
            _clientRepository = clientRepository;
            _organizationRepository = organizationRepository;
            _projectDetailsService = projectDetailsService;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjects()
        {
            var projects = await _projectRepository.GetAllAsync();
            var clients = await _clientRepository.GetAllAsync();
            var organizations = await _organizationRepository.GetAllAsync();

            return projects.Select(p => new ProjectDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ClientId = p.ClientId,
                ClientName = clients.FirstOrDefault(c => c.ClientId == p.ClientId)?.ClientName,
                TotalArea = p.TotalArea,
                TotalFloors = p.TotalFloors,
                Location = p.Location,
                LocationAddress = p.LocationAddress,
                LocationCoordinates = p.LocationCoordinates,
                ProfitMargin = p.ProfitMargin,
                OrganizationId = p.OrganizationId,
                OrganizationName = organizations.FirstOrDefault(o => o.OrganizationId == p.OrganizationId)?.OrganizationName,
                LastModified = p.LastModified
            }).OrderByDescending(p => p.LastModified).ToList();
        }

        public async Task<ProjectDto?> GetProjectById(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null) return null;

            var client = await _clientRepository.GetByIdAsync(project.ClientId);

            return new ProjectDto
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ClientId = project.ClientId,
                ClientName = client?.ClientName,
                TotalArea = project.TotalArea,
                Location = project.Location,
                DistributionCenterId = project.DistributionCenterId??0,
                LocationAddress = project.LocationAddress,
                LocationCoordinates = project.LocationCoordinates,
                ProfitMargin = project.ProfitMargin,
                OrganizationId = project.OrganizationId,
                OrganizationName = "Todo Organization",
            };
        }

        public async Task<Project> AddProject(ProjectDto project)
        {
            var newProject = new Project
            {
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                ClientId = project.ClientId,
                TotalArea = project.TotalArea,
                TotalFloors = project.TotalFloors,
                DistributionCenterId = project.DistributionCenterId,
                Location = project.Location,
                LocationAddress = project.LocationAddress,
                LocationCoordinates = project.LocationCoordinates,
                ProfitMargin = project.ProfitMargin,
                OrganizationId = project.OrganizationId,
            };

            return await _projectRepository.AddAsync(newProject);
        }

        public async Task<ProjectDto?> UpdateProject(ProjectDto project)
        {
            var existingProject = await _projectRepository.GetByIdAsync(project.ProjectId);
            if (existingProject == null) return null;
            
            existingProject.ProjectName = project.ProjectName;
            existingProject.Description = project.Description;
            existingProject.StartDate = project.StartDate;
            existingProject.EndDate = project.EndDate;
            existingProject.ClientId = project.ClientId;
            existingProject.TotalArea = project.TotalArea;
            existingProject.DistributionCenterId = project.DistributionCenterId;
            existingProject.TotalFloors = project.TotalFloors;
            existingProject.Location = project.Location;    
            existingProject.LocationAddress = project.LocationAddress;
            existingProject.LocationCoordinates = project.LocationCoordinates;
            existingProject.ProfitMargin = project.ProfitMargin;

            var result =  await _projectRepository.UpdateAsync(existingProject);
            
            var projectDetails = await _projectDetailsService.GetProjectDetails(existingProject.ProjectId);
            if (projectDetails != null)
            {
                await _projectDetailsService.UpsertProjectDetails(projectDetails, existingProject.ProjectId);
            }

            return project;
        }

        public async Task<bool> DeleteProject(int id)
        {
            return await _projectRepository.DeleteAsync(id);
        }
    }
}
