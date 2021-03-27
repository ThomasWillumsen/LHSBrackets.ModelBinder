using System;
using System.Collections.Generic;
using LHSBrackets.ModelBinder.Example.Database;

namespace LHSBrackets.ModelBinder.Example.Controllers.Rentals
{
    public class BooksFilterRequest : FilterRequest
    {
        public FilterOperations<Guid?> AuthorId { get; set; } = new FilterOperations<Guid?>();
        public FilterOperations<DateTime?> ReleaseDate { get; set; } = new FilterOperations<DateTime?>();
        public FilterOperations<int?> CategoryId { get; set; } = new FilterOperations<int?>();
        public FilterOperations<string> Name { get; set; } = new FilterOperations<string>();
        // public FilterOperations<string> CategoryName { get; set; } = new FilterOperations<string>();
        public FilterOperations<DifficultyEnum?> Difficulty { get; set; } = new FilterOperations<DifficultyEnum?>();


        public override IEnumerable<(string PropertyName, Action<string> BindValue)> GetBinders()
        {
            var binders = new List<(string PropertyName, Action<string> BindValue)>();

            binders.AddRange(BuildFilterOperationBinders(AuthorId, nameof(AuthorId)));
            binders.AddRange(BuildFilterOperationBinders(ReleaseDate, nameof(ReleaseDate)));
            // binders.AddRange(BuildFilterOperationBinders(CategoryName, nameof(CategoryName)));
            binders.AddRange(BuildFilterOperationBinders(Name, nameof(Name)));
            binders.AddRange(BuildFilterOperationBinders(CategoryId, nameof(CategoryId)));
            binders.AddRange(BuildFilterOperationBinders(Difficulty, nameof(Difficulty)));

            return binders;
        }
    }
}