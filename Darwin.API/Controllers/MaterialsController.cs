using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetAllMaterials()
        {
            return Ok(await _materialService.GetAllMaterials());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterialById(int id)
        {
            var material = await _materialService.GetMaterialById(id);
            if (material == null)
            {
                return NotFound();
            }
            return Ok(material);
        }

        [HttpPost]
        public async Task<ActionResult<Material>> AddMaterial(Material material)
        {
            var newMaterial = await _materialService.AddMaterial(material);
            return CreatedAtAction(nameof(GetMaterialById), new { id = newMaterial.MaterialId }, newMaterial);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterial(int id, Material material)
        {
            if (id != material.MaterialId)
            {
                return BadRequest();
            }

            await _materialService.UpdateMaterial(material);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            await _materialService.DeleteMaterial(id);
            return NoContent();
        }

        [HttpGet("index")]
        public async Task<ActionResult<IEnumerable<MaterialIndexDto>>> GetMaterialIndex()
        {
            return Ok(await _materialService.GetMaterialIndex());
        }
    }
}
