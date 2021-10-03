using LHSBrackets.ModelBinder.EF;
using LHSBrackets.ModelBinder.Example.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LHSBrackets.ModelBinder.Example.Controllers.Rentals
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public BooksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(
            [FromQuery] BooksFilterRequest filterRequest,
            [FromQuery] string? someOtherRandomQuery
        )
        {
            var books = await _dbContext.Books
                .Include(x => x.Category)
                .ApplyFilters(x => x.AuthorId, filterRequest.AuthorId)
                .ApplyFilters(x => x.ReleaseDate, filterRequest.ReleaseDate)
                .ApplyFilters(x => x.CategoryId, filterRequest.CategoryId)
                .ApplyFilters(x => x.Name, filterRequest.Name)
                .ApplyFilters(x => x.Category.Name, filterRequest.CategoryName)
                .ApplyFilters(x => x.Price, filterRequest.Price)
                .ApplyFilters(x => x.Difficulty, filterRequest.Difficulty)
                .ToListAsync();

            return Ok(books);
        }
    }
}