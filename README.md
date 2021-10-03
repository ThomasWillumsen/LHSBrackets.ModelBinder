# LHSBrackets.ModelBinder

A .NET modelbinder for parsing query parameters that use the LHS Brackets syntax commonly used in REST APIs.

**Platforms** - .NET 5.0

Supported operators:

- [eq] - _equal to_
- [ne] - _not equal to_
- [gt] - _greater than_
- [gte] - _greater than or equal to_
- [lt] - _less than_
- [lte] - _less than or equal to_
- [in] - _contained in array_
- [nin] - _not contained in array_

Example request: `GET https://localhost:3000/api/books?releaseDate[gte]=2021-01-01&authorId[in]=1,2,3&price[lt]=10`

### Model binding

[LHSBrackets.ModelBinder](https://www.nuget.org/packages/LHSBrackets.ModelBinder/)

The model binder is using en-GB culture for now. Might be updated to take a culture input at a later date.

```csharp
// Startup.cs
services.AddControllers(options => {
    options.ModelBinderProviders.Insert(0, new FilterModelBinderProvider());
})
```

```csharp
// Define your own request model that derives from FilterRequest
public class BooksFilterRequest : FilterRequest
    {
        public FilterOperations<Guid> AuthorId { get; set; }
        public FilterOperations<DateTime> ReleaseDate { get; set; }
        public FilterOperations<decimal?> Price { get; set; }
    }
```

```csharp
// Controller action - GET request
[HttpGet]
public async Task<IActionResult> GetBooks(
    [FromQuery] BooksFilterRequest filterRequest, // this uses the LHSBrackets model binder
    [FromQuery] string? someOtherRandomQuery // this uses the built-in model binder
)
{
    // stuff
}
```

### Entity Framework

[LHSBrackets.ModelBinder.EF](https://www.nuget.org/packages/LHSBrackets.ModelBinder.EF/)

You can apply the filters to Linq statements for database queries.
This will apply all LHS bracket-operations from the filter request that have values.

```csharp
[HttpGet]
public async Task<IActionResult> GetBooks([FromQuery] BooksFilterRequest filterRequest)
{
    var books = await _dbContext.Books
        .ApplyFilters(x => x.AuthorId, filterRequest.AuthorId)
        .ApplyFilters(x => x.ReleaseDate, filterRequest.ReleaseDate)
        .ApplyFilters(x => x.Price, filterRequest.Price)
        .ToListAsync();
}

```

### Swagger (upcoming)

_nuget: LHSBrackets.ModelBinder.Swashbuckle_

Swashbuckle support for generating accurate openapi 3.0 specifications coming up.
