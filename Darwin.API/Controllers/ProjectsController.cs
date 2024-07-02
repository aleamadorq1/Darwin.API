using Microsoft.AspNetCore.Mvc;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IProjectDetailsService _projectDetailsService;

        public ProjectsController(IProjectService projectService, IProjectDetailsService projectDetailsService)
        {
            _projectService = projectService;
            _projectDetailsService = projectDetailsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAllProjects()
        {
            return Ok(await _projectService.GetAllProjects());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpPost]
        public async Task<ActionResult<Project>> AddProject(ProjectDto project)
        {
            var newProject = await _projectService.AddProject(project);
            return CreatedAtAction(nameof(GetProjectById), new { id = newProject.ProjectId }, newProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, ProjectDto project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            await _projectService.UpdateProject(project);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteProject(id);
            return NoContent();
        }
    }
}
