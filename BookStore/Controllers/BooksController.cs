using BookStore.DataAccess;
using BookStore.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    //GET: /api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        var books = await _context.Books.Include(b => b.Category).ToListAsync(); // her kitap ile birlikte kategorisinide getiriyorum
        return Ok(books);
    }

    // GET: /api/books/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        var book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
        {
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        return Ok(book);
    }

    // GET: /api/books/category/{categoryId}
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId)
    {
        var books = await _context.Books.Where(b => b.CategoryId == categoryId).ToListAsync();

        if (!books.Any())
        {
            return NotFound(new { message = "Bu kategoride kitap bulunamadı." });
        }

        return Ok(books);
    }

    // GET: /api/books/search?title={title} 
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return BadRequest(new { message = "Arama terimi boş olamaz." });
        }

        var books = await _context.Books
            .Where(b => b.Title.Contains(title))
            .ToListAsync();

        if (!books.Any())
        {
            return NotFound(new { message = "Eşleşen kitap bulunamadı." });
        }

        return Ok(books);
    }


    // POST: /api/books
    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] Book book)
    {
        if (book == null)
        {
            return BadRequest(new { message = "Geçersiz kitap verisi." });
        }

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return Ok(book); 
    }

    // PUT: /api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] Book updatedBook)
    {
        if (updatedBook == null || id != updatedBook.Id)
        {
            return BadRequest(new { message = "Geçersiz kitap verisi." });
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.ISBN = updatedBook.ISBN;
        book.Price = updatedBook.Price;
        book.Stock = updatedBook.Stock;
        book.PublicationYear = updatedBook.PublicationYear;
        book.CategoryId = updatedBook.CategoryId;

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return Ok(book);
    }

    // DELETE: /api/books/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound(new { message = "Kitap bulunamadı." });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Kitap başarıyla silindi." });
    }


}
