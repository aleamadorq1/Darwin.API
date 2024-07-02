using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{

    public interface IMaterialService
    {
        Task<IEnumerable<Material>> GetAllMaterials();
        Task<Material> GetMaterialById(int id);
        Task<Material> AddMaterial(Material material);
        Task<Material> UpdateMaterial(Material material);
        Task<bool> DeleteMaterial(int id);
        Task<IEnumerable<MaterialIndexDto>> GetMaterialIndex();
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

        public async Task<IEnumerable<Material>> GetAllMaterials()
        {
            return await _materialRepository.GetAllAsync();
        }

        public async Task<Material> GetMaterialById(int id)
        {
            return await _materialRepository.GetByIdAsync(id);
        }

        public async Task<Material> AddMaterial(Material material)
        {
            return await _materialRepository.AddAsync(material);
        }

        public async Task<Material> UpdateMaterial(Material material)
        {
            return await _materialRepository.UpdateAsync(material);
        }

        public async Task<bool> DeleteMaterial(int id)
        {
            return await _materialRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<MaterialIndexDto>> GetMaterialIndex()
        {
            var materialDtos = await _context.Materials
                .Join(_context.Suppliers,
                    material => material.SupplierId,
                    supplier => supplier.SupplierId,
                    (material, supplier) => new { material, supplier })
                .Join(_context.Categories,
                    ms => ms.material.CategoryId,
                    category => category.CategoryId,
                    (ms, category) => new MaterialIndexDto
                    {
                        MaterialId = ms.material.MaterialId,
                        MaterialName = ms.material.MaterialName,
                        Sku = ms.material.Sku,
                        UnitPrice = (decimal)ms.material.UnitPrice,
                        Uom = ms.material.Uom,
                        TaxStatus = ms.material.TaxStatus,
                        CifPrice = (decimal)ms.material.CifPrice,
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

