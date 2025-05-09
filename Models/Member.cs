using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Library.Data;

namespace Library.Models
{
    [Table("members")]
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string CardNumber { get; set; }

        [Required]
        public string Email { get; set; }

        public List<Borrow> Borrows { get; set; } = new List<Borrow>();
    }
}