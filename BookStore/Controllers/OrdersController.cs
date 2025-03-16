using BookStore.DataAccess;
using BookStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrdersController : Controller
{

    private readonly AppDbContext _context;

    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /api/orders/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Book)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound(new { message = "Sipariş Bulunamadı." });
        }

        return order;
    }

    // POST: /api/orders
    [HttpPost]
    public async Task<IActionResult> CreateORder([FromBody] Order order)
    {
        if (order== null)
        {
            return BadRequest(new { message = "Geçersiz sipariş verisi." });
        }

        var book = await _context.Books.FindAsync(order.BookId);
        if (book == null)
        {
            return NotFound(new { message = "Sipariş oluşturulamadı. Kitap bulunamadı." });
        }

        //Stok kontrolü
        if (book.Stock < order.Quantity)
        {
            return BadRequest(new { message = "Yetersiz stok! Sipariş oluşturulamadı." });
        }

        //Stoğu güncelle
        book.Stock -= order.Quantity;
        _context.Books.Update(book);

        //siparişi kaydett
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(order);
    }

    // PUT: /api/orders/{id}/status 
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound(new { message = "Sipariş bulunamadı." });
        }

        order.Status = status;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sipariş durumu güncellendi.", status = order.Status });
    }




}
