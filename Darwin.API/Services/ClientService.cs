using System;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClients();
        Task<Client> GetClientById(int id);
        Task<Client> AddClient(Client client);
        Task<Client> UpdateClient(Client client);
        Task<bool> DeleteClient(int id);
    }

    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;

        public ClientService(IRepository<Client> clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<Client> GetClientById(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<Client> AddClient(Client client)
        {
            return await _clientRepository.AddAsync(client);
        }

        public async Task<Client> UpdateClient(Client client)
        {
            return await _clientRepository.UpdateAsync(client);
        }

        public async Task<bool> DeleteClient(int id)
        {
            return await _clientRepository.DeleteAsync(id);
        }
    }
}

