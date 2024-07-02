using System;
using Alpha.API.Dtos;
using Alpha.API.Models;
using Alpha.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Alpha.API.Services
{
    public interface IModuleMaterialsService
    {
        Task<ModulesMaterial> GetModuleMaterialById(int id);
        Task<ModulesMaterial> AddModuleMaterial(ModulesMaterial moduleMaterial);
        Task<ModulesMaterial> UpdateModuleMaterial(ModulesMaterial moduleMaterial);
        Task<bool> DeleteModuleMaterial(int id);
        Task<IEnumerable<ModuleMaterialsDto>> GetModuleMaterialsByModuleId(int moduleId);
    }

    public class ModuleMaterialsService : IModuleMaterialsService
    {
        private readonly IModuleMaterialsRepository _moduleMaterialsRepository;

        public ModuleMaterialsService(IModuleMaterialsRepository moduleMaterialsRepository)
        {
            _moduleMaterialsRepository = moduleMaterialsRepository;
        }

        public async Task<ModulesMaterial> GetModuleMaterialById(int id)
        {
            return await _moduleMaterialsRepository.GetByIdAsync(id);
        }

        public async Task<ModulesMaterial> AddModuleMaterial(ModulesMaterial moduleMaterial)
        {
            return await _moduleMaterialsRepository.AddAsync(moduleMaterial);
        }

        public async Task<ModulesMaterial> UpdateModuleMaterial(ModulesMaterial moduleMaterial)
        {
            return await _moduleMaterialsRepository.UpdateAsync(moduleMaterial);
        }

        public async Task<bool> DeleteModuleMaterial(int id)
        {
            return await _moduleMaterialsRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ModuleMaterialsDto>> GetModuleMaterialsByModuleId(int moduleId)
        {
            var moduleMaterials = await _moduleMaterialsRepository.GetByModuleIdAsync(moduleId);
            return moduleMaterials.Select(mm => new ModuleMaterialsDto
            {
                ModuleMaterialId = mm.ModuleMaterialId,
                ModuleId = mm.ModuleId,
                ModuleName = mm.Module.ModuleName,
                MaterialId = mm.MaterialId,
                MaterialName = mm.Material.MaterialName,
                Quantity = mm.Quantity
            }).ToList();
        }
    }
}

