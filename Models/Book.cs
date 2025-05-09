using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models
{
    [Table("books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string ISBN { get; set; }
        public int ReleaseYear { get; set; }


        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        public ICollection<Borrow> Borrows { get; set; } = new List<Borrow>();
    }
}