using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface IModulesCompositeService
    {
        Task<IEnumerable<ModulesCompositeDto>> GetAllModulesComposites();
        Task<ModulesCompositeDto> GetModulesCompositeById(int id);
        Task<ModulesCompositeDto> AddModulesComposite(ModulesCompositeDto modulesComposite);
        Task<ModulesCompositeDto> UpdateModulesComposite(ModulesCompositeDto modulesComposite);
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
            if (modulesComposite == null) return new ModulesCompositeDto();

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

        public async Task<ModulesCompositeDto> AddModulesComposite(ModulesCompositeDto modulesComposite)
        {
            var newModuleComposite = new ModulesComposite
            {
                CompositeName = modulesComposite.CompositeName ?? "",
                Description = modulesComposite.Description ?? "",
                ModuleCompositeDetails = modulesComposite.ModuleCompositeDetails.Select(mcd => new ModuleCompositeDetail
                {
                    ModuleId = mcd.ModuleId,
                    Quantity = mcd.Quantity
                }).ToList()
            };
            var addedModulesComposite = await _modulesCompositeRepository.AddAsync(newModuleComposite);

            return modulesComposite;
        }


        public async Task<ModulesCompositeDto> UpdateModulesComposite(ModulesCompositeDto modulesComposite)
        {
            var existingModulesComposite = await _modulesCompositeRepository.GetByIdAsync(modulesComposite.ModuleCompositeId);
            if (existingModulesComposite == null) return new ModulesCompositeDto();

            existingModulesComposite.CompositeName = modulesComposite.CompositeName ?? existingModulesComposite.CompositeName;
            existingModulesComposite.Description = modulesComposite.Description ?? existingModulesComposite.Description;

            var updatedModulesComposite = await _modulesCompositeRepository.UpdateAsync(existingModulesComposite);

            var existingModuleCompositeDetails = await _modulesCompositeDetailRepository.FindAsync(mcd => mcd.ModuleCompositeId == modulesComposite.ModuleCompositeId);

            // Update and remove existing details
            foreach (var existingModuleCompositeDetail in existingModuleCompositeDetails)
            {
                var moduleCompositeDetail = modulesComposite.ModuleCompositeDetails.FirstOrDefault(mcd => mcd.ModuleCompositeDetailId == existingModuleCompositeDetail.ModuleCompositeDetailId);
                if (moduleCompositeDetail == null)
                {
                    await _modulesCompositeDetailRepository.DeleteAsync(existingModuleCompositeDetail.ModuleCompositeDetailId);
                }
                else
                {
                    existingModuleCompositeDetail.ModuleId = moduleCompositeDetail.ModuleId;
                    existingModuleCompositeDetail.Quantity = moduleCompositeDetail.Quantity;
                    await _modulesCompositeDetailRepository.UpdateAsync(existingModuleCompositeDetail);
                }
            }

            // Add new details
            foreach (var moduleCompositeDetail in modulesComposite.ModuleCompositeDetails ?? new List<ModuleCompositeDetailDto>())
            {
                if (!existingModuleCompositeDetails.Any(mcd => mcd.ModuleCompositeDetailId == moduleCompositeDetail.ModuleCompositeDetailId))
                {
                    var newModuleCompositeDetail = new ModuleCompositeDetail
                    {
                        ModuleCompositeId = modulesComposite.ModuleCompositeId,
                        ModuleId = moduleCompositeDetail.ModuleId,
                        Quantity = moduleCompositeDetail.Quantity
                    };
                    await _modulesCompositeDetailRepository.AddAsync(newModuleCompositeDetail);
                }
            }

            // Return the updated DTO
            return new ModulesCompositeDto
            {
                ModuleCompositeId = updatedModulesComposite.ModuleCompositeId,
                CompositeName = updatedModulesComposite.CompositeName,
                Description = updatedModulesComposite.Description,
                ModuleCompositeDetails = (await _modulesCompositeDetailRepository.FindAsync(mcd => mcd.ModuleCompositeId == updatedModulesComposite.ModuleCompositeId)).Select(mcd => new ModuleCompositeDetailDto
                {
                    ModuleCompositeDetailId = mcd.ModuleCompositeDetailId,
                    ModuleId = mcd.ModuleId,
                    ModuleName = mcd.Module?.ModuleName,
                    Quantity = mcd.Quantity
                }).ToList()
            };
        }


        public async Task<bool> DeleteModulesComposite(int id)
        {
            var moduleCompositeDetails = await _modulesCompositeDetailRepository.FindAsync(mcd => mcd.ModuleCompositeId == id);
            await _modulesCompositeDetailRepository.DeleteRangeAsync(moduleCompositeDetails);
            return await _modulesCompositeRepository.DeleteAsync(id);
        }
    }
}

