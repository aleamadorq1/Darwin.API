using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface IModulesLaborService
    {
        Task<ModulesLabor> GetModulesLaborById(int id);
        Task<ModulesLabor> AddModulesLabor(ModulesLabor modulesLabor);
        Task<ModulesLabor> UpdateModulesLabor(ModulesLabor modulesLabor);
        Task<bool> DeleteModulesLabor(int id);
        Task<IEnumerable<ModulesLaborDto>> GetModulesLaborByModuleId(int moduleId);
    }

    public class ModulesLaborService : IModulesLaborService
    {
        private readonly IModulesLaborRepository _modulesLaborRepository;

        public ModulesLaborService(IModulesLaborRepository modulesLaborRepository)
        {
            _modulesLaborRepository = modulesLaborRepository;
        }

        public async Task<ModulesLabor> GetModulesLaborById(int id)
        {
            return await _modulesLaborRepository.GetByIdAsync(id);
        }

        public async Task<ModulesLabor> AddModulesLabor(ModulesLabor modulesLabor)
        {
            return await _modulesLaborRepository.AddAsync(modulesLabor);
        }

        public async Task<ModulesLabor> UpdateModulesLabor(ModulesLabor modulesLabor)
        {
            return await _modulesLaborRepository.UpdateAsync(modulesLabor);
        }

        public async Task<bool> DeleteModulesLabor(int id)
        {
            return await _modulesLaborRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ModulesLaborDto>> GetModulesLaborByModuleId(int moduleId)
        {
            var modulesLabor = await _modulesLaborRepository.GetByModuleIdAsync(moduleId);
            return modulesLabor.Select(ml => new ModulesLaborDto
            {
                ModuleLaborId = ml.ModuleLaborId,
                ModuleId = ml.ModuleId,
                ModuleName = ml.Module.ModuleName,
                LaborId = ml.LaborId,
                LaborType = ml.Labor.LaborType,
                HoursRequired = ml.HoursRequired
            }).ToList();
        }
    }
}

