using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface ITaxRateService
    {
        Task<IEnumerable<TaxRateDto>> GetAllTaxRates();
        Task<TaxRateDto> GetTaxRateById(int id);
        Task<TaxRateDto> AddTaxRate(TaxRateDto taxRate);
        Task<TaxRateDto> UpdateTaxRate(TaxRateDto taxRate);
        Task<bool> DeleteTaxRate(int id);
    }

    public class TaxRateService : ITaxRateService
    {
        private readonly IRepository<TaxRate> _taxRateRepository;

        public TaxRateService(IRepository<TaxRate> taxRateRepository)
        {
            _taxRateRepository = taxRateRepository;
        }

        public async Task<IEnumerable<TaxRateDto>> GetAllTaxRates()
        {
            var result = await _taxRateRepository.GetAllAsync();
            return result.Select(t => new TaxRateDto
            {
                TaxRateId = t.TaxRateId,
                Rate = t.Rate,
                Description = t.Description + " - " + (t.Rate * 100) + "%"
            });
        }

        public async Task<TaxRateDto> GetTaxRateById(int id)
        {
            var taxRate = await _taxRateRepository.GetByIdAsync(id);
            return new TaxRateDto
            {
                TaxRateId = taxRate.TaxRateId,
                Rate = taxRate.Rate,
                Description = taxRate.Description + " - " + (taxRate.Rate *100) + "%"
            };
        }

        public async Task<TaxRateDto> AddTaxRate(TaxRateDto taxRate)
        {
            var newTaxRate = await _taxRateRepository.AddAsync(new Models.TaxRate
            {
                Rate = taxRate.Rate,
                Description = taxRate.Description
            });
            return new TaxRateDto
            {
                TaxRateId = newTaxRate.TaxRateId,
                Rate = newTaxRate.Rate,
                Description = newTaxRate.Description
            };
        }

        public async Task<TaxRateDto> UpdateTaxRate(TaxRateDto taxRate)
        {
            var updatedTaxRate = await _taxRateRepository.UpdateAsync(new Models.TaxRate
            {
                TaxRateId = taxRate.TaxRateId,
                Rate = taxRate.Rate,
                Description = taxRate.Description
            });
            return new TaxRateDto
            {
                TaxRateId = updatedTaxRate.TaxRateId,
                Rate = updatedTaxRate.Rate,
                Description = updatedTaxRate.Description
            };
        }

        public async Task<bool> DeleteTaxRate(int id)
        {
            return await _taxRateRepository.DeleteAsync(id);
        }
    }
}