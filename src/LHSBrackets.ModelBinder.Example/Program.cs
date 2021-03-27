using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using LHSBrackets.ModelBinder.Example.Database;
using System;

namespace LHSBrackets.ModelBinder.Example
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                await MigrateDb(scope);
                await SeedDb(scope);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static async Task MigrateDb(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
            await dbContext.Database.MigrateAsync();
        }

        private static async Task SeedDb(IServiceScope scope)
        {
            var categories = new List<Category>()
            {
                new Category("Drama"),
                new Category("Fantasy"),
                new Category("Furry"),
                new Category("Science fiction"),
            };

            var authors = new List<Author>()
            {
                new Author("Jodle Birge"),
                new Author("Birthe Kjær"),
                new Author("Lis"),
                new Author("Per"),
            };

            var books = new List<Book>()
            {
                new Book("Fugtige Flemming", new DateTime(2021, 03, 20), DifficultyEnum.EasyPeasyLemonSqueezy, 19.99m){Category = categories[0], Author = authors[1]},
                new Book("Sure Søren", new DateTime(2021, 03, 25), DifficultyEnum.PrettyFuckingDifficult, 100m){Category = categories[0], Author = authors[1]},
                new Book("Kradsbørstige Kurt", new DateTime(2021, 03, 29), DifficultyEnum.PrettyFuckingDifficult, 55.995m){Category = categories[1], Author = authors[1]},
                new Book("Hurtige Henning", new DateTime(2021, 04, 10), DifficultyEnum.Medium, 10m){Category = categories[2], Author = authors[2]},
                new Book("Overvægtige Ove", new DateTime(2021, 05, 20), DifficultyEnum.RatherEasy, 5m){Category = categories[3], Author = authors[3]},
            };

            var dbContext = scope.ServiceProvider.GetService<AppDbContext>()!;
            if (!await dbContext.Categories.AnyAsync())
                await dbContext.Categories.AddRangeAsync(categories);

            if (!await dbContext.Authors.AnyAsync())
                await dbContext.Authors.AddRangeAsync(authors);

            if (!await dbContext.Books.AnyAsync())
                await dbContext.Books.AddRangeAsync(books);

            await dbContext.SaveChangesAsync();
        }
    }
}