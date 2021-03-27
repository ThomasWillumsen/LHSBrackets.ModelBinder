# LHSBrackets.ModelBinder

A .NET modelbinder for parsing query parameters that use the LHS Brackets syntax commonly used in REST APIs.

**Package** TODO
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

The model binder is using en-GB culture for now. TODO: take this as a configuration input instead.

```csharp
TODO: code examples
```
