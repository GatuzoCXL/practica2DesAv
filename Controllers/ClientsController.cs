using Microsoft.AspNetCore.Mvc;
using practica_VargasLeonardo.Application.Interfaces;
using practica_VargasLeonardo.Domain.Entities;

namespace practica_VargasLeonardo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ClientsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Client>> GetClient(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        
        if (client == null)
        {
            return NotFound();
        }

        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> CreateClient(Client client)
    {
        await _unitOfWork.Clients.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClient), new { id = client.Clientid }, client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClient(int id, Client client)
    {
        if (id != client.Clientid)
        {
            return BadRequest();
        }

        var existingClient = await _unitOfWork.Clients.GetByIdAsync(id);
        if (existingClient == null)
        {
            return NotFound();
        }

        _unitOfWork.Clients.Update(client);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(id);
        if (client == null)
        {
            return NotFound();
        }

        _unitOfWork.Clients.Remove(client);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Client>>> SearchClients([FromQuery] string name)
    {
        var clients = await _unitOfWork.Clients.FindAsync(c => c.Name.Contains(name));
        return Ok(clients);
    }
}
