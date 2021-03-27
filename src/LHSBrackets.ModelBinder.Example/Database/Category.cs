using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings
namespace LHSBrackets.ModelBinder.Example.Database
{
    public class Category
    {
        public Category(string name)
        {
            Name = name;
            Books = new List<Book>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Book> Books { get; set; }
    }
}
