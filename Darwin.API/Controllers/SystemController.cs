using Darwin.API.Dtos;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly ISystemService _SystemService;

        public SystemController(ISystemService SystemService)
        {
            _SystemService = SystemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemDto>>> GetAllSystem()
        {
            return Ok(await _SystemService.GetAllSystems());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SystemDto>> GetSystemById(int id)
        {
            return Ok(await _SystemService.GetSystemById(id));
        }

        [HttpPost]
        public async Task<ActionResult<SystemDto>> AddSystem(SystemDto system)
        {
            return Ok(await _SystemService.AddSystem(system));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SystemDto>> UpdateSystem(int id, SystemDto system)
        {
            if (id != system.SystemId)
            {
                return BadRequest();
            }

            return Ok(await _SystemService.UpdateSystem(system));
        }


    }
}

