using Microsoft.AspNetCore.Mvc;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesCompositeController : ControllerBase
    {
        private readonly IModulesCompositeService _modulesCompositeService;

        public ModulesCompositeController(IModulesCompositeService modulesCompositeService)
        {
            _modulesCompositeService = modulesCompositeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModulesCompositeDto>>> GetAllModulesComposites()
        {
            return Ok(await _modulesCompositeService.GetAllModulesComposites());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModulesCompositeDto>> GetModulesCompositeById(int id)
        {
            var modulesComposite = await _modulesCompositeService.GetModulesCompositeById(id);
            if (modulesComposite == null)
            {
                return NotFound();
            }
            return Ok(modulesComposite);
        }

        [HttpPost]
        public async Task<ActionResult<ModulesComposite>> AddModulesComposite(ModulesComposite modulesComposite)
        {
            var newModulesComposite = await _modulesCompositeService.AddModulesComposite(modulesComposite);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModulesComposite(int id, ModulesComposite modulesComposite)
        {
            if (id != modulesComposite.ModuleCompositeId)
            {
                return BadRequest();
            }

            await _modulesCompositeService.UpdateModulesComposite(modulesComposite);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModulesComposite(int id)
        {
            await _modulesCompositeService.DeleteModulesComposite(id);
            return NoContent();
        }
    }
}
