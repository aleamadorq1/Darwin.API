using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Module>>> GetAllModules()
        {
            return Ok(await _moduleService.GetAllModules());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Module>> GetModuleById(int id)
        {
            var module = await _moduleService.GetModuleById(id);
            if (module == null)
            {
                return NotFound();
            }
            return Ok(module);
        }

        [HttpPost]
        public async Task<ActionResult<Module>> AddModule(Module module)
        {
            var newModule = await _moduleService.AddModule(module);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateModule(int id, Module module)
        {
            if (id != module.ModuleId)
            {
                return BadRequest();
            }

            await _moduleService.UpdateModule(module);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            await _moduleService.DeleteModule(id);
            return NoContent();
        }

        [HttpGet("index")]
        public async Task<ActionResult<IEnumerable<ModuleIndexDto>>> GetModuleIndex()
        {
            return Ok(await _moduleService.GetModuleIndex());
        }
    }
}

