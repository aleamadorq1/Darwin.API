using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Darwin.API.Services
{
    public interface IModuleService
    {
        Task<IEnumerable<Module>> GetAllModules();
        Task<ModuleDto> GetModuleById(int id);
        Task<Module> AddModule(Module module);
        Task<ModuleDto> UpdateModule(ModuleDto module);
        Task<bool> DeleteModule(int id);
        Task<IEnumerable<ModuleDto>> GetModuleIndex();
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

        public async Task<ModuleDto> GetModuleById(int id)
        {
            var result =await _moduleRepository.FindAsync(m=>m.ModuleId == id, query =>query.Include(m => m.ModulesMaterials).ThenInclude(m=>m.Material), query=>query.Include(m => m.ModulesLabors).ThenInclude(l=>l.Labor), query=>query.Include(m => m.System));
            if (result == null)
            {
                return null;
            }

            return result.Select(m => new ModuleDto
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                ModuleSystem = m.System.Description,
                SystemId = m.SystemId,
                Description = m.Description,
                ModuleMaterials = m.ModulesMaterials.Select(mm => new ModuleMaterialsDto
                {
                    MaterialId = mm.MaterialId,
                    ModuleId = mm.ModuleId,
                    ModuleMaterialId = mm.ModuleMaterialId,
                    MaterialName = mm.Material.MaterialName,
                    Quantity = mm.Quantity,
                }).ToList(),
                ModuleLabors = m.ModulesLabors.Select(ml => new ModulesLaborDto
                {
                    LaborId = ml.LaborId,
                    ModuleId = ml.ModuleId,
                    ModuleLaborId = ml.ModuleLaborId,
                    LaborType = ml.Labor.LaborType,
                    HoursRequired = ml.HoursRequired,
                }).ToList()
            }).FirstOrDefault();
        }

        public async Task<Module> AddModule(Module module)
        {
            return await _moduleRepository.AddAsync(module);
        }

        public async Task<bool> DeleteModule(int id)
        {
            return await _moduleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ModuleDto>> GetModuleIndex()
        {
            var modules = await _moduleRepository.FindAsync(m=>m.ModuleId > 0, query =>query.Include(m => m.ModulesMaterials).ThenInclude(m=>m.Material), query=>query.Include(m => m.ModulesLabors).ThenInclude(l=>l.Labor), query=>query.Include(m => m.System));

            return modules.Select(m => new ModuleDto
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                ModuleSystem = m.System.Description,
                SystemId = m.SystemId,
                Description = m.Description,
                LastModified = m.LastModified ,
                ModuleMaterials = m.ModulesMaterials.Select(mm => new ModuleMaterialsDto
                {
                    MaterialId = mm.MaterialId,
                    ModuleId = mm.ModuleId,
                    ModuleMaterialId = mm.ModuleMaterialId,
                    MaterialName = mm.Material.MaterialName,
                    Quantity = mm.Quantity,
                }).ToList(),
                ModuleLabors = m.ModulesLabors.Select(ml => new ModulesLaborDto
                {
                    LaborId = ml.LaborId,
                    ModuleId = ml.ModuleId,
                    ModuleLaborId = ml.ModuleLaborId,
                    LaborType = ml.Labor.LaborType,
                    HoursRequired = ml.HoursRequired,
                }).ToList()
            }).ToList();
        }

        public async Task<ModuleDto> UpdateModule(ModuleDto module)
        {
            var query =await _moduleRepository.FindAsync(m=>m.ModuleId == module.ModuleId, query =>query.Include(m => m.ModulesMaterials).ThenInclude(m=>m.Material), query=>query.Include(m => m.ModulesLabors).ThenInclude(l=>l.Labor), query=>query.Include(m => m.System));
            var existingModule = query.FirstOrDefault();
            if (existingModule == null)
            {
                return null;
            }
            else
            {
                existingModule.ModuleName = module.ModuleName;
                existingModule.SystemId = module.SystemId;
                existingModule.Description = module.Description;
            }
            foreach (var material in module.ModuleMaterials)
            {
                var queryMaterials = await _moduleMaterialRepository.FindAsync(m=> m.ModuleMaterialId == material.ModuleMaterialId);
                var existingMaterial = queryMaterials.FirstOrDefault();
                if (existingMaterial == null)
                {
                    existingMaterial = new ModulesMaterial
                    {
                        ModuleId = module.ModuleId,
                        MaterialId = material.MaterialId,
                        Quantity = material.Quantity
                    };
                    await _moduleMaterialRepository.AddAsync(existingMaterial);
                }
                else
                {
                    existingMaterial.MaterialId = material.MaterialId;
                    existingMaterial.Quantity = material.Quantity;
                    await _moduleMaterialRepository.UpdateAsync(existingMaterial);
                }
            }
            
            foreach (var existingMaterial in existingModule.ModulesMaterials)
            {
                if (!module.ModuleMaterials.Any(m => m.MaterialId == existingMaterial.MaterialId))
                {
                    await _moduleMaterialRepository.DeleteAsync(existingMaterial.ModuleMaterialId);
                }
            }

            foreach (var labor in module.ModuleLabors)
            {
                var queryLabor = await _moduleLaborRepository.FindAsync(l =>l.ModuleLaborId ==labor.ModuleLaborId);
                var existingLabor = queryLabor.FirstOrDefault();
                if (existingLabor == null)
                {
                    existingLabor = new ModulesLabor
                    {
                        ModuleId = module.ModuleId,
                        LaborId = labor.LaborId,
                        HoursRequired = labor.HoursRequired ?? 0
                    };
                    await _moduleLaborRepository.AddAsync(existingLabor);
                }
                else
                {
                    existingLabor.LaborId = labor.LaborId;
                    existingLabor.HoursRequired = labor.HoursRequired ?? 0;
                    await _moduleLaborRepository.UpdateAsync(existingLabor);
                }
            }
            foreach (var existingLabor in existingModule.ModulesLabors)
            {
                if (!module.ModuleLabors.Any(l => l.LaborId == existingLabor.LaborId))
                {
                    await _moduleLaborRepository.DeleteAsync(existingLabor.ModuleLaborId);
                }
            }
            await _moduleRepository.UpdateAsync(existingModule);
            return module;
        }


    }
}

