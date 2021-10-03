using System;
using System.Collections.Generic;
using LHSBrackets.ModelBinder.Example.Database;

namespace LHSBrackets.ModelBinder.Example.Controllers.Rentals
{
    #pragma warning disable CS8618
    public class BooksFilterRequest : FilterRequest
    {
        public FilterOperations<Guid>? AuthorId { get; set; }
        public FilterOperations<DateTime>? ReleaseDate { get; set; }
        public FilterOperations<int>? CategoryId { get; set; }
        public FilterOperations<decimal?>? Price { get; set; }
        public FilterOperations<string>? Name { get; set; }
        public FilterOperations<string>? CategoryName { get; set; }
        public FilterOperations<DifficultyEnum>? Difficulty { get; set; }
    }
}