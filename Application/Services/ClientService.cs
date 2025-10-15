using practica_VargasLeonardo.Application.DTOs;
using practica_VargasLeonardo.Application.Interfaces;
using practica_VargasLeonardo.Domain.Entities;

namespace practica_VargasLeonardo.Application.Services;

public class ClientService : IClientService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ClientDto>> GetAllClientsAsync()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync();
        return clients.Select(c => new ClientDto
        {
            Clientid = c.Clientid,
            Name = c.Name,
            Email = c.Email
        });
    }

    public async Task<ClientDto?> GetClientByIdAsync(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null) return null;

        return new ClientDto
        {
            Clientid = client.Clientid,
            Name = client.Name,
            Email = client.Email
        };
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto)
    {
        var client = new Client
        {
            Name = createClientDto.Name,
            Email = createClientDto.Email
        };

        await _unitOfWork.Clients.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return new ClientDto
        {
            Clientid = client.Clientid,
            Name = client.Name,
            Email = client.Email
        };
    }

    public async Task<bool> UpdateClientAsync(int id, UpdateClientDto updateClientDto)
    {
        var existingClient = await _unitOfWork.Clients.GetByIdAsync(id);
        if (existingClient == null) return false;

        existingClient.Name = updateClientDto.Name;
        existingClient.Email = updateClientDto.Email;

        _unitOfWork.Clients.Update(existingClient);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null) return false;

        _unitOfWork.Clients.Remove(client);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<ClientDto>> SearchClientsByNameAsync(string name)
    {
        var clients = await _unitOfWork.Clients.FindAsync(c => c.Name.Contains(name));
        return clients.Select(c => new ClientDto
        {
            Clientid = c.Clientid,
            Name = c.Name,
            Email = c.Email
        });
    }
}
