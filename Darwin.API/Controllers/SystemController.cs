using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
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

    }
}

