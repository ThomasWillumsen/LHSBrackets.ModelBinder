using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings
namespace LHSBrackets.ModelBinder.Example.Database
{
    public class Book
    {
        public Book(string name, DateTime releaseDate, DifficultyEnum difficulty)
        {
            Name = name;
            ReleaseDate = releaseDate;
            Difficulty = difficulty;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public int CategoryId { get; set; }
        public Guid AuthorId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public Author Author { get; set; }
    }


    public enum DifficultyEnum
    {
        EasyPeasyLemonSqueezy,
        RatherEasy,
        Medium,
        Difficult,
        PrettyFuckingDifficult
    }
}
