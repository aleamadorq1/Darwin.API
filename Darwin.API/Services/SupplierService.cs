using System;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliers();
        Task<Supplier> GetSupplierById(int id);
        Task<Supplier> AddSupplier(Supplier supplier);
        Task<Supplier> UpdateSupplier(Supplier supplier);
        Task<bool> DeleteSupplier(int id);
    }

    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _supplierRepository;

        public SupplierService(IRepository<Supplier> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            return await _supplierRepository.GetAllAsync();
        }

        public async Task<Supplier> GetSupplierById(int id)
        {
            return await _supplierRepository.GetByIdAsync(id);
        }

        public async Task<Supplier> AddSupplier(Supplier supplier)
        {
            return await _supplierRepository.AddAsync(supplier);
        }

        public async Task<Supplier> UpdateSupplier(Supplier supplier)
        {
            return await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task<bool> DeleteSupplier(int id)
        {
            return await _supplierRepository.DeleteAsync(id);
        }
    }
}

