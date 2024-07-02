using Microsoft.AspNetCore.Mvc;
using Alpha.API.Dtos;
using Alpha.API.Models;
using Alpha.API.Services;

namespace Alpha.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectDetailsController : ControllerBase
    {
        private readonly IProjectDetailsService _projectDetailsService;

        public ProjectDetailsController(IProjectDetailsService projectDetailsService)
        {
            _projectDetailsService = projectDetailsService;
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
