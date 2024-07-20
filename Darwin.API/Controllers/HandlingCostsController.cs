using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandlingCostController : ControllerBase
    {
        private readonly IHandlingCostService _handlingCostService;

        public HandlingCostController(IHandlingCostService handlingCostService)
        {
            _handlingCostService = handlingCostService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HandlingCostDto>>> GetAllHandlingCosts()
        {
            return Ok(await _handlingCostService.GetAllHandlingCosts());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HandlingCostDto>> GetHandlingCostById(int id)
        {
            return Ok(await _handlingCostService.GetHandlingCostById(id));
        }

        [HttpPost]
        public async Task<ActionResult<HandlingCostDto>> AddHandlingCost(HandlingCostDto handlingCost)
        {
            return Ok(await _handlingCostService.AddHandlingCost(handlingCost));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<HandlingCostDto>> UpdateHandlingCost(int id, HandlingCostDto handlingCost)
        {
            if (id != handlingCost.HandlingCostId)
            {
                return BadRequest();
            }

            return Ok(await _handlingCostService.UpdateHandlingCost(handlingCost));
        }
    }
}