using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesLaborController : ControllerBase
    {
        private readonly IModulesLaborService _modulesLaborService;

        public ModulesLaborController(IModulesLaborService modulesLaborService)
        {
            _modulesLaborService = modulesLaborService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModulesLabor>> GetModulesLaborById(int id)
        {
            var modulesLabor = await _modulesLaborService.GetModulesLaborById(id);
            if (modulesLabor == null)
            {
                return NotFound();
            }
            return Ok(modulesLabor);
        }

        [HttpPost]
        public async Task<ActionResult<ModulesLabor>> AddModulesLabor(ModulesLabor modulesLabor)
        {
            var newModulesLabor = await _modulesLaborService.AddModulesLabor(modulesLabor);
            return CreatedAtAction(nameof(GetModulesLaborById), new { id = newModulesLabor.ModuleLaborId }, newModulesLabor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModulesLabor(int id, ModulesLabor modulesLabor)
        {
            if (id != modulesLabor.ModuleLaborId)
            {
                return BadRequest();
            }

            await _modulesLaborService.UpdateModulesLabor(modulesLabor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModulesLabor(int id)
        {
            await _modulesLaborService.DeleteModulesLabor(id);
            return NoContent();
        }

        [HttpGet("module/{moduleId}")]
        public async Task<ActionResult<IEnumerable<ModulesLaborDto>>> GetModulesLaborByModuleId(int moduleId)
        {
            var modulesLabor = await _modulesLaborService.GetModulesLaborByModuleId(moduleId);
            return Ok(modulesLabor);
        }
    }
}

