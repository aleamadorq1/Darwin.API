using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{

    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDto>> GetAllMaterials();
        Task<MaterialDto> GetMaterialById(int id);
        Task<MaterialDto> AddMaterial(MaterialDto material);
        Task<MaterialDto> UpdateMaterial(MaterialDto material);
        Task<bool> DeleteMaterial(int id);
        Task<IEnumerable<MaterialDto>> GetMaterialIndex();
    }

    public class MaterialService : IMaterialService
    {
        private readonly IRepository<Material> _materialRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly AlphaDbContext _context;

        public MaterialService(
            IRepository<Material> materialRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Category> categoryRepository,
            AlphaDbContext context)
        {
            _materialRepository = materialRepository;
            _supplierRepository = supplierRepository;
            _categoryRepository = categoryRepository;
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
                TaxStatus = m.TaxStatus,
                CifPrice = m.CifPrice??0,
                SupplierId = m.SupplierId,
                Supplier = suppliers.FirstOrDefault(s => s.SupplierId == m.SupplierId)?.SupplierName,
                CategoryId = m.CategoryId,
                Category = categories.FirstOrDefault(c => c.CategoryId == m.CategoryId)?.CategoryName
            }).ToList();
        }

        public async Task<MaterialDto> GetMaterialById(int id)
        {
            var material = await _materialRepository.GetByIdAsync(id);
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
                    TaxStatus = material.TaxStatus,
                    CifPrice = material.CifPrice,
                    SupplierId = material.SupplierId,
                    Supplier = supplier.SupplierName,
                    CategoryId = material.CategoryId,
                    Category = category.CategoryName
                };
            }
        }

        public async Task<MaterialDto> AddMaterial(MaterialDto material)
        {
            var newMaterial = new Material
            {
                MaterialName = material.MaterialName,
                Sku = material.Sku,
                UnitPrice = material.UnitPrice,
                Uom = material.Uom,
                TaxStatus = material.TaxStatus,
                CifPrice = material.CifPrice,
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
                TaxStatus = result.TaxStatus,
                CifPrice = result.CifPrice,
                SupplierId = result.SupplierId,
                CategoryId = result.CategoryId};
        }

        public async Task<MaterialDto> UpdateMaterial(MaterialDto material)
        {
            var existingMaterial = await _materialRepository.GetByIdAsync(material.MaterialId);
            if (existingMaterial == null)
            {
                return null;
            }
            else
            {
                existingMaterial.MaterialName = material.MaterialName;
                existingMaterial.Sku = material.Sku;
                existingMaterial.UnitPrice = material.UnitPrice;
                existingMaterial.Uom = material.Uom;
                existingMaterial.TaxStatus = material.TaxStatus;
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
            var materialDtos = await _context.Materials
                .Join(_context.Suppliers,
                    material => material.SupplierId,
                    supplier => supplier.SupplierId,
                    (material, supplier) => new { material, supplier })
                .Join(_context.Categories,
                    ms => ms.material.CategoryId,
                    category => category.CategoryId,
                    (ms, category) => new MaterialDto
                    {
                        MaterialId = ms.material.MaterialId,
                        MaterialName = ms.material.MaterialName,
                        Sku = ms.material.Sku,
                        UnitPrice = ms.material.UnitPrice,
                        Uom = ms.material.Uom,
                        TaxStatus = ms.material.TaxStatus,
                        CifPrice = ms.material.CifPrice,
                        SupplierId = ms.supplier.SupplierId,
                        Supplier = ms.supplier.SupplierName,
                        CategoryId = category.CategoryId,
                        Category = category.CategoryName
                    })
                .ToListAsync();

            return materialDtos;
        }
    }
}

