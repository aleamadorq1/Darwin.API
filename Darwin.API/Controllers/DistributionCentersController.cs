using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistributionCentersController : ControllerBase
    {
        private readonly IDistributionCenterService _dcService;

        public DistributionCentersController(IDistributionCenterService dcService)
        {
            _dcService = dcService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DistributionCenterDto>>> GetAllDistributionCenters()
        {
            return Ok(await _dcService.GetAllDistributionCenters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DistributionCenterDto>> GetDistributionCenterById(int id)
        {
            var distributionCenter = await _dcService.GetDistributionCenterById(id);
            if (distributionCenter == null)
            {
                return NotFound();
            }
            return Ok(distributionCenter);
        }

        [HttpPost]
        public async Task<ActionResult<DistributionCenterDto>> AddDistributionCenter(DistributionCenterDto distributionCenter)
        {
            var newDC = await _dcService.AddDistributionCenter(distributionCenter);
            return CreatedAtAction(nameof(AddDistributionCenter), new { id = newDC.DistributionCenterId }, distributionCenter);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDistributionCenters(int id, DistributionCenterDto distributionCenter)
        {
            if (id != distributionCenter.DistributionCenterId)
            {
                return BadRequest();
            }

            await _dcService.UpdateDistributionCenter(distributionCenter);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDistributionCenters(int id)
        {
            await _dcService.DeleteDistributionCenter(id);
            return NoContent();
        }
    }
}
