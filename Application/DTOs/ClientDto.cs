namespace practica_VargasLeonardo.Application.DTOs;

public class ClientDto
{
    public int Clientid { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class CreateClientDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}

public class UpdateClientDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}
