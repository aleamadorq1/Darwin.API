using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface IDistributionCenterService
    {
        Task<IEnumerable<DistributionCenterDto>> GetAllDistributionCenters();
        Task<DistributionCenterDto> GetDistributionCenterById(int id);
        Task<DistributionCenterDto> AddDistributionCenter(DistributionCenterDto client);
        Task<DistributionCenterDto> UpdateDistributionCenter(DistributionCenterDto client);
        Task<bool> DeleteDistributionCenter(int id);
    }

    public class DistributionCenterService : IDistributionCenterService
    {
        private readonly IRepository<DistributionCenter> _distributionCenterRepository;

        public DistributionCenterService(IRepository<DistributionCenter> distributionCenterRepository)
        {
            _distributionCenterRepository = distributionCenterRepository;
        }

        public async Task<IEnumerable<DistributionCenterDto>> GetAllDistributionCenters()
        {
            var result = await _distributionCenterRepository.GetAllAsync();
            return result.Select(c => new DistributionCenterDto
            {
                DistributionCenterId = c.DistributionCenterId,
                Name = c.Name,
                Location= c.Location,
                LocationAddress = c.LocationAddress,
                LocationCoordinates = c.LocationCoordinates,
                LastModified = c.LastModified
            }).ToList();
        }

        public async Task<DistributionCenterDto> GetDistributionCenterById(int id)
        {
            var result = await _distributionCenterRepository.GetByIdAsync(id);
            return new DistributionCenterDto
            {
                DistributionCenterId = result.DistributionCenterId,
                Name = result.Name,
                Location= result.Location,
                LocationAddress = result.LocationAddress,
                LocationCoordinates = result.LocationCoordinates,
                LastModified = result.LastModified
            };
        }

        public async Task<DistributionCenterDto> AddDistributionCenter(DistributionCenterDto center)
        {
            DistributionCenter newDistibutionCenter = new DistributionCenter
            {
                Name = center.Name,
                Location = center.Location,
                LocationAddress = center.LocationAddress,
                LocationCoordinates = center.LocationCoordinates,
                LastModified = DateTime.Now
            };
            await _distributionCenterRepository.AddAsync(newDistibutionCenter);
            return center; 
        }

        public async Task<DistributionCenterDto> UpdateDistributionCenter(DistributionCenterDto center)
        {
            var existingCenter = await _distributionCenterRepository.GetByIdAsync(center.DistributionCenterId);
            existingCenter.Name = center.Name;
            existingCenter.Location = center.Location;
            existingCenter.LocationAddress = center.LocationAddress;
            existingCenter.LocationCoordinates = center.LocationCoordinates;
            existingCenter.LastModified = DateTime.Now;
            await _distributionCenterRepository.UpdateAsync(existingCenter);
            return center;
        }

        public async Task<bool> DeleteDistributionCenter(int id)
        {
            return await _distributionCenterRepository.DeleteAsync(id);
        }
    }
}

