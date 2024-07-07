using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface ISystemService
    {
        Task<IEnumerable<SystemDto>> GetAllSystems();
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

    }
}

