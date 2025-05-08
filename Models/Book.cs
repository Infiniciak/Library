using Library.Models;
using System;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}