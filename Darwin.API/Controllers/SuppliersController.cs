using Darwin.API.Models;
using Darwin.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Darwin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAllSuppliers()
        {
            return Ok(await _supplierService.GetAllSuppliers());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplierById(int id)
        {
            var supplier = await _supplierService.GetSupplierById(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> AddSupplier(Supplier supplier)
        {
            var newSupplier = await _supplierService.AddSupplier(supplier);
            return CreatedAtAction(nameof(GetSupplierById), new { id = newSupplier.SupplierId }, newSupplier);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }

            await _supplierService.UpdateSupplier(supplier);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            await _supplierService.DeleteSupplier(id);
            return NoContent();
        }
    }
}

