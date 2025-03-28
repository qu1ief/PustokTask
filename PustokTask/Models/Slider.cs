using System.ComponentModel.DataAnnotations;

namespace PustokTask.Models
{
    public class Slider : BaseEntity
    {

        [Required]
        [StringLength(50)]
        public string Title { get;set; }
        [Required]
        [StringLength(200)]
        public string Description { get;set; }
        [Required]
        [StringLength(200)]
        public string Image { get;set; }
        [Required]
        [StringLength(100)]
        public string ButtonLink { get;set; }
        [Required]
        [StringLength(50)]
        public string ButtonText { get;set; }
        public int Order { get;set; }
    }
}
