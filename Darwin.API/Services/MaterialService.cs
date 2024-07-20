using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{

    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDto>> GetAllMaterials();
        Task<MaterialDto?> GetMaterialById(int id);
        Task<MaterialDto?> AddMaterial(MaterialDto material);
        Task<MaterialDto?> UpdateMaterial(MaterialDto material);
        Task<bool> DeleteMaterial(int id);
        Task<IEnumerable<MaterialDto>> GetMaterialIndex();
    }

    public class MaterialService : IMaterialService
    {
        private readonly IRepository<Material> _materialRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<TaxRate> _taxRateRepository;
        private readonly IRepository<HandlingCost> _handlingCostRepository;
        private readonly AlphaDbContext _context;

        public MaterialService(
            IRepository<Material> materialRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Category> categoryRepository,
            IRepository<TaxRate> taxRateRepository,
            IRepository<HandlingCost> handlingCostRepository,
            AlphaDbContext context)
        {
            _materialRepository = materialRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
            _taxRateRepository = taxRateRepository;
            _handlingCostRepository = handlingCostRepository;
            _context = context;
        }

        public async Task<IEnumerable<MaterialDto>> GetAllMaterials()
        {
            var materials = await _materialRepository.GetAllAsync();
            var suppliers = await _supplierRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            return materials.Select(m => new MaterialDto
            {
                MaterialId = m.MaterialId,
                MaterialName = m.MaterialName,
                Sku = m.Sku,
                UnitPrice = m.UnitPrice,
                Uom = m.Uom,
                TaxRateId = m.TaxRateId,
                HandlingCostId = m.HandlingCostId,
                CifPrice = m.CifPrice,
                SupplierId = m.SupplierId,
                Supplier = suppliers.FirstOrDefault(s => s.SupplierId == m.SupplierId)?.SupplierName,
                CategoryId = m.CategoryId,
                Category = categories.FirstOrDefault(c => c.CategoryId == m.CategoryId)?.CategoryName
            }).ToList();
        }

        public async Task<MaterialDto?> GetMaterialById(int id)
        {
            var result = await _materialRepository.FindAsync(m => m.MaterialId == id, query => query.Include<Material, object>(m => m.TaxRate), query => query.Include<Material, object>(m => m.HandlingCost));
            var material = result.FirstOrDefault();
            if (material == null)
            {
                return null; 
            }
            else
            {
                var supplier = await _supplierRepository.GetByIdAsync(material.SupplierId);
                var category = await _categoryRepository.GetByIdAsync(material.CategoryId);
        
                return new MaterialDto
                {
                    MaterialId = material.MaterialId,
                    MaterialName = material.MaterialName,
                    Sku = material.Sku,
                    UnitPrice = material.UnitPrice,
                    Uom = material.Uom,
                    TaxRateId = material.TaxRateId,
                    TaxRate = material.TaxRate?.Rate ?? 0,
                    HandlingCostId = material.HandlingCostId,
                    HandlingCost = material.HandlingCost?.Cost ?? 0,
                    CifPrice = material.CifPrice,
                    SupplierId = material.SupplierId,
                    Supplier = supplier.SupplierName,
                    CategoryId = material.CategoryId,
                    Category = category.CategoryName
                };
            }
        }

        public async Task<MaterialDto?> AddMaterial(MaterialDto material)
        {
            var newMaterial = new Material
            {
                MaterialName = material.MaterialName ?? "",
                Sku = material.Sku,
                UnitPrice = material.UnitPrice,
                Uom = material.Uom,
                TaxRateId = material.TaxRateId,
                HandlingCostId = material.HandlingCostId,
                CifPrice = material.CifPrice ?? 0,
                SupplierId = material.SupplierId,
                CategoryId = material.CategoryId
                };
            var result = await _materialRepository.AddAsync(newMaterial);
            return new MaterialDto
            {
                MaterialId = result.MaterialId,
                MaterialName = result.MaterialName,
                Sku = result.Sku,
                UnitPrice = result.UnitPrice,
                Uom = result.Uom,
                TaxRateId = result.TaxRateId,
                HandlingCostId = result.HandlingCostId,
                CifPrice = result.CifPrice,
                SupplierId = result.SupplierId,
                CategoryId = result.CategoryId
            };
        }

        public async Task<MaterialDto?> UpdateMaterial(MaterialDto material)
        {
            var existingMaterial = await _materialRepository.GetByIdAsync(material.MaterialId);
            if (existingMaterial == null)
            {
                return null;
            }
            else
            {
                existingMaterial.MaterialName = material.MaterialName ?? existingMaterial.MaterialName;
                existingMaterial.Sku = material.Sku;
                existingMaterial.UnitPrice = material.UnitPrice;
                existingMaterial.Uom = material.Uom;
                existingMaterial.TaxRateId = material.TaxRateId;
                existingMaterial.HandlingCostId = material.HandlingCostId;
                existingMaterial.CifPrice = material.CifPrice??0;
                existingMaterial.SupplierId = material.SupplierId;
                existingMaterial.CategoryId = material.CategoryId;
            }
            await _materialRepository.UpdateAsync(existingMaterial);
            return material;
        }

        public async Task<bool> DeleteMaterial(int id)
        {
            return await _materialRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaterialDto>> GetMaterialIndex()
        {
            var taxRates = await _taxRateRepository.GetAllAsync();
            var handlingCosts = await _handlingCostRepository.GetAllAsync();
            var materialDtos = await _context.Materials
                .Join(_context.Suppliers,
                    material => material.SupplierId,
                    supplier => supplier.SupplierId,
                    (material, supplier) => new { material, supplier })
                .Join(_context.Categories,
                    ms => ms.material.CategoryId,
                    category => category.CategoryId,
                    (ms, category) => new { ms, category })
                .Select(msc => new
                {
                    msc.ms.material,
                    msc.ms.supplier,
                    msc.category
                })
                .ToListAsync();

            var result = materialDtos.Select(m => new MaterialDto
            {
                MaterialId = m.material.MaterialId,
                MaterialName = m.material.MaterialName,
                Sku = m.material.Sku,
                UnitPrice = m.material.UnitPrice,
                Uom = m.material.Uom,
                TaxRateId = m.material.TaxRateId,
                TaxRate = taxRates.FirstOrDefault(t => t.TaxRateId == m.material.TaxRateId)?.Rate ?? 0,
                HandlingCostId = m.material.HandlingCostId,
                HandlingCost = handlingCosts.FirstOrDefault(h => h.HandlingCostId == m.material.HandlingCostId)?.Cost ?? 0,
                CifPrice = m.material.CifPrice,
                SupplierId = m.supplier.SupplierId,
                Supplier = m.supplier.SupplierName,
                CategoryId = m.category.CategoryId,
                Category = m.category.CategoryName
            });

            return result;
        }
    }
}

