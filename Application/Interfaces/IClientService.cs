using practica_VargasLeonardo.Application.DTOs;

namespace practica_VargasLeonardo.Application.Interfaces;

public interface IClientService
{
    Task<IEnumerable<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto);
    Task<bool> UpdateClientAsync(int id, UpdateClientDto updateClientDto);
    Task<bool> DeleteClientAsync(int id);
    Task<IEnumerable<ClientDto>> SearchClientsByNameAsync(string name);
}
