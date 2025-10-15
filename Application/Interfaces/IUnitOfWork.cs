using practica_VargasLeonardo.Domain.Entities;

namespace practica_VargasLeonardo.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Client> Clients { get; }
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<Orderdetail> OrderDetails { get; }
    
    Task<int> SaveChangesAsync();
    int SaveChanges();
}
