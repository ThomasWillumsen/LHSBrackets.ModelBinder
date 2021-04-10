using System;
using System.Collections.Generic;
using LHSBrackets.ModelBinder.Example.Database;

namespace LHSBrackets.ModelBinder.Example.Controllers.Rentals
{
    public class BooksFilterRequest
    {
        public Filters<Guid>? AuthorId { get; set; }
        public Filters<DateTime>? ReleaseDate { get; set; }
        public Filters<int>? CategoryId { get; set; }
        public Filters<decimal>? Price { get; set; }
        public Filters<string>? Name { get; set; }
        public Filters<string>? CategoryName { get; set; }
        public Filters<DifficultyEnum>? Difficulty { get; set; }
    }
}