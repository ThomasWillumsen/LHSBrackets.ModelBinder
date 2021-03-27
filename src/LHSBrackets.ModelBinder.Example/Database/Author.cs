using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable warnings
namespace LHSBrackets.ModelBinder.Example.Database
{
    public class Author
    {
        public Author(string fullName)
        {
            FullName = fullName;
            Books = new List<Book>();
        }

        [Key]
        public Guid Id { get; set; }
        public string FullName { get; set; }

        public IEnumerable<Book> Books { get; set; }
    }
}
