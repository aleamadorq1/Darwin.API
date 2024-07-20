using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuleMaterialsController : ControllerBase
    {
        private readonly IModuleMaterialsService _moduleMaterialsService;

        public ModuleMaterialsController(IModuleMaterialsService moduleMaterialsService)
        {
            _moduleMaterialsService = moduleMaterialsService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModulesMaterial>> GetModuleMaterialById(int id)
        {
            var moduleMaterial = await _moduleMaterialsService.GetModuleMaterialById(id);
            if (moduleMaterial == null)
            {
                return NotFound();
            }
            return Ok(moduleMaterial);
        }

        [HttpPost]
        public async Task<ActionResult<ModulesMaterial>> AddModuleMaterial(ModulesMaterial moduleMaterial)
        {
            var newModuleMaterial = await _moduleMaterialsService.AddModuleMaterial(moduleMaterial);
            return CreatedAtAction(nameof(GetModuleMaterialById), new { id = newModuleMaterial.ModuleMaterialId }, newModuleMaterial);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModuleMaterial(int id, ModulesMaterial moduleMaterial)
        {
            if (id != moduleMaterial.ModuleMaterialId)
            {
                return BadRequest();
            }

            await _moduleMaterialsService.UpdateModuleMaterial(moduleMaterial);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModuleMaterial(int id)
        {
            await _moduleMaterialsService.DeleteModuleMaterial(id);
            return NoContent();
        }

        [HttpGet("module/{moduleId}")]
        public async Task<ActionResult<IEnumerable<ModuleMaterialsDto>>> GetModuleMaterialsByModuleId(int moduleId)
        {
            var moduleMaterials = await _moduleMaterialsService.GetModuleMaterialsByModuleId(moduleId);
            return Ok(moduleMaterials);
        }
    }
}

