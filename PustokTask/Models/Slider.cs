using PustokTask.Areas.Manage.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PustokTask.Models
{
    public class Slider : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Desc { get; set; }
        public string ImgName { get; set; }
        [Required]
        public string ButtonLink { get; set; }
        [Required]
        [StringLength(80)]
        public string ButtonText { get; set; }
        [Required]
        public int Order { get; set; }
        [NotMapped]
        [TypeAttribute("image/png", "image/jpeg")]
        public IFormFile Photo { get; set; }
    }
}
