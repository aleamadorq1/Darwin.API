using System;
using Alpha.API.Dtos;
using Alpha.API.Models;
using Alpha.API.Repositories;

namespace Alpha.API.Services
{
    public interface ILaborService
    {
        Task<IEnumerable<Labor>> GetAllLabor();
        Task<Labor> GetLaborById(int id);
        Task<Labor> AddLabor(Labor labor);
        Task<Labor> UpdateLabor(Labor labor);
        Task<bool> DeleteLabor(int id);
        Task<IEnumerable<LaborIndexDto>> GetLaborIndex();
    }

    public class LaborService : ILaborService
    {
        private readonly IRepository<Labor> _laborRepository;

        public LaborService(IRepository<Labor> laborRepository)
        {
            _laborRepository = laborRepository;
        }

        public async Task<IEnumerable<Labor>> GetAllLabor()
        {
            return await _laborRepository.GetAllAsync();
        }

        public async Task<Labor> GetLaborById(int id)
        {
            return await _laborRepository.GetByIdAsync(id);
        }

        public async Task<Labor> AddLabor(Labor labor)
        {
            return await _laborRepository.AddAsync(labor);
        }

        public async Task<Labor> UpdateLabor(Labor labor)
        {
            return await _laborRepository.UpdateAsync(labor);
        }

        public async Task<bool> DeleteLabor(int id)
        {
            return await _laborRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<LaborIndexDto>> GetLaborIndex()
        {
            var laborList = await _laborRepository.GetAllAsync();
            return laborList.Select(l => new LaborIndexDto
            {
                LaborId = l.LaborId,
                LaborType = l.LaborType,
                HourlyRate = l.HourlyRate,
                Description = l.Description,
                LastModified = l.LastModified
            }).ToList();
        }
    }
}

