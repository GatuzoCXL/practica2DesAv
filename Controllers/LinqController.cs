using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using practica_VargasLeonardo.Domain.Entities;
using practica_VargasLeonardo.Application.Interfaces;

namespace practica_VargasLeonardo.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class LinqController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LinqController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    //1. Obtener todos los clientes
    [HttpGet("clients")]
    public async Task<ActionResult<IEnumerable<Client>>> GetAllClients()
    {
        var clients = await _unitOfWork.Clients.GetAllAsync();
        return Ok(clients);
    }

    ///parte1
    [HttpGet("exercise1/clients-by-name/{name}")]
    [ProducesResponseType(typeof(IEnumerable<Client>), 200)]
    public async Task<ActionResult<IEnumerable<Client>>> GetClientsBySpecificName(string name)
    {
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        
        //uso de linQ where para el filtro de clientes cuyo nombre contenga el valor específico
        var filteredClients = allClients
            .Where(c => c.Name.Contains(name))  
            .ToList();                      
        
        return Ok(filteredClients);
    }

    // 2. Buscar clientes por nombre (contiene texto)
    [HttpGet("clients/search/{name}")]
    public async Task<ActionResult<IEnumerable<Client>>> SearchClientsByName(string name)
    {
        var clients = await _unitOfWork.Clients.FindAsync(c => c.Name.Contains(name));
        return Ok(clients);
    }

    ///parte2
    [HttpGet("exercise2/products-price-greater-than/{price}")]
    [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsWithPriceGreaterThanExercise(decimal price)
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //uso de linQ where para el filtro de productos con precio mayor al valor proporcionado
        var expensiveProducts = allProducts
            .Where(p => p.Price > price) 
            .ToList();                   
        
        return Ok(expensiveProducts);
    }

    //productos con precio mayor a un valor específico
    [HttpGet("products/price-greater-than/{price}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsWithPriceGreaterThan(decimal price)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.Price > price);
        return Ok(products);
    }

    //productos por descripción (contiene texto)
    [HttpGet("products/description/{description}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByDescription(string description)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.Description != null && p.Description.Contains(description));
        return Ok(products);
    }

    //ordenes de un cliente específico
    [HttpGet("orders/client/{clientId}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByClient(int clientId)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.Clientid == clientId);
        return Ok(orders);
    }

    //ordenes por rango de fechas
    [HttpGet("orders/date-range")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var orders = await _unitOfWork.Orders.FindAsync(o => o.Orderdate >= startDate && o.Orderdate <= endDate);
        return Ok(orders);
    }

    //detalles de orden por ID de orden
    [HttpGet("order-details/order/{orderId}")]
    public async Task<ActionResult<IEnumerable<Orderdetail>>> GetOrderDetailsByOrderId(int orderId)
    {
        var orderDetails = await _unitOfWork.OrderDetails.FindAsync(od => od.Orderid == orderId);
        return Ok(orderDetails);
    }

    ///parte3
    [HttpGet("exercise3/order-products/{orderId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetProductsInOrderWithProjection(int orderId)
    {
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //filtro de detalles de la orden específica y proyectar nombre del producto y cantidad
        var orderProducts = allOrderDetails
            .Where(od => od.Orderid == orderId)  
            .Select(od => new                   
            {
                ProductName = allProducts.FirstOrDefault(p => p.Productid == od.Productid)?.Name ?? "Producto no encontrado",
                Quantity = od.Quantity
            });
        
        return Ok(orderProducts);
    }

    //productos ordenados por precio descendente
    [HttpGet("products/ordered-by-price-desc")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsOrderedByPriceDesc()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var orderedProducts = allProducts.OrderByDescending(p => p.Price);
        return Ok(orderedProducts);
    }

    //clientes que han realizado órdenes
    [HttpGet("clients/with-orders")]
    public async Task<ActionResult<IEnumerable<Client>>> GetClientsWithOrders()
    {
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        
        var clientsWithOrders = allClients.Where(c => allOrders.Any(o => o.Clientid == c.Clientid));
        return Ok(clientsWithOrders);
    }

    //productos que nunca han sido ordenados
    [HttpGet("products/never-ordered")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsNeverOrdered()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        
        var neverOrderedProducts = allProducts.Where(p => !allOrderDetails.Any(od => od.Productid == p.Productid));
        return Ok(neverOrderedProducts);
    }

    //clientes ordenados por cantidad de órdenes (solo entidades)
    [HttpGet("clients/ordered-by-activity")]
    public async Task<ActionResult<IEnumerable<Client>>> GetClientsOrderedByActivity()
    {
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        
        //clientes por cantidad de órdenes (más activos primero)
        var clientsOrderedByActivity = allClients
            .OrderByDescending(c => allOrders.Count(o => o.Clientid == c.Clientid));
        
        return Ok(clientsOrderedByActivity);
    }

    //productos ordenados por popularidad (solo entidades)
    [HttpGet("products/ordered-by-popularity")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsOrderedByPopularity()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        
        //orden de productos por cantidad total vendida (más populares primero)
        var productsOrderedByPopularity = allProducts
            .OrderByDescending(p => allOrderDetails.Where(od => od.Productid == p.Productid).Sum(od => od.Quantity));
        
        return Ok(productsOrderedByPopularity);
    }

    // 13. FILTRAR con Where() - Productos en rango de precio
    [HttpGet("products/price-range")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsInPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var productsInRange = allProducts.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
        return Ok(productsInRange);
    }

    // 14. TRANSFORMAR con Select() - Solo nombres de clientes
    [HttpGet("clients/names-only")]
    public async Task<ActionResult<IEnumerable<string>>> GetClientNamesOnly()
    {
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        var clientNames = allClients.Select(c => c.Name);
        return Ok(clientNames);
    }

    // 15. ORDENAR con OrderBy() - Productos por precio ascendente
    [HttpGet("products/ordered-by-price-asc")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsOrderedByPriceAsc()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var orderedProducts = allProducts.OrderBy(p => p.Price);
        return Ok(orderedProducts);
    }

    // 16. CONTAR productos por rango de precio
    [HttpGet("products/count-by-price-range")]
    public async Task<ActionResult<object>> GetProductCountByPriceRange()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        // Contar productos en diferentes rangos de precio
        var priceRanges = new
        {
            Cheap = allProducts.Count(p => p.Price < 50),
            Medium = allProducts.Count(p => p.Price >= 50 && p.Price < 100),
            Expensive = allProducts.Count(p => p.Price >= 100)
        };
        
        return Ok(priceRanges);
    }

    // 17. AGREGAR con Average() - Estadísticas de precios
    [HttpGet("products/price-statistics")]
    public async Task<ActionResult<object>> GetPriceStatistics()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        // Estadísticas usando diferentes agregaciones
        var stats = new
        {
            AveragePrice = allProducts.Average(p => p.Price),
            MinPrice = allProducts.Min(p => p.Price),
            MaxPrice = allProducts.Max(p => p.Price),
            TotalProducts = allProducts.Count()
        };
        
        return Ok(stats);
    }

    // 18. COMBINAR múltiples operaciones LINQ
    [HttpGet("products/complex-query")]
    public async Task<ActionResult<IEnumerable<Product>>> GetComplexProductQuery()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        // Combinar: Where() + OrderByDescending() + Take()
        var complexQuery = allProducts
            .Where(p => p.Price > 10 && p.Description != null)  
            .OrderByDescending(p => p.Price)                 
            .Take(5);                                           
        
        return Ok(complexQuery);
    }

    ///parte4
    [HttpGet("exercise4/order-total-quantity/{orderId}")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult<object>> GetTotalQuantityByOrder(int orderId)
    {
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        
        //filtrado por OrderId, proyectar Quantity y sumar
        var totalQuantity = allOrderDetails
            .Where(od => od.Orderid == orderId)  
            .Select(od => od.Quantity)           
            .Sum();                              
        
        return Ok(new { OrderId = orderId, TotalQuantity = totalQuantity });
    }

    ///parte5
    [HttpGet("exercise5/most-expensive-product")]
    [ProducesResponseType(typeof(Product), 200)]
    public async Task<ActionResult<Product>> GetMostExpensiveProduct()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //orden por precio descendente y tomar el primero
        var mostExpensive = allProducts
            .OrderByDescending(p => p.Price)  
            .FirstOrDefault();               
        
        if (mostExpensive == null)
            return NotFound("No products found");
            
        return Ok(mostExpensive);
    }

    ///parte6
    [HttpGet("exercise6/orders-after-date/{date}")]
    [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersAfterDate(DateTime date)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        
        //filtro d e pedidos después de la fecha específica
        var ordersAfterDate = allOrders
            .Where(o => o.Orderdate > date)  
            .ToList();                     
        
        return Ok(ordersAfterDate);
    }

    ///parte7
    [HttpGet("exercise7/average-product-price")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult<object>> GetAverageProductPrice()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //precios y calcular promedio
        var averagePrice = allProducts
            //listo.Select()  
            .Average(p => p.Price);           
        
        return Ok(new { AveragePrice = averagePrice, TotalProducts = allProducts.Count() });
    }

    ///parte8
    [HttpGet("exercise8/products-without-description")]
    [ProducesResponseType(typeof(IEnumerable<Product>), 200)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsWithoutDescription()
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //filtrado de productos donde Description sea nulo o vacío
        var productsWithoutDescription = allProducts
            .Where(p => string.IsNullOrEmpty(p.Description))
            .ToList();                                        
        
        return Ok(productsWithoutDescription);
    }

    ///parte9
    [HttpGet("exercise9/client-with-most-orders")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult<object>> GetClientWithMostOrders()
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        
        //pedidos por clientid, ordenar por cantidad y proyectar
        var clientWithMostOrders = allOrders
            .GroupBy(o => o.Clientid)                  
            .OrderByDescending(g => g.Count())         
            .Select(g => new                          
            {
                ClientId = g.Key,
                ClientName = allClients.FirstOrDefault(c => c.Clientid == g.Key)?.Name ?? "Cliente no encontrado",
                OrderCount = g.Count()
            })
            .FirstOrDefault();                        
        
        return Ok(clientWithMostOrders);
    }

    ///parte10
    [HttpGet("exercise10/all-order-details-with-products")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetAllOrderDetailsWithProducts()
    {
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //obtener todos los detalles y proyectar productname y quantity
        var orderDetailsWithProducts = allOrderDetails
            .Select(od => new                         
            {
                OrderId = od.Orderid,
                ProductName = allProducts.FirstOrDefault(p => p.Productid == od.Productid)?.Name ?? "Producto no encontrado",
                Quantity = od.Quantity
            })
            .ToList();                                  
        
        return Ok(orderDetailsWithProducts);
    }

    ///parte11
    [HttpGet("exercise11/products-sold-to-client/{clientId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetProductsSoldToClient(int clientId)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        
        //filtro de ordene por clientid y proyectar productname
        var productsSoldToClient = allOrders
            .Where(o => o.Clientid == clientId) 
            .SelectMany(o => allOrderDetails.Where(od => od.Orderid == o.Orderid))  
            .Select(od => new                       
            {
                ProductId = od.Productid,
                ProductName = allProducts.FirstOrDefault(p => p.Productid == od.Productid)?.Name ?? "Producto no encontrado",
                Quantity = od.Quantity
            })
            .Distinct()                                 
            .ToList();
        
        return Ok(productsSoldToClient);
    }

    ///parte11
    [HttpGet("exercise11-alt/unique-products-sold-to-client/{clientId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetUniqueProductsSoldToClient(int clientId)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        var allProducts = await _unitOfWork.Products.GetAllAsync();
        var uniqueProducts = allOrderDetails
            .Where(od => allOrders.Any(o => o.Orderid == od.Orderid && o.Clientid == clientId))  
            .Select(od => allProducts.FirstOrDefault(p => p.Productid == od.Productid))        
            .Where(p => p != null)                                                             
            .Distinct()                                                                        
            .Select(p => new                                                                   
            {
                ProductId = p.Productid,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price
            })
            .ToList();
        
        return Ok(uniqueProducts);
    }

    ///parte12
    [HttpGet("exercise12/clients-who-bought-product/{productId}")]
    [ProducesResponseType(typeof(IEnumerable<object>), 200)]
    public async Task<ActionResult<IEnumerable<object>>> GetClientsWhoBoughtProduct(int productId)
    {
        var allOrderDetails = await _unitOfWork.OrderDetails.GetAllAsync();
        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        var allClients = await _unitOfWork.Clients.GetAllAsync();
        
        //filtro de detalle de orden por productd y proyectar clientname
        var clientsWhoBoughtProduct = allOrderDetails
            .Where(od => od.Productid == productId)   
            .Select(od => allOrders.FirstOrDefault(o => o.Orderid == od.Orderid))  
            .Where(o => o != null)                     
            .Select(o => new                             
            {
                ClientId = o.Clientid,
                ClientName = allClients.FirstOrDefault(c => c.Clientid == o.Clientid)?.Name ?? "Cliente no encontrado"
            })
            .Distinct()                                 
            .ToList();
        
        return Ok(clientsWhoBoughtProduct);
    }
}
