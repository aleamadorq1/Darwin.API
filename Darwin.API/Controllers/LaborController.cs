using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaborController : ControllerBase
    {
        private readonly ILaborService _laborService;

        public LaborController(ILaborService laborService)
        {
            _laborService = laborService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Labor>>> GetAllLabor()
        {
            return Ok(await _laborService.GetAllLabor());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Labor>> GetLaborById(int id)
        {
            var labor = await _laborService.GetLaborById(id);
            if (labor == null)
            {
                return NotFound();
            }
            return Ok(labor);
        }

        [HttpPost]
        public async Task<ActionResult<Labor>> AddLabor(Labor labor)
        {
            var newLabor = await _laborService.AddLabor(labor);
            return CreatedAtAction(nameof(GetLaborById), new { id = newLabor.LaborId }, newLabor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLabor(int id, Labor labor)
        {
            if (id != labor.LaborId)
            {
                return BadRequest();
            }

            await _laborService.UpdateLabor(labor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLabor(int id)
        {
            await _laborService.DeleteLabor(id);
            return NoContent();
        }

        [HttpGet("index")]
        public async Task<ActionResult<IEnumerable<LaborIndexDto>>> GetLaborIndex()
        {
            return Ok(await _laborService.GetLaborIndex());
        }
    }
}

