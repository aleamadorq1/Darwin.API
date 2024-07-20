using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{

    public interface IHandlingCostService
    {
        Task<IEnumerable<HandlingCostDto>> GetAllHandlingCosts();
        Task<HandlingCostDto> GetHandlingCostById(int id);
        Task<HandlingCostDto> AddHandlingCost(HandlingCostDto handlingCost);
        Task<HandlingCostDto> UpdateHandlingCost(HandlingCostDto handlingCost);
        Task<bool> DeleteHandlingCost(int id);
    }

    public class HandlingCostService : IHandlingCostService
    {
        private readonly IRepository<HandlingCost> _handlingCostRepository;

        public HandlingCostService(IRepository<HandlingCost> handlingCostRepository)
        {
            _handlingCostRepository = handlingCostRepository;
        }

        public async Task<IEnumerable<HandlingCostDto>> GetAllHandlingCosts()
        {
            var result = await _handlingCostRepository.GetAllAsync();
            return result.Select(h => new HandlingCostDto
            {
                HandlingCostId = h.HandlingCostId,
                Cost = h.Cost,
                Description = h.Description + " - " + h.Cost + "$"
            });
        }

        public async Task<HandlingCostDto> GetHandlingCostById(int id)
        {
            var handlingCost = await _handlingCostRepository.GetByIdAsync(id);
            return new HandlingCostDto
            {
                HandlingCostId = handlingCost.HandlingCostId,
                Cost = handlingCost.Cost,
                Description = handlingCost.Description + " - " + handlingCost.Cost + "$"
            };
        }

        public async Task<HandlingCostDto> AddHandlingCost(HandlingCostDto handlingCost)
        {
            var newHandlingCost = await _handlingCostRepository.AddAsync(new HandlingCost
            {
                Cost = handlingCost.Cost,
                Description = handlingCost.Description
            });
            return new HandlingCostDto
            {
                HandlingCostId = newHandlingCost.HandlingCostId,
                Cost = newHandlingCost.Cost,
                Description = newHandlingCost.Description + " - " + newHandlingCost.Cost + "$"
            };
        }

        public async Task<HandlingCostDto> UpdateHandlingCost(HandlingCostDto handlingCost)
        {
            var updatedHandlingCost = await _handlingCostRepository.UpdateAsync(new HandlingCost
            {
                HandlingCostId = handlingCost.HandlingCostId,
                Cost = handlingCost.Cost,
                Description = handlingCost.Description
            });
            return new HandlingCostDto
            {
                HandlingCostId = updatedHandlingCost.HandlingCostId,
                Cost = updatedHandlingCost.Cost,
                Description = updatedHandlingCost.Description  + " - " + updatedHandlingCost.Cost + "$"
            };
        }

        public async Task<bool> DeleteHandlingCost(int id)
        {
            return await _handlingCostRepository.DeleteAsync(id);
        }
    }
}