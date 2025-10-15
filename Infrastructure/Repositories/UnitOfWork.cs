using practica_VargasLeonardo.Application.Interfaces;
using practica_VargasLeonardo.Domain.Entities;
using practica_VargasLeonardo.Infrastructure.Data;

namespace practica_VargasLeonardo.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly dbContextpractica2 _context;
    private bool _disposed = false;
    
    private IGenericRepository<Client>? _clients;
    private IGenericRepository<Product>? _products;
    private IGenericRepository<Order>? _orders;
    private IGenericRepository<Orderdetail>? _orderDetails;

    public UnitOfWork(dbContextpractica2 context)
    {
        _context = context;
    }

    public IGenericRepository<Client> Clients =>
        _clients ??= new GenericRepository<Client>(_context);

    public IGenericRepository<Product> Products =>
        _products ??= new GenericRepository<Product>(_context);

    public IGenericRepository<Order> Orders =>
        _orders ??= new GenericRepository<Order>(_context);

    public IGenericRepository<Orderdetail> OrderDetails =>
        _orderDetails ??= new GenericRepository<Orderdetail>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
