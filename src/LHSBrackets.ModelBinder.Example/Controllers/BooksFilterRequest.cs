using System;
using System.Collections.Generic;
using LHSBrackets.ModelBinder.Example.Database;

namespace LHSBrackets.ModelBinder.Example.Controllers.Rentals
{
    // use nullable types
    public class BooksFilterRequest : FilterRequest
    {
        public FilterOperations<Guid?> AuthorId { get; set; }
        public FilterOperations<DateTime?> ReleaseDate { get; set; }
        public FilterOperations<int?> CategoryId { get; set; }
        public FilterOperations<decimal?> Price { get; set; }
        public FilterOperations<string> Name { get; set; }
        public FilterOperations<string> CategoryName { get; set; }
        public FilterOperations<DifficultyEnum?> Difficulty { get; set; }


        public override IEnumerable<(string PropertyName, Action<string> BindValue)> GetPropertyBinders()
        {
            var binders = new List<(string PropertyName, Action<string> BindValue)>();

            binders.AddRange(base.GetPropertyBinder(AuthorId, nameof(AuthorId)));
            binders.AddRange(base.GetPropertyBinder(ReleaseDate, nameof(ReleaseDate)));
            binders.AddRange(base.GetPropertyBinder(CategoryName, nameof(CategoryName)));
            binders.AddRange(base.GetPropertyBinder(Price, nameof(Price)));
            binders.AddRange(base.GetPropertyBinder(Name, nameof(Name)));
            binders.AddRange(base.GetPropertyBinder(CategoryId, nameof(CategoryId)));
            binders.AddRange(base.GetPropertyBinder(Difficulty, nameof(Difficulty)));

            return binders;
        }
    }
}