using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxRateController : ControllerBase
    {
        private readonly ITaxRateService _taxRateService;

        public TaxRateController(ITaxRateService taxRateService)
        {
            _taxRateService = taxRateService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaxRateDto>>> GetAllTaxRates()
        {
            return Ok(await _taxRateService.GetAllTaxRates());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaxRateDto>> GetTaxRateById(int id)
        {
            return Ok(await _taxRateService.GetTaxRateById(id));
        }

        [HttpPost]
        public async Task<ActionResult<TaxRateDto>> AddTaxRate(TaxRateDto taxRate)
        {
            return Ok(await _taxRateService.AddTaxRate(taxRate));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaxRateDto>> UpdateTaxRate(int id, TaxRateDto taxRate)
        {
            if (id != taxRate.TaxRateId)
            {
                return BadRequest();
            }

            return Ok(await _taxRateService.UpdateTaxRate(taxRate));
        }
    }
}