using System;
using Alpha.API.Dtos;
using Alpha.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpha.API.Repositories;

namespace Alpha.API.Services
{
    public interface IModulesCompositeService
    {
        Task<IEnumerable<ModulesCompositeDto>> GetAllModulesComposites();
        Task<ModulesCompositeDto> GetModulesCompositeById(int id);
        Task<ModulesComposite> AddModulesComposite(ModulesComposite modulesComposite);
        Task<ModulesComposite> UpdateModulesComposite(ModulesComposite modulesComposite);
        Task<bool> DeleteModulesComposite(int id);
    }

    public class ModulesCompositeService : IModulesCompositeService
    {
        private readonly IModulesCompositeRepository _modulesCompositeRepository;
        private readonly IRepository<ModuleCompositeDetail> _modulesCompositeDetailRepository;

        public ModulesCompositeService(IModulesCompositeRepository modulesCompositeRepository, IRepository<ModuleCompositeDetail> modulesCompositeDetailRepository)
        {
            _modulesCompositeRepository = modulesCompositeRepository;
            _modulesCompositeDetailRepository = modulesCompositeDetailRepository;
        }

        public async Task<IEnumerable<ModulesCompositeDto>> GetAllModulesComposites()
        {
            var modulesComposites = await _modulesCompositeRepository.GetAllWithDetailsAsync();
            return modulesComposites.Select(mc => new ModulesCompositeDto
            {
                ModuleCompositeId = mc.ModuleCompositeId,
                CompositeName = mc.CompositeName,
                Description = mc.Description,
                ModuleCompositeDetails = mc.ModuleCompositeDetails.Select(mcd => new ModuleCompositeDetailDto
                {
                    ModuleCompositeDetailId = mcd.ModuleCompositeDetailId,
                    ModuleId = mcd.ModuleId,
                    ModuleName = mcd.Module?.ModuleName,
                    Quantity = mcd.Quantity
                }).ToList()
            }).ToList();
        }

        public async Task<ModulesCompositeDto> GetModulesCompositeById(int id)
        {
            var modulesComposite = await _modulesCompositeRepository.GetWithDetailsByIdAsync(id);
            if (modulesComposite == null) return null;

            return new ModulesCompositeDto
            {
                ModuleCompositeId = modulesComposite.ModuleCompositeId,
                CompositeName = modulesComposite.CompositeName,
                Description = modulesComposite.Description,
                ModuleCompositeDetails = modulesComposite.ModuleCompositeDetails?.Select(mcd => new ModuleCompositeDetailDto
                {
                    ModuleCompositeDetailId = mcd.ModuleCompositeDetailId,
                    ModuleId = mcd.ModuleId,
                    ModuleName = mcd.Module?.ModuleName,
                    Quantity = mcd.Quantity
                }).ToList() ?? new List<ModuleCompositeDetailDto>()
            };
        }

        public async Task<ModulesComposite> AddModulesComposite(ModulesComposite modulesComposite)
        {
            var addedModulesComposite = await _modulesCompositeRepository.AddAsync(modulesComposite);

            return addedModulesComposite;
        }


        public async Task<ModulesComposite> UpdateModulesComposite(ModulesComposite modulesComposite)
        {
            await _modulesCompositeRepository.UpdateAsync(modulesComposite);

            // Delete Modules
            var existingModuleDetails = await _modulesCompositeRepository.GetByModuleCompositeIdAsync(modulesComposite.ModuleCompositeId);

            await _modulesCompositeDetailRepository.DeleteRangeAsync(existingModuleDetails);


            // Add new labor and material records
            await _modulesCompositeDetailRepository.AddRangeAsync(modulesComposite.ModuleCompositeDetails);

            return modulesComposite;
        }

        public async Task<bool> DeleteModulesComposite(int id)
        {
            return await _modulesCompositeRepository.DeleteAsync(id);
        }
    }
}

