using Microsoft.EntityFrameworkCore;
using Library.Models;

namespace Library.Data
{
    public class LibraryDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=Library;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.Book)
                .WithMany()
                .HasForeignKey(b => b.BookId);

            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.Member)
                .WithMany(m => m.Borrows)
                .HasForeignKey(b => b.MemberId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany()
                .HasForeignKey(b => b.CategoryId);


            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fantastyka" },
                new Category { Id = 2, Name = "Nauka" },
                new Category { Id = 3, Name = "Historia" }
            );

            modelBuilder.Entity<Member>().HasData(
                new Member { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan@example.com" },
                new Member { Id = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna@example.com" }
            );
        }
    }
}