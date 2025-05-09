using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models;
using Library.Data;

public class BookService
{
    private readonly LibraryDbContext _db;

    public BookService(LibraryDbContext db)
    {
        _db = db;
    }

    // CREATE
    public async Task CreateBookAsync(Book book)
    {
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
    }

    // READ ALL
    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _db.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Borrows)
            .ToListAsync();
    }

    // READ BY ID
    public async Task<Book> GetBookByIdAsync(int id)
    {
        return await _db.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Include(b => b.Borrows)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    // READ CHUNKS (for pagination)
    public async Task<List<Book>> GetBooksPagedAsync(int pageNumber, int pageSize)
    {
        return await _db.Books
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // FILTERING (by author)
    public async Task<List<Book>> GetBooksByAuthorAsync(string author)
    {
        return await _db.Books
            .Where(b => b.Author.Contains(author))
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // FILTERING (by release year)
    public async Task<List<Book>> GetBooksByReleaseYearAsync(int year)
    {
        return await _db.Books
            .Where(b => b.ReleaseYear == year)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // SORTING (by title)
    public async Task<List<Book>> GetBooksSortedByTitleAsync()
    {
        return await _db.Books
            .OrderBy(b => b.Title)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // SORTING (by author)
    public async Task<List<Book>> GetBooksSortedByAuthorAsync()
    {
        return await _db.Books
            .OrderBy(b => b.Author)
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // UPDATE
    public async Task UpdateBookAsync(Book updatedBook)
    {
        var existing = await _db.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == updatedBook.Id);

        if (existing == null) return;

        existing.Title = updatedBook.Title;
        existing.Author = updatedBook.Author;
        existing.ISBN = updatedBook.ISBN;
        existing.ReleaseYear = updatedBook.ReleaseYear;

        // Update categories if needed
        if (updatedBook.BookCategories != null)
        {
            existing.BookCategories = updatedBook.BookCategories;
        }

        await _db.SaveChangesAsync();
    }

    // DELETE
    public async Task DeleteBookAsync(int id)
    {
        var book = await _db.Books
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book != null)
        {
            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
        }
    }

    // SEARCH by title, author or ISBN
    public async Task<List<Book>> SearchBooksAsync(string searchTerm)
    {
        return await _db.Books
            .Where(b => b.Title.Contains(searchTerm) ||
                       b.Author.Contains(searchTerm) ||
                       (b.ISBN != null && b.ISBN.Contains(searchTerm)))
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }

    // Get books currently borrowed
    public async Task<List<Book>> GetBorrowedBooksAsync()
    {
        return await _db.Books
            .Where(b => b.Borrows.Any(br => br.ReturnDate == null))
            .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
            .ToListAsync();
    }
}