using Microsoft.EntityFrameworkCore;
using Library.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows.Forms;
using System;
using Microsoft.Extensions.Logging.Debug;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.IO;

    namespace Library.Data
    {

        public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>

        {
            public LibraryDbContext CreateDbContext(string[] args)
            {
                // Get environment

                string environment = "Development";

                // Build config
                IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Library"))
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: true)
                    .Build();

                // Get connection string
                var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
                var connectionString = config.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);

                return new LibraryDbContext(optionsBuilder.Options);
            }
        }
    }
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }


        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }



        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<LibraryDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().ToTable("books");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<BookCategory>().ToTable("books_categories");
            modelBuilder.Entity<Member>().ToTable("members");
            modelBuilder.Entity<Borrow>().ToTable("borrows");
            // Configure join entity
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);

            SeedData(modelBuilder);
        }
    private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            var categories = new List<Category>
    {
        new Category { Id = 1, Name = "Fantasy" },
        new Category { Id = 2, Name = "Science Fiction" },
        new Category { Id = 3, Name = "Romance" },
        new Category { Id = 4, Name = "Thriller" },
        new Category { Id = 5, Name = "Biography" }
    };
            modelBuilder.Entity<Category>().HasData(categories);

            // Seed Books
            var books = new List<Book>
    {
        new Book { Id = 1, Title = "The Hobbit", Author = "J.R.R. Tolkien", ISBN = "9780261102217", ReleaseYear = 1937 },
        new Book { Id = 2, Title = "Dune", Author = "Frank Herbert", ISBN = "9780441172719", ReleaseYear = 1965 },
        new Book { Id = 3, Title = "Pride and Prejudice", Author = "Jane Austen", ISBN = "9780141439518", ReleaseYear = 1813 }
    };
            modelBuilder.Entity<Book>().HasData(books);

            // Seed BookCategories (many-to-many relationship)
            var bookCategories = new List<BookCategory>
    {
        new BookCategory { BookId = 1, CategoryId = 1 }, // The Hobbit - Fantasy
        new BookCategory { BookId = 2, CategoryId = 2 }, // Dune - Sci-Fi
        new BookCategory { BookId = 3, CategoryId = 3 }  // Pride and Prejudice - Romance
    };
            modelBuilder.Entity<BookCategory>().HasData(bookCategories);

            // Seed Members
            var members = new List<Member>
    {
        new Member { Id = 1, Name = "John", Surname = "Doe", CardNumber = "M001", Email = "john.doe@example.com" },
        new Member { Id = 2, Name = "Jane", Surname = "Smith", CardNumber = "M002", Email = "jane.smith@example.com" }
    };
            modelBuilder.Entity<Member>().HasData(members);

            // Seed Borrows
            var borrows = new List<Borrow>
    {
        new Borrow {
            Id = 1,
            BookId = 1,
            MemberId = 1,
            BorrowDate = DateTime.Now.AddDays(-14),
            ReturnDate = null
        },
        new Borrow {
            Id = 2,
            BookId = 2,
            MemberId = 2,
            BorrowDate = DateTime.Now.AddDays(-7),
            ReturnDate = DateTime.Now.AddDays(-1)
        }
    };
            modelBuilder.Entity<Borrow>().HasData(borrows);
        }
    }
}