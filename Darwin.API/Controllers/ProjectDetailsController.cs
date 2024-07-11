using Microsoft.AspNetCore.Mvc;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectDetailsController : ControllerBase
    {
        private readonly IProjectDetailsService _projectDetailsService;
        private readonly IProjectCostsService _projectCostsService;

        public ProjectDetailsController(IProjectDetailsService projectDetailsService, IProjectCostsService projectCostsService)
        {
            _projectDetailsService = projectDetailsService;
            _projectCostsService = projectCostsService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailsDto>> GetProjectDetails(int id)
        {
            var result = await _projectDetailsService.GetProjectDetails(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("{id}/costs")]
        public async Task<ActionResult<IEnumerable<ProjectCostDetailsDto>>> GetProjectCosts(int id)
        {
            var result = await _projectCostsService.GetProjectCosts(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("{id}/costs")]
        public async Task<ActionResult<IEnumerable<ProjectModuleDto>>> UpdateProjectCosts(ProjectCostDetailsDto project, int id)
        {
            var result = await _projectCostsService.UpdateProjectCosts(project);
            if (result == false)
            {
                return NotFound();
            }
            return Ok(id);
        }


        [HttpPost("{id}")]
        public async Task<ActionResult> UpsertProjectDetails(ProjectDetailsDto projectDetails, int id)
        {
            var result = await _projectDetailsService.UpsertProjectDetails(projectDetails, id);
            
            if (result == false)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
