using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokTask.Models
{
    public class Book:BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get;set; }
        [Required]
        [StringLength(200)]
        public string Description { get;set; }
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string ProductCode { get;set; }
        public bool Instock { get;set; }
        public bool IsFeatured { get;set; }
        public bool IsNew { get;set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get;set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get;set; }
        public int  Rate { get;set; }
        public int AuthorId { get;set; }
        public Author Author { get;set; }
        public int GenreId { get;set; }
        public Genre Genre { get;set; }
        public List<BookImage> BookImages { get;set; }
        public List<BookTag> BookTags { get; set; }

        public List<BookComment> BookComments { get; set; }

        public Book()
        {
            BookComments = new List<BookComment>();
        }

    }
}
