using LHSBrackets.ModelBinder.EF;
using LHSBrackets.ModelBinder.Example.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            [FromQuery][ModelBinder(typeof(FilterModelBinder))] BooksFilterRequest filters
        )
        {
            var books = await _dbContext.Books
                .ApplyFilters(x => x.AuthorId, filters.AuthorId)
                .ApplyFilters(x => x.ReleaseDate, filters.ReleaseDate)
                .ApplyFilters(x => x.CategoryId, filters.CategoryId)
                .ApplyFilters(x => x.Name, filters.Name)
                // .ApplyFilters(x => x.Category.Name, filters.CategoryName)
                .ApplyFilters(x => x.Difficulty, filters.Difficulty)
                .ToListAsync();

            return Ok(books);
        }
    }
}