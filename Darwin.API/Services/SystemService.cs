using Darwin.API.Dtos;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface ISystemService
    {
        Task<IEnumerable<SystemDto>> GetAllSystems();
        Task<SystemDto> GetSystemById(int id);
        Task<SystemDto> AddSystem(SystemDto system);
        Task<SystemDto> UpdateSystem(SystemDto system);
        Task<bool> DeleteSystem(int id);
    }

    public class SystemService : ISystemService
    {
        private readonly IRepository<Models.System> _systemRepository;

        public SystemService(IRepository<Models.System> systemRepository)
        {
            _systemRepository = systemRepository;
        }

        public async Task<IEnumerable<SystemDto>> GetAllSystems()
        {
            var result = await _systemRepository.GetAllAsync();
            return result.Select(s => new SystemDto
            {
                SystemId = s.SystemId,
                Description = s.Description
            });
        }

        public async Task<SystemDto> GetSystemById(int id)
        {
            var system = await _systemRepository.GetByIdAsync(id);
            return new SystemDto
            {
                SystemId = system.SystemId,
                Description = system.Description
            };
        }

        public async Task<SystemDto> AddSystem(SystemDto system)
        {
            var newSystem = await _systemRepository.AddAsync(new Models.System
            {
                Description = system.Description??string.Empty
            });
            return new SystemDto
            {
                SystemId = newSystem.SystemId,
                Description = newSystem.Description
            };
        }

        public async Task<SystemDto> UpdateSystem(SystemDto system)
        {
            var updatedSystem = await _systemRepository.UpdateAsync(new Models.System
            {
                SystemId = system.SystemId,
                Description = system.Description ?? string.Empty
            });
            return new SystemDto
            {
                SystemId = updatedSystem.SystemId,
                Description = updatedSystem.Description
            };
        }

        public async Task<bool> DeleteSystem(int id)
        {
            return await _systemRepository.DeleteAsync(id);
        }

    }
}

