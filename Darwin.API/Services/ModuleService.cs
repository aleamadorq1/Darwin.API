﻿using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{
    public interface IModuleService
    {
        Task<IEnumerable<Module>> GetAllModules();
        Task<Module> GetModuleById(int id);
        Task<Module> AddModule(Module module);
        Task<Module> UpdateModule(Module module);
        Task<bool> DeleteModule(int id);
        Task<IEnumerable<ModuleIndexDto>> GetModuleIndex();
    }

    public class ModuleService : IModuleService
    {
        private readonly IRepository<Module> _moduleRepository;
        private readonly IModuleMaterialsRepository _moduleMaterialRepository;
        private readonly IModulesLaborRepository _moduleLaborRepository;
        private readonly IRepository<Models.System> _systemRepository;
        private readonly AlphaDbContext _context;

        public ModuleService(IRepository<Module> moduleRepository, IModuleMaterialsRepository moduleMaterialsRepository, IModulesLaborRepository modulesLaborRepository, AlphaDbContext dbContext, IRepository<Models.System> systemRepository)
        {
            _moduleRepository = moduleRepository;
            _moduleMaterialRepository = moduleMaterialsRepository;
            _moduleLaborRepository = modulesLaborRepository;
            _context = dbContext;
            _systemRepository = systemRepository;
        }

        public async Task<IEnumerable<Module>> GetAllModules()
        {
            return await _moduleRepository.GetAllAsync();
        }

        public async Task<Module> GetModuleById(int id)
        {
            return await _moduleRepository.GetByIdAsync(id);
        }

        public async Task<Module> AddModule(Module module)
        {
            return await _moduleRepository.AddAsync(module);
        }

        public async Task<Module> UpdateModule2(Module module)
        {
            return await _moduleRepository.UpdateAsync(module);
        }

        public async Task<bool> DeleteModule(int id)
        {
            return await _moduleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ModuleIndexDto>> GetModuleIndex()
        {
            var modules = await _moduleRepository.GetAllAsync();
            var systems = await _systemRepository.GetAllAsync();

            return modules.Select(m => new ModuleIndexDto
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                ModuleType = systems.FirstOrDefault(s => s.SystemId == m.SystemId)?.Description,
                Description = m.Description
            }).ToList();
        }

        public async Task<Module> UpdateModule(Module module)
        {
            await _moduleRepository.UpdateAsync(module);

            // Delete existing labor and material records
            var existingModuleMaterials = await _moduleMaterialRepository.GetByModuleIdAsync(module.ModuleId);
            var existingModuleLabor = await _moduleLaborRepository.GetByModuleIdAsync(module.ModuleId);

            await _moduleMaterialRepository.DeleteRangeAsync(existingModuleMaterials);
            await _moduleLaborRepository.DeleteRangeAsync(existingModuleLabor);

            // Add new labor and material records
            await _moduleMaterialRepository.AddRangeAsync(module.ModulesMaterials);
            await _moduleLaborRepository.AddRangeAsync(module.ModulesLabors);

            return module;
        }


    }
}

